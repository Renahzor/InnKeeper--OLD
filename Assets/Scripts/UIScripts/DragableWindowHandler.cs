using UnityEngine;
using UnityEngine.EventSystems;

public class DragableWindowHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform windowBeingDragged;
    RectTransform canvasPosition;
    Vector2 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        windowBeingDragged = gameObject.GetComponent<RectTransform>();
        canvasPosition = gameObject.GetComponentInParent<Canvas>().transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(windowBeingDragged, eventData.position, eventData.pressEventCamera, out offset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasPosition, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            windowBeingDragged.localPosition = localPointerPosition - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {        
        windowBeingDragged = null;
    }
}
