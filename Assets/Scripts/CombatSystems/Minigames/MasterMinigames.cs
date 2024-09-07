using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterMinigames : MonoBehaviour
{
    [SerializeField] private UITransitionManager _uiManager;

    [SerializeField] private EnergyBar energySlider;
    
    [SerializeField] private GameObject[] minigames;

    [SerializeField] private GameObject endBattleReticle;

    private bool endingBattle = false;
    //----------------------------------------------------------------------------------------

    public void MinigameOneOn()
    {
        _uiManager._currGame = minigames[0];
        ActivateMinigame();
    }
    
    public void MinigameTwoOn()
    {
        _uiManager._currGame = minigames[1];
        ActivateMinigame();
    }
    
    public void MinigameThreeOn()
    {
        _uiManager._currGame = minigames[2];
        ActivateMinigame();
    }
    
    public void MinigameFourOn()
    {
        _uiManager._currGame = minigames[3];
        ActivateMinigame();
    }

    void ActivateMinigame()
    {
        _uiManager.Transition();
        energySlider.UpdateEnergy(10);
    }
}
