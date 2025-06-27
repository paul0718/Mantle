using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    public static DebugScript DebugInstance;
    
    [SerializeField] private GameObject chargeUpGame;
    [SerializeField] private GameObject interceptGame;
    [SerializeField] private GameObject repairGame;
    [SerializeField] private GameObject blockGame;

    [SerializeField] private EndFightCutscene endFightCutscene;

    [SerializeField] private bool battleBuild;

    [SerializeField] private GameObject destruct;

    private GameObject tempCharge;
    private GameObject tempIntercept;
    private void Awake()
    {
        // if (DebugInstance != null)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        // DebugInstance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StateManager.Instance.currentState = StateManager.BattleState.Win;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            destruct.SetActive(true);
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SequenceManager.Instance.SequenceID = 3;
                SceneManager.LoadScene("BattleScene");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SequenceManager.Instance.SequenceID = 6;
                SceneManager.LoadScene("BattleScene");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SequenceManager.Instance.SequenceID = 9;
                SceneManager.LoadScene("BattleScene");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SequenceManager.Instance.SequenceID = 12;
                SceneManager.LoadScene("BattleScene");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SequenceManager.Instance.SequenceID = 14;
                SceneManager.LoadScene("BattleScene");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SequenceManager.Instance.SequenceID = 17;
                SceneManager.LoadScene("BattleScene");
            }
        }
        
        
        //debug to call enemy games
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MasterMinigames.Instance.currentMinigame = MasterMinigames.Instance.defendMinigames[0];
                CoverManager.Instance.Switch(true, true, false);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                MasterMinigames.Instance.currentMinigame = MasterMinigames.Instance.defendMinigames[1];
                CoverManager.Instance.Switch(true, true, false);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                MasterMinigames.Instance.currentMinigame = MasterMinigames.Instance.defendMinigames[2];
                CoverManager.Instance.Switch(true, false, false);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                MasterMinigames.Instance.currentMinigame = MasterMinigames.Instance.defendMinigames[3];
                CoverManager.Instance.Switch(true, true, true);
            }
        }
        
        //go back to main menu keys
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene("MainMenu");
                SequenceManager.Instance.SequenceID = 1;
                SequenceManager.Instance.dateNum = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))//play ending cutscene for seq 9
        {
            endFightCutscene.StartCoroutine(endFightCutscene.PlayEndSequence());
        }
    }
}