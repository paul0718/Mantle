using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    private Animator splashAnim;

    [SerializeField] private GameObject warningText;
    // Start is called before the first frame update
    void Start()
    {
        splashAnim = GetComponent<Animator>();
        StartCoroutine(EndWarning());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator EndWarning()
    {
        yield return new WaitForSeconds(4f);
        warningText.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        splashAnim.SetTrigger("PlaySplashAnim");
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
