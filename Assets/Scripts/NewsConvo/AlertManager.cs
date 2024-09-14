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
    [SerializeField] private GameObject newsOffline;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject newsWindow;
    [SerializeField] private GameObject newsTaskbar;
    
    [SerializeField] private ConvoManager dialogueScript;

    public bool alertActive;
    

    private void Start()
    {
        newsOffline.SetActive(alertActive);
        playButton.SetActive(!alertActive);
        newsWindow.SetActive(!alertActive);
        newsTaskbar.SetActive(!alertActive);
        if (alertActive)
        {
            StartCoroutine(ShowAlert());
        }
    }

    private IEnumerator ShowAlert()
    {
        yield return new WaitForSeconds(2);
        //TODO: alert fade-in animation?
        alertObj.SetActive(true);
        alertTxt.DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void AlertReceived()
    {   
        dialogueScript.currentConvo = "Alert";
        dialogueScript.UpdateText();
        alertObj.SetActive(false);
    }
}
