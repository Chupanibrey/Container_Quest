using System;

[Serializable]
class InventoryEntityStatus : IInventoryEntityStatus
{
    public int entityAmount;
    public bool isEntityUsed;

    public int Amount { get => entityAmount; set => entityAmount = value; }
    public bool IsUsed { get => isEntityUsed; set => isEntityUsed = value; }

    public InventoryEntityStatus()
    {
        entityAmount = 0;
        isEntityUsed = false;
    }
}