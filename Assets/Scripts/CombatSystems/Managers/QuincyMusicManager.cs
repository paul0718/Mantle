using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class QuincyMusicManager : MonoBehaviour
{
    public static QuincyMusicManager Instance { get; private set; }
    public AudioMixerGroup audioMixerGroup;
    public AudioClip preIntroClip;
    public AudioClip introAClip;
    public AudioClip introBClip;
    public AudioClip headClip;
    public AudioClip bodyClip;

    private AudioSource preIntroSource;
    private AudioSource introASource;
    private AudioSource introBSource;
    private AudioSource headSource;
    private AudioSource bodySource;

    private float introStartTime;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (SequenceManager.Instance.SequenceID != 6)
        {
            Destroy(gameObject);
            return;
        }
        preIntroSource = CreateAudioSource("preIntroSource", preIntroClip, loop: false);
        introASource = CreateAudioSource("introASource", introAClip, loop: false);
        introBSource = CreateAudioSource("introBSource", introBClip, loop: false);
        headSource = CreateAudioSource("headSource", headClip, loop: false);
        bodySource = CreateAudioSource("bodySource", bodyClip, loop: true);
        preIntroSource.outputAudioMixerGroup = audioMixerGroup;
        introASource.outputAudioMixerGroup = audioMixerGroup;
        introBSource.outputAudioMixerGroup = audioMixerGroup;
        headSource.outputAudioMixerGroup = audioMixerGroup;
        bodySource.outputAudioMixerGroup = audioMixerGroup;

        preIntroSource.Play();
        introStartTime = Time.time;
    }
    private int index = 0;
    
    void Update()
    {
        if (!preIntroSource.isPlaying && index == 0)
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headSource.Play();
                index = 3;
            }
            else
            {
                introASource.Play();
                index = 1;
            }
        }
        if (!introASource.isPlaying && index == 1)
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headSource.Play();
                index = 3;
            }
            else
            {
                introBSource.Play();
                index = 2;
            }
        }
        if (!introBSource.isPlaying && index == 2)
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headSource.Play();
                index = 3;
            }
            else
            {
                introASource.Play();
                index = 1;
            }
        }
        if (!headSource.isPlaying && index == 3)
        {
            bodySource.Play();
            index = 4;
        }
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
}
