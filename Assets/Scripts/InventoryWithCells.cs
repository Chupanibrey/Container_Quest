using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryWithCells : IInventory
{
    // События добавления и удаления сущности из инвентаря.
    public event Action<GameObject, IInventoryEntity, int> OnInventoryEntityAddedEvent;
    public event Action<GameObject, Type, int> OnInventoryEntityRemovedEvent;

    public int capacity { get; set; }
    public bool isFilled => cells.All(cell => cell.isFilled);

    List<IInventoryCell> cells; // Все клетки инвентаря

    public InventoryWithCells(int capacity)
    {
        this.capacity = capacity;
        cells = new List<IInventoryCell>(capacity);

        for (int i = 0; i < capacity; i++)
            cells.Add(new InventoryCell());
    }

    public IInventoryEntity[] GetAllEntities() // Получить массив всех сущностей в инвентаре (без учета пустых клеток).
    {
        var allEntity = new List<IInventoryEntity>();

        foreach (var cell in cells)
            if (!cell.isEmpty)
                allEntity.Add(cell.entity);

        return allEntity.ToArray();
    } 

    public IInventoryEntity[] GetAllEntities(Type entityType) // Получить массив всех сущностей указанного типа в инвентаре (без учета пустых клеток).
    {
        var allEntityOfType = new List<IInventoryEntity>();
        var cellsOfType = cells.FindAll(cell => !cell.isEmpty && cell.entityType == entityType);

        foreach (var cell in cellsOfType)
            allEntityOfType.Add(cell.entity);

        return allEntityOfType.ToArray();
    } 

    public IInventoryEntity GetEntity(Type entityType) // Получить сущность указанного типа из инвентаря.
    {
        return cells.Find(cell => cell.entityType == entityType).entity;
    }

    public int GetEntityAmount(Type entityType) // Получить общее количество сущностей указанного типа в инвентаре.
    {
        int amount = 0;
        var allEntityCells = cells.FindAll(cell => !cell.isEmpty && cell.entityType == entityType);

        foreach (var entityCell in allEntityCells)
            amount += entityCell.amount;

        return amount;
    }

    public IInventoryEntity[] GetUsedEntities() // Получить массив используемых сущностей.
    {
        var requiredCells = cells.FindAll(cell => !cell.isEmpty && cell.entity.isUsed);
        var entityUsed = new List<IInventoryEntity>();

        foreach (var cell in requiredCells)
            entityUsed.Add(cell.entity);

        return entityUsed.ToArray();
    }

    public bool TryAdd(GameObject sender, IInventoryEntity entity)
    {
        // Пытаемся найти клетку с такой же сущностью, но не заполненную до конца.
        var cellWithSameEntityButNotEmpty = cells.Find(cell => !cell.isEmpty && cell.entityType == entity.type && !cell.isFilled);

        if (cellWithSameEntityButNotEmpty != null)
            return TryAddToCell(sender, cellWithSameEntityButNotEmpty, entity);

        // Если не нашли подходящую клетку, ищем пустую клетку для добавления сущности.
        var emptyCell = cells.Find(cell => cell.isEmpty);

        if (emptyCell != null)
            return TryAddToCell(sender, emptyCell, entity);

        return false; // Нету места в инвентаре
    }

    bool TryAddToCell(GameObject sender, IInventoryCell cell, IInventoryEntity entity)
    {
        bool placed = cell.amount + entity.amount <= entity.maxEntityLimitInInventory; // Сколько помещается сущности в клетку.
        int amountToAdd = placed ? entity.amount : entity.maxEntityLimitInInventory - cell.amount; // Сколько сущности можно добавить в клетку.
        int amountExcess = entity.amount - amountToAdd; // Сколько сущности останется.

        // Создаем клон сущности и устанавливаем количество, которое добавляем в клетку.
        var cloneEntity = entity.Clone();
        cloneEntity.amount = amountToAdd;

        if (cell.isEmpty)
            cell.SetEntity(cloneEntity);
        else
            cell.entity.amount += amountToAdd;

        Debug.Log($"Сущность добавленна в инвентарь. Тип сущности: {entity.type}, количество: {amountToAdd}");
        OnInventoryEntityAddedEvent?.Invoke(sender, entity, amountToAdd);

        if (amountExcess <= 0) // Если осталась лишняя сущность, рекурсивно вызываем TryAdd, чтобы перейти в следующую клетку.
            return true;

        entity.amount = amountExcess;
        return TryAdd(sender, entity);
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

            if (cell.amount >= amountToRemove) // Проверяем можем ли мы удалить из клетки данное количество сущности
            {
                cell.entity.amount -= amountToRemove;

                if (cell.amount <= 0)
                    cell.Clear();

                Debug.Log($"Сущность удалена из инвентаря. Тип сушности: {entityType}, количество: {amountToRemove}");
                OnInventoryEntityRemovedEvent?.Invoke(sender, entityType, amountToRemove);

                break; // Завершаем цикл, так как удаление выполнено.
            }

            // Если в клетке меньше сущностей, чем нужно удалить, очищаем клетку и продолжаем поиск следующей.
            int amountRemoved = cell.amount;
            amountToRemove -= cell.amount;
            cell.Clear();

            Debug.Log($"Сущность удалена из инвентаря. Тип сушности: {entityType}, количество: {amountRemoved}");
            OnInventoryEntityRemovedEvent?.Invoke(sender, entityType, amountRemoved);
        }
    }

    public bool HasEntity(Type type, out IInventoryEntity entity) // Поиск сущности указанного типа в инвентаре и присваивание её "entity".
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