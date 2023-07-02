using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryWithCells : IInventory
{
    // ������� ���������� � �������� �������� �� ���������.
    public event Action<GameObject, IInventoryEntity, int> OnInventoryEntityAddedEvent;
    public event Action<GameObject, Type, int> OnInventoryEntityRemovedEvent;

    public int capacity { get; set; }
    public bool isFilled => cells.All(cell => cell.isFilled);

    List<IInventoryCell> cells; // ��� ������ ���������

    public InventoryWithCells(int capacity)
    {
        this.capacity = capacity;
        cells = new List<IInventoryCell>(capacity);

        for (int i = 0; i < capacity; i++)
            cells.Add(new InventoryCell());
    }

    public IInventoryEntity[] GetAllEntities() // �������� ������ ���� ��������� � ��������� (��� ����� ������ ������).
    {
        var allEntity = new List<IInventoryEntity>();

        foreach (var cell in cells)
            if (!cell.isEmpty)
                allEntity.Add(cell.entity);

        return allEntity.ToArray();
    } 

    public IInventoryEntity[] GetAllEntities(Type entityType) // �������� ������ ���� ��������� ���������� ���� � ��������� (��� ����� ������ ������).
    {
        var allEntityOfType = new List<IInventoryEntity>();
        var cellsOfType = cells.FindAll(cell => !cell.isEmpty && cell.entityType == entityType);

        foreach (var cell in cellsOfType)
            allEntityOfType.Add(cell.entity);

        return allEntityOfType.ToArray();
    } 

    public IInventoryEntity GetEntity(Type entityType) // �������� �������� ���������� ���� �� ���������.
    {
        return cells.Find(cell => cell.entityType == entityType).entity;
    }

    public int GetEntityAmount(Type entityType) // �������� ����� ���������� ��������� ���������� ���� � ���������.
    {
        int amount = 0;
        var allEntityCells = cells.FindAll(cell => !cell.isEmpty && cell.entityType == entityType);

        foreach (var entityCell in allEntityCells)
            amount += entityCell.amount;

        return amount;
    }

    public IInventoryEntity[] GetUsedEntities() // �������� ������ ������������ ���������.
    {
        var requiredCells = cells.FindAll(cell => !cell.isEmpty && cell.entity.isUsed);
        var entityUsed = new List<IInventoryEntity>();

        foreach (var cell in requiredCells)
            entityUsed.Add(cell.entity);

        return entityUsed.ToArray();
    }

    public bool TryAdd(GameObject sender, IInventoryEntity entity)
    {
        // �������� ����� ������ � ����� �� ���������, �� �� ����������� �� �����.
        var cellWithSameEntityButNotEmpty = cells.Find(cell => !cell.isEmpty && cell.entityType == entity.type && !cell.isFilled);

        if (cellWithSameEntityButNotEmpty != null)
            return TryAddToCell(sender, cellWithSameEntityButNotEmpty, entity);

        // ���� �� ����� ���������� ������, ���� ������ ������ ��� ���������� ��������.
        var emptyCell = cells.Find(cell => cell.isEmpty);

        if (emptyCell != null)
            return TryAddToCell(sender, emptyCell, entity);

        return false; // ���� ����� � ���������
    }

    bool TryAddToCell(GameObject sender, IInventoryCell cell, IInventoryEntity entity)
    {
        bool placed = cell.amount + entity.amount <= entity.maxEntityLimitInInventory; // ������� ���������� �������� � ������.
        int amountToAdd = placed ? entity.amount : entity.maxEntityLimitInInventory - cell.amount; // ������� �������� ����� �������� � ������.
        int amountExcess = entity.amount - amountToAdd; // ������� �������� ���������.

        // ������� ���� �������� � ������������� ����������, ������� ��������� � ������.
        var cloneEntity = entity.Clone();
        cloneEntity.amount = amountToAdd;

        if (cell.isEmpty)
            cell.SetEntity(cloneEntity);
        else
            cell.entity.amount += amountToAdd;

        Debug.Log($"�������� ���������� � ���������. ��� ��������: {entity.type}, ����������: {amountToAdd}");
        OnInventoryEntityAddedEvent?.Invoke(sender, entity, amountToAdd);

        if (amountExcess <= 0) // ���� �������� ������ ��������, ���������� �������� TryAdd, ����� ������� � ��������� ������.
            return true;

        entity.amount = amountExcess;
        return TryAdd(sender, entity);
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

            if (cell.amount >= amountToRemove) // ��������� ����� �� �� ������� �� ������ ������ ���������� ��������
            {
                cell.entity.amount -= amountToRemove;

                if (cell.amount <= 0)
                    cell.Clear();

                Debug.Log($"�������� ������� �� ���������. ��� ��������: {entityType}, ����������: {amountToRemove}");
                OnInventoryEntityRemovedEvent?.Invoke(sender, entityType, amountToRemove);

                break; // ��������� ����, ��� ��� �������� ���������.
            }

            // ���� � ������ ������ ���������, ��� ����� �������, ������� ������ � ���������� ����� ���������.
            int amountRemoved = cell.amount;
            amountToRemove -= cell.amount;
            cell.Clear();

            Debug.Log($"�������� ������� �� ���������. ��� ��������: {entityType}, ����������: {amountRemoved}");
            OnInventoryEntityRemovedEvent?.Invoke(sender, entityType, amountRemoved);
        }
    }

    public bool HasEntity(Type type, out IInventoryEntity entity) // ����� �������� ���������� ���� � ��������� � ������������ � "entity".
    {
        entity = GetEntity(type);

        return entity != null;
    }

    public IInventoryCell[] GetAllCells(Type entityType)
    {
        return cells.FindAll(cell => !cell.isEmpty && cell.entityType == entityType).ToArray();
    }

    public IInventoryCell[] GetAllCells()
    {
        return cells.ToArray();
    }
}