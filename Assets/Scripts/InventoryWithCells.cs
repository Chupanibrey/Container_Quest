using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryWithCells : IInventory
{
    // ������� ���������� � �������� �������� �� ���������.
    public event Action<object, IInventoryEntity, int> OnInventoryEntityAddedEvent;
    public event Action<object, Type, int> OnInventoryEntityRemovedEvent;
    public event Action<object> OnInventoryStatusChangedEvent;

    public int Capacity { get; set; }
    public bool IsFilled => cells.All(cell => cell.IsFilled);

    List<IInventoryCell> cells; // ��� ������ ���������

    public InventoryWithCells(int capacity)
    {
        this.Capacity = capacity;
        cells = new List<IInventoryCell>(capacity);

        for (int i = 0; i < capacity; i++)
            cells.Add(new InventoryCell());
    }

    public IInventoryEntity[] GetAllEntities() // �������� ������ ���� ��������� � ��������� (��� ����� ������ ������).
    {
        var allEntity = new List<IInventoryEntity>();

        foreach (var cell in cells)
            if (!cell.IsEmpty)
                allEntity.Add(cell.Entity);

        return allEntity.ToArray();
    } 

    public IInventoryEntity[] GetAllEntities(Type entityType) // �������� ������ ���� ��������� ���������� ���� � ��������� (��� ����� ������ ������).
    {
        var allEntityOfType = new List<IInventoryEntity>();
        var cellsOfType = cells.FindAll(cell => !cell.IsEmpty && cell.EntityType == entityType);

        foreach (var cell in cellsOfType)
            allEntityOfType.Add(cell.Entity);

        return allEntityOfType.ToArray();
    } 

    public IInventoryEntity GetEntity(Type entityType) // �������� �������� ���������� ���� �� ���������.
    {
        return cells.Find(cell => cell.EntityType == entityType).Entity;
    }

    public int GetEntityAmount(Type entityType) // �������� ����� ���������� ��������� ���������� ���� � ���������.
    {
        int amount = 0;
        var allEntityCells = cells.FindAll(cell => !cell.IsEmpty && cell.EntityType == entityType);

        foreach (var entityCell in allEntityCells)
            amount += entityCell.Amount;

        return amount;
    }

    public IInventoryEntity[] GetUsedEntities() // �������� ������ ������������ ���������.
    {
        var requiredCells = cells.FindAll(cell => !cell.IsEmpty && cell.Entity.Status.IsUsed);
        var entityUsed = new List<IInventoryEntity>();

        foreach (var cell in requiredCells)
            entityUsed.Add(cell.Entity);

        return entityUsed.ToArray();
    }

    public bool TryAdd(GameObject sender, IInventoryEntity entity)
    {
        // �������� ����� ������ � ����� �� ���������, �� �� ����������� �� �����.
        var cellWithSameEntityButNotEmpty = cells.Find(cell => !cell.IsEmpty && cell.EntityType == entity.Type && !cell.IsFilled);

        if (cellWithSameEntityButNotEmpty != null)
            return TryAddToCell(sender, cellWithSameEntityButNotEmpty, entity);

        // ���� �� ����� ���������� ������, ���� ������ ������ ��� ���������� ��������.
        var emptyCell = cells.Find(cell => cell.IsEmpty);

        if (emptyCell != null)
            return TryAddToCell(sender, emptyCell, entity);

        return false; // ���� ����� � ���������
    }

    bool TryAddToCell(GameObject sender, IInventoryCell cell, IInventoryEntity entity)
    {
        bool placed = cell.Amount + entity.Status.Amount <= entity.Info.MaxEntityLimitInInventory; // ������� ���������� �������� � ������.
        int amountToAdd = placed ? entity.Status.Amount : entity.Info.MaxEntityLimitInInventory - cell.Amount; // ������� �������� ����� �������� � ������.
        int amountExcess = entity.Status.Amount - amountToAdd; // ������� �������� ���������.

        // ������� ���� �������� � ������������� ����������, ������� ��������� � ������.
        var cloneEntity = entity.Clone();
        cloneEntity.Status.Amount = amountToAdd;

        if (cell.IsEmpty)
            cell.SetEntity(cloneEntity);
        else
            cell.Entity.Status.Amount += amountToAdd;

        Debug.Log($"�������� ���������� � ���������. ��� ��������: {entity.Type}, ����������: {amountToAdd}");
        OnInventoryEntityAddedEvent?.Invoke(sender, entity, amountToAdd);
        OnInventoryStatusChangedEvent?.Invoke(sender);

        if (amountExcess <= 0) // ���� �������� ������ ��������, ���������� �������� TryAdd, ����� ������� � ��������� ������.
            return true;

        entity.Status.Amount = amountExcess;
        return TryAdd(sender, entity);
    }

    public void TransitFromCellToCell(object sender, IInventoryCell fromCell, IInventoryCell toCell)
    {
        if (fromCell.IsEmpty || toCell.IsFilled)
            return;

        if (!toCell.IsEmpty && fromCell.EntityType != toCell.EntityType)
            return;

        var cellCapacity = fromCell.Capacity;
        var placed = fromCell.Amount + toCell.Amount <= cellCapacity;
        var amountToAdd = placed ? fromCell.Amount : cellCapacity - toCell.Amount;
        var amountExcess = fromCell.Amount - amountToAdd;

        if(toCell.IsEmpty)
        {
            toCell.SetEntity(fromCell.Entity);
            fromCell.Clear();
            OnInventoryStatusChangedEvent?.Invoke(sender);
        }

        toCell.Entity.Status.Amount += amountToAdd;

        if (placed)
            fromCell.Clear();
        else
            fromCell.Entity.Status.Amount = amountExcess;

        OnInventoryStatusChangedEvent?.Invoke(sender);
    }

    public void Remove(GameObject sender, Type entityType, int amount = 1)
    {
        var cellsWithEntity = GetAllCells(entityType);

        if (cellsWithEntity.Length == 0)
            return;

        int amountToRemove = amount; // ���������� ���������� ���������, ������� ����� �������.
        int count = cellsWithEntity.Length;

        // �������� �� ������� �������� � �����, ����� ������� ������� �� ������ � ������� �����������.
        for (int i = count - 1; i >= 0; i--)
        {
            var cell = cellsWithEntity[i];

            if (cell.Amount >= amountToRemove) // ��������� ����� �� �� ������� �� ������ ������ ���������� ��������
            {
                cell.Entity.Status.Amount -= amountToRemove;

                if (cell.Amount <= 0)
                    cell.Clear();

                Debug.Log($"�������� ������� �� ���������. ��� ��������: {entityType}, ����������: {amountToRemove}");
                OnInventoryEntityRemovedEvent?.Invoke(sender, entityType, amountToRemove);
                OnInventoryStatusChangedEvent?.Invoke(sender);

                break; // ��������� ����, ��� ��� �������� ���������.
            }

            // ���� � ������ ������ ���������, ��� ����� �������, ������� ������ � ���������� ����� ���������.
            int amountRemoved = cell.Amount;
            amountToRemove -= cell.Amount;
            cell.Clear();

            Debug.Log($"�������� ������� �� ���������. ��� ��������: {entityType}, ����������: {amountRemoved}");
            OnInventoryEntityRemovedEvent?.Invoke(sender, entityType, amountRemoved);
            OnInventoryStatusChangedEvent?.Invoke(sender);
        }
    }

    public bool HasEntity(Type type, out IInventoryEntity entity) // ����� �������� ���������� ���� � ��������� � ������������ � "entity".
    {
        entity = GetEntity(type);

        return entity != null;
    }

    public IInventoryCell[] GetAllCells(Type entityType)
    {
        return cells.FindAll(cell => !cell.IsEmpty && cell.EntityType == entityType).ToArray();
    }

    public IInventoryCell[] GetAllCells()
    {
        return cells.ToArray();
    }
}