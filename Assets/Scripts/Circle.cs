using System;

public class Circle : IInventoryEntity
{
    public IInventoryEntityInfo Info { get; }

    public IInventoryEntityStatus Status { get; }

    public Type Type => GetType();

    public Circle(IInventoryEntityInfo info)
    {
        this.Info = info;
        Status = new InventoryEntityStatus();
    }

    public IInventoryEntity Clone()
    {
        var clonedCircle = new Circle(Info);
        clonedCircle.Status.Amount = Status.Amount;
        return clonedCircle;
    }
}