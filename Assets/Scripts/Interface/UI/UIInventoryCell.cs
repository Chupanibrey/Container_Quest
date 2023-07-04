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
        // Получаем информацию о перетаскиваемой сущности из события.
        var otherEntityUI = eventData.pointerDrag.GetComponent<UIInventoryEntity>();
        var otherCellUI = otherEntityUI.GetComponentInParent<UIInventoryCell>();
        // Получаем ячейку, из которой была перемещена сущность, и саму ячейку, куда перемещена сущность.
        var otherCell = otherCellUI.Cell;
        var inventory = UIInventory.inventory;

        inventory.TransitFromCellToCell(this, otherCell, Cell);

        // После перемещения обновляем отображение текущей и другой ячеек.
        Refresh();
        otherCellUI.Refresh();
    }

    public void Refresh()
    {
        if (Cell != null)
            uiInventoryEntity.Refresh(Cell);
    }
}