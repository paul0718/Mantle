using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private bool playingCredits;


    void Start()
    {
        if (SequenceManager.Instance.SequenceID == 19)
            ShowLogo();
    }

    private void Update()
    {
        if (playingCredits)
        {
            scrollParent.anchoredPosition += new Vector2(0, spacing/timing * Time.deltaTime);
            /*if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                Time.timeScale = 3;
            }
            else
            {
                Time.timeScale = 1;
            }*/
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
        Debug.Log("Credits over!");
        SceneTransition.Instance.FadeToBlack();
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