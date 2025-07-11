using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    /*
     Arm
        -Target
        -End
        -Mid
        -Root
     */
    
    [SerializeField] private Transform end;
    private Arm arm;
    
    private float xMin, xMax, yMin, yMax;

    void Start()
    {
        arm = transform.parent.GetComponent<Arm>();
        
        RectTransform UICanvasRectTrans =  GameObject.Find("UICanvas").GetComponent<RectTransform>();
        xMin = UICanvasRectTrans.position.x + UICanvasRectTrans.rect.xMin * UICanvasRectTrans.transform.lossyScale.x;
        xMax = UICanvasRectTrans.position.x + UICanvasRectTrans.rect.xMax * UICanvasRectTrans.transform.lossyScale.x;
        yMin = UICanvasRectTrans.position.y + 0.5f;
        yMax = UICanvasRectTrans.position.y + UICanvasRectTrans.rect.yMax * UICanvasRectTrans.transform.lossyScale.y;
    }
    
    private bool IsOutOfBox()
    {
        float xWorld = transform.position.x;
        float yWorld = transform.position.y;
        return xWorld > xMax || yWorld > yMax || xWorld < xMin || yWorld < yMin;
    }

    void OnMouseDown()
    {
        arm.SetDragging(true);
    }
    
    void OnMouseDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
    }

    void OnMouseUp()
    {
        arm.SetDragging(false);
        transform.localPosition = end.localPosition;
        
        if (IsOutOfBox())
        {
            float clampedX = Mathf.Clamp(transform.position.x, xMin, xMax);
            float clampedY = Mathf.Clamp(transform.position.y, yMin, yMax);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}
