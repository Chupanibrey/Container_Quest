using UnityEngine;
using UnityEngine.EventSystems;

public class UICell : MonoBehaviour, IDropHandler
{
    public virtual void OnDrop(PointerEventData eventData)
    {
        var otherEntityTransform = eventData.pointerDrag.transform;
        otherEntityTransform.SetParent(transform); // ��������� �������� ������ ��������� ������
        otherEntityTransform.localPosition = Vector3.zero; // �������� ��������� ������� �������� ������������ ������.
    }
}