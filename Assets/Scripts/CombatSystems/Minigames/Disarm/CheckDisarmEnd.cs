using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CheckDisarmEnd : MonoBehaviour
{
    [SerializeField] private Disarm disarmScript;

    [HideInInspector] private bool win = false;

    public void Update()
    {
        if (disarmScript.gameDone)
        {
            disarmScript.scannerBG.transform.DOMove(new Vector3(0, -2.5f, 0), 1f);
            if (win)
            {
                disarmScript.gameDone = false;
                GameObject.Find("BattleManagers").GetComponent<GridManager>().UpdateDotPosition(-50, 50);
                //GameObject.Find("BattleManagers").GetComponent<GridManager>().UpdateDotPosition(-100, 100);
                GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Disarm", "W");
            }
            else
            {
                disarmScript.gameDone = false;
                GameObject.Find("BattleManagers").GetComponent<GridManager>().UpdateDotPosition(0, -50);
                GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Disarm", "L");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerRetical")
        {
            win = true;
        }
    }
}
