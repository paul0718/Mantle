using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerResults : MonoBehaviour
{
    [SerializeField] private PlayerGame gameScript;
    
    [HideInInspector] public bool checkResults = false;

    private int collision = 0;

    public void CheckResult()
    {
        if (collision != 0)
        {
            Debug.Log("win");
            gameScript.UpdateDot(true);
        }
        else if(collision == 0)
        {
            gameScript.UpdateDot(false);
            Debug.Log("lose");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            collision++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            collision--;
        }
    }
}
