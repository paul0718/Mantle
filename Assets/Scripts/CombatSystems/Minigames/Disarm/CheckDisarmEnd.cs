using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CheckDisarmEnd : MonoBehaviour
{
    [SerializeField] private Disarm disarmScript;
    [SerializeField] private StateManager stateScript;

    [SerializeField] private GameObject reticle;
    [SerializeField] private GameObject enemyWeapon;

    [SerializeField] private Color winColor;

    public void Update()
    {
        if (disarmScript.gameDone)
        {
            disarmScript.scannerBG.transform.DOMove(new Vector3(0, -2.5f, 0), 1f);
            var battle = SequenceManager.Instance.CurrentBattle;
            if (Physics2D.OverlapCircle(transform.position, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Reticle"))) //on hit
            {
                if (MetricManagerScript.instance != null)
                { 
                    MetricManagerScript.instance.LogString("Disarm", "Win");
                }
                disarmScript.gameDone = false;
                GridManager.Instance.UpdateDotPosition(battle.Minigames[0].WinEffect, GridManager.MiniGame.Disarm, true);
                GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(true);
                BarkManager.Instance.ShowGameBark("Disarm", true);
                reticle.GetComponent<SpriteRenderer>().color = winColor;
            }
            else
            {
                if (MetricManagerScript.instance != null)
                { 
                    MetricManagerScript.instance.LogString("Disarm", "Lose");
                }
                disarmScript.gameDone = false;
                GridManager.Instance.UpdateDotPosition(battle.Minigames[0].LoseEffect, GridManager.MiniGame.Disarm,false); //on miss
                GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(false);
                BarkManager.Instance.ShowGameBark("Disarm", false);
                reticle.GetComponent<Animator>().Play("ReticleBlink");
            }
            StartCoroutine(HideReticle());
        }
    }

    private IEnumerator HideReticle()
    {
        if (Physics2D.OverlapCircle(transform.position, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Reticle")))
            yield return new WaitForSeconds(1);
        else
            yield return new WaitForSeconds(2);
        reticle.SetActive(false);
        gameObject.SetActive(false);
    }
}
