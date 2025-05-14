using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class FanUnit : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler,IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private GameObject fan;
    [SerializeField] private Image indicator;
    [SerializeField] private WindGameManager windSys;
    [SerializeField] private Image arrow;
    [SerializeField] private Animator overheatAnimator;
    private RectTransform fanRectTrans;
    private Quaternion fanDefaultRotation;
    private Vector3 fanDefaultPosition;
    private Tween fanTween;
    
    // Fan States
    private enum Mode
    {
        Low,
        Med,
        High,
        Ultra
    }
    private Mode fanMode = Mode.Low;
    private bool freezed = false;    // freeze everything
    private bool canSlowDown = true;
    
    [Header("Thresholds & Limits")]
    [SerializeField] private float overheatRotation = 10;
    private float[] rotationThresholds = {0.4f, 0.6f, 0.8f, 1f}; // used for determining the current mode of the fan

    [Header("Speed Settings")]
    [SerializeField] private float speedCoefficient = 10;
    [SerializeField] private float baseAccelRate = 50;
    [SerializeField] private float decelRate = 75f;
    [SerializeField] private float holdTime = 2f;
    private float rotation = 0;
    private float targetSpeed = 0;      // The speed we want the fan to spin at a given moment
    private float currSpeed = 0;        // The actual speed of the fan, this is to show the effect of inertia
    private float accelRate = 0;
    
    [Header("Shake Settings")] 
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private float vibration = 50;
    [SerializeField] private float randomness = 90;
    // private float[] shakeStrengths = { 0.02f, 0.06f, 0.1f, 0.2f };
    
    // Local Variables
    private Vector3 initVec; // Vector from origin to mouse before player starts dragging
    private Vector3 prevVec; // Vector from origin to mouse in the previous "frame"
    private Vector3 currVec; // Vector from origin to mouse in THIS "frame"
    private Vector3 fanCenterPosition;  // The origin of ALL vectors in this script, in WORLD SPACE
    public static Color[] colors = new Color[] {Color.green, Color.yellow, Color.red, Color.red};

    private Tween signalTween;

    void OnEnable()
    {
        freezed = false;
        if (fanRectTrans != null)
        {
            StartFanShake();
        }
    }
    
    void Start()
    {
        windSys = transform.parent.GetComponent<WindGameManager>();
        indicator.color = colors[0];
        fanRectTrans = fan.GetComponent<RectTransform>();
        fanDefaultRotation = fanRectTrans.localRotation;
        fanDefaultPosition = fanRectTrans.localPosition;
        fanCenterPosition = fanRectTrans.position;
        StartFanShake();
    }
    
    void Update()
    {
        if (freezed)
        {
            DoFanSpin();
            return;
        }
        CheckOverheat();
        UpdateFanMode();
        UpdateFanSpeed();
        DoFanSpin();
    }
    
    void OnDisable()
    {
        fanTween?.Kill(); // Stop Fan Shake
        fanRectTrans.localPosition = fanDefaultPosition;
        fanRectTrans.localRotation = fanDefaultRotation;
        currSpeed = 0f;
        targetSpeed = 0f;
        rotation = 0f;
        fan.GetComponent<Image>().color = Color.white;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (freezed)
        {
            return;
        }
        canSlowDown = false;
        // Store the initial mouse position when the player starts dragging
        initVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - fanCenterPosition; // For World Space Canvas
        // initVec = new Vector3(eventData.position.x, eventData.position.y, fanCenterPosition.z) - fanCenterPosition; // For Overlay Canvas
        prevVec = initVec;
    }
    public void OnDrag(PointerEventData eventData) 
    {
        arrow.color = Color.cyan;
        if (freezed)
        {
            return;
        }
        // Calculate the angle between the previous and current directions
        currVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - fanCenterPosition; // For World Space Canvas
        // currVec = new Vector3(eventData.position.x, eventData.position.y, fanCenterPosition.z) - fanCenterPosition; // For Overlay Canvas
        float angle = (-1) * Vector2.SignedAngle(prevVec, currVec); // [-180,180]; method takes Z-axis' perspective
        float expectedDegree = GetVecDegree(prevVec) + angle; // Unlike GetVecDegree(currVec), this can exceed 360 and can be used to determine full rotation
        if (angle < 0 && expectedDegree < 0) // CCW and passing initial position in a wrong way
        {
            initVec = currVec; // resetting initVec so that the fan doesn't lose any speed (3 related)
        }
        else if (angle >= 0 && expectedDegree >= 36) // CW and completes 1/10 of a full rotation
        {
            rotation += 0.1f;
            initVec = currVec;
        }
        prevVec = currVec;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        arrow.color = Color.black;
        if (freezed)
        {
            return;
        }
        StartCoroutine(HoldSpeed());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CursorManager.Instance.StartCursorAnimation();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        CursorManager.Instance.StopCursorAnimation();
    }

    public bool IsAtLimit()
    {
        return (fanMode == Mode.Ultra);
    }
    public float GetCurrSpeed() => rotation;
    public void Freeze() => freezed = true;
    public void SetOverheat(float overheat) => overheatRotation = overheat;
    
    private bool CheckOverheat()
    {
        if (rotation > overheatRotation)
        {
            freezed = true;
            fan.GetComponent<Image>().color = Color.red;
            overheatAnimator.SetTrigger("TriggerOverheat");
            windSys.SetOverheat();
            return true;
        };
        return false;
    }
    private void UpdateFanMode()
    {
        Mode newMode = (rotation < rotationThresholds[(int)Mode.Low] * overheatRotation) ? Mode.Low :
                        (rotation < rotationThresholds[(int)Mode.Med] * overheatRotation) ? Mode.Med :
                        (rotation < rotationThresholds[(int)Mode.High] * overheatRotation) ? Mode.High :
                        Mode.Ultra;
   
        if (newMode == fanMode) return;
        fanMode = newMode;
        // UpdateShakeTween();
        UpdateSignal();
    }
    private void UpdateFanSpeed()
    {
        targetSpeed = GetSpeed(rotation);
        // Slow down if necessary
        if (canSlowDown)
        {
            rotation -= 0.5f * Time.deltaTime;
            rotation = Mathf.Max(rotation, 0);
            if (currSpeed > targetSpeed)
            {
                currSpeed -= decelRate * Time.deltaTime;
                currSpeed = Mathf.Max(currSpeed, targetSpeed);
            }
        }
        // Gradually accelerate to the target speed to show inertia
        if (currSpeed < targetSpeed)
        {
            float speedDiff = targetSpeed - currSpeed;
            accelRate = baseAccelRate + speedDiff;
            currSpeed += accelRate * Time.deltaTime;
            currSpeed = Mathf.Min(currSpeed, targetSpeed);
        }
    }
    private void DoFanSpin()
    {
        fanRectTrans.Rotate(0f, 0f, -currSpeed * speedCoefficient * Time.deltaTime);
    }
    
    private void UpdateSignal()
    {
        if (fanMode == Mode.Ultra)
        {
            // Create a sequence to animate the flash
            signalTween = indicator.DOColor(Color.white, 0.125f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
            return;
        }
        if (signalTween != null)
        {
            signalTween.Kill();
        }
        indicator.color = colors[(int)fanMode];
    }
    private float GetSpeed(float inputRotation)
    {
        if (inputRotation <= 3) // 9x
        {
            return inputRotation * 9;
        }
        return Mathf.Pow(inputRotation + 1.5f, 2) + 6.75f; // (x+1.5)^2 + 6.75, same slope and same value at x=3
    }
    private float GetVecDegree(Vector3 newVec)
    {
        // float angle = -Vector2.SignedAngle(Vector3.up, input);
        // angle = (angle<0) ? (360 - angle) : angle; // Equivalent to angle = (360 + angle) % 360;
        return (360 - Vector2.SignedAngle(initVec, newVec)) % 360;
    }
    private IEnumerator HoldSpeed()
    {
        yield return new WaitForSeconds(holdTime);
        canSlowDown = true;
    }

    private void StartFanShake()
    {
        fanTween = fanRectTrans.DOShakePosition(duration, new Vector3(0.02f, 0.02f, 0f), (int)vibration, randomness).SetLoops(-1);
    }
    
}
