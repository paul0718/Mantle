using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndFightCutscene : MonoBehaviour
{
    [SerializeField] private GameObject barkBubble;
    [SerializeField] private TMP_Text barkText;
    private Vector2 origSize;

    [SerializeField] private Animator[] explodeAnims;

    [SerializeField] private SceneTransition panel;


    public IEnumerator PlayEndSequence()
    {
        AudioManager.Instance.PlayOneShot(SFXNAME.Pulsing);
        barkBubble.SetActive(true);
        barkText.text = "NO! WHAT DID YOU DO?!";
        StartCoroutine(ResizeBubble());
        
        yield return new WaitForSeconds(3.0f);
        barkText.text = "You hit Vyzzar's cosmic core!";
        StartCoroutine(ResizeBubble());
        
        yield return new WaitForSeconds(3.0f);
        barkText.text = "It's going to explode!";
        StartCoroutine(ResizeBubble());
        AudioManager.Instance.PlayOneShot(SFXNAME.HospitalDestroyed);
        
        yield return new WaitForSeconds(4.0f);
        SteamIntegration.Instance.UnlockAchievement("ACH_BLOW_HOSPITAL");
        barkBubble.SetActive(false);
        explodeAnims[0].gameObject.SetActive(true);
        explodeAnims[0].SetTrigger("StartExplosion");
        AudioManager.Instance.PlayOneShot(SFXNAME.VyzzarExplosion);
        
        
        yield return new WaitForSeconds(1.0f);
        explodeAnims[1].gameObject.SetActive(true);
        explodeAnims[1].SetTrigger("StartExplosion");
        AudioManager.Instance.PlayOneShot(SFXNAME.VyzzarExplosion);
        
        yield return new WaitForSeconds(1.0f);
        explodeAnims[2].gameObject.SetActive(true);
        explodeAnims[2].SetTrigger("StartExplosion");
        AudioManager.Instance.PlayOneShot(SFXNAME.VyzzarExplosion);
        yield return new WaitForSeconds(3f);
        panel.FadeToBlack();
    }

    private IEnumerator ResizeBubble()
    {
        origSize = barkText.GetComponent<RectTransform>().sizeDelta;
        for (int i = 0; i < 30; i++) //for loop to avoid infinite loop
        {
            yield return null;
            if (barkText.textInfo.lineCount > 5)
                barkText.GetComponent<RectTransform>().sizeDelta += new Vector2(10, 0);
            else
                break;
            if (i == 29)
            {
                barkText.GetComponent<RectTransform>().sizeDelta = origSize;
            }
        }
        barkBubble.GetComponent<SpriteRenderer>().size = new Vector2(Mathf.Max(8, Mathf.Round(barkText.preferredWidth/230 + 6)), Mathf.Round(barkText.preferredHeight/30 + 3));
        barkBubble.transform.GetChild(0).localPosition = new Vector3(0.6f + barkBubble.GetComponent<SpriteRenderer>().size.x * 0.5f, 0, 0);
        barkBubble.SetActive(true);
    }
}
