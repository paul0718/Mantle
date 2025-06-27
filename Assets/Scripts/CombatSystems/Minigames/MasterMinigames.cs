using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MasterMinigames : MonoBehaviour
{

    [SerializeField] private EnergyBar energySlider;
    [SerializeField] private Image gridImage;

    public Button[] minigameButtons;
    public GameObject[] minigames;
    public GameObject[] defendMinigames;
    [HideInInspector]
    public GameObject currentMinigame;
    
    public static MasterMinigames Instance { get; private set; }
    private bool lockedIn = true;
    private void Start()
    {
        Instance = this;
        for (int i = 0; i < minigameButtons.Length; i++) 
        {
            var p = i;
            minigameButtons[i].onClick.RemoveAllListeners();
            var em = SequenceManager.Instance.CurrentBattle.EnemyMinigames;
            var g = SequenceManager.Instance.CurrentBattle.Minigames;
            if (g.ContainsKey(p))
            {
                minigameButtons[p].interactable = true;
                minigameButtons[p].image.raycastTarget = true;
                minigameButtons[p].onClick.AddListener(() =>
                {
                    CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.LockIn);
                    currentMinigame = minigames[p];
                    
                    // gridImage.DOFade(0, 0.25f).SetLoops(2, LoopType.Yoyo);
                    StartCoroutine(GridBlink());

                    energySlider.energyBlink = true;

                    GridManager.Instance.StartDotAnimation(p);

                    string temp = currentMinigame.name;
                    if (SequenceManager.Instance.SequenceID > 10)
                    {
                        temp = CheckMinigameNames(currentMinigame.name);
                    }

                    ChatLogManager.Instance.ShowText("Activating " + temp);
                    lockedIn = false;
                });
            }
            else
            {
                minigameButtons[p].interactable = false;
                minigameButtons[p].image.raycastTarget = false;
            }
            
        }

        CoreButton.Instance.RegisterFunction(CoreButton.FunctionType.Respond, () =>
        {
            currentMinigame = defendMinigames[StateManager.Instance.enemyChoice];
            CoverManager.Instance.Switch(true, StateManager.Instance.enemyChoice != 2);
            GridManager.Instance.StopDotAnimation();
            GridManager.Instance.GridClosing();
            CoverManager.Instance.SetDefendButton(false);
            CoreButton.Instance.ClearCoreButton();
        });

        CoreButton.Instance.RegisterFunction(CoreButton.FunctionType.LockIn, () =>
        {
            if (lockedIn) return;
            CoreButton.Instance.ClearCoreButton();
            GridManager.Instance.StopDotAnimation();
            CoverManager.Instance.Switch(true, false);
            energySlider.UpdateEnergy(10);
            lockedIn = true;
        });
    }
    public void SetGame()
    {
        if (!currentMinigame.activeSelf)
        {
            energySlider.transform.parent.DOLocalMove(new Vector3(-146.1f, 196, 1), 1, false);
            currentMinigame.SetActive(true);
            energySlider.energyBlink = false;
            GridManager.Instance.GridClosing();
        }
        else
        {
            currentMinigame.SetActive(false);
            if (currentMinigame.GetComponent<Fluid>() != null)
                currentMinigame.GetComponent<Fluid>().ResetGame();
            currentMinigame = null;
        }
    }
    public void DisableAllButtons()
    {
        foreach(var b in minigameButtons)
        {
            b.image.raycastTarget = false;
            b.interactable = false;
        }
    }

    IEnumerator GridBlink()
    {
        Color temp = gridImage.color;

        DOTween.Kill("GridBlink");
        
        temp.a = 0.8f;
        gridImage.color = temp;
        gridImage.DOFade(0, 0.25f).SetLoops(6, LoopType.Yoyo).SetId("GridBlink");
        yield break;
    }
    
    private string CheckMinigameNames(string currName)
    {
        if (currName == "Disarm")
            return "Killing Shot";
        else if (currName == "Image Projection")
            return "Image Projection";
        else if (currName == "Wind Breeze")
            return "Wind Blast";
        else if (currName == "Sticky Goo")
            return "Sticky Acid";
        else if (currName == "Audio Wave")
            return "Piercing Audio";

        return "";
    }
}
