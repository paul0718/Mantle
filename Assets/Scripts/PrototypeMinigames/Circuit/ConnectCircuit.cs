using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectCircuit : MonoBehaviour
{
    [HideInInspector] public GridSystem gridScript;
    
    public enum Type
    {
        Add,
        Subtract
    };
    
    [SerializeField] private GameObject menu;

    private List<GameObject> lineObjects = new List<GameObject>();

    private bool firstCircuitChose = false;

    private Type currentType;
    
    GameObject firstCircuit;
    GameObject secondCircuit;

    private float currentVal = 0;

    private float firstCircuitVal = 0;
    private float secondCircuitVal = 0;

    [SerializeField] private GameObject[] circuits;

    private void OnEnable()
    {
        SetMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if(hit.collider != null)
            {
                if (hit.transform.CompareTag("FromCircuit"))
                {
                    if (!firstCircuitChose)
                    {
                        firstCircuit = hit.transform.gameObject;
                        firstCircuitVal = firstCircuit.GetComponent<CircuitInfo>().val;
                        OpenAddSubMenu();
                    }
                    else if (firstCircuitChose)
                    {
                        secondCircuit = hit.transform.gameObject;
                        secondCircuitVal = secondCircuit.GetComponent<CircuitInfo>().val;
                        DrawLine();

                        firstCircuitChose = false;
                        firstCircuit.tag = "ToCircuit";
                    }
                }
                else if (hit.transform.CompareTag("ToCircuit"))
                {
                    if (firstCircuitChose)
                    {
                        secondCircuit = hit.transform.gameObject;
                        secondCircuitVal = secondCircuit.GetComponent<CircuitInfo>().val;
                        DrawLine();

                        firstCircuitChose = false;
                    }
                }
            }
        }
    }

    public void OpenAddSubMenu()
    {
        menu.SetActive(true);
    }

    public void Add()
    {
        currentType = Type.Add;
        menu.SetActive(false);
        firstCircuitChose = true;
    }

    public void Subtract()
    {
        currentType = Type.Subtract;
        menu.SetActive(false);
        firstCircuitChose = true;
    }

    public void DrawLine()
    {
        var go = new GameObject();
        var lr = go.AddComponent<LineRenderer>();
        lineObjects.Add(go);
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        if (currentType == Type.Add)
        {
            lr.startColor = Color.blue;
            lr.endColor = Color.blue;
        }
        else if (currentType == Type.Subtract)
        {
            lr.startColor = Color.cyan;
            lr.endColor = Color.cyan;
        }
        lr.startWidth = 0.5f;
        lr.endWidth = 0.5f;
        lr.sortingOrder = -1;
                        
        lr.SetPosition(0, firstCircuit.transform.position);
        lr.SetPosition(1, secondCircuit.transform.position);

        firstCircuit.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.red;
        
        CalcValue();
    }

    public void CalcValue()
    {
        if (currentType == Type.Add)
        {
            currentVal = firstCircuitVal + secondCircuitVal;
        }
        else
        {
            currentVal = firstCircuitVal - secondCircuitVal;
        }
        
        firstCircuit.GetComponent<CircuitInfo>().UpdateInfo(currentVal, secondCircuit);
        secondCircuit.GetComponent<CircuitInfo>().UpdateInfo(currentVal, firstCircuit);
        firstCircuit.GetComponent<CircuitInfo>().AddRestOfList(secondCircuit);
        secondCircuit.GetComponent<CircuitInfo>().AddRestOfList(firstCircuit);

        CheckResult();
    }
    
    public void CheckResult()
    {
        if (firstCircuit.GetComponent<CircuitInfo>().connectedCircuits.Count == 5)
        {
            if (currentVal == 30)
            {
                gridScript.ElectricPulse(true);
                foreach (GameObject l in lineObjects)
                {
                    l.GetComponent<LineRenderer>().startColor = Color.green;
                    l.GetComponent<LineRenderer>().endColor = Color.green;
                }

                StartCoroutine(DestroyLines());
            }
        }
        else if (firstCircuit.GetComponent<CircuitInfo>().connectedCircuits.Count == 6)
        {
            if (currentVal == 30)
            {
                gridScript.ElectricPulse(true);
                foreach (GameObject l in lineObjects)
                {
                    l.GetComponent<LineRenderer>().startColor = Color.green;
                    l.GetComponent<LineRenderer>().endColor = Color.green;
                }
                StartCoroutine(DestroyLines());
            }
            else
            {
                gridScript.ElectricPulse(false);
                foreach (GameObject l in lineObjects)
                {
                    l.GetComponent<LineRenderer>().startColor = Color.red;
                    l.GetComponent<LineRenderer>().endColor = Color.red;
                }
                StartCoroutine(DestroyLines());
            }
        }
    }

    public IEnumerator DestroyLines()
    {
        yield return new WaitForSeconds(1.8f);
        foreach (GameObject l in lineObjects)
        {
            Destroy(l);
        }
        lineObjects.Clear();
    }

    void SetMinigame()
    {
        currentVal = 0;
        firstCircuitVal = 0;
        secondCircuitVal = 0;
        firstCircuit = null;
        secondCircuit = null;

        foreach (GameObject c in circuits)
        {
            c.tag = "FromCircuit";
        }
    }
}
