using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventoryCell : UICell
{
    [SerializeField] UIInventoryEntity uiInventoryEntity;

    public IInventoryCell Cell { get; private set; }

    UIInventory UIInventory;

    void Awake()
    {
        UIInventory = GetComponentInParent<UIInventory>();
    }

    public void SetCell(IInventoryCell newCell)
    {
        Cell = newCell;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        var otherEntityUI = eventData.pointerDrag.GetComponent<UIInventoryEntity>();
        var otherCellUI = otherEntityUI.GetComponentInParent<UIInventoryCell>();
        var otherCell = otherCellUI.Cell;
        var inventory = UIInventory.inventory;

        inventory.TransitFromCellToCell(this, otherCell, Cell);

        Refresh();
        otherCellUI.Refresh();
    }

    public void Refresh()
    {
        if (Cell != null)
            uiInventoryEntity.Refresh(Cell);
    }
}