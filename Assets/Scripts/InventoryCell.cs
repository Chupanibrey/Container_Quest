using System;

public class InventoryCell : IInventoryCell
{
    public int amount => isEmpty ? 0 : entity.amount;

    public int capacity { get; private set; }

    public bool isFilled => amount == capacity;

    public bool isEmpty => entity == null;

    public Type entityType => entity?.type;

    public IInventoryEntity entity { get; private set; }

    // Присваивание сушности к клетке
    public void SetEntity(IInventoryEntity entity)
    {
        // избегаем перезаписи
        if (!isEmpty)
            return;

        this.entity = entity;
        this.capacity = entity.maxEntityLimitInInventory;
    }

    public void Clear()
    {
        if (isEmpty)
            return;

        entity.amount = 0;
        entity = null;
    }
}