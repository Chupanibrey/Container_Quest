using System;

class Square : IInventoryEntity
{
    public IInventoryEntityInfo Info { get; }

    public IInventoryEntityStatus Status { get; }

    public Type Type => GetType();

    public Square(IInventoryEntityInfo info)
    {
        this.Info = info;
        Status = new InventoryEntityStatus();
    }

    public IInventoryEntity Clone()
    {
        var clonedSquare = new Square(Info);
        clonedSquare.Status.Amount = Status.Amount;
        return clonedSquare;
    }
}