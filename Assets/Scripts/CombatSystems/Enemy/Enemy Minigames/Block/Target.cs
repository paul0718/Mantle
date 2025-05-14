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

    void Start()
    {
        arm = transform.parent.GetComponent<Arm>();
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
    }
}
