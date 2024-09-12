using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewsConvoManager : MonoBehaviour
{
    GoogleSheetsDB googleSheetsDB;
    public GoogleSheet txtSheet;

    [SerializeField] private Animator casterAnim;

    [SerializeField] private TMP_Text newsText;
    [SerializeField] private TMP_Text convoText;
    [SerializeField] private GameObject newsWindow;
    [SerializeField] private GameObject convoWindow;
    [SerializeField] private GameObject scrollArea;

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;

    [SerializeField] private float typingSpeed;
    private bool isAddingRichTextTag = false;

    private int currRow = 1;

    private bool convoDone = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        googleSheetsDB = gameObject.GetComponent<GoogleSheetsDB>();
        googleSheetsDB.OnDownloadComplete += DoneDownload;
    }
    
    void DoneDownload()
    {
        Debug.Log("done download");
    }

    public void UpdateText()
    {
        //read from news conversation spreadsheet
        int txtSheetIndex = googleSheetsDB.sheetTabNames.IndexOf("NewsConvo");

        txtSheet = googleSheetsDB.dataSheets[txtSheetIndex];

        //check who is speak and call the correct function to display their dialogue
        if (txtSheet.GetRowData(currRow.ToString(), "Name") == "NewsAnchor")
        {
            WriteNewsDialogue(currRow.ToString(), "Dialogue");
        }
        else if(txtSheet.GetRowData(currRow.ToString(), "Name") == "Leo" && !convoDone)
        {
            WriteConvoDialogue(currRow.ToString(), "Dialogue", false);
        }
        else if (txtSheet.GetRowData(currRow.ToString(), "Name") == "Mantle" && !convoDone)
        {
            WriteConvoDialogue(currRow.ToString(), "Dialogue", true);
        }
        else if(txtSheet.GetRowData(currRow.ToString(), "Name") == "END")
        {
            convoDone = true;
            currRow = 1;
            playButton.SetActive(true);
            Debug.Log("done");
        }
        currRow++;
    }

    //display news conversation to the correct location
    public void WriteNewsDialogue(string row, string column)
    {
        newsText.text = "";
        StartCoroutine(TypeGeneralText(newsText, txtSheet.GetRowData(row, column), true));
    }

    //display conversation dialogue, how to do it depends on if it's Mantle or Leo
    public void WriteConvoDialogue(string row, string column, bool Mantle)
    {
        string temp = txtSheet.GetRowData(row, column) + "\n\n";

        //Resize scroll area
        float goalHeight = Mathf.Max(460, convoText.textBounds.extents.y*2.4f); //TODO: don't hard code this?
        scrollArea.GetComponent<RectTransform>().sizeDelta = new Vector3(740, goalHeight, 1);
        scrollArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (goalHeight-460)/2);
        //Auto scroll text as it hits bottom of screen
        scrollArea.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Mathf.Max(0, (goalHeight-460)/2) - 10);

        if (Mantle)
        {
            StartCoroutine(MantleText(convoText, temp));
            //convoText.text += txtSheet.GetRowData(row, column) + "\n";
        }
        else
        {
            StartCoroutine(TypeGeneralText(convoText, temp, false));
        }
    }
    
    //Type out the text for News Caster and for Leo
    IEnumerator TypeGeneralText(TMP_Text textField, string fullText, bool news)
    {
        foreach (char letter in fullText)
        {
            if (news)
                yield return new WaitUntil(() => newsWindow.activeSelf && newsWindow.transform.parent.localScale.x >= 1.3f);
            else
                yield return new WaitUntil(() => convoWindow.activeSelf && convoWindow.transform.parent.localScale.x >= 1);
            if (news)
            {
                casterAnim.SetBool("isTalking", true);
            }
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                textField.text += letter;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                textField.text += letter; 
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        if (news)
        {
            casterAnim.SetBool("isTalking", false);
            yield return new WaitForSeconds(0.5f);
            UpdateText();
        }
        else
        {
            if (txtSheet.GetRowData(currRow.ToString(), "Name") == "NewsAnchor")
            {
                yield return new WaitForSeconds(0.5f);
            }
            UpdateText();
        }
    }

    
    //Type out text for Mantle, after doing the "..." portion
    IEnumerator MantleText(TMP_Text textField, string finalText)
    {
        string waitText = "<align=right>...";
        for (int i = 0; i < 3; i++)
        {
            foreach (var c in waitText)
            {
                if (c == '<' || isAddingRichTextTag)
                {
                    isAddingRichTextTag = true;
                    textField.text += c;
                    if (c == '>')
                    {
                        isAddingRichTextTag = false;
                    }
                }
                else
                {
                    textField.text += c; 
                    yield return new WaitForSeconds(4 * typingSpeed);
                }
            }
            convoText.text = convoText.text.Substring(0, convoText.text.Length - 3);
        }
        convoText.text += finalText;
        if (txtSheet.GetRowData(currRow.ToString(), "Name") == "NewsAnchor")
        {
            yield return new WaitForSeconds(0.5f);
        }
        UpdateText();
    }

    public void StartNewsConvo()
    {
        UpdateText();
        playButton.SetActive(false);
        pauseButton.SetActive(false);
    }
}
