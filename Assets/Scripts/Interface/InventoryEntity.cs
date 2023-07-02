using System;

public interface InventoryEntity
{
    bool isUsed { get; set; }
    int maxEntityLimitInInventory { get; }
    int amount { get; set; }
    Type type { get; } // Синтаксический сахар, чтобы не вызывать метод GetType()

    InventoryEntity Clone(); // Создает и возвращает копию этой сущности.
}