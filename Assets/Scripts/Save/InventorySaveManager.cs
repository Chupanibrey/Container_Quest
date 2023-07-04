using UnityEngine;
using System.Collections.Generic;

public class InventorySaveManager : MonoBehaviour
{
    [SerializeField] InventoryEntityInfo squareInfo;
    [SerializeField] InventoryEntityInfo circleInfo;

    public void SaveInventory(InventoryWithCells inventory)
    {
        var inventoryData = new List<string>();
        var inventoryMassive = inventory.GetAllCells();
        var cointCells = inventory.GetAllCells().Length;


        // ���������� �� ���� ������� ��������� � ����������� ������ ��������� � ������
        for(int i = 0; i < cointCells; i++)
        {
            if (!inventoryMassive[i].IsEmpty)
            {
                var entityInfo = inventoryMassive[i].Entity.Info;
                var entityData = $"{entityInfo.Id},{inventoryMassive[i].Amount},{i}";
                inventoryData.Add(entityData);
            }
        }

        // ��������� ������ � PlayerPrefs � ���� ������, ����������� �������� ';'
        var saveData = string.Join(";", inventoryData.ToArray());
        PlayerPrefs.SetString("InventoryData", saveData);
        PlayerPrefs.Save();
    }

    // ����� ��� �������� ��������� �� PlayerPrefs
    public void LoadInventory(InventoryWithCells inventory)
    {
        var saveData = PlayerPrefs.GetString("InventoryData");

        if (!string.IsNullOrEmpty(saveData))
        {
            var inventoryData = saveData.Split(';');

            foreach(var cell in inventory.GetAllCells())
                cell.Clear();

            foreach (var entityData in inventoryData)
            {
                var entityInfoData = entityData.Split(',');
                var entityId = entityInfoData[0];
                var amount = int.Parse(entityInfoData[1]);
                var cellNumber = int.Parse(entityInfoData[2]);

                var entity = CreateEntity(entityId); // ����� ��� ��������� �������� �� � Id
                entity.Status.Amount = amount;

                if (entity != null)
                {
                    // ������ ������ ������ ������ � ������� � �� ��������
                    var allCell = inventory.GetAllCells();

                    if (allCell != null)
                        inventory.TryAddToCell(null, allCell[cellNumber], entity);
                }
            }
        }
    }

    IInventoryEntity CreateEntity(string entityInfo)
    {
        if (entityInfo == circleInfo.Id)
            return new Circle(circleInfo);
        else if (entityInfo == squareInfo.Id)
            return new Square(squareInfo);
        else
            return null;
    }
}