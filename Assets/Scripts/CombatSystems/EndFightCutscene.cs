using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndFightCutscene : MonoBehaviour
{
    [SerializeField] private GameObject bubbleGO;

    [SerializeField] private TMP_Text bubbleText;

    [SerializeField] private Animator[] explodeAnims;

    [SerializeField] private AudioSource beepSound;

    [SerializeField] private AudioSource explodeSound;

    [SerializeField] private GameObject transitionPanel;

    public IEnumerator PlayEndSequence()
    {
        bubbleGO.SetActive(true);
        beepSound.Play();
        bubbleText.text = "NO! WHAT DID YOU DO?!";
        yield return new WaitForSeconds(3.0f);
        bubbleText.text = "You shot my arsenic armor!";
        yield return new WaitForSeconds(3.0f);
        bubbleText.text = "It's going to explode!";
        yield return new WaitForSeconds(3.0f);
        bubbleGO.SetActive(false);
        explodeAnims[0].gameObject.SetActive(true);
        explodeAnims[0].SetTrigger("StartExplosion");
        yield return new WaitForSeconds(1.0f);
        explodeAnims[1].gameObject.SetActive(true);
        explodeAnims[1].SetTrigger("StartExplosion");
        yield return new WaitForSeconds(1.0f);
        explodeAnims[2].gameObject.SetActive(true);
        explodeAnims[2].SetTrigger("StartExplosion");
        yield return new WaitForSeconds(2.5f);
        TransitionToNewsConvo();
    }

    public void TransitionToNewsConvo()
    {
        transitionPanel.GetComponent<Image>().DOFade(1, 1.5f).onComplete = CallNextScene;
    }

    void CallNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayExplosionSound()
    {
        explodeSound.Play();
    }
}
