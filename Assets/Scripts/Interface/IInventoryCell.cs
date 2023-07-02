using System;

public interface IInventoryCell
{
    int amount { get; }
    int capacity { get; }
    bool isFilled { get; } // Проверяет, заполнена ли ячейка хотя бы одной сущностью.
    bool isEmpty { get; }
    Type entityType { get; } // Синтаксический сахар, чтобы не вызывать метод GetType()

    IInventoryEntity entity { get; }

    void SetEntity(IInventoryEntity entity);
    void Clear();
}