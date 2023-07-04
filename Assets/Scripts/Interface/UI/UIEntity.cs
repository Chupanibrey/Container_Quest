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
        var cellTransform = rectTransform.parent; // �������� ������ �� ������ ���������.
        cellTransform.SetAsLastSibling(); // ���������� ������� �������� � ����� ������ ��������, �������� ����� ������������ ������ ���.
        canvasGroup.blocksRaycasts = false; // ��������� �������� ������� ���� ���� ���������, ����� ����� ���� ������� ��������� � ������.
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor; // ����������� ��������������� ������, ����������� ��� ����������� �� ��������
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;// ���������� �������� �� ��������� �������.
        canvasGroup.blocksRaycasts = true; // ������������ �������� ������� ���� �� ���� �������� ����� ��������� ��������������.
    }
}