using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public GameObject pauseMainPanel;
    public RectTransform pauseGameObject;
    public List<PausePanel> panels;
    public Button volumeButton;
    public Button brightnessButton;
    public Button restartButton;
    public Button creditButton;
    public Button creditBackButton;
    public Button menuButton;
    public Button exitButton;
    public Button resumeButton;

    public bool paused = false;
    public float startY = 600;
    public float endY = -852 / 2;

    [SerializeField] private GameObject panoplyObject;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        volumeButton.onClick.AddListener(() =>
        {
            ShowSubPanel<PauseVolumePanel>();
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
        });
        //brightnessButton.onClick.AddListener(() => ShowSubPanel<PauseBrightnessPanel>());
        restartButton.onClick.AddListener(() => {
            Time.timeScale = 1;
            SceneTransition.Instance.SwitchScene("BattleScene");
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
        });
        menuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneTransition.Instance.SwitchScene("MainMenu");
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
        });
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
        }
        );
        resumeButton.onClick.AddListener(() => {
            AudioManager.Instance.UnPauseSFX();
            if (MainMenuManager.Instance != null)
            {
                MainMenuManager.Instance.ShowLogo();
                MainMenuManager.Instance.ActivateOtherButtons();
                pauseGameObject.DOAnchorPosY(startY, 0.2f);
            }
            else
            {
                var p = pauseGameObject.anchoredPosition;
                p.y = startY;
                pauseGameObject.anchoredPosition = p;
                HideMainPanel();
            }
            paused = false;
            Time.timeScale = 1; 
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
        }) ;
        if (MainMenuManager.Instance != null)
        {
            creditBackButton.onClick.AddListener(() =>
            {
                pauseGameObject.DOAnchorPosY(endY, 0.2f).SetUpdate(true);
                MainMenuManager.Instance.creditTransform.DOAnchorPosY(1000, 0.2f).SetUpdate(true);
                AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
            });
            creditButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
                pauseGameObject.DOMoveY(-1000, 0.2f).SetUpdate(true);
                MainMenuManager.Instance.creditTransform.DOAnchorPosY(0, 0.2f).SetUpdate(true);

            });
        }
        else
        {
            creditButton.gameObject.SetActive(false);
        }
    }

    public void ShowMainPanel()
    {
        if (SceneManager.GetActiveScene().name!="MainMenu"&& SequenceManager.Instance.cutsceneSeq.Contains(SequenceManager.Instance.SequenceID))
        {
            panoplyObject.SetActive(false);   
        }
        pauseMainPanel.gameObject.SetActive(true);
       
    }
    public void HideMainPanel()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" && SequenceManager.Instance.cutsceneSeq.Contains(SequenceManager.Instance.SequenceID))
        {
            panoplyObject.SetActive(true);   
        }
        pauseMainPanel.gameObject.SetActive(false);
        
    }
    public void ShowSubPanel<T>() where T : PausePanel
    {
        HideMainPanel();
        foreach (var p in panels)
            if (p is T)
                p.Init();
            else
                p.Hide();
    }
    public void HideSubPanel()
    {
        ShowMainPanel();
        foreach (var p in panels)
            p.Hide();
    }
    public void Pause()
    {
        AudioManager.Instance.PauseSFX();
        paused = true;
        if (MainMenuManager.Instance != null)
            MainMenuManager.Instance.HideLogo();
        pauseGameObject.DOAnchorPosY(endY, 0.2f).onComplete += () =>
        {
            Time.timeScale = 0;
        };
        ShowMainPanel();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !paused && SceneManager.GetActiveScene().name != "MainMenu") 
        {
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && paused)
        {
            resumeButton.onClick.Invoke();
        }
    }
}
