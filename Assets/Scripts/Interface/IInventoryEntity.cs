using System;

public interface IInventoryEntity
{
    bool isUsed { get; set; }
    int maxEntityLimitInInventory { get; }
    int amount { get; set; }
    Type type { get; } // Синтаксический сахар, чтобы не вызывать метод GetType()

    IInventoryEntity Clone(); // Создает и возвращает копию этой сущности.
}