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
    [SerializeField] private Button receiveButton;

    [Header("Windows")]
    [SerializeField] private GameObject newsWindow;
    [SerializeField] private GameObject newsTaskbar;
    [SerializeField] private GameObject enemyInfo;
    [SerializeField] private GameObject enemyTaskbar;
    [SerializeField] private GameObject dotTutorial;
    [SerializeField] private GameObject dotTaskbar;
    
    [Header("Misc")]
    [SerializeField] private ConvoManager dialogueScript;
    [SerializeField] private AudioSource alertSound;
    public bool alertActive { get => SequenceManager.Instance.CurrentDesktop.IsAlertScene; }
    

    private void Start()
    {
        newsOffline.SetActive(alertActive);
        playButton.SetActive(!alertActive);
        newsWindow.SetActive(!alertActive);
        newsTaskbar.SetActive(!alertActive);
        enemyInfo.SetActive(alertActive);
        enemyTaskbar.SetActive(alertActive);
        dotTutorial.SetActive(alertActive);
        dotTaskbar.SetActive(alertActive);
        if (alertActive && SequenceManager.Instance.SequenceID != 11)
        {
            StartCoroutine(ShowAlert());
        }
    }

    private IEnumerator ShowAlert()
    {
        yield return null;
        alertObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Villain: " + DesktopSequenceManager.Instance.currentEnemy;
        alertObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Location: " + DesktopSequenceManager.Instance.currentLocation;
        if (!(SequenceManager.Instance.SequenceID == 2))
            yield return new WaitForSeconds(3.5f);
        alertObj.SetActive(true);
        receiveButton.onClick.AddListener(receiveButton.SetTrigger);
        AudioManager.Instance.PlayLoop(SFXNAME.Alert, ()=>receiveButton.GetAndResetTrigger());
        AudioManager.Instance.SetSFXVolume(SFXNAME.Alert, 0.5f);
        alertTxt.DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo);
        yield return null;
    }

    public void AlertReceived()
    {   
        DesktopSequenceManager.Instance.SetEnemyImageDotInfo();
        alertSound.Stop();
        dialogueScript.currentConvo = "Alert";
        dialogueScript.UpdateText();
        alertObj.SetActive(false);
    }
}
