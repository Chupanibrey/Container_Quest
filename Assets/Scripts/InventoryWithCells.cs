using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryWithCells : IInventory
{
    // События добавления и удаления сущности из инвентаря.
    public event Action<object, IInventoryEntity, int> OnInventoryEntityAddedEvent;
    public event Action<object, Type, int> OnInventoryEntityRemovedEvent;
    public event Action<object> OnInventoryStatusChangedEvent;

    public int Capacity { get; set; }
    public bool IsFilled => cells.All(cell => cell.IsFilled);

    List<IInventoryCell> cells; // Все клетки инвентаря

    public InventoryWithCells(int capacity)
    {
        this.Capacity = capacity;
        cells = new List<IInventoryCell>(capacity);

        for (int i = 0; i < capacity; i++)
            cells.Add(new InventoryCell());
    }

    public IInventoryEntity[] GetAllEntities() // Получить массив всех сущностей в инвентаре (без учета пустых клеток).
    {
        var allEntity = new List<IInventoryEntity>();

        foreach (var cell in cells)
            if (!cell.IsEmpty)
                allEntity.Add(cell.Entity);

        return allEntity.ToArray();
    } 

    public IInventoryEntity[] GetAllEntities(Type entityType) // Получить массив всех сущностей указанного типа в инвентаре (без учета пустых клеток).
    {
        var allEntityOfType = new List<IInventoryEntity>();
        var cellsOfType = cells.FindAll(cell => !cell.IsEmpty && cell.EntityType == entityType);

        foreach (var cell in cellsOfType)
            allEntityOfType.Add(cell.Entity);

        return allEntityOfType.ToArray();
    } 

    public IInventoryEntity GetEntity(Type entityType) // Получить сущность указанного типа из инвентаря.
    {
        return cells.Find(cell => cell.EntityType == entityType).Entity;
    }

    public int GetEntityAmount(Type entityType) // Получить общее количество сущностей указанного типа в инвентаре.
    {
        int amount = 0;
        var allEntityCells = cells.FindAll(cell => !cell.IsEmpty && cell.EntityType == entityType);

        foreach (var entityCell in allEntityCells)
            amount += entityCell.Amount;

        return amount;
    }

    public IInventoryEntity[] GetUsedEntities() // Получить массив используемых сущностей.
    {
        var requiredCells = cells.FindAll(cell => !cell.IsEmpty && cell.Entity.Status.IsUsed);
        var entityUsed = new List<IInventoryEntity>();

        foreach (var cell in requiredCells)
            entityUsed.Add(cell.Entity);

        return entityUsed.ToArray();
    }

    public bool TryAdd(GameObject sender, IInventoryEntity entity)
    {
        // Пытаемся найти клетку с такой же сущностью, но не заполненную до конца.
        var cellWithSameEntityButNotEmpty = cells.Find(cell => !cell.IsEmpty && cell.EntityType == entity.Type && !cell.IsFilled);

        if (cellWithSameEntityButNotEmpty != null)
            return TryAddToCell(sender, cellWithSameEntityButNotEmpty, entity);

        // Если не нашли подходящую клетку, ищем пустую клетку для добавления сущности.
        var emptyCell = cells.Find(cell => cell.IsEmpty);

        if (emptyCell != null)
            return TryAddToCell(sender, emptyCell, entity);

        return false; // Нету места в инвентаре
    }

    bool TryAddToCell(GameObject sender, IInventoryCell cell, IInventoryEntity entity)
    {
        bool placed = cell.Amount + entity.Status.Amount <= entity.Info.MaxEntityLimitInInventory; // Сколько помещается сущности в клетку.
        int amountToAdd = placed ? entity.Status.Amount : entity.Info.MaxEntityLimitInInventory - cell.Amount; // Сколько сущности можно добавить в клетку.
        int amountExcess = entity.Status.Amount - amountToAdd; // Сколько сущности останется.

        // Создаем клон сущности и устанавливаем количество, которое добавляем в клетку.
        var cloneEntity = entity.Clone();
        cloneEntity.Status.Amount = amountToAdd;

        if (cell.IsEmpty)
            cell.SetEntity(cloneEntity);
        else
            cell.Entity.Status.Amount += amountToAdd;

        Debug.Log($"Сущность добавленна в инвентарь. Тип сущности: {entity.Type}, количество: {amountToAdd}");
        OnInventoryEntityAddedEvent?.Invoke(sender, entity, amountToAdd);
        OnInventoryStatusChangedEvent?.Invoke(sender);

        if (amountExcess <= 0) // Если осталась лишняя сущность, рекурсивно вызываем TryAdd, чтобы перейти в следующую клетку.
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

        int amountToRemove = amount; // Определяем количество сущностей, которое нужно удалить.
        int count = cellsWithEntity.Length;

        // Проходим по клеткам сущности с конца, чтобы удалить сначала из клеток с большим количеством.
        for (int i = count - 1; i >= 0; i--)
        {
            var cell = cellsWithEntity[i];

            if (cell.Amount >= amountToRemove) // Проверяем можем ли мы удалить из клетки данное количество сущности
            {
                cell.Entity.Status.Amount -= amountToRemove;

                if (cell.Amount <= 0)
                    cell.Clear();

                Debug.Log($"Сущность удалена из инвентаря. Тип сушности: {entityType}, количество: {amountToRemove}");
                OnInventoryEntityRemovedEvent?.Invoke(sender, entityType, amountToRemove);
                OnInventoryStatusChangedEvent?.Invoke(sender);

                break; // Завершаем цикл, так как удаление выполнено.
            }

            // Если в клетке меньше сущностей, чем нужно удалить, очищаем клетку и продолжаем поиск следующей.
            int amountRemoved = cell.Amount;
            amountToRemove -= cell.Amount;
            cell.Clear();

            Debug.Log($"Сущность удалена из инвентаря. Тип сушности: {entityType}, количество: {amountRemoved}");
            OnInventoryEntityRemovedEvent?.Invoke(sender, entityType, amountRemoved);
            OnInventoryStatusChangedEvent?.Invoke(sender);
        }
    }

    public bool HasEntity(Type type, out IInventoryEntity entity) // Поиск сущности указанного типа в инвентаре и присваивание её "entity".
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