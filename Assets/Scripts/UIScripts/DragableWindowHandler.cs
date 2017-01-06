using UnityEngine;
using UnityEngine.EventSystems;

public class DragableWindowHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static RectTransform windowBeingDragged;
    Vector3 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        windowBeingDragged = gameObject.GetComponent<RectTransform>();
        offset = windowBeingDragged.anchoredPosition - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        windowBeingDragged.anchoredPosition = Input.mousePosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        windowBeingDragged.anchoredPosition = Input.mousePosition + offset;
        windowBeingDragged = null;
    }
}
