using UnityEngine;
using UnityEngine.Serialization;

public class SpriteTimer : MonoBehaviour
{
    private float duration = 5f; // Total time in seconds
    [SerializeField] private float stepTime = 0.5f;
    [SerializeField] private bool smoothTimer = false;
    private float stepCount = 1f;
    private float elapsed = 0f;
    private Material mat;
    private float fill = 0f;
    private bool started = false;

    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        stepTime = smoothTimer ? Time.deltaTime : stepTime;
        mat.SetColor("_Color", Color.white);
    }

    void Update()
    {
        if (!started) return;
        
        elapsed += Time.deltaTime;
        if (smoothTimer)
        {
            fill = 1f - (elapsed / duration);
        }
        else
        {
            float currentStep = Mathf.Floor(elapsed / stepTime);
            fill = 1f - (currentStep / stepCount);
        }
        
        mat.SetFloat("_Fill", fill);
        if (elapsed >= duration)
        {
            started = false;
            enabled = false;
        }
    }
    
    public void SetTimer(float duration)
    {
        this.duration = duration;
        stepCount = duration / stepTime;
    }
    
    public void ResetTimer()
    {
        elapsed = 0f;
        fill = 1f;
        mat.SetFloat("_Fill", fill);
        mat.SetColor("_Color", Color.white);
        started = false;
        enabled = true;
    }

    public void StartTimer()
    {
        started = true;
    }

    public void StopTimer()
    {
        started = false;
    }

    public void ChangeColor()
    {
        mat.SetColor("_Color", Color.red);
    }
}