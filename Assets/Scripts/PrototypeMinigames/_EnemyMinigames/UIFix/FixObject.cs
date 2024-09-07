using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixObject : MonoBehaviour
{
    [SerializeField] private MoveObjects manager;
    
    [SerializeField] private GameObject fixSpot;
    
    private bool dragging = false;

    private bool justLetGo = false;

    private Vector3 offset;

    private void OnEnable()
    {
        justLetGo = false;
        dragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }

    private void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
        if (!justLetGo)
        {
            justLetGo = true;
        }
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (justLetGo)
        {
            if (other.gameObject == fixSpot)
            {
                GetComponent<Rigidbody2D>().gravityScale = 0;
                transform.position = other.transform.position;
                transform.rotation = other.transform.rotation;
            }
        }
    }

    public bool CheckResult()
    {
        if (transform.position == fixSpot.transform.position)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
