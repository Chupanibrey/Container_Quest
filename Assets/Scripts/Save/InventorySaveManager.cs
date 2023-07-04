using UnityEngine;
using System.Collections.Generic;

public class InventorySaveManager : MonoBehaviour
{
    [SerializeField] InventoryEntityInfo squareInfo;
    [SerializeField] InventoryEntityInfo circleInfo;

    private const int maxCellCount = 20;

    public void SaveInventory(InventoryWithCells inventory)
    {
        var inventoryData = new List<string>();

        // Проходимся по всем ячейкам инвентаря и преобразуем данные сущностей в строки
        foreach (var cell in inventory.GetAllCells())
        {
            if (!cell.IsEmpty)
            {
                var entityInfo = cell.Entity.Info;
                var entityData = $"{entityInfo.Id},{cell.Amount},{cell.CellNumber}";
                inventoryData.Add(entityData);
            }
        }

        // Сохраняем данные в PlayerPrefs в виде строки, разделенной символом ';'
        var saveData = string.Join(";", inventoryData);
        PlayerPrefs.SetString("InventoryData", saveData);
        PlayerPrefs.Save();
    }

    public void LoadInventory(InventoryWithCells inventory)
    {
        var saveData = PlayerPrefs.GetString("InventoryData");

        if (!string.IsNullOrEmpty(saveData))
        {
            var inventoryData = saveData.Split(';');
            var allCells = inventory.GetAllCells();

            for (int i = 0; i < maxCellCount; i++)
                allCells[i].Clear();

            foreach (var entityData in inventoryData)
            {
                var entityInfoData = entityData.Split(',');
                if (entityInfoData.Length < 3)
                    continue;

                var entityId = entityInfoData[0];
                var amount = int.Parse(entityInfoData[1]);
                var cellNumber = int.Parse(entityInfoData[2]);

                var entity = CreateEntity(entityId); // Метод для получения сущности по её Id
                if (entity == null)
                    continue;

                entity.Status.Amount = amount;

                if (cellNumber >= 0 && cellNumber < maxCellCount)
                {
                    var cell = allCells[cellNumber];
                    if (cell.IsEmpty)
                        inventory.TryAddToCell(null, cell, entity);
                }
            }
        }
    }

    private IInventoryEntity CreateEntity(string entityId)
    {
        if (entityId == circleInfo.Id)
            return new Circle(circleInfo);
        else if (entityId == squareInfo.Id)
            return new Square(squareInfo);
        else
            return null;
    }
}