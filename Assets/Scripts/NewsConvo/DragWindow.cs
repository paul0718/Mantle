using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Vector2 diffPoint; // Stores the offset between the mouse click and the window position
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        Cursor.lockState = CursorLockMode.Confined; // Keeps the cursor within the application window
    }

    // Called when the user presses down on the UI element
    public void OnPointerDown(PointerEventData eventData)
    {
        // Convert the mouse position to local space relative to the RectTransform
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            canvas.worldCamera,
            out diffPoint
        );
        diffPoint *= rectTransform.localScale;
    }

    // Called when the user drags the UI element
    public void OnDrag(PointerEventData eventData)
    {
        // Bring the dragged window to the front
        transform.SetSiblingIndex(transform.parent.childCount - 2);

        Vector2 localMousePos;
        // Convert the mouse position to local space relative to the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localMousePos
        );

        if (GetComponent<DesktopInteractions>().maximized)
        {
            // If the window is maximized, only allow movement along the X-axis
            rectTransform.anchoredPosition = new Vector2(localMousePos.x - diffPoint.x, rectTransform.anchoredPosition.y);
        }
        else
        {
            // Move the window while maintaining the initial offset from the cursor
            rectTransform.anchoredPosition = new Vector2(localMousePos.x - diffPoint.x, Mathf.Max(localMousePos.y-diffPoint.y, -400 - rectTransform.sizeDelta.y*rectTransform.localScale.y/2));
        }
    }
}
