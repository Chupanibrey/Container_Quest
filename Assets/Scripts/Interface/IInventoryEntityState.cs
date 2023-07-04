public interface IInventoryEntityStatus
{
    int Amount { get; set; }
    bool IsUsed { get; set; }
    int CellNumber { get; set; }
}