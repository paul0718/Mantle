using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnagaMusicManager : MonoBehaviour
{
    public static EnagaMusicManager Instance { get; private set; }
    public AudioMixerGroup audioMixerGroup;
    public AudioClip preIntroAClip;
    public AudioClip preIntroBClip;
    public AudioClip introAClip;
    public AudioClip introBClip;
    public AudioClip headAClip;
    public AudioClip headBClip;
    public AudioClip bodyClip;

    private AudioSource preIntroASource;
    private AudioSource preIntroBSource;
    private AudioSource introASource;
    private AudioSource introBSource;
    private AudioSource headASource;
    private AudioSource headBSource;
    private AudioSource bodySource;
    
    private double nextScheduleTime;
    private bool useScheduled = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (SequenceManager.Instance.SequenceID != 3)
        {
            Destroy(gameObject);
            return;
        }
        
        preIntroASource = CreateAudioSource("preIntroASource", preIntroAClip, loop: false);
        preIntroBSource = CreateAudioSource("preIntroBSource", preIntroBClip, loop: false);

        introASource = CreateAudioSource("introASource", introAClip, loop: false);
        introBSource = CreateAudioSource("introBSource", introBClip, loop: false);

        headASource = CreateAudioSource("headASource", headAClip, loop: false);
        headBSource = CreateAudioSource("headBSource", headBClip, loop: false);

        bodySource = CreateAudioSource("bodySource", bodyClip, loop: true);

        preIntroASource.outputAudioMixerGroup = audioMixerGroup;
        preIntroBSource.outputAudioMixerGroup = audioMixerGroup;
        introASource.outputAudioMixerGroup = audioMixerGroup;
        introBSource.outputAudioMixerGroup = audioMixerGroup;
        headASource.outputAudioMixerGroup = audioMixerGroup;
        headBSource.outputAudioMixerGroup = audioMixerGroup;
        bodySource.outputAudioMixerGroup = audioMixerGroup;

        preIntroASource.Play();
        index = 0;
    }
    private AudioSource CreateAudioSource(string name, AudioClip clip, bool loop)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(this.transform);
        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = loop;
        return src;
    }
    private int index = 0;
    
    void Update()
    {
        if (!useScheduled)
        {
            if (!preIntroASource.isPlaying && index == 0)
            {
                if (StateManager.Instance.currentState == StateManager.BattleState.Start)
                {
                    headBSource.Play();
                    index = 5;

                    nextScheduleTime = AudioSettings.dspTime + headBSource.clip.length;
                }
                else
                {
                    preIntroBSource.Play();
                    index = 1;

                    nextScheduleTime = AudioSettings.dspTime + preIntroBSource.clip.length;
                }

                // After this transition, switch to scheduled mode
                useScheduled = true;
            }
        }
        else
        {
            // Gapless scheduling for everything after
            double dspTime = AudioSettings.dspTime;

            if (dspTime >= nextScheduleTime - 0.05)
            {
                switch (index)
                {
                    case 1: // after preIntroB
                        if (StateManager.Instance.currentState == StateManager.BattleState.Start)
                        {
                            ScheduleNext(headASource, 4);
                        }
                        else
                        {
                            ScheduleNext(introASource, 2);
                        }
                        break;

                    case 2:
                        if (StateManager.Instance.currentState == StateManager.BattleState.Start)
                        {
                            ScheduleNext(headBSource, 5);
                        }
                        else
                        {
                            ScheduleNext(introBSource, 3);
                        }
                        break;

                    case 3:
                        if (StateManager.Instance.currentState == StateManager.BattleState.Start)
                        {
                            ScheduleNext(headASource, 4);
                        }
                        else
                        {
                            ScheduleNext(introASource, 2);
                        }
                        break;

                    case 4:
                    case 5:
                        ScheduleNext(bodySource, 6);
                        break;
                }
            }
        }
    }

    void ScheduleNext(AudioSource nextSource, int nextIndex)
    {
        // Schedule to start exactly when the previous clip ends
        nextSource.PlayScheduled(nextScheduleTime);
        index = nextIndex;

        // Update the next schedule time for the following clip
        nextScheduleTime += nextSource.clip.length;
    }

}
