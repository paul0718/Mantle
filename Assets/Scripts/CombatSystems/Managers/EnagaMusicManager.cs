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
        if (!preIntroASource.isPlaying && index == 0) 
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headBSource.Play();
                index = 5;
            }
            else
            {
                preIntroBSource.Play();
                index = 1;
            }
        }
        if (!preIntroBSource.isPlaying && index == 1)
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headASource.Play();
                index = 4;
            }
            else
            {
                introASource.Play();
                index = 2;
            }
        }
        if (!introASource.isPlaying && index == 2)
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headBSource.Play();
                index = 5;
            }
            else
            {
                introBSource.Play();
                index = 3;
            }
        }
        if (!introBSource.isPlaying && index == 3)
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headASource.Play();
                index = 4;
            }
            else
            {
                introASource.Play();
                index = 2;
            }
        }
        if (!headASource.isPlaying && index == 4)
        {
            bodySource.Play();
            index = 6;
        }
        if (!headBSource.isPlaying && index == 5)
        {
            bodySource.Play();
            index = 6;
        }
    }
}
