using UnityEngine;
using UnityEngine.UI;

public class UIInventoryEntity : UIEntity
{
    [SerializeField] Image imageIcon;
    [SerializeField] Text textAmount;

    public IInventoryEntity Entity { get; private set; }

    public void Refresh(IInventoryCell cell)
    {
        if (cell.IsEmpty)
        {
            Cleanup();
            return;
        }

        Entity = cell.Entity;
        imageIcon.sprite = Entity.Info.SpriteIcon;
        imageIcon.gameObject.SetActive(true);

        var textAmountEnabled = cell.Amount > 1;
        textAmount.gameObject.SetActive(textAmountEnabled);

        if (textAmountEnabled)
            textAmount.text = $"x{cell.Amount.ToString()}";
    }

    void Cleanup()
    {
        textAmount.gameObject.SetActive(false);
        imageIcon.gameObject.SetActive(false);
    }
}