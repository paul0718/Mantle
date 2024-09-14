using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AlertManager : MonoBehaviour
{
    [SerializeField] private GameObject alertObj;
    [SerializeField] private TMP_Text alertTxt;
    
    [SerializeField] private ConvoManager dialogueScript;
    [SerializeField] private GoogleSheetsDB sheetsScript;

    public bool alertActive;
    

    private void Start()
    {
        if (alertActive)
        {
            //TODO: could wait a few seconds
            alertObj.SetActive(true);
            alertTxt.DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void AlertReceived()
    {   
        dialogueScript.currentConvo = "Alert";
        dialogueScript.UpdateText();
        alertObj.SetActive(false);
    }
}
