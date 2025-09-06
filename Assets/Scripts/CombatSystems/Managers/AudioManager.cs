using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioMixer audioMixer;
    public AudioSource BGMAudioSource;
    public List<AudioSource> SFXAudioSources;
    public Dictionary<SFXNAME, AudioClip> sfxs= new Dictionary<SFXNAME, AudioClip>();
    public AudioLibrary SFXLibrary;
    public AudioLibrary BGMLibrary;
    public List<AudioLibrary> battleBGM;
    private Sequence sequence;
    private int BGMIndex = 0;
    private bool nextFlag = false;
    private AudioSource AvailableSFXAudioSource { get => SFXAudioSources.Find(a => !a.isPlaying); }
    private void Awake()
    {
        Instance = this;
        foreach (SFXNAME e in Enum.GetValues(typeof(SFXNAME)))
        {
            sfxs.Add(e, null);
        }
        if (SFXLibrary.library != null)
        {
            foreach (var s in SFXLibrary.library)
            {
                //Debug.Log(s.name);
                string validName = SFXEnumGenerator.MakeValidEnumName(s.name);
                if (IsEnumMember(validName, out SFXNAME e))
                {
                    if (sfxs.ContainsKey(e))
                    {
                        sfxs[e] = s;
                    }
                }
            }
        }
    }
    public static bool IsEnumMember<T>(string value, out T enumValue) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, out enumValue))
        {
            return true;
        }

        enumValue = default;
        return false;
    }
    private void Start()
    {
        if (SequenceManager.Instance.SequenceID != 18 && SequenceManager.Instance.SequenceID != 1)
        {
            var bgm = PlayerPrefs.GetFloat("BGM", -1);
            var sfx = PlayerPrefs.GetFloat("SFX", -1);
            if (bgm != -1)
                SetMasterVolumn("BGM",bgm);
            if (sfx != -1)
                SetMasterVolumn("SFX",sfx);

            FadeIn();
        }
    }
   
    public void PlayOneShot(SFXNAME name, float volume = -1)
    {
        var audioSource = SFXAudioSources[0];
        if (audioSource == null)
        {
            Debug.LogWarning("Audio Source Not Available"); 
            return;
        }
        if (volume == -1)
        {
            float dB = 0;
            audioMixer.SetFloat("SFX1", dB);
        }
        else
        {
            float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
            audioMixer.SetFloat("SFX1", dB);
        }
        audioSource.PlayOneShot(sfxs[name]);
    }
    public void PlayLoop(SFXNAME name)
    {
        var audioSource = AvailableSFXAudioSource;
        if (audioSource == null)
        {
            Debug.LogWarning("Audio Source Not Available");
            return;
        }
        audioSource.clip = sfxs[name];
        audioSource.loop = true;
        audioSource.Play();
    }
    public void StopLoop(SFXNAME name)
    {
        var audioSource = SFXAudioSources.Find(a => a.clip == sfxs[name]);
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }
    private Dictionary<AudioSource, Func<bool>> stopFunctions = new Dictionary<AudioSource, Func<bool>>();
    public void PlayLoop(SFXNAME name, Func<bool> stopCondition)
    {
        var audioSource = AvailableSFXAudioSource;
        audioSource.clip = sfxs[name];
        audioSource.loop = true;
        audioSource.Play();
        stopFunctions.Add(audioSource, stopCondition);
    }
    public void SetSFXVolume(SFXNAME name, float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        

        for (int i = 0; i < SFXAudioSources.Count; i++)
        {
            if (SFXAudioSources[i].clip == sfxs[name])
            {
                audioMixer.SetFloat("SFX" + (i + 1), dB);
            }
        }
    }
    public void SetBGMLibrary(string name)
    {
        AudioLibrary newBGM = battleBGM.FirstOrDefault(s => s.name == name);
        if (newBGM == null)
            Debug.LogWarning("No BGM matching name " + name + ". Use Vyzzar's BGM.");
        else
            BGMLibrary = newBGM;
    }
    public void SetMasterVolumn(string type, float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat(type, dB);
    }
    public void PlayNextBGM()
    {
        if (SequenceManager.Instance.SequenceID == 14 && BGMIndex == 0)  
        {
            int currentTime = (int)(BGMAudioSource.time * 10000.0f);
            int waitTime = 28325 - currentTime % 28325;
            float realTime = waitTime / 10000.0f;
            
            sequence = DOTween.Sequence();
            sequence.AppendInterval(realTime);
            sequence.AppendCallback(() =>
            {
                if (SceneManager.GetActiveScene().name == "BattleScene")
                {
                    Debug.Log("Stop!");
                    BGMAudioSource.Stop();
                }
            });
            sequence.SetUpdate(true);
        }
        nextFlag = true;
    }
    public void PlayNextPhase()
    {
        if (SequenceManager.Instance.SequenceID == 19)
        {
            int currentTime = (int)(BGMAudioSource.time * 10000.0f);
            int waitTime = 23760 - currentTime % 23760;
            float realTime = waitTime / 10000.0f;
            sequence = DOTween.Sequence();
            sequence.AppendInterval(realTime);
            sequence.AppendCallback(() =>
            {
                if (SceneManager.GetActiveScene().name == "BattleScene")
                {
                    BGMIndex = 5;
                    BGMAudioSource.Stop();
                } 
                    
              
            });
            sequence.SetUpdate(true);
        }
    }
    public void FadeIn()
    {
        audioMixer.DOSetFloat("Master", 0, 1.5f);
        float startVolume = 0.001f; 
        float targetVolume = 1.0f;

        if (SceneManager.GetActiveScene().name == "BattleScene" || SceneManager.GetActiveScene().name == "MainMenu")
            audioMixer.SetFloat("Master", targetVolume);
        else
        {
            DOTween.To(() => startVolume, x => {
                startVolume = x;
                audioMixer.SetFloat("Master", Mathf.Log10(startVolume) * 20);
            }, targetVolume, 1.5f);
        }
    }
    public void FadeOut()
    {
        //BGMAudioSource.DOFade(0, 1.5f);
        float startVolume = 1f;
        float targetVolume = 0.001f; 
        DOTween.To(() => startVolume, x => {
            startVolume = x;
            audioMixer.SetFloat("Master", Mathf.Log10(startVolume) * 20);
        }, targetVolume, 1.5f);

    }
    private void Update()
    {
        if (SequenceManager.Instance != null && SequenceManager.Instance.SequenceID == 1 && SceneManager.GetActiveScene().name=="MainMenu")
            return;
        if (SequenceManager.Instance != null && SequenceManager.Instance.SequenceID == 18 && SceneManager.GetActiveScene().name=="S18Comic")
            return;
        if (SequenceManager.Instance != null && SequenceManager.Instance.SequenceID == 17 && SceneManager.GetActiveScene().name=="BattleScene")
            return;
        if (SequenceManager.Instance != null && SequenceManager.Instance.SequenceID == 12 && SceneManager.GetActiveScene().name == "BattleScene")
            return;
        if (SequenceManager.Instance != null && SequenceManager.Instance.SequenceID == 6 && SceneManager.GetActiveScene().name == "BattleScene")
            return;
        if (SequenceManager.Instance != null && SequenceManager.Instance.SequenceID == 3 && SceneManager.GetActiveScene().name == "BattleScene")
            return;
        if (BGMLibrary != null && !BGMAudioSource.isPlaying) 
        {
            if (nextFlag)
            {
                if (SequenceManager.Instance.SequenceID == 19 && BGMIndex == 4)
                {
                    BGMIndex = 1;
                }
                else if(SequenceManager.Instance.SequenceID == 19 && BGMIndex == 5)
                        {

                }
                else
                {
                    BGMIndex++;
                }
                    
                nextFlag = false;
            }
            BGMAudioSource.clip = BGMLibrary.library[BGMIndex];
            if (SceneManager.GetActiveScene().name != "DateScene")
            {
                BGMAudioSource.Play();
            }
            if (SequenceManager.Instance.SequenceID == 19 && BGMIndex <= 4) 
            {
                PlayNextBGM();
            }

        }
        if (stopFunctions != null)
        {
            stopFunctions = stopFunctions
            .Where(pair => pair.Key.gameObject.activeInHierarchy)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var pair in stopFunctions)
            {
                if (pair.Value.Invoke())
                {
                    pair.Key.Stop();
                    pair.Key.loop = false;
                    break;
                }
            }
        }
    }
    public void PauseSFX()
    {
        foreach (var a in SFXAudioSources)
            a.Pause();
    }
    public void UnPauseSFX()
    {
        foreach (var a in SFXAudioSources)
            a.UnPause();
    }
    private void OnDestroy()
    {
        Debug.Log("Audio Manager Destroyed");
        sequence.Kill(false);
    }
}
