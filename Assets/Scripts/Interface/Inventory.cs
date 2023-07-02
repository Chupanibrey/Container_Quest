using System;
using UnityEngine;

public interface Inventory
{
    int capacity { get; }
    bool isFilled { get; }

    int GetEntityAmount(Type entityType);
    InventoryEntity GetEntity(Type entityType); // Получить сущность определенного типа из инвентаря.
    InventoryEntity[] GetAllEntities();
    InventoryEntity[] GetAllEntities(Type entityType); // Получить все сущности определенного типа, содержащиеся в инвентаре.
    InventoryEntity[] GetUsedEntities();

    bool Add(GameObject sender, InventoryEntity entity); // Параметр "sender" используется для отслеживания источника добавления сущности.
    void Remove(GameObject sender, Type entityType, int amount = 1); // Параметр "amount" указывает количество сущностей для удаления (по умолчанию 1). 
    bool HasEntity(Type type, out InventoryEntity entity); // Проверяет, есть ли сущность определенного типа в инвентаре и возвращает её в случае истины метода.
}