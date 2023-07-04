using System;

[Serializable]
class InventoryEntityStatus : IInventoryEntityStatus
{
    public int entityAmount;
    public int entityCellNumber;
    public bool isEntityUsed;

    public int Amount { get => entityAmount; set => entityAmount = value; }
    public bool IsUsed { get => isEntityUsed; set => isEntityUsed = value; }
    public int CellNumber { get => entityCellNumber; set => entityCellNumber = value; }

    public InventoryEntityStatus()
    {
        entityAmount = 0;
        isEntityUsed = false;
    }
}