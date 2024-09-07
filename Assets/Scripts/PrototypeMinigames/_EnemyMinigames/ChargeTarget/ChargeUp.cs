using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUp : MonoBehaviour
{
    private bool gameStarted = false;

    [SerializeField] private Slider slider;
    
    [SerializeField] float currentQuantity;
    [SerializeField] float desiredQuantity;

    [SerializeField] float MovementPerSecond = 2.0f;

    [SerializeField] private Image[] targets;

    [SerializeField] private AudioSource winSound;

    [SerializeField] private GameObject aura;

    private int count = 0;

    private void OnEnable()
    {
        foreach (var t in targets)
        {
            t.color = Color.white;
        }

        count = 0;
        currentQuantity = 0;
        slider.value = 0;
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        aura.SetActive(true);
        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            currentQuantity = Mathf.MoveTowards(
                currentQuantity,
                desiredQuantity,
                MovementPerSecond * Time.deltaTime);
            slider.value = currentQuantity;

            if (slider.value == desiredQuantity)
            {
                gameStarted = false;
                GameObject.Find("BattleManagers").GetComponent<GridManager>().UpdateDotPosition(0, -50);
                GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Charge Interrupt", "L");
            }
        }
    }

    public void ClickTarget()
    {
        if (slider.value >= 21.5f && slider.value <= 28.5f && targets[0].color != Color.green)
        {
            targets[0].color = Color.green;
            count++;
        }
        else if (slider.value >= 47.5f && slider.value <= 53.5f && targets[1].color != Color.green)
        {
            targets[1].color = Color.green;
            count++;
        }
        else if (slider.value >= 75.5f && slider.value <= 78.5f && targets[2].color != Color.green)
        {
            targets[2].color = Color.green;
            count++;
        }

        if (count == 3)
        {
            winSound.Play();
            aura.SetActive(false);
            gameStarted = false;
            GameObject.Find("BattleManagers").GetComponent<GridManager>().UpdateDotPosition(0, 0);
            GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Charge Interrupt", "W");
        }
    }

    public void StartGame()
    {
        gameStarted = true;
    }
}
