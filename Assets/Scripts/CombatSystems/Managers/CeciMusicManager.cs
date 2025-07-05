using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class CecilMusicManager : MonoBehaviour
{
    public static CecilMusicManager Instance { get; private set; }
    public AudioMixerGroup audioMixerGroup;
    public AudioClip preIntroClip;
    public AudioClip introClip;
    public AudioClip headClip;
    public AudioClip bodyClip;

    private AudioSource preIntroSource;
    private AudioSource introSource;
    private AudioSource headSource;
    private AudioSource bodySource;

    private float introStartTime;
    private bool hasStartedFight = false;
    private bool hasPlayedHead = false;
    private bool postBattleStarted = false;
    private bool isFadingOut = false;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (SequenceManager.Instance.SequenceID != 12)
        {
            Destroy(gameObject);
            return;
        }
        preIntroSource = CreateAudioSource("preIntroSource", preIntroClip, loop: false);
        introSource = CreateAudioSource("introSource", introClip, loop: false);
        headSource = CreateAudioSource("headSource", headClip, loop: true);
        bodySource = CreateAudioSource("bodySource", bodyClip, loop: true);
        preIntroSource.outputAudioMixerGroup = audioMixerGroup;
        introSource.outputAudioMixerGroup = audioMixerGroup;
        headSource.outputAudioMixerGroup = audioMixerGroup;
        bodySource.outputAudioMixerGroup = audioMixerGroup;

        bodySource.volume = 0f;

        preIntroSource.Play();
        introStartTime = Time.time;
    }
    private int index = 0;
    void Update()
    {
        Debug.Log(StateManager.Instance.currentState);
        if (!preIntroSource.isPlaying && index == 0)
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headSource.Play();
                index = 2;
            }
            else
            {
                introSource.Play();
                index = 1;
            }
        }
        if (!introSource.isPlaying && index == 1)
        {
            if (StateManager.Instance.currentState == StateManager.BattleState.Start)
            {
                headSource.Play();
                index = 2;
            }
            else
            {
                introSource.Play();
                index = 1;
            }
        }
        if (!headSource.isPlaying && index == 2)
        {
            bodySource.Play();
            index = 3;
        }
        if (!bodySource.isPlaying && index == 3)
        {
            bodySource.Play();
            index = 3;
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
