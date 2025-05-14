using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VyzzarArc : MonoBehaviour
{
    public bool paused;
    
    [HideInInspector] public float currentAngle;
    public float endAngle;
    [SerializeField] private float[] randomAngles;

    [SerializeField] private float speed;
    [SerializeField] private Vector3 centerPoint;
    [SerializeField] private float radius;

    private Vector3 startingPos;

    [SerializeField] private Transform boxes;
    [SerializeField] private Color circleColor;


    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        if (!paused)
        {
            if (currentAngle < 20)
                currentAngle += (10 + currentAngle/2)/20 * speed * Time.deltaTime;
            else
                currentAngle += speed * Time.deltaTime;
            float rads = -1 * (180 + currentAngle) * Mathf.Deg2Rad;
            transform.position = centerPoint + new Vector3(Mathf.Cos(rads)*radius, Mathf.Sin(rads)*radius/2, 0);
        }

        foreach (float angle in randomAngles)
        {
            if (angle-currentAngle < 20 && angle-currentAngle > 0) //approaching
            {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).localScale = new Vector3(1, 1, 1)*(0.15f + 0.65f*(angle-currentAngle)/10f);
            }
            else if (angle-currentAngle > -20 && angle-currentAngle < 0) //leaving
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(0).localScale = new Vector3(1, 1, 1)*(0.35f - 0.65f*(angle-currentAngle)/10f);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            bool hit = false;
            for (int i = 0; i < randomAngles.Length; i++)
            {
                if (Mathf.Abs(currentAngle-randomAngles[i]) < 10)
                {
                    if (boxes.GetChild(i).GetComponent<SpriteRenderer>().color != Color.green)
                    {
                        GetComponent<Animator>().Play("SuccessfulClick");
                        boxes.GetChild(i).GetComponent<SpriteRenderer>().color = Color.green;
                        AudioManager.Instance.PlayOneShot(SFXNAME.HitMark);
                        hit = true;
                    }     
                }
            }
            if (!hit)
            {
                AudioManager.Instance.PlayOneShot(SFXNAME.MissMark);
            }
        }
    }

    public void Reset()
    {
        currentAngle = 0;
        transform.position = startingPos;

        //generate random timings
        randomAngles = new float[]{UnityEngine.Random.Range(20, endAngle-20), UnityEngine.Random.Range(20, endAngle-20), UnityEngine.Random.Range(20, endAngle-20)};
        while (Mathf.Abs(randomAngles[1]-randomAngles[0]) < 40)
            randomAngles[1] = UnityEngine.Random.Range(20, endAngle-20);
        while (Mathf.Abs(randomAngles[2]-randomAngles[1]) < 40 || Mathf.Abs(randomAngles[2]-randomAngles[0]) < 40)
            randomAngles[2] = UnityEngine.Random.Range(20, endAngle-20);
        Array.Sort(randomAngles);
    }
}
