using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opertoon.Panoply;

public class PanoplyTestControl : MonoBehaviour
{
    [SerializeField] private GameObject _mouseInstruct;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PanoplyCore.targetStep == 0)
        {
            _mouseInstruct.SetActive(true);
        }
        else
        {
            _mouseInstruct.SetActive(false);
        }
    }
}
