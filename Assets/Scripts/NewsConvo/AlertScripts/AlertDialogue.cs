using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlertDialogue : MonoBehaviour
{
    GoogleSheetsDB googleSheetsDB;
    public GoogleSheet txtSheet;

    [SerializeField] private TMP_Text convoText;

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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateText();
        }
    }

    public void UpdateText()
    {
        int txtSheetIndex = googleSheetsDB.sheetTabNames.IndexOf("Alert");

        txtSheet = googleSheetsDB.dataSheets[txtSheetIndex];

        if(txtSheet.GetRowData(currRow.ToString(), "Name") == "Leo" && !convoDone)
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
            Debug.Log("done");
        }

        currRow++;
    }

    public void WriteConvoDialogue(string row, string column, bool Mantle)
    {
        string temp = txtSheet.GetRowData(row, column) + "\n";
        Debug.Log(temp);
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
    
    IEnumerator TypeGeneralText(TMP_Text textField, string fullText, bool news)
    {
        foreach (char letter in fullText)
        {
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

        UpdateText();
    }

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
                    yield return new WaitForSeconds(5 * typingSpeed);
                }
            }
            convoText.text = convoText.text.Substring(0, convoText.text.Length - 3);
        }
        convoText.text += finalText;

        yield return new WaitForSeconds(1.5f);
        UpdateText();
    }

    public void StartAlertConvo()
    {
        UpdateText();
    }
}
