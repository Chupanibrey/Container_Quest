using System;

public interface IInventoryEntity
{
    IInventoryEntityInfo Info { get; }
    IInventoryEntityStatus Status { get; }
    Type Type { get; } // Синтаксический сахар, чтобы не вызывать метод GetType()

    IInventoryEntity Clone(); // Создает и возвращает копию этой сущности.
}