using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PixelCrushers.DialogueSystem;
using Random = UnityEngine.Random;

public class EnemyInfo : MonoBehaviour
{
    //for bark system
    [SerializeField] private Transform npc;
    [SerializeField] private Transform player;

    [SerializeField] BarkManager barkManager;
    [SerializeField] private TMP_Text barkBubbleText;
    [SerializeField] private GameObject barkBubble;
    private List<string> barkStrings = new List<string>();

    private int _totalBarks = 3;
    private string _fullBarkText;
    //----------------------------------------------------------------------------

    public void UpdateBark(string gameName, string result)
    {
        //Read bark sheet, set variable text, and call bark
        barkStrings.Clear();
        for (int i = 0; i < _totalBarks; i++)
        {
            barkStrings.Add(barkManager.ReturnBark(gameName, result + i.ToString()));
        }

        CallBark();
    }

    public void CallBark()//display bark on screen
    {
        barkBubble.SetActive(true);
        int randIndex = Random.Range(0, _totalBarks);
        barkBubbleText.text = barkStrings[randIndex];
        StartCoroutine(CloseBark());
    }

    IEnumerator CloseBark()//turn off the bark after x seconds
    {
        yield return new WaitForSeconds(4.0f);
        barkBubbleText.text = "";
        barkBubble.SetActive(false);
    }
}
