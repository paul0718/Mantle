using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CheckEyeEnd : MonoBehaviour
{
    [SerializeField] private GridSystem gridScript;
    [SerializeField] private CreateTears tearsScript;

    [SerializeField] private Collider2D goalCollider;
    [SerializeField] private Collider2D overflowCollider;

    private bool gameDone = false;
    private bool win = false;
    [HideInInspector] public bool overflow = false;
    public void Update()
    {
        if (gameDone)
        {
            if (overflow && !win)
            {
                Debug.Log("overflow");
                Lose();
            }
            else
            {
                if (win)
                {
                    foreach (GameObject t in tearsScript.tearsCreated)
                    {
                        Destroy(t);
                    }
                    gridScript.EyeAttack(true);
                }
                else
                {
                    Lose();
                }
            }
            

            gameDone = false;
        }
    }

    private void Lose()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        foreach (GameObject t in tearsScript.tearsCreated)
        {
            Destroy(t);
        }
        gridScript.EyeAttack(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tear"))
        {
            win = true;
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    public IEnumerator ActivateCollider()
    {
        yield return new WaitForSeconds(3.0f);
        overflowCollider.enabled = true;
        goalCollider.enabled = true;
        yield return new WaitForSeconds(1.0f);
        gameDone = true;
    }
}
