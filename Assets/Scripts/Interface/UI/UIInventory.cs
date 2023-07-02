 using UnityEngine;

class UIInventory : MonoBehaviour
{
    public InventoryWithCells inventory { get; private set; }

    void Awale()
    {
        inventory = new InventoryWithCells(15);
    }
}