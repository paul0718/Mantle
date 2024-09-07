using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverflowCheck : MonoBehaviour
{
    [SerializeField] private CheckEyeEnd endCheck;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tear"))
        {
            endCheck.overflow = true;
        }
    }
}
