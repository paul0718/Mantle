using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private GridSystem gridScript;
    
    private Camera myCam;
    private Vector3 screenPos;
    private float angleOffset;
    private Collider2D col;
    [SerializeField] private GameObject circleObject;

    private float prevAngleOffset;
    private bool isRotating = false;
    private float prevAngle;
    private float angle;

    [SerializeField] private Slider slider;

    private float valueToAdd = 0.05f;

    private float extraSpinDistace = 0.5f;

    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioSource winAudio;
    [SerializeField] private AudioSource loseAudio;

    // Start is called before the first frame update
    void Start()
    {
        myCam = Camera.main;
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        SetMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = myCam.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            // if (col == Physics2D.OverlapPoint(mousePos))
            // {
            //     
            // }
            isRotating = true;
            if (isRotating)
            {
                screenPos = myCam.WorldToScreenPoint(circleObject.transform.position);
                Vector3 vec3 = Input.mousePosition - screenPos;
                angleOffset = (Mathf.Atan2(circleObject.transform.right.y, circleObject.transform.right.x) - Mathf.Atan2(vec3.y, vec3.x)) *
                              Mathf.Rad2Deg;
            }
        }
        if (Input.GetMouseButton(0))
        {
            isRotating = true;
            if (isRotating)
            {
                if (!audio.isPlaying)
                {
                    audio.Play();
                }
                Vector3 vec3 = Input.mousePosition - screenPos;
                angle = Mathf.Atan2(vec3.y, vec3.x) * Mathf.Rad2Deg;

                if (angle < prevAngle)
                {
                    circleObject.transform.eulerAngles = new Vector3(0, 0, angle + angleOffset);
                }
                if (angle != prevAngle)
                {
                    slider.value += valueToAdd * Time.deltaTime;
                    valueToAdd += 0.0009f;
                }
                prevAngle = angle;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
            StartCoroutine(ExcessSpin());
        }
        
        
    }

    IEnumerator ExcessSpin()
    {
        float countDown = 1f;
        for (int i=0; i < 10000; i++) 
        {
            while (countDown >= 0)
            {
                slider.value += valueToAdd * Time.deltaTime;
                valueToAdd -= 0.00005f;
                circleObject.transform.eulerAngles = new Vector3(0, 0, 
                    circleObject.transform.eulerAngles.z - extraSpinDistace);
                extraSpinDistace -= 0.0005f;
                countDown -= Time.smoothDeltaTime;
                yield return null;
            }

            valueToAdd = 0.00005f;
            extraSpinDistace = 0.5f;

            if (slider.value >= 0.78f && slider.value <= 0.92f)
            {
                winAudio.Play();
                gridScript.FlashLight(true);
            }
            else if(slider.value > 0.92f)
            {
                loseAudio.Play();
                gridScript.FlashLight(false);
            }
            audio.Stop();
        }
    }

    void SetMinigame()
    {
        slider.value = 0;
    }
}
