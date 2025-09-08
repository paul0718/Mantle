using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance {  get; private set; }
    [SerializeField] private RectTransform headlineTxt1;
    [SerializeField] private RectTransform headlineTxt2;
    public RectTransform logo;
    [SerializeField] private float headlineSpeed;
    
    //temp audio fix
    [SerializeField] private AudioSource bgmSource;
    private bool bgmStarted = false;

    public Button newGameButton;
    public Button continueButton;
    public Button quitButton;
    public Button optionButton;

    public RectTransform creditTransform;

    public GameObject steamPage;

    private readonly int logoNormalY = -25;
    private readonly int logoHideY = -1025;
    
    public AudioMixerGroup audioMixerGroup;
    public AudioClip headClip;
    public AudioClip bodyClip;
    private AudioSource headSource;
    private AudioSource bodySource;
    private int index = 0;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        bgmStarted = false;
        
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            int savedID = PlayerPrefs.GetInt("SequenceID");
            int savedDateNum = PlayerPrefs.GetInt("DateNum");
            continueButton.gameObject.SetActive(savedID != 0);
            newGameButton.onClick.AddListener(() =>
            {
                NewGame();
                AudioManager.Instance.PlayOneShot(SFXNAME.MainMenu);
            });
            continueButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayOneShot(SFXNAME.MainMenu);
                SequenceManager.Instance.dateNum = savedDateNum;
                if (savedID > 17)
                {
                    savedID = 17;
                }
                SceneTransition.Instance.SwitchScene(savedID);

            });
            quitButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayOneShot(SFXNAME.MainMenu);
                Application.Quit();
            });
            optionButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayOneShot(SFXNAME.MainMenu);
                if (!PauseManager.Instance.paused)
                {
                    PauseManager.Instance.Pause();
                    newGameButton.interactable = false;
                    continueButton.interactable = false;
                }
            });
            
            headSource = CreateAudioSource("headSource", headClip, loop: false);
            bodySource = CreateAudioSource("bodySource", bodyClip, loop: true);
            
            headSource.outputAudioMixerGroup = audioMixerGroup;
            bodySource.outputAudioMixerGroup = audioMixerGroup;

            audioMixerGroup.audioMixer.SetFloat("MenuBGM", -7);
            
            headSource.Play();
        }
    }

    private void Update()
    {
        //Scrolling headlines
        headlineTxt1.anchoredPosition += new Vector2(-headlineSpeed*Time.deltaTime, 0);
        headlineTxt2.anchoredPosition += new Vector2(-headlineSpeed*Time.deltaTime, 0);
        if (headlineTxt1.anchoredPosition.x < -(headlineTxt1.GetComponent<TextMeshProUGUI>().preferredWidth-200))
        {
            float newX = headlineTxt2.anchoredPosition.x + headlineTxt2.GetComponent<TextMeshProUGUI>().preferredWidth/2 + headlineTxt1.GetComponent<TextMeshProUGUI>().preferredWidth/2;
            headlineTxt1.anchoredPosition = new Vector2(newX+17, headlineTxt1.anchoredPosition.y);
        }
        if (headlineTxt2.anchoredPosition.x < -(headlineTxt2.GetComponent<TextMeshProUGUI>().preferredWidth-200))
        {
            float newX = headlineTxt1.anchoredPosition.x + headlineTxt1.GetComponent<TextMeshProUGUI>().preferredWidth/2 + headlineTxt2.GetComponent<TextMeshProUGUI>().preferredWidth/2;
            headlineTxt2.anchoredPosition = new Vector2(newX+17, headlineTxt2.anchoredPosition.y);
        }

        if (!headSource.isPlaying && index == 0)
        {
            bodySource.Play();
            index = 1;
        }

        // if (SceneManager.GetActiveScene().name == "MainMenu")
        // {
        //     if (!bgmStarted && bgmSource.clip!=null)
        //     {
        //         StartCoroutine(GoToBodyBGM());
        //         bgmStarted = true;
        //     }
        // }
    }

    private void NewGame()
    {
        SequenceManager.Instance.dateNum = 0;
        AudioManager.Instance.FadeOut();
        SceneTransition.Instance.FadeToBlack();
    }
    public void ShowLogo()
    {
        logo.DOAnchorPosY(logoNormalY, 0.2f).SetUpdate(true);
    }
    public void HideLogo()
    {
        logo.DOAnchorPosY(logoHideY, 0.2f).SetUpdate(true);
    }

    IEnumerator GoToBodyBGM()
    {
        yield return new WaitForSeconds(bgmSource.clip.length/2);
        AudioManager.Instance.PlayNextBGM();
        Debug.Log("play body");
    }

    public void ActivateOtherButtons()
    {
        newGameButton.interactable = true;
        continueButton.interactable = true;
    }
    
    private AudioSource CreateAudioSource(string name, AudioClip clip, bool loop)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(this.transform);
        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = loop;
        return src;
    }
}
