using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BarkManager : MonoBehaviour
{
    public TMP_Text barkText;
    public GameObject barkBubble;
    private Vector2 origSize;
    private bool skip;
    public enum MultipleBarkType
    {
        Intro,
        Win,
        Lose,
        Special,
        Destruct
    }
    public static BarkManager Instance { get; private set; }


    public void Start()
    {
        Instance = this;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && SequenceManager.Instance.SequenceID != 17) 
            skip = true;
    }

    public void ShowGameBark(string gameName, bool win)
    {
        Sequence sequence = DOTween.Sequence();
        var data = BattleSequenceManager.Instance.barkData;
        barkText.text = data[gameName][(win ? "W" : "L") + Random.Range(0, 3)];
        StartCoroutine(ResizeBubble());
        sequence.AppendInterval(4f);
        sequence.AppendCallback(() =>
        {
            barkBubble.SetActive(false);
            barkText.GetComponent<RectTransform>().sizeDelta = origSize;
        });
    }

    public IEnumerator ShowIntroOutroBark(MultipleBarkType type, bool kill = false, Action onComplete = null)
    {
        skip = false;
        List<string> dialogueList = new List<string>();

        var data = type switch
        {
            MultipleBarkType.Win => BattleSequenceManager.Instance.outroData,
            MultipleBarkType.Lose => BattleSequenceManager.Instance.outroData,
            MultipleBarkType.Destruct => BattleSequenceManager.Instance.destructData,
            MultipleBarkType.Special => BattleSequenceManager.Instance.specialData,
            _ => BattleSequenceManager.Instance.introData
        };

        string tableName = type switch
        {
            MultipleBarkType.Win => kill ? "Kill Dialogue" : "Capture Dialogue",
            MultipleBarkType.Lose => "Player Lose Dialogue",
            _ => "Dialogue"
        };

        foreach (var entry in data)
        {
            if (entry.Value.TryGetValue(tableName, out var dialogue))
            {
                dialogueList.Add(dialogue);
            }
        }

        barkBubble.SetActive(true);
        foreach (var dialogue in dialogueList)
        {
            barkText.text = dialogue;
            StartCoroutine(ResizeBubble());

            float timer = 3f;
            while (!skip && timer > 0f)
            {
                yield return new WaitForSeconds(0.1f);
                timer -= 0.1f;
            }
            skip = false;
            barkText.GetComponent<RectTransform>().sizeDelta = origSize;
        }
        barkBubble.SetActive(false);

        switch (type)
        {
            case MultipleBarkType.Intro:
                StateManager.Instance.DoneIntroDialogue();
                break;
            case MultipleBarkType.Win:
                AudioManager.Instance.FadeOut();
                SceneTransition.Instance.FadeToBlack();
                break;
            case MultipleBarkType.Lose:
                StateManager.Instance.playingDialogue = false;
                break;
            case MultipleBarkType.Special:
                break;
            case MultipleBarkType.Destruct:
                break;
        }

        onComplete?.Invoke();
    }

    public IEnumerator ResizeBubble()
    {
        origSize = barkText.GetComponent<RectTransform>().sizeDelta;
        for (int i = 0; i < 30; i++) //for loop to avoid infinite loop
        {
            yield return null;
            if (barkText.textInfo.lineCount > 5)
                barkText.GetComponent<RectTransform>().sizeDelta += new Vector2(10, 0);
            else
                break;
            if (i == 29)
            {
                barkText.GetComponent<RectTransform>().sizeDelta = origSize;
            }
        }
        if (barkText.text == "Ready? This is your final battle.")
        {
            barkText.GetComponent<RectTransform>().sizeDelta -= new Vector2(20, 0);
            yield return null;
        }
        barkBubble.GetComponent<SpriteRenderer>().size = new Vector2(Mathf.Max(8, Mathf.Round(barkText.preferredWidth/230 + 6.5f)), Mathf.Round(barkText.preferredHeight/30 + 3));
        barkBubble.transform.GetChild(0).localPosition = new Vector3(0.6f + barkBubble.GetComponent<SpriteRenderer>().size.x * 0.5f, 0, 0);
        barkBubble.SetActive(true);
    }
}
