using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public Button backButton;
    public bool Active { get => gameObject.activeInHierarchy; }
    public virtual void Init() {
        backButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.PauseMenu);
            Hide();
        });
    }
    public virtual void Show() { gameObject.SetActive(true); }
    public virtual void Hide() { gameObject.SetActive(false); }
}
