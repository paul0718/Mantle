using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreateTears : MonoBehaviour
{
    private Camera myCam;
    private Vector3 screenPos;
    private float angleOffset;
    private Collider2D col;
    [SerializeField] private GameObject circleObject;
    [SerializeField] private GameObject clockEye;
    [SerializeField] private float clockEyeSpeed = 1.0f;

    [SerializeField] private CheckEyeEnd eyeEndScript;
    
    [SerializeField] private GameObject tears;
    [HideInInspector]
    public List<GameObject> tearsCreated = new List<GameObject>();

    private float prevAngleOffset;
    private bool isRotating = false;
    private float angle;
    private float prevAngle;

    [SerializeField] private AudioSource audio;
    
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
                //Cursor.visible = false;
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
                circleObject.transform.eulerAngles = new Vector3(0, 0, angle + angleOffset);
                
                if (angle != prevAngle)
                {
                    SpawnTear();
                    clockEye.GetComponent<Rigidbody2D>().rotation -= clockEyeSpeed * Time.deltaTime;
                }
                
                clockEyeSpeed += 20.0f * Time.deltaTime;
                if (clockEyeSpeed > 200)
                {
                    clockEyeSpeed = 100;
                }

                prevAngle = angle;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            audio.Stop();
            clockEyeSpeed = 10.0f;
            isRotating = false;
            eyeEndScript.StartCoroutine(eyeEndScript.ActivateCollider());
            this.enabled = false;
        }
    }

    public void SpawnTear()
    {
        tearsCreated.Add(Instantiate(tears, new Vector3(0.03f, -2.72f, 0), Quaternion.identity));
    }

    void SetMinigame()
    {
        tearsCreated.Clear();
        eyeEndScript.gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
        eyeEndScript.gameObject.GetComponent<Collider2D>().enabled = false;
    }
}
