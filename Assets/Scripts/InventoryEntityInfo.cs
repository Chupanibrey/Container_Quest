using UnityEngine;

[CreateAssetMenu(fileName ="InventoryEntityInfo", menuName ="Gameplay/Entity/Create new EntityInfo")]
class InventoryEntityInfo : ScriptableObject, IInventoryEntityInfo
{
    [SerializeField] string id;
    [SerializeField] string title;
    [SerializeField] string description;
    [SerializeField] int maxEntityLimitInInventory;
    [SerializeField] Sprite spriteIcon;

    public string Id => id;

    public string Title => title;

    public string Description => description;

    public int MaxEntityLimitInInventory => maxEntityLimitInInventory;

    public Sprite SpriteIcon => spriteIcon;
}