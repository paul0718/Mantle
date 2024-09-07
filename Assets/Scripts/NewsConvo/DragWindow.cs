using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class DragWindow : MonoBehaviour, IDragHandler
{
    private Vector2 mousePos = new Vector2();
    private Vector2 startPos = new Vector2();
    private Vector2 diffPoint = new Vector2();

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UpdateMousePos();
        }

        if (Input.GetMouseButtonDown(0))
        {
            UpdateStartPos();
            UpdateDiffPoint();
        }
    }

    void UpdateMousePos()
    {
        mousePos.x = Input.mousePosition.x;
        mousePos.y = Input.mousePosition.y;
    }

    void UpdateStartPos()
    {
        startPos.x = GetComponent<RectTransform>().position.x;
        startPos.y = GetComponent<RectTransform>().position.y;
    }

    void UpdateDiffPoint()
    {
        diffPoint = mousePos - startPos;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
        GetComponent<RectTransform>().position = mousePos - diffPoint;
    }
}
