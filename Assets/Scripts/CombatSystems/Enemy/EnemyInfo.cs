using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Random = UnityEngine.Random;

public class EnemyInfo : MonoBehaviour
{
    public static EnemyInfo Instance {  get; private set; }
    private void Start()
    {
        Instance = this;
    }
    private void OnMouseDown()
    {
        Debug.Log("shoot");
        StateManager.Instance.CheckEndBattle();
    }

    public void ChangePose(bool playerWon)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        if (playerWon)
        {
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        
    }

    public void ResetPose()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
    public void SetCollider(bool visible)
    {
        GetComponent<PolygonCollider2D>().enabled = visible;
    }
}
