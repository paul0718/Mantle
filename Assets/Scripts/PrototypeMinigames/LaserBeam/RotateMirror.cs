using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class RotateMirror : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool rotateUp;
    [SerializeField] private bool rotateDown;

    [SerializeField] private MoveMirror mirrorScript;
    [SerializeField] private float rotationSpeed;
    
    private bool pointerDown;
    private float pointerDownTimer;

    [SerializeField]
    private float requiredHoldTime;

    public UnityEvent onLongClick;

    [SerializeField]
    private Image fillImage;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        Debug.Log("OnPointerUp");
    }

    private void Update()
    {
        if (pointerDown)
        {
            if (mirrorScript.currMirror != null)
            {
                if (rotateUp)
                {
                    if (mirrorScript.currMirror.CompareTag("RightMirror"))
                    {
                        mirrorScript.currMirror.transform.Rotate(new Vector3(0, 0, -(Time.deltaTime * rotationSpeed)));
                    }
                    else
                    {
                        mirrorScript.currMirror.transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed)); 
                    }
                }
                else if (rotateDown)
                {
                    if (mirrorScript.currMirror.CompareTag("RightMirror"))
                    {
                        mirrorScript.currMirror.transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));
                    }
                    else
                    {
                        mirrorScript.currMirror.transform.Rotate(new Vector3(0, 0, -(Time.deltaTime * rotationSpeed)));
                    }
                }
            }
            
            pointerDownTimer += Time.deltaTime;
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
        }
    }

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        fillImage.fillAmount = 0;
    }
}
