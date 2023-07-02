using System;

public class InventoryCell : IInventoryCell
{
    public int Amount => IsEmpty ? 0 : Entity.Status.Amount;

    public int Capacity { get; private set; }

    public bool IsFilled => !IsEmpty && Amount == Capacity;

    public bool IsEmpty => Entity == null;

    public Type EntityType => Entity?.Type;

    public IInventoryEntity Entity { get; private set; }

    // Присваивание сушности к клетке
    public void SetEntity(IInventoryEntity entity)
    {
        // избегаем перезаписи
        if (!IsEmpty)
            return;

        this.Entity = entity;
        this.Capacity = entity.Info.MaxEntityLimitInInventory;
    }

    public void Clear()
    {
        if (IsEmpty)
            return;

        Entity.Status.Amount = 0;
        Entity = null;
    }
}