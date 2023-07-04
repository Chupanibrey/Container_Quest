 using UnityEngine;

class UIInventory : MonoBehaviour
{
    [SerializeField] InventoryEntityInfo squareInfo;
    [SerializeField] InventoryEntityInfo circleInfo;
    InventorySaveManager saveManager;
    UIInventoryRandomizer randomazer;

    public InventoryWithCells inventory => randomazer.inventory;

    void Start()
    {
        saveManager = GetComponent<InventorySaveManager>();
        var uiCells = GetComponentsInChildren<UIInventoryCell>(); 
        randomazer = new UIInventoryRandomizer(squareInfo, circleInfo, uiCells);
        randomazer.FillCells(); // Заполняем ячейки инвентаря случайными сущностями

        // Загрузка инвентаря при старте игры
        saveManager.LoadInventory(inventory);
    }

    public void SaveInventory()
    {
        saveManager.SaveInventory(inventory);
    }
}