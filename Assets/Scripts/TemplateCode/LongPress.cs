using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPress : MonoBehaviour
{
    private bool isClick;
    private float countdown = 0;
    
    private void OnMouseDown()
    {
        isClick = true;
    }

    private void OnMouseUp()
    {
        isClick = false;
    }

    private void Update()
    {
        if (isClick)
        {
            countdown += Time.deltaTime;
            Debug.Log(countdown);
            if (countdown > 2)
            {
                //do something
            }
        }
        else
        {
            countdown = 0;
        }
    }

}
