using System;

public interface InventoryCell
{
    int amount { get; }
    int capacity { get; }
    bool isFilled { get; } // Проверяет, заполнена ли ячейка хотя бы одной сущностью.
    bool isEmpty { get; }
    Type entityType { get; } // Синтаксический сахар, чтобы не вызывать метод GetType()

    InventoryEntity entity { get; }

    void SetEntity(InventoryEntity entity);
    void Clear();
}