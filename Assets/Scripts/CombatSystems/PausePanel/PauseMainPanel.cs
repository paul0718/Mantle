using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseMainPanel :PausePanel
{
    public Button volumeButton;
    public Button restartButton;
    public Button creditButton;
    public Button menuButton;
    public Button exitButton;
    public float startY = 600;
    public float endY = 0;
    private RectTransform rectTransform;

    public override void Hide()
    {
        if (MainMenuManager.Instance != null)
            rectTransform.DOAnchorPosY(startY, 0.2f).onComplete += base.Hide;
        else
        {
            var p = rectTransform.anchoredPosition;
            p.y = startY;
            rectTransform.anchoredPosition = p;
            base.Hide();
        }
    }
    public override void Show()
    {
        base.Show();
        rectTransform.DOAnchorPosY(endY, 0.2f).SetUpdate(true);
    }
    public void HideDown()
    {
        rectTransform.DOAnchorPosY(-startY, 0.2f).SetUpdate(true).onComplete += base.Hide;
    }
    public override void Init()
    {
        base.Init();
        rectTransform = GetComponent<RectTransform>();
        volumeButton.onClick.AddListener(() =>
        {
            PauseManager.Instance.ShowPanel<PauseVolumePanel>();
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
            base.Hide();
        });
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
        backButton.onClick.AddListener(() => {
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
            if (MainMenuManager.Instance != null)
            {
                MainMenuManager.Instance.ShowLogo();
                MainMenuManager.Instance.ActivateOtherButtons();
            }
            Hide();
            PauseManager.Instance.Resume();
        });
        creditButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
            HideDown();
            PauseManager.Instance.ShowPanel<PauseCreditPanel>();

        });
    }
}
