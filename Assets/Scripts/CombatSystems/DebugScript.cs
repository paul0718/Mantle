using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    [SerializeField] private BarkManager ts;
    [SerializeField] private EnemyInfo ei;

    [SerializeField] private StateManager sm;

    [SerializeField] private GridManager gm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))//activate explosion sequence end game
        {
            EndFightCutscene temp = GameObject.Find("ExplosionSequence").GetComponent<EndFightCutscene>();
            StartCoroutine(temp.PlayEndSequence());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))//transition to next scene
        {
            GameObject.Find("ExplosionSequence").GetComponent<EndFightCutscene>().TransitionToNewsConvo();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))//Move dot
        {
            gm.UpdateDotPosition(-120, 160);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))//end game in kill state
        {
            sm.EndBattle(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))//end game in capture state
        {
            sm.EndBattle(false);
        }
    }
}