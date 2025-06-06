using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DesktopInteractions : MonoBehaviour
{
    [SerializeField] private GameObject taskbarIcon;
    [SerializeField] private Vector3 minimizeTo;
    [SerializeField] private Vector2 maximizeTo;
    [SerializeField] private float maximizedSize;
    private Vector3 startingPos;

    private float startingScale;
    private Vector3 lastPos;
    [HideInInspector] public bool maximized;
    private GameObject mantleVisionTxt;

    //new & email tab-switching
    [SerializeField] private GameObject newsTab;
    [SerializeField] private GameObject newsWebsite;
    [SerializeField] private GameObject emailTab;
    [SerializeField] private GameObject emailWebsite;
    [SerializeField] private GameObject newEmailAlert;
    [SerializeField] private TMPro.TextMeshProUGUI url;

    [SerializeField] private SceneTransition sceneTransition;

    [SerializeField] private TextMeshProUGUI visionText;

    private void Start()
    {
        mantleVisionTxt = GameObject.Find("MantleVisionText");
        startingPos = GetComponent<RectTransform>().anchoredPosition;
        lastPos = startingPos;
        startingScale = transform.localScale.x;
    }

    public void MinimizeWindow()
    {
        lastPos = GetComponent<RectTransform>().anchoredPosition;
        StartCoroutine(Resize(0, minimizeTo));
        // if (AudioManager.Instance.typing && name == "Cmd Prompt")
        // {
        //     AudioManager.Instance.StopLoop(SFXNAME.Typing);
        //     AudioManager.Instance.typing = true;
        // }
    }

    public void CloseWindow()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        taskbarIcon.transform.GetChild(0).gameObject.SetActive(false);
        
        //reset position & scale
        transform.localScale = new Vector3(1, 1, 1) * startingScale;
        GetComponent<RectTransform>().anchoredPosition = startingPos;
        transform.GetChild(0).gameObject.SetActive(false);
        maximized = false;
        if (AudioManager.Instance.typing && name == "Cmd Prompt")
        {
            AudioManager.Instance.StopLoop(SFXNAME.Typing);
            AudioManager.Instance.typing = true;
        }
    }

    public void OpenWindow()
    {   
        if (AudioManager.Instance.typing && name == "Cmd Prompt" && (!transform.GetChild(1).gameObject.activeSelf || transform.GetChild(1).localScale.x == 0))
            AudioManager.Instance.PlayLoop(SFXNAME.Typing);
            
        BringToFront();
        if (taskbarIcon.transform.GetChild(0).gameObject.activeSelf && transform.localScale.x <= 0) //if window minimized
        {
            if (maximized)
                StartCoroutine(Resize(maximizedSize, lastPos));
            else
                StartCoroutine(Resize(startingScale, lastPos));
        }
        else //if window closed
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        taskbarIcon.transform.GetChild(0).gameObject.SetActive(true);
        taskbarIcon.GetComponent<Animator>().Play("TaskbarIconClick");

        if (name == "DotTutorial")
            transform.GetChild(1).GetComponent<Animator>().SetTrigger("PlayTutorial");
    }

    public void BringToFront()  //separate function b/c called from button OnClick()
    {
        transform.SetSiblingIndex(transform.parent.childCount - 2);
    }

    public void Fullscreen()
    {
        if (maximized)
        {
            BringToFront();
            mantleVisionTxt.transform.SetSiblingIndex(transform.parent.childCount - 1);
            StartCoroutine(Resize(startingScale, lastPos));
        }
        else
        {
            transform.SetSiblingIndex(transform.parent.childCount - 1);
            lastPos = GetComponent<RectTransform>().anchoredPosition;
            StartCoroutine(Resize(maximizedSize, maximizeTo));
        }
        maximized = !maximized;
        transform.GetChild(0).gameObject.SetActive(maximized);
    }

    private IEnumerator Resize(float targetScale, Vector2 targetPos)
    {
        float scaleChange = targetScale - transform.localScale.x;
        Vector2 posChange = targetPos - GetComponent<RectTransform>().anchoredPosition;
        for (int i = 0; i < 10; i++)
        {
            transform.localScale += (new Vector3(1, 1, 1) * scaleChange)/10;
            GetComponent<RectTransform>().anchoredPosition += posChange/10;
            yield return new WaitForSeconds(0.005f);
        }
        transform.localScale = new Vector3(1, 1, 1) * targetScale;
    }

    public void GoToNextBattle()
    {
        AudioManager.Instance.FadeOut();
        sceneTransition.FadeToBlack();
    }

    public void SwitchTab(bool news)
    {
        newEmailAlert.SetActive(false);
        newsTab.SetActive(news);
        //newsWebsite.SetActive(news);
        emailTab.SetActive(!news);
        emailWebsite.SetActive(!news);
        if (news)
        {
            url.text = "https://www.amourtianews.com/live";
        }
        else
        {
            url.text  = "https://www.securemail.com/u/0/#inbox";
        }
    }
}
