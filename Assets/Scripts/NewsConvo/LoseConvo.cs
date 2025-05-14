using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoseConvo : MonoBehaviour
{
    public Dictionary<string, Dictionary<string, string>> loseData = new Dictionary<string, Dictionary<string, string>>();    
    
    [SerializeField] private List<TMP_Text> newsCaptions;
    private int captionIndex;
    private int currRow;
    [SerializeField] private float typingSpeed;


    [SerializeField] private Animator casterAnim;


    void Start()
    {
        var data = SequenceManager.Instance.CurrentBattle;
        var lose = Resources.Load<TextAsset>(data.LoseSceneDialoguePath).text;
        loseData = DialogUtils.ParseJsonToDictionary(lose);
        UpdateText();
    }

    
    public void UpdateText()
    {
        currRow++;
        if (!(loseData[currRow.ToString()]["Name"] == "END"))
            StartCoroutine(TypeNewsText(loseData[currRow.ToString()]["Dialogue"]));
    }

    IEnumerator TypeNewsText(string fullText)
    {
        string currentWord = "";
        newsCaptions[captionIndex].text += " ";
        bool isAddingTag = false;
        AudioManager.Instance.PlayLoop(SFXNAME.mumble);
        foreach (char letter in fullText)
        {
            casterAnim.SetBool("isTalking", true);
            if (letter == '<' || isAddingTag)
            {
                isAddingTag = true;
                newsCaptions[captionIndex].text += letter;
                if (letter == '>')
                {
                    isAddingTag = false;
                }
            }
            /*else if (picMarkers.Contains(letter))
            {
                SetPictures(letter);
            }*/
            else
            {
                AudioManager.Instance.SetSFXVolume(SFXNAME.mumble, 0.6f);
                currentWord += letter;
                char[] punctuation = new char[]{' ', '.', '!', '?'};
                if (System.Array.Exists(punctuation, c => c == letter))
                {
                    newsCaptions[captionIndex].text += currentWord;
                    currentWord = "";
                    if (letter != ' ')
                        yield return new WaitForSeconds(0.5f);
                }
                if (newsCaptions[captionIndex].textBounds.extents.y*2f > 30)
                {
                    //move overhanging word to next line
                    string txt = newsCaptions[captionIndex].text;
                    int value = 0;
                    for (int i = txt.Length-2; i > 0; i--)
                    {
                        if (txt[i] == ' ')
                        {
                            value = i;
                            break;
                        }
                    }
                    string txtToMove = txt.Substring(value+1);
                    newsCaptions[captionIndex].text = txt.Substring(0, value);
                    if (captionIndex == 0)
                    {
                        captionIndex = 1;
                    }
                    else 
                    {
                        string textToMove = newsCaptions[1].text;
                        newsCaptions[0].text = textToMove;
                        newsCaptions[1].text = "";
                    }
                    newsCaptions[captionIndex].text += txtToMove;
                }
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        casterAnim.SetBool("isTalking", false);
        AudioManager.Instance.StopLoop(SFXNAME.mumble);
        yield return null;
        if (newsCaptions[captionIndex].textBounds.extents.y*2f > 30)
        {
            //move overhanging word to next line
            string txt = newsCaptions[captionIndex].text;
            int value = 0;
            for (int i = txt.Length-2; i > 0; i--)
            {
                if (txt[i] == ' ')
                {
                    value = i;
                    break;
                }
            }
            string txtToMove = txt.Substring(value+1);
            newsCaptions[captionIndex].text = txt.Substring(0, value);
            if (captionIndex == 0)
            {
                captionIndex = 1;
            }
            else 
            {
                string textToMove = newsCaptions[1].text;
                newsCaptions[0].text = textToMove;
                newsCaptions[1].text = "";
            }
            newsCaptions[captionIndex].text += txtToMove;
        }

        yield return new WaitForSeconds(1.5f);
        UpdateText();
    }
}
