using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockMinigame : MonoBehaviour
{
    [SerializeField] private GameObject[] attackCircles;
    [SerializeField] private GameObject[] errorSigns;

    [SerializeField] private Animator enemyAnimator;

    private int numCirclesToSpawn = 0;
    
    private int circlesSpawned = 0;

    private int circlesStopped = 0;

    private float spawnInterval = 0.5f;

    private bool gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameStart();
        }
    }

    //play animation and sound first
    public void PlayAnim()
    {
        enemyAnimator.SetTrigger("Attack");
        enemyAnimator.gameObject.GetComponent<AudioSource>().Play();
        circlesSpawned++;

        if (circlesSpawned < numCirclesToSpawn)
        {
            Invoke("PlayAnim", spawnInterval);
        }
        else
        {
            circlesSpawned = 0;
            Invoke("ChooseCircle", 0.5f);
        }
    }

    //after playing animation and sound, spawn the actual circle attacks
    public void ChooseCircle()
    {
        if (gameStarted)
        {
            int randomIndex = Random.Range(0, attackCircles.Length);
        
            attackCircles[randomIndex].SetActive(true);

            circlesSpawned++;

            if (circlesSpawned < numCirclesToSpawn)
            {
                Invoke("ChooseCircle", spawnInterval);
            }
        }
    }

    //call next round of attacks after specified time
    public void NextRound(float time)
    {
        Invoke("GameStart", time);
    }

    //when a circle attack has been stopped, checked if all in the current round have been stopped
    public void CircleStopped()
    {
        circlesStopped++;
        if (circlesStopped >= numCirclesToSpawn)
        {
            Debug.Log("stopped all circles");
            NextRound(1f);
        }
    }

    //if enemy attack hits, activate error sign
    public void AttackHit()
    {
        Debug.Log("attack hit");
        gameStarted = false;
        foreach (var a in attackCircles)
        {
            a.SetActive(false);
        }

        if (errorSigns[0].activeSelf)
        {
            errorSigns[1].SetActive(true);
        }
        else
        {
            errorSigns[0].SetActive(true);
            NextRound(1.5f);
        }
    }

    //start game and next round if game already started
    public void GameStart()
    {
        gameStarted = true;
        if (gameStarted)
        {
            circlesStopped = 0;
            circlesSpawned = 0;
            numCirclesToSpawn = Random.Range(2, 4);
            float tempInterval = Random.Range(0.3f, .5f);
            spawnInterval = Mathf.Round(tempInterval * 10.0f) * 0.1f;
            Debug.Log(spawnInterval);
            PlayAnim();
        }
    }
}
