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
    public List<PausePanel> panels;
    public GameObject background;
    public bool paused = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        foreach(var panel in panels)
            panel.Init();
    }
    public void ShowPanel<T>() where T : PausePanel
    {
        foreach (var p in panels)
            if (p is T)
                p.Show();
    }
    public void HidePanel<T>() where T : PausePanel
    {
        foreach (var p in panels)
            if (p is T) 
                p.Hide();
    }
    public bool GetPanelState<T>() where T :PausePanel 
    {
        foreach (var p in panels)
            if (p is T)
                return p.gameObject.activeInHierarchy;
        return false;
    }
    public T GetPanel<T>() where T :PausePanel
    {
        foreach (var p in panels)
            if (p is T)
                return p as T;
        return null;
    }
    public PausePanel GetCurrentPanel()
    {
        foreach (var p in panels)
            if (p.Active) return p;
        return null;
    }
    public void Pause()
    {
       
        if (MainMenuManager.Instance == null)
        {
            background.SetActive(true);
            Time.timeScale = 0;
            AudioManager.Instance.PauseSFX();
        }
        else
        {
            MainMenuManager.Instance.HideLogo();
        }
        paused = true;
        ShowPanel<PauseMainPanel>();
    }
    public void Resume()
    {
        AudioManager.Instance.UnPauseSFX();
        if (MainMenuManager.Instance == null)
            background.SetActive(false);
        else
            MainMenuManager.Instance.ShowLogo();
        paused = false;
        Time.timeScale = 1;
        
    }
    private void Update()
    {
        if (SequenceManager.Instance.SequenceID == 19) return;
        if (Input.GetKeyDown(KeyCode.Escape) && !paused && SceneManager.GetActiveScene().name != "MainMenu") 
        {
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && paused)
        {
            
            if (GetPanelState<PauseMainPanel>())
            {
                GetCurrentPanel().Hide();
                Resume();
            }
            else
            {
                GetCurrentPanel().Hide();
                GetPanel<PauseMainPanel>().Show();
            }
            
        }
    }
}
