using UnityEngine;
using UnityEngine.EventSystems;

public class UIEntity : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    CanvasGroup canvasGroup;
    Canvas mainCanvas;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var cellTransform = rectTransform.parent; // Получаем ссылку на клетку инвентаря.
        cellTransform.SetAsLastSibling(); // Перемещаем текущую сущность в конец списка объектов, сущность будет отображаться поверх них.
        canvasGroup.blocksRaycasts = false; // Блокируем перехват событий мыши этой сущностью, чтобы можно было попасть рейкастом в клетку.
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor; // Компенсация масштабирования холста, перемещение вне зависимости от масштаба
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;// Возвращаем сущность на начальную позицию.
        canvasGroup.blocksRaycasts = true; // Разблокируем перехват событий мыши на этой сущности после окончания перетаскивания.
    }
}