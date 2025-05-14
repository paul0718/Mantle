using DG.Tweening;
using JsonModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioWave : MonoBehaviour
{
    public static AudioWave Instance { get; private set; }
    public Button tuneButton;
    public Button checkButton;
    public AudioSource[] audioSources;
    public AudioWaveButton[] audioWaveButtons;
    public Animator audioVFX;

    private Sequence sequence;
    [SerializeField] private AudioSource bgmAudio;
    private float originalBGMVol;

    public TMP_Text resultText;

    private List<int> targetTune = new List<int>();
    private List<int> currentTune = new List<int>();

    public Image leftPipe;
    public Sprite normalLeftPipe;
    public Sprite highlightLeftPipe;
    public Image[] miniLights;

    public AudioClip[] normalAudioClips;
    public AudioClip[] evilAudioClips;

    private void Start()
    {
        originalBGMVol = bgmAudio.volume;
        Instance = this;
        tuneButton.onClick.AddListener(TuneButtonFunction);
        checkButton.onClick.AddListener(CheckButtonFunction);
        for (int i = 0; i < 3; i++)
        {
            audioSources[i].clip = SequenceManager.Instance.SequenceID >= 10 ? evilAudioClips[i] : normalAudioClips[i];
            audioSources[i].volume = SequenceManager.Instance.SequenceID >= 10 ? 1 : 0.3f;
;        }
    }
    private void OnEnable()
    {
        ResetAudioWave();
    }
    private void TuneButtonFunction()
    {
        bgmAudio.volume = originalBGMVol;
        if (targetTune.Count == 0)
        {
            for (int i = 0; i < audioWaveButtons.Length; i++)
                targetTune.Add(Random.Range(0, 3));
        }

        bgmAudio.volume /= 2;
        if (bgmAudio.volume <= 0.1f)
            bgmAudio.volume = 0.1f;
        sequence = DOTween.Sequence();
        foreach(var index in targetTune)
        {
            var p = index;
            sequence.AppendCallback(() => audioSources[p].Play());
            sequence.AppendInterval(1f);
        }
        leftPipe.sprite = highlightLeftPipe;
        tuneButton.interactable = false;
        Invoke("ActivateTuneButton", 6f);
        checkButton.interactable = true;
    }

    private void ActivateTuneButton()
    {
        leftPipe.sprite = normalLeftPipe;
        tuneButton.interactable = true;
    }
    
    private void CheckButtonFunction()
    {
        CancelInvoke();
        sequence.Kill();
        currentTune.Clear();
        //resultText.transform.parent.gameObject.SetActive(true);
        //resultText.text = "Checking...";
        foreach(var b in audioWaveButtons)
        {
            currentTune.Add(b.state);
        }

        int numMiss = -2;
        checkButton.interactable = false;
        bool succeeded = true;
        for (int i = 0; i < targetTune.Count; i++)
        {
            int a = targetTune[i];
            int b = currentTune[i];
            if (a != b)
            {
                numMiss = i;
                succeeded = false;
                break;
            }
        }
        sequence = DOTween.Sequence();
        int c = 0;
        foreach(var index in currentTune)
        {
            var p = index;
            
            sequence.AppendCallback(() => audioSources[p].Play());
            sequence.AppendCallback(() => CheckingAnimation(c++, numMiss));
            sequence.AppendInterval(1f);
        }
        
        var battle = SequenceManager.Instance?.CurrentBattle;
        sequence.AppendCallback(() =>
        {
            CheckingAnimation(-1, numMiss);
            //resultText.text = succeeded ? "Win!" : "Lose!";
            bgmAudio.volume = originalBGMVol;
            if (succeeded)
            {
                audioVFX.gameObject.SetActive(true);
                audioVFX.SetTrigger("PlayAudioVFX");
                if (MetricManagerScript.instance != null)
                { 
                    MetricManagerScript.instance.LogString("Audio Wave", "Win");
                }
                GridManager.Instance?.UpdateDotPosition(battle.Minigames[4].WinEffect, GridManager.MiniGame.Audio, succeeded);
            }
            else
            {
                if (MetricManagerScript.instance != null)
                { 
                    MetricManagerScript.instance.LogString("Audio Wave", "Lose");
                }
                GridManager.Instance?.UpdateDotPosition(battle.Minigames[4].LoseEffect, GridManager.MiniGame.Audio, succeeded);
            }
            
            
           BarkManager.Instance.ShowGameBark("Audio Wave", true);
        });

    }
    private void CheckingAnimation(int index, int numMiss)
    {
        foreach (var m in miniLights)
        {
            m.color = Color.white;
        }
        for (int i = 0; i < miniLights.Length; i++) 
        {
            if (i == index)
            {
                if (i == numMiss)
                    miniLights[i].color = Color.red;
                miniLights[i].gameObject.SetActive(true);
            }
            else
                miniLights[i].gameObject.SetActive(false);
        }
    }
    public void ResetAudioWave()
    {
        audioVFX.gameObject.SetActive(false);
        targetTune.Clear();
        currentTune.Clear();
        tuneButton.interactable = true;
        checkButton.interactable = false;
        foreach(var b in audioWaveButtons)
        {
            b.ResetHandle();
        }
        resultText.transform.parent.gameObject.SetActive(false);
    }

    public void PlayOneShot(int index)
    {
        audioSources[index].Play();
    }
}
