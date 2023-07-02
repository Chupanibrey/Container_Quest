 using UnityEngine;

class UIInventory : MonoBehaviour
{
    [SerializeField] InventoryEntityInfo squareInfo;
    [SerializeField] InventoryEntityInfo circleInfo;
    UIInventoryRandomizer randomazer;

    public InventoryWithCells inventory => randomazer.inventory;

    void Start()
    {
        var uiCells = GetComponentsInChildren<UIInventoryCell>();
        randomazer = new UIInventoryRandomizer(squareInfo, circleInfo, uiCells);
        randomazer.FillCells();
    }
}