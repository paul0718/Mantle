using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    private Animator splashAnim;
    // Start is called before the first frame update
    void Start()
    {
        splashAnim = GetComponent<Animator>();
        splashAnim.SetTrigger("PlaySplashAnim");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void PlaySplashAudio()
    {
        AudioManager.Instance.PlayOneShot(SFXNAME.SplashScreen);
    }

    public void FadeOutBGM()
    {
        AudioManager.Instance.FadeOut();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
