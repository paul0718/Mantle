using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
//using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Part : MonoBehaviour
{
    private RepairManager repairManager;
    private GameObject slot; // needs a Collider2D with isTrigger ON
    private float knockOffForce = 15;
    private float gravityFactor = 1;
    
    private bool isInPosition = true;
    private Quaternion currRotation;
    private Vector3 offset;
    private Collider2D slotCollider;
    private Collider2D thisCollider;
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Awake()
    {
        slot = transform.parent.gameObject;
        slotCollider = slot.GetComponent<Collider2D>();
        slotCollider.isTrigger = false;
        thisCollider = GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // otherwise some collisions are not detected
        DisablePhysics();
        SetBackToSlot();
    }

    public void OnEnable()
    {
        
    }

    public void OnDisable()
    {
        DisablePhysics();
        SetBackToSlot();
    }

    public void OnMouseDown()
    {
        if (isInPosition)
        {
            return;
        }
        DisablePhysics();
        currRotation = transform.rotation;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    
    public void OnMouseDrag()
    {
        if (isInPosition)
        {
            return;
        }
        transform.rotation = currRotation;
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
    }

    public void OnMouseUp()
    {
        if (isInPosition)
        {
            return;
        }
        if (thisCollider.bounds.Intersects(slotCollider.bounds))
        {
            SetBackToSlot();
            isInPosition = true;
            DisablePhysics();
            repairManager.NewPartFixed();
        }
        else
        {
            EnablePhysics();
        }
    }

    public void KnockOff()
    {
        isInPosition = false;
        slotCollider.isTrigger = true;
        EnablePhysics();
        ShootOut();
    }

    public void SetRepairManager(RepairManager newManager)
    {
        repairManager = newManager;
    }
    
    public void SetForce(float newForce)
    {
        knockOffForce = newForce;
    }

    public void SetGravity(float newGravity)
    {
        gravityFactor = newGravity;
    }
    
    private void EnablePhysics()
    {
        rb.simulated = true;
        rb.gravityScale = gravityFactor;
    }

    private void DisablePhysics()
    {
        rb.simulated = false;
        rb.gravityScale = 0;
    }

    private void SetBackToSlot()
    {
        slotCollider.isTrigger = false;
        transform.position = slot.transform.position;
        transform.rotation = slot.transform.rotation;
    }
    
    private void ShootOut()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.AddForce(randomDirection * knockOffForce, ForceMode2D.Impulse);
    }
}
