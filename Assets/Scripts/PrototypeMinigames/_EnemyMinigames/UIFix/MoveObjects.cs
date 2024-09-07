using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveObjects : MonoBehaviour
{
    [SerializeField] private GameObject blocker;
    
    [SerializeField] private GameObject enemyAttack;
    
    [SerializeField] private GameObject[] objectsToMove;

    [SerializeField] private float timer;
    [SerializeField] private float resetTimer;
    
    [SerializeField] private Vector3 impulse;

    [SerializeField] private AudioSource shootSound;

    [SerializeField] private AudioSource winSound;

    private bool bumpObjects = false;
    private bool startTimer = false;
    [HideInInspector] public bool canCheck = false;
    
    private void OnEnable()
    {
        blocker.SetActive(true);
        timer = resetTimer;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSecondsRealtime(2f);
        blocker.SetActive(false);
        Shoot();
        startTimer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Shoot();
                timer = resetTimer;
            }
        }

        if (canCheck)
        {
            CheckResults();
        }
    }

    private void FixedUpdate()
    {
        if (bumpObjects)
        {
            bumpObjects = false;
            for (int i = 0; i < 3; i++)
            {
                int index = Random.Range(0, objectsToMove.Length - 1);
                Debug.Log(index);
                impulse = new Vector3(Random.Range(7, 10), Random.Range(7, 20), 0);
                objectsToMove[index].GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
                objectsToMove[index].GetComponent<Rigidbody2D>().gravityScale = 1;
                canCheck = true;
            }
        }
    }

    void Shoot()
    {
        shootSound.Play();
        enemyAttack.transform.DOScale(new Vector3(26, 26, 0), 0.5f)
            .SetEase(Ease.Linear).OnComplete(ResetAttack);
    }

    void ResetAttack()
    {
        bumpObjects = true;
        startTimer = true;
        enemyAttack.transform.localScale = new Vector3(0, 0, 0);
    }

    public void CheckResults()
    {
        bool check = false;
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            check = objectsToMove[i].GetComponent<FixObject>().CheckResult();

            if (!check)
            {
                return;
            }
        }
        
        if (check)
        {
            winSound.Play();
            GameObject.Find("ResultGO").GetComponent<EndResults>().UpdateResult(true);
            startTimer = false;
            bumpObjects = false;
            canCheck = false;
        }
    }
}
