using UnityEngine;

public interface IInventoryEntityInfo
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    int MaxEntityLimitInInventory { get; }
    Sprite SpriteIcon { get; }
}