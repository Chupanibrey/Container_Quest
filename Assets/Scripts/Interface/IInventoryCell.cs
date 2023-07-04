using System;

public interface IInventoryCell
{
    int Amount { get; }
    int Capacity { get; }
    bool IsFilled { get; } // Проверяет, заполнена ли ячейка хотя бы одной сущностью.
    bool IsEmpty { get; }
    Type EntityType { get; } // Синтаксический сахар, чтобы не вызывать метод GetType()
    int CellNumber { get; set; }

    IInventoryEntity Entity { get; }

    void SetEntity(IInventoryEntity entity);
    void Clear();
}