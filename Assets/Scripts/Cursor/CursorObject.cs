using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.StartCursorAnimation();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.StopCursorAnimation();
    }


}
