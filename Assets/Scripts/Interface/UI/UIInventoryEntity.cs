using UnityEngine;
using UnityEngine.UI;

public class UIInventoryEntity : UIEntity
{
    [SerializeField] Image imageIcon;
    [SerializeField] Text textAmount;

    public IInventoryEntity Entity { get; private set; }

    public void Refresh(IInventoryCell cell) // ћетод Refresh обновл€ет отображение UIInventoryEntity на основе данных из €чейки инвентар€.
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

    
    void Cleanup() // ћетод Cleanup скрывает отображение иконки и текста с количеством сущности.
    {
        textAmount.gameObject.SetActive(false);
        imageIcon.gameObject.SetActive(false);
    }
}