using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public Button backButton;
    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
            PauseManager.Instance.HideSubPanel();
        });
    }
    public virtual void Init() { gameObject.SetActive(true); }
    public virtual void Hide() { gameObject.SetActive(false); }
}
