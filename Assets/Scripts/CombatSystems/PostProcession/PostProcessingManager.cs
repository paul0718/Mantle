using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PostProcessingManager : MonoBehaviour
{
    private static Glitch g;
    public static void ShowGlitch(float duration = 0.5f)
    {
        g = Camera.main.gameObject.GetComponent<Glitch>();
        g.enabled = true;
        g.glitch = 1;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(duration);
        AudioManager.Instance.PlayLoop(SFXNAME.Glitch);
        AudioManager.Instance.SetSFXVolume(SFXNAME.Glitch, 0.3f);
        sequence.AppendCallback(() => EndGlitch());
    }

    private static void EndGlitch()
    {
        g.glitch = 0;
        AudioManager.Instance.StopLoop(SFXNAME.Glitch);
    }
}
