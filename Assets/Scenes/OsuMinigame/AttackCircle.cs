using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AttackCircle : MonoBehaviour
{
    [SerializeField] private BlockMinigame bm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        bm.CircleStopped();
        gameObject.SetActive(false);
    }

    public void AttackHit()
    {
        bm.AttackHit();
    }
}
