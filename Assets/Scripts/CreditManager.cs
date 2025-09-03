using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class CreditManager : MonoBehaviour
{
    [SerializeField] private CreditLine[] lines;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject headerPrefab;
    [SerializeField] private GameObject specialPrefab;
    [SerializeField] private RectTransform scrollParent;

    [SerializeField] private float spacing;
    [SerializeField] private float timing;

    [SerializeField] private Image logoBG;
    [SerializeField] private Image logo;

    [SerializeField] private GameObject skipText;
    private bool skipCredits = false;
    
    [SerializeField] AudioMixerGroup audioMixerGroup;
    [SerializeField] AudioClip introClip;
    [SerializeField] AudioClip[] headClips;
    [SerializeField] AudioClip bodyClip;

    private AudioSource introSource;
    private AudioSource headSource;
    private AudioSource bodySource;

    private int index = 0;
    
    private bool playingCredits;


    void Start()
    {
        if (SequenceManager.Instance.SequenceID == 19)
        {
            ShowLogo();
            introSource = CreateAudioSource("introSource", introClip, loop: false);
            headSource = CreateAudioSource("headSource", headClips[0], loop: false);
            bodySource = CreateAudioSource("bodySource", bodyClip, loop: true);
            
            introSource.outputAudioMixerGroup = audioMixerGroup;
            headSource.outputAudioMixerGroup = audioMixerGroup;
            bodySource.outputAudioMixerGroup = audioMixerGroup;
            
            introSource.Play();
        }
    }

    private void ControlMusic()
    {
        if (!introSource.isPlaying && index == 0)
        {
            if (playingCredits)
            {
                PlayBodyMusic();
            }
            else
            {
                headSource.Play();
                index = 1; 
            }
        }
        if (!headSource.isPlaying && index == 1)
        {
            if (playingCredits)
            {
                PlayBodyMusic();
            }
            else
            {
                headSource.clip = headClips[1];
                headSource.Play();
                index = 2;
            }
        }
        if (!headSource.isPlaying && index == 2)
        {
            if (playingCredits)
            {
                PlayBodyMusic();
            }
            else
            {
                headSource.clip = headClips[2];
                headSource.Play();
                index = 3;
            }
        }
        if (!headSource.isPlaying && index == 3)
        {
            if (playingCredits)
            {
                PlayBodyMusic();
            }
            else
            {
                headSource.clip = headClips[3];
                headSource.Play();
                index = 4;
            }
        }
        if (!headSource.isPlaying && index == 4)
        {
            if (playingCredits)
            {
                PlayBodyMusic();
            }
            else
            {
                headSource.clip = headClips[0];
                headSource.Play();
                index = 1;
            }
        }
    }

    private void PlayBodyMusic()
    {
        bodySource.Play();
        index = 5;
    }

    private void Update()
    {
        ControlMusic();
        if (playingCredits)
        {
            scrollParent.anchoredPosition += new Vector2(0, spacing/timing * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (skipCredits)
                {
                    SceneTransition.Instance.FadeToBlack();
                }
                else
                {
                    skipText.SetActive(true);
                    skipCredits = true;
                    StartCoroutine(SkipCredits()); 
                }
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                Time.timeScale = 3;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    private void ShowLogo()
    {
        logoBG.gameObject.SetActive(true);
        Sequence s = DOTween.Sequence();
        s.AppendInterval(2f);
        s.Append(logo.DOFade(1, 4f));
        s.AppendInterval(2f);
        s.Append(logoBG.DOFade(0, 3f));
        s.Join(logo.DOFade(0, 3f)).onComplete = TransitionToCredits;
    }

    private void TransitionToCredits()
    {
        logoBG.gameObject.SetActive(false);
    }

    public IEnumerator PlayCredits()
    {
        playingCredits = true;
        yield return new WaitUntil(() => scrollParent.GetChild(0).GetComponent<RectTransform>().position.y > -2f);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].role == "header")
            {
                GameObject newLine = Instantiate(headerPrefab, Vector2.zero, Quaternion.identity, scrollParent);
                newLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, scrollParent.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y - (i+1)*spacing);
                newLine.GetComponent<TextMeshProUGUI>().text = lines[i].name;
            }
            else
            {
                if (lines[i].role == "special")
                {
                    GameObject newLine = Instantiate(specialPrefab, Vector2.zero, Quaternion.identity, scrollParent);
                    newLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, scrollParent.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y - (i+1)*spacing);
                    newLine.GetComponent<TextMeshProUGUI>().text = lines[i].name;
                }
                else
                {
                    GameObject newLine = Instantiate(linePrefab, Vector2.zero, Quaternion.identity, scrollParent);
                    newLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, scrollParent.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y - (i+1)*spacing);
                    newLine.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lines[i].name;
                    newLine.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = lines[i].role; 
                    if (lines[i].heroName != "")
                        newLine.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "\"" + lines[i].heroName + "\"";
                    else
                        newLine.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
            yield return new WaitForSeconds(timing);
        }

        yield return new WaitUntil(() => scrollParent.localPosition.y > 6000);
            //GetChild(scrollParent.childCount-1).GetComponent<RectTransform>().position.y > 600);
        SceneTransition.Instance.FadeToBlack();
    }

    IEnumerator SkipCredits()
    {
        yield return new WaitForSeconds(4f);
        skipText.SetActive(false);
        skipCredits = false;
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




[System.Serializable]
public class CreditLine
{
    public string name;
    public string role;
    public string heroName;
    //public string heroName;
}