using System;
using UnityEngine;

public interface IInventory
{
    int Capacity { get; }
    bool IsFilled { get; }

    int GetEntityAmount(Type entityType);
    IInventoryEntity GetEntity(Type entityType); // Получить сущность определенного типа из инвентаря.
    IInventoryEntity[] GetAllEntities();
    IInventoryEntity[] GetAllEntities(Type entityType); // Получить все сущности определенного типа, содержащиеся в инвентаре.
    IInventoryEntity[] GetUsedEntities();

    bool TryAdd(object sender, IInventoryEntity entity); // Параметр "sender" используется для отслеживания источника добавления сущности.
    void Remove(object sender, Type entityType, int amount = 1); // Параметр "amount" указывает количество сущностей для удаления (по умолчанию 1). 
    bool HasEntity(Type type, out IInventoryEntity entity); // Проверяет, есть ли сущность определенного типа в инвентаре и возвращает её в случае истины метода.
}