using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialBlockManager : MonoBehaviour
{
    private bool gameStarted = false;

    [HideInInspector] public int count = 0;
    [HideInInspector] public int countHit = 0;

    [SerializeField] private SpawnProjectile spawnManager;

    private void OnEnable()
    {
        count = 0;
        countHit = 0;
        gameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (count == 5)
        {
            if (countHit == 5)
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
            spawnManager.StartCoroutine(spawnManager.SpawnProjectilesForDial());
        }
    }
}
