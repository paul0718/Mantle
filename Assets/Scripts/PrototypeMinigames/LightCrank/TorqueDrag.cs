using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TorqueDrag : MonoBehaviour 
{
    [SerializeField] float torque = 1.0f;
    private Rigidbody2D rb;
    
    // public float speed; // spin speed
    // float angle1; // angle1 will be negative to normal angle
    // float angle;
    
    private bool pressing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // void FixedUpdate()
    // {
    //     if (pressing)
    //     {
    //         faceMouse();
    //     }
    //     
    // }
    //
    // private void OnMouseDown()
    // {
    //     //pressing = true;
    // }
    //
    // private void OnMouseUp()
    // {
    //     //pressing = false;
    // }
    //
    // void faceMouse()
    // {
    //     Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);     
    //     angle = Mathf.Atan2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y) * 180 / Mathf.PI; //Get mouse angle
    //     
    //     rb.rotation %= 360; // dont remember why i did this but better dont remove it
    //     
    //     angle = (angle + rb.rotation); // Sum up rigidbody and mouse angle
    //     
    //     if (angle < 0) angle1 = 360.0f + angle;
    //     else angle1 = 360.0f - angle; // calculates negative angle
    //     if (Mathf.Abs(angle) > Mathf.Abs(angle1) && angle < 0)
    //         angle = angle1;
    //     if (Mathf.Abs(angle) > Mathf.Abs(angle1) && angle > 0)
    //         angle = angle1 * -1; // from my testing i found out that by writing these ifs rigid body stops doing awkward 360 turnadounds and spins trough closest path to mouse
    //     rb.AddTorque(-angle / 180 * speed);
    // }
    //
    private void OnMouseDrag()
    {
        
        rb.AddTorque (torque * -Input.GetAxis ("Mouse X"));
       
        rb.AddTorque (torque * Input.GetAxis ("Mouse Y"));
    }
    
}
