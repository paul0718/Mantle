using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
Usage Instructions

= Should be attached to the <World Space> Canvas with multiple FanUnitEmptyParent objects whose structure looks like the following:
    - FanUnitEmptyParent
        - Slider(temperature bar)
        - FanUnitSprite
        
See FanUnitController.cs for details
*/

public class WindGameManager : MonoBehaviour
{
    [SerializeField] private Slider mainSlider; // slider on top, shows total amount of wind
    [SerializeField] private List<FanUnit> fanUnits = new List<FanUnit>(); // List for scripts attached to fans

    private int goalLow = 0;
    private int goalHigh = 1; // target range
    [SerializeField] private int goalLowMin = 10; // minimum of total rotations we want players to complete
    [SerializeField] private int goalRange = 5;
    [SerializeField] private int maxTotal = 30;
    [SerializeField] private float[] fanPercentage = {0.7f, 0.5f, 0.4f};
    private int[] overheatRots = { 0, 0, 0 };
    private float currTotal = 0; // current and max value of the total slider, min = 0 by default
    private bool overheats = false;
    private bool hasEnded = false;

    [SerializeField] private float holdTime = 2f;
    private float inRangeTime = 0f;
    
    [SerializeField] private RectTransform indicator;

    [SerializeField] private Animator winEffectAnimator;
    
    [SerializeField] private AudioClip winSound, loseSound;
    [SerializeField] private AudioClip spinSound;
    [SerializeField] private AudioClip alarmSound;
    
    private AudioSource spinAudioSource;
    private AudioSource alarmAudioSource;
    private AudioSource endSoundAudioSource;
    
    void OnEnable() // Resets Minigame
    {
        overheats = hasEnded = false;
        currTotal = 0;
        RandomizeGoal();
        RandomizeFanOverheats();
        spinAudioSource?.Play();
    }
    
    void Start()
    {
        InitSlider();
        overheats = hasEnded = false;
        inRangeTime = 0f;
        InitOverheat();
        InitAudio();
    }
    
    void Update()
    {
        if (hasEnded || CheckAndHandleGameEnd())
        {
            return;
        }
        currTotal = 0;
        foreach (FanUnit fanUnit in fanUnits)
        {
            currTotal += fanUnit.GetCurrSpeed();
        }
        mainSlider.value = (float) currTotal; // Update Slider
        UpdateAudio();
    }
    
    // For fans
    public void SetOverheat() => overheats = true;

    void OnDisable()
    {
        alarmAudioSource.Stop();
        spinAudioSource.Stop();
        spinAudioSource.pitch = 1;
    }
    

    private void InitAudio()
    {
        // Init audio
        var audioSources = gameObject.GetComponents<AudioSource>();
        
        spinAudioSource = audioSources[0];
        spinAudioSource.clip = spinSound;
        spinAudioSource.loop = true;
        spinAudioSource.playOnAwake = false;
        spinAudioSource.Play();
        
        alarmAudioSource = audioSources[1];
        alarmAudioSource.clip = alarmSound;
        alarmAudioSource.loop = true;
        alarmAudioSource.playOnAwake = false;
        
        endSoundAudioSource = audioSources[2];
        endSoundAudioSource.loop = false;
        endSoundAudioSource.playOnAwake = false;
    }
    
    private void UpdateAudio()
    {
        spinAudioSource.pitch = 1 + Math.Min(currTotal / maxTotal, 1) * 2;
        foreach (FanUnit fanUnit in fanUnits)
        {
            if (fanUnit.IsAtLimit())
            {
                if (!alarmAudioSource.isPlaying)
                {
                    alarmAudioSource.Play();
                }
                return;
            }
        }

        if (alarmAudioSource.isPlaying)
        {
            alarmAudioSource.Stop();
        }
    }
    
    private void InitSlider()
    {
        mainSlider.value = 0f;
        mainSlider.maxValue = maxTotal;
        RandomizeGoal();
    }
    
    private void RandomizeGoal()
    {
        goalLow = Random.Range(goalLowMin, maxTotal - goalRange);
        goalHigh = goalLow + goalRange;
        
        // left, pos y, pos x = 0
        // right = 1
        // height = 14
        float sliderRange = mainSlider.maxValue - mainSlider.minValue;
        float normalizedMin = (goalLow - mainSlider.minValue) / sliderRange;
        float normalizedMax = (goalHigh - mainSlider.minValue) / sliderRange;
        
        indicator.anchorMin = new Vector2(normalizedMin, indicator.anchorMin.y); // left side of rectangle
        indicator.anchorMax = new Vector2(normalizedMax, indicator.anchorMax.y); // right side of rectangle
        indicator.localScale = Vector3.one; // fill area
    }
    
    private void InitOverheat()
    {
        for (int i = 0; i < fanUnits.Count; i++)
        {
            overheatRots[i] = Mathf.CeilToInt(fanPercentage[i] * maxTotal);
        }
        RandomizeFanOverheats();
    }
    
    private void RandomizeFanOverheats()
    {
        ShuffleOverheats();
        for (int i = 0; i < fanUnits.Count; i++)
        {
            fanUnits[i].SetOverheat(overheatRots[i]);
        }
    }

    private void ShuffleOverheats()
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < overheatRots.Length; t++ )
        {
            int tmp = overheatRots[t];
            int i = Random.Range(t, overheatRots.Length);
            overheatRots[t] = overheatRots[i];
            overheatRots[i] = tmp;
        }
    }

    private bool CheckAndHandleGameEnd()
    {
        if (overheats) // Lose due to overheat
        {
            //winEffectAnimator.SetTrigger("TriggerOverheat");
            if (alarmAudioSource.isPlaying)
            {
                alarmAudioSource.Stop();
            }
            EndsGame(false);
            return true;
        }
        
        // Update in-range timer
        if ((goalLow <= currTotal) && (currTotal <= goalHigh))
        {
            inRangeTime += Time.deltaTime;
        }
        else
        {
            inRangeTime = 0;
        }
        
        // Wins as target has been hit for a certain amount of time
        if (inRangeTime >= holdTime)
        {
            winEffectAnimator.SetTrigger("TriggerWind");
            EndsGame(true);
            return true;
        }
        return false;
    }
    
    private void EndsGame(bool playerWins)
    {
        hasEnded = true;
        foreach (FanUnit fanUnit in fanUnits) // Paul: should we keep the fans spinning?
        {
            fanUnit.Freeze();
        }
        
        endSoundAudioSource.clip = playerWins ? winSound : loseSound;
        endSoundAudioSource.PlayOneShot(endSoundAudioSource.clip);

        var battle = BattleSequenceManager.Instance.minigames;
        if (playerWins)
        {
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Wind Breeze", "Win");
            }
            GridManager.Instance.UpdateDotPosition(battle[2].WinEffect, GridManager.MiniGame.Vent, playerWins);
        }
        else
        {
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Wind Breeze", "Lose");
            }
            GridManager.Instance.UpdateDotPosition(battle[2].LoseEffect, GridManager.MiniGame.Vent, playerWins);
        }
        
        GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(playerWins);
        BarkManager.Instance.ShowGameBark("Wind", playerWins);
        //GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Wind", winStr);
    }
}
