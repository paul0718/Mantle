using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StateManager : MonoBehaviour
{
    [SerializeField] private ChatLogManager _chatManager;
    [SerializeField] private UITransitionManager _uiManager;

    //objects for when the battle is won, player is choosing to kill or capture
    [SerializeField] private GameObject _endReticle;
    [SerializeField] private GameObject _actionBlocker;
    [SerializeField] private GameObject[] _endWeapons;
    [SerializeField] private GameObject[] _enemyButtons;
    [SerializeField] private AudioSource _gunSound;
    [SerializeField] private AudioSource _taserSound;
    [SerializeField] private EndFightCutscene _endSequenceManager;

    [HideInInspector] public int enemyChoice;

    //bools for when ending battle to know if killing or capturing
    private bool _endingBattle = false;
    private bool _killing = false;
    private bool _capturing = false;
    private bool _canEndBattle = false;
    public enum BattleState
    {
        Player,
        Enemy,
        End
    };

    [HideInInspector] public BattleState currState = BattleState.Player;

    [HideInInspector] public bool battleStarted = false;

    private void Start()
    {
        battleStarted = true;
        _chatManager.UpdateState();
    }

    //change turn state
    public void UpdateState()
    {
        if (currState == StateManager.BattleState.Enemy)
        {
            _actionBlocker.SetActive(false);
            currState = StateManager.BattleState.Player;
        }
        else if(currState == BattleState.Player)
        {
            _actionBlocker.SetActive(true);
            currState = StateManager.BattleState.Enemy;
        }
        Debug.Log(currState);

        if (currState == BattleState.Enemy)//if its now enemy turn, randomly choose an enemy minigame to play
        {
            enemyChoice = Random.Range(0, 2);//when we have more minigames, switch statement be better
            if (enemyChoice == 0)
            {
                _uiManager.OpenRightPanels(true); 
            }
            else if (enemyChoice == 1)
            {
                _uiManager.OpenRightPanels(false);
            }
        }
        _chatManager.UpdateState();
    }

    void Update()
    {
        //display the targeting reticle at mouse position when ending the game
        if (_endingBattle)
        {
            Vector2 mouseCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _endReticle.transform.position = mouseCursorPos;
            if (Input.GetMouseButton(0) && _canEndBattle)
            {
                if (_killing)
                {
                    _gunSound.Play();
                    _endSequenceManager.StartCoroutine(_endSequenceManager.PlayEndSequence());
                }
                else if (_capturing)
                {
                    _taserSound.Play();
                    _endSequenceManager.StartCoroutine(_endSequenceManager.PlayEndSequence());
                }

                _canEndBattle = false;
            }
        }
    }

    //Battle is in end state. Open the panels on the right and show either the gun or taser
    public void EndBattle(bool kill)
    {
        _canEndBattle = true;
        currState = BattleState.End;
        foreach (var b in _enemyButtons)
        {
            b.SetActive(false);
        }
        foreach (var e in _endWeapons)
        {
            e.SetActive(true);
        }

        _uiManager.OpenRightPanels(kill);
    }

    //setting current end action to kill
    public void KillWeapon()
    {
        SetEndAction();
        _killing = true;
    }

    //settting current end action to capture
    public void CaptureWeapon()
    {
        SetEndAction();
        _capturing = true;
    }

    //activate targeting reticle and block minigame actions
    void SetEndAction()
    {
        Cursor.visible = false;
        _endingBattle = true;
        _endReticle.SetActive(true);
        _actionBlocker.SetActive(true);
    }
}
