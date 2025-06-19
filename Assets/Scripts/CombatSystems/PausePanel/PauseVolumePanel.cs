using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class PauseVolumePanel : PausePanel
{
    public Slider BGMSlider;
    public Slider SFXSlider;
    public override void Show()
    {
        base.Show();
        var audioMixer = AudioManager.Instance.audioMixer;

        if (audioMixer.GetFloat("BGM", out float bgmValue))
        {
            BGMSlider.value = Mathf.Pow(10, bgmValue / 20f);
        }
        if (audioMixer.GetFloat("SFX", out float sfxValue))
        {
            SFXSlider.value = Mathf.Pow(10, sfxValue / 20f);
        }

        BGMSlider.onValueChanged.AddListener(a =>
        {
            float dB = Mathf.Log10(Mathf.Max(a, 0.0001f)) * 20;
            audioMixer.SetFloat("BGM", dB);
        });
        SFXSlider.onValueChanged.AddListener(a =>
        {
            float dB = Mathf.Log10(Mathf.Max(a, 0.0001f)) * 20;
            audioMixer.SetFloat("SFX", dB);
        });
    }
    public override void Hide()
    {
        base.Hide();
        PlayerPrefs.SetFloat("BGM",BGMSlider.value);
        PlayerPrefs.SetFloat("SFX", SFXSlider.value);
        PauseManager.Instance.ShowPanel<PauseMainPanel>();
    }

}
