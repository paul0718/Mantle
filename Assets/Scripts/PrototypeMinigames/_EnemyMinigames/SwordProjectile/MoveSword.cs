using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSword : MonoBehaviour
{
    [SerializeField] private Transform enemyTransform;

    [SerializeField] private SwordGameManager swordManager;
    
    private bool dragging = false;

    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }

    private void OnMouseDown()
    {
        if (!swordManager.gameStarted)
        {
            enemyTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            swordManager.StartGame();
        }
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }
}
