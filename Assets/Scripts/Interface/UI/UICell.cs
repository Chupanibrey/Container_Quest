using UnityEngine;
using UnityEngine.EventSystems;

public class UICell : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var otherEntityTransform = eventData.pointerDrag.transform;
        otherEntityTransform.SetParent(transform); // Переносим сущность внутрь выбранной клетки
        otherEntityTransform.localPosition = Vector3.zero; // Обнуляем локальную позицию сущности относительно клетки.
    }
}