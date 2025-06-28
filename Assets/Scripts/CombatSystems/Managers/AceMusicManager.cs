using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AceMusicManager : MonoBehaviour
{
    public static AceMusicManager Instance {  get; private set; }
    public AudioMixerGroup audioMixerGroup;
    public AudioClip IntroClip;
    public AudioClip HeadClip;
    public AudioClip BodyClip;
    public AudioClip BodyAltClip;

    private AudioSource introSource;
    private AudioSource headSource;
    private AudioSource bodySource;
    private AudioSource bodyAltSource;

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
        if (SequenceManager.Instance.SequenceID != 17)
        {
            Destroy(gameObject);
            return;
        }
        introSource = CreateAudioSource("IntroSource", IntroClip, loop: false);
        headSource = CreateAudioSource("HeadSource", HeadClip, loop: false);
        bodySource = CreateAudioSource("BodySource", BodyClip, loop: true);
        bodyAltSource = CreateAudioSource("BodyAltSource", BodyAltClip, loop: true);
        introSource.outputAudioMixerGroup = audioMixerGroup;
        headSource.outputAudioMixerGroup = audioMixerGroup;
        bodySource.outputAudioMixerGroup = audioMixerGroup;
        bodyAltSource.outputAudioMixerGroup = audioMixerGroup;

        bodyAltSource.volume = 0f;

        introSource.Play();
        introStartTime = Time.time;
    }

    void Update()
    {
        if (!hasPlayedHead && hasStartedFight && Time.time - introStartTime >= 15f && !headSource.isPlaying)
        {
            PlayHead();
        }

        if (postBattleStarted)
        {
            bodySource.volume = Mathf.MoveTowards(bodySource.volume, 0f, Time.deltaTime * 0.5f);
            bodyAltSource.volume = Mathf.MoveTowards(bodyAltSource.volume, 1f, Time.deltaTime * 0.5f);
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

    private void PlayHead()
    {
        headSource.Play();
        hasPlayedHead = true;
        Invoke(nameof(PlayBody), HeadClip.length);
    }

    private void PlayBody()
    {
        bodySource.Play();
        bodyAltSource.Play();
    }


    public void OnFightStart()
    {
        hasStartedFight = true;
    }

    public void OnPostBattleDialogueStart()
    {
        postBattleStarted = true;
    }
}
