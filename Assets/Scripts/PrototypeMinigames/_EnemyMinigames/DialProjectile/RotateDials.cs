using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateDials : MonoBehaviour
{
    // public List<float> rotationStop = new List<float>();
    // public float sensitivity;
    // public float dialValue = 0;

    [SerializeField] private bool leftRight;
    [SerializeField] private bool upDown;
    
    private Camera myCam;
    private Vector3 screenPos;
    private float angleOffset;
    private Collider2D col;

    private float prevAngleOffset;
    private bool isRotating = false;
    private float prevAngle;
    private float angle;

    private GameObject hitObject;

    [SerializeField] private DialBlockManager gameManager;
    [SerializeField] private Transform retical;
    [SerializeField] private float distanceToMove;
    void Start()
    {
        myCam = Camera.main;
        col = GetComponent<Collider2D>();
    }
    private void Update()
    {
        Vector3 mousePos = myCam.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if(hit.collider != null)
            {
                gameManager.StartGame();
                hitObject = hit.transform.gameObject;
                isRotating = true;
                screenPos = myCam.WorldToScreenPoint(hitObject.transform.position);
                Vector3 vec3 = Input.mousePosition - screenPos;
                angleOffset = (Mathf.Atan2(hitObject.transform.right.y, hitObject.transform.right.x) - Mathf.Atan2(vec3.y, vec3.x)) *
                              Mathf.Rad2Deg; 
            }
            
            // if (col == Physics2D.OverlapPoint(mousePos))
            // {
            //     
            // }
        }
    
        if (Input.GetMouseButton(0))
        {
            if (isRotating)
            {
                // if (col == Physics2D.OverlapPoint(mousePos))
                // {
                //     
                // }
                Vector3 vec3 = Input.mousePosition - screenPos;
                angle = Mathf.Atan2(vec3.y, vec3.x) * Mathf.Rad2Deg;
                hitObject.transform.eulerAngles = new Vector3(0, 0, angle + angleOffset);
                Vector3 temp = retical.position;
                if (angle < prevAngle)
                {
                    if (hitObject.name == "L/RKnob")
                    {
                        retical.position = new Vector3(temp.x + distanceToMove, temp.y, temp.z);
                    }
                    else if (hitObject.name == "U/DKnob")
                    {
                        retical.position = new Vector3(temp.x, temp.y - distanceToMove, temp.z);
                    }
                }
                else if (angle > prevAngle)
                {
                    if (hitObject.name == "L/RKnob")
                    {
                        retical.position = new Vector3(temp.x - distanceToMove, temp.y, temp.z);
                    }
                    else if (hitObject.name == "U/DKnob")
                    {
                        retical.position = new Vector3(temp.x, temp.y + distanceToMove, temp.z);
                    }
                }
                prevAngle = angle;
            }
        }
    }

    private void OnMouseUp()
    {
        isRotating = false;
    }
}
    
    // // Update is called once per frame
    // void Update()
    // {
    //     dialValue = Mathf.Clamp(dialValue, 0, rotationStop.Count-1);
    //     
    //     index = (int) dialValue;
    //     TurnDialTo(rotationStop[index]);
    // }
    //
    // // Dragging on the gameobject increases or decreases dialvalue
    // void OnMouseDrag ()
    // {
    //     float dragMove = Input.GetAxis("Mouse X") * sensitivity;
    //     dialValue += dragMove;
    // }
    //
    // // Rotates the knob to angle set in rotationStop
    // void TurnDialTo (float position)
    // {
    //     transform.rotation = Quaternion.Euler(0, 0, position);
    // }

