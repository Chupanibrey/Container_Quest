using System;

public class Ball : IInventoryEntity
{
    public IInventoryEntityInfo Info { get; }

    public IInventoryEntityStatus Status { get; }

    public Type Type => GetType();

    public Ball(IInventoryEntityInfo info)
    {
        this.Info = info;
        Status = new InventoryEntityStatus();
    }

    public IInventoryEntity Clone()
    {
        var clonedBall = new Ball(Info);
        clonedBall.Status.Amount = Status.Amount;
        return clonedBall;
    }
}