using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticalCheck : MonoBehaviour
{
    private bool objectInRange = false;
    [SerializeField] private float countDown;
    [SerializeField] float initalCountDown;

    private GameObject enemyAttack;

    [SerializeField] private SpawnProjectile spawnScript;

    [SerializeField] private AudioSource hitSound;
    
    // Update is called once per frame
    void Update()
    {
        if (objectInRange)
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                GetComponent<SpriteRenderer>().color = Color.green;
                GameObject.Find("DialBlock").GetComponent<DialBlockManager>().count++;
                GameObject.Find("DialBlock").GetComponent<DialBlockManager>().countHit++;
                hitSound.Play();
                Destroy(enemyAttack);
                countDown = initalCountDown;
                StartCoroutine(ResetRetical());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            enemyAttack = other.gameObject;
            objectInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            countDown = initalCountDown;
            objectInRange = false;
        }
    }

    IEnumerator ResetRetical()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
