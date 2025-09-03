using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CutsceneSequenceManager : MonoBehaviour
{
    [SerializeField] private GameObject aceDeadComic;
    [SerializeField] private GameObject mantleDeadComic;
    
    public AudioMixerGroup audioMixerGroup;
    public AudioClip loseMantleHeadClip;
    public AudioClip loseMantleBodyClip;
    public AudioClip winMantleHeadClip;
    public AudioClip winMantleBodyClip;

    private AudioSource headSource;
    private AudioSource bodySource;

    private void Start()
    {
        if (SequenceManager.Instance.SequenceID == 18)
        {
            if (SequenceManager.Instance.aceIsDead)
            {
                headSource = CreateAudioSource("headSource", winMantleHeadClip, loop: false);
                bodySource = CreateAudioSource("introASource", winMantleBodyClip, loop: true);
                mantleDeadComic.SetActive(false);
                aceDeadComic.SetActive(true);
            }
            else
            {
                headSource = CreateAudioSource("headSource", loseMantleHeadClip, loop: false);
                bodySource = CreateAudioSource("introASource", loseMantleBodyClip, loop: true);
                aceDeadComic.SetActive(false);
                mantleDeadComic.SetActive(true);
            }
        }
        
        headSource.outputAudioMixerGroup = audioMixerGroup;
        bodySource.outputAudioMixerGroup = audioMixerGroup;

        audioMixerGroup.audioMixer.SetFloat("BGM", -7);

        headSource.Play();
    }

    private int index = 0;
    void Update()
    {
        if (!headSource.isPlaying && index == 0)
        {
            bodySource.Play();
            index = 1;
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
