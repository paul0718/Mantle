using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircuitInfo : MonoBehaviour
{
    public float val;

    [HideInInspector] public List<GameObject> connectedCircuits = new List<GameObject>();

    [SerializeField] float initialVal;
    private void OnEnable()
    {
        SetMinigame();
    }

    public void UpdateInfo(float n, GameObject circuit)
    {
        val = n;
        transform.GetChild(0).GetComponent<TextMeshPro>().text = val.ToString();

        connectedCircuits.Add(circuit);

        foreach (var c in connectedCircuits)
        {
            c.GetComponent<CircuitInfo>().val = n;
            c.transform.GetChild(0).GetComponent<TextMeshPro>().text = val.ToString();
        }
    }

    public void AddRestOfList(GameObject circuit)
    {
        foreach (var a in circuit.GetComponent<CircuitInfo>().connectedCircuits)
        {
            if (!connectedCircuits.Contains(a)) 
            {
                connectedCircuits.Add(a);
            }
        }
        
    }

    void SetMinigame()
    {
        connectedCircuits.Clear();
        
        val = initialVal;
        transform.GetChild(0).GetComponent<TextMeshPro>().text = val.ToString();
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.green;
    }
}
