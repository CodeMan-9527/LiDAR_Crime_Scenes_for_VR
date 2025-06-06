using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Makes a UI panel draggable by the user.
/// Attach to the root RectTransform GameObject of the menu.
/// Requires the panel to have a Graphic (e.g., Image) or a CanvasGroup blocking raycasts to receive pointer events.
/// </summary>
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private Vector2 pointerOffset;
    private RectTransform canvasRectTransform;
    private RectTransform panelRectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        panelRectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
            canvasRectTransform = parentCanvas.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Called when drag starts; stores initial pointer offset.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset);
    }

    /// <summary>
    /// Called during drag; moves panel accordingly.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (panelRectTransform == null || parentCanvas == null) return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {
            panelRectTransform.localPosition = localPointerPosition - pointerOffset;
        }
    }
}

