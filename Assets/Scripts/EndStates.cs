using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndStates : MonoBehaviour
{
    [SerializeField] private GameObject cover;

    [SerializeField] private GameObject killResult;

    [SerializeField] private GameObject surrenderResult;
    
    //1 is kill, 2 is surrender
    [SerializeField] private int endStateNum;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyDot"))
        {
            cover.SetActive(true);
            if (endStateNum == 1)
            {
                killResult.SetActive(true);
            }
            else if (endStateNum == 2)
            {
                surrenderResult.SetActive(true);
            }
        }
    }
}
