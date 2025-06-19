using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCreditPanel : PausePanel
{
    public float startY = 600;
    public float endY = 0;
    private RectTransform rectTransform;
    public override void Show()
    {
        base.Show();
        rectTransform.DOAnchorPosY(endY, 0.2f).SetUpdate(true);

    }
    public override void Hide()
    {
        rectTransform.DOAnchorPosY(startY, 0.2f).SetUpdate(true).onComplete += base.Hide;
    }
    public override void Init()
    {
        base.Init();
        rectTransform = GetComponent<RectTransform>();
        backButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
            Hide();
            PauseManager.Instance.ShowPanel<PauseMainPanel>();
        });
    }
}
