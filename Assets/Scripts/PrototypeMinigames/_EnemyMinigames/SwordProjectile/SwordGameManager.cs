using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordGameManager : MonoBehaviour
{
    [HideInInspector] public bool gameStarted = false;
    
    [HideInInspector] public int count = 0;
    [HideInInspector] public int countHit = 0;

    [SerializeField] private SpawnProjectile spawnManager;

    private void OnEnable()
    {
        gameStarted = false;
        count = 0;
        countHit = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (count == 8)
        {
            GameObject.Find("Enemy").transform.localScale = new Vector3(1, 1, 1);
            if (countHit == 8)
            {
                GameObject.Find("ResultGO").GetComponent<EndResults>().UpdateResult(true);
            }
            else
            {
                GameObject.Find("ResultGO").GetComponent<EndResults>().UpdateResult(false);
            }
        }
    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            spawnManager.StartCoroutine(spawnManager.SpawnProjectilesForSword());
        }
    }
}
