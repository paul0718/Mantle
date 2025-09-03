using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class ConvoManager : MonoBehaviour
{
    private DesktopSequenceManager desktopSequence;
    public string currentConvo;
    
    //temp audio fix
    //Fixed! ;)

    [Header("References")]
    [SerializeField] private AlertManager alertManager;
    [SerializeField] private GameObject toNextScene;
    [SerializeField] private Animator casterAnim;
    [SerializeField] private CreditManager creditManager;

    [Header("News Text")]
    [SerializeField] private TMP_Text newsTitle;
    [SerializeField] private TMP_Text newsText;
    [SerializeField] private TMP_Text convoTextLeft;
    [SerializeField] private TMP_Text convoTextRight;
    [SerializeField] private GameObject newsWindow;
    [SerializeField] private GameObject convoWindow;
    [SerializeField] private TMP_Text[] articleTexts;

    [Header("Pictures")]
    [SerializeField] private GameObject acePic;
    [SerializeField] private GameObject harbPic;
    [SerializeField] private GameObject breakNewsPic;
    [SerializeField] private GameObject hospitalPic;
    [SerializeField] private GameObject newsFrame;
    
    [Header("Captions")]
    [SerializeField] private List<TMP_Text> newsCaptions;
    [SerializeField] private GameObject captionObj;
    private int captionIndex;

    [Header("Scroll")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject scrollArea;
    private float goalHeight;
    [SerializeField] private float scrollPadding;
    private float scrollHeight;
    private float textYPos;
    private bool firstScroll;

    [Header("Emails")]
    [SerializeField] private GameObject[] aceEmails;
    private int emailIndex;
    private bool firstEmailScroll = true;
    [SerializeField] private GameObject currentEmail;
    [SerializeField] private GameObject emailSidebar;
    [SerializeField] private GameObject newEmailAlert;
    [SerializeField] private Color readEmailColor;

    [Header("News Objects")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject newsHover;
    [SerializeField] private GameObject headlineText;
    [SerializeField] private GameObject headlineTextPrefab;
    private Tween headlineOUT;
    private Tween headlineIN;

    [Header("News Info")]
    [SerializeField] private float typingSpeed;

    [SerializeField] private int currRow;
    private bool MantleLast;
    private bool firstRow = true;
    private bool convoDone = false;
    private bool endOnConvoFinish;
    [SerializeField] private bool paused = true;
    [SerializeField] private bool firstPlay = true;
    public int timesPlayed;
    
    private int convoStart = 1;
    private int convoEnd;
    private bool writingConvo;
    private bool connecting;
    [HideInInspector] public bool newsWait;
    private bool ignoreHover;

    private char[] picMarkers = new[] { '(', ')', '{', '}', '~', '@' };

    [SerializeField] private ScreenshotToQuad screenCrack;

    public void Start()
    {
        scrollHeight = scrollArea.GetComponent<RectTransform>().sizeDelta.y;
        textYPos = scrollArea.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y;
        Cursor.visible = true;
        desktopSequence = DesktopSequenceManager.Instance;
        if (!desktopSequence.isAlertActive)
        {
            SetUpNewsDetails();
        }
        if (SequenceManager.Instance.SequenceID == 16)
        {
            PlayPauseNewsConvo(true);
        }
        else if (SequenceManager.Instance.SequenceID == 19)
        {
            //UpdateText();
        }
        else
        {
            StartCoroutine(ConnectionLog());
        }

        //make sure correct # of emails display
        if (SequenceManager.Instance.SequenceID == 16)
        {
            return;
        }
        if (SequenceManager.Instance.SequenceID > 10)
        {
            StartCoroutine(SendEmail(false));
            StartCoroutine(SendEmail(false));
            StartCoroutine(SendEmail(false));
        }
        else if (SequenceManager.Instance.SequenceID > 7)
        {
            StartCoroutine(SendEmail(false));
            StartCoroutine(SendEmail(false));
        }
        else if (SequenceManager.Instance.SequenceID > 4)
        {
            StartCoroutine(SendEmail(false));
        }

        if (SequenceManager.Instance.SequenceID == 19)
            StartCoroutine(SendEmail(true));
    }

    private IEnumerator ConnectionLog()
    {
        connecting = true;
        yield return new WaitForSeconds(2);
        string heroName = (SequenceManager.Instance.SequenceID <= 4) ? "HeroRobot.Ver.1.0.23 connected" : "Mantle connected";
        convoTextLeft.text += ("<color=#FFFFFF>" + heroName + "\n");
        if (SequenceManager.Instance.SequenceID != 16)
        {
            convoTextRight.text += ("<color=#202E37>" + heroName + "\n"); 
            yield return new WaitForSeconds(2);
            convoTextLeft.text += "Leo_User connected\n\n</color>";
            convoTextRight.text += "Leo_User connected\n\n</color>";
        }
        else
        {
            convoTextLeft.text += "</color>";
            convoTextRight.text += ("<color=#000000>" + heroName + "\n</color>");
            yield return new WaitForSeconds(3);
        }
        
        connecting = false;
        if (SequenceManager.Instance.SequenceID == 11)
            UpdateText();
    }

    public void UpdateText()
    {   
        currRow++;
        //check who is speaking and call the correct function to display their dialogue
        if (desktopSequence.sheetData[currRow.ToString()]["Name"] == "NewsAnchor" || desktopSequence.sheetData[currRow.ToString()]["Name"] == "Reporter")
        {
            StartCoroutine(TypeNewsText(desktopSequence.sheetData[currRow.ToString()]["NewsDialogue"]));
        }
        else if (desktopSequence.sheetData[currRow.ToString()]["Name"] == "Mantle" || desktopSequence.sheetData[currRow.ToString()]["Name"] == "Leo")
        {
            WriteConvoDialogue(false);
        }
        else if (desktopSequence.sheetData[currRow.ToString()]["Name"] == "Headline")
        {
            SetHeadline();
        }
        else if (desktopSequence.sheetData[currRow.ToString()]["Name"] == "Email")
        {
            if(!convoDone)
                StartCoroutine(SendEmail(true));
            
            playButton.SetActive(true);
            paused = true;
            currRow = 0;
            convoDone = true;
            casterAnim.SetBool("isTalking", false);
        }
        else if(desktopSequence.sheetData[currRow.ToString()]["Name"] == "END")
        {
            if(SequenceManager.Instance.SequenceID != 19)
                StartCoroutine(EndScene(alertManager.alertActive));
        }
    }

    public IEnumerator EndScene(bool isAlert)
    {
        endOnConvoFinish = false;
        if (SequenceManager.Instance.SequenceID == 16)
        {
            GameObject.Find("TVScreen").SetActive(false);
            convoStart = 1;
            convoEnd = 7;
            scrollHeight = 980;
            StartCoroutine(ConnectionLog());
            convoTextRight.color = new Color(1, 0.7793231f, 0);
            convoTextLeft.color = new Color(1, 0.7793231f, 0);
            WriteConvoDialogue(true);
        }
        else
        {
            yield return new WaitUntil(() => !writingConvo);
            yield return new WaitForSeconds(4);
            
            string signOff = (isAlert) ? "-Preparing for Launch-" : "-Preparing for Recharge-";
            convoTextLeft.text += ("\n\n<color=#FFFFFF>" + signOff);
            convoTextRight.text += ("\n\n<color=#202E37>" + signOff);
            scrollPadding = 100;
            ScrollText();

            toNextScene.transform.SetAsLastSibling();
            toNextScene.SetActive(true);
            if (desktopSequence.isAlertActive)
            {
                toNextScene.transform.GetChild(0).gameObject.SetActive(true);
                toNextScene.transform.GetChild(0).DOLocalMove(new Vector2(675, -374), 0.5f).SetEase(Ease.Linear);
            }
            else if (!desktopSequence.isAlertActive)
            {
                toNextScene.transform.GetChild(1).gameObject.SetActive(true);
                toNextScene.transform.GetChild(1).DOLocalMove(new Vector2(675, -374), 0.5f).SetEase(Ease.Linear);
            }
        }
    }
    
    //Type out the text for News Caster
    IEnumerator TypeNewsText(string fullText)
    {
        //wait until convo catches up
        if (newsWait && timesPlayed < 2)
        {
            yield return new WaitUntil(() => !writingConvo);
            newsWait = false;
        }
        string currentWord = "";
        newsCaptions[captionIndex].text += " ";
        bool isAddingTag = false;
        bool addingGlitch = false;
        string glitchText = "";
        AudioManager.Instance.PlayLoop(SFXNAME.mumble);
        foreach (char letter in fullText)
        {
            //pause conversation when window minimized or closed
            yield return new WaitUntil(() => newsWindow.activeSelf && newsWindow.transform.parent.localScale.x >= 1.3f && !paused);
            
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
            else if (letter == '=')
            {
                if (addingGlitch)
                {
                    float time = float.Parse(glitchText);
                    PostProcessingManager.ShowGlitch(time);
                }
                addingGlitch = !addingGlitch;
                glitchText = "";
            }
            else if (addingGlitch)
            {
                glitchText += letter;
            }
            else if (picMarkers.Contains(letter))
            {
                SetPictures(letter);
            }
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
                //newsCaptions[captionIndex].text += letter;
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

        CheckConvoTriggers(currRow);
        yield return new WaitForSeconds(1.5f);
        UpdateText();
    }
    

    void CheckConvoTriggers(int row, bool emailConvo=false)
    {
        if (desktopSequence.sheetData[(row).ToString()]["Trigger"] != "" && (timesPlayed < 2 || emailConvo))
        {
            string trigger = desktopSequence.sheetData[(row).ToString()]["Trigger"];
            if (trigger[0] == '*')
                newsWait = true;
            trigger = trigger.Replace("*", "");
            if (trigger.Contains('-'))
            {
                convoStart = int.Parse(trigger.Split('-')[0]);
                convoEnd = int.Parse(trigger.Split('-')[1]);
            }
            else
            {
                convoStart = int.Parse(trigger);
                convoEnd = int.Parse(trigger);
            }
            if (emailConvo)
                endOnConvoFinish = true;
            WriteConvoDialogue(true);
        }
        else if (emailConvo)
        {
            StartCoroutine(EndScene(alertManager.alertActive));
        }
    }

    //display conversation dialogue, how to do it depends on if it's Mantle or Leo
    public void WriteConvoDialogue(bool simultaneous)
    {
        writingConvo = simultaneous;
        bool Mantle = false;
        string temp = "";
        if (simultaneous)
        {
            Mantle = desktopSequence.sheetData[convoStart.ToString()]["ConvoName"] == "Mantle";
            temp = desktopSequence.sheetData[convoStart.ToString()]["ConvoDialogue"];
        }
        else
        {
            Mantle = desktopSequence.sheetData[currRow.ToString()]["Name"] == "Mantle";
            temp = desktopSequence.sheetData[currRow.ToString()]["ConvoDialogue"];
        }

        //spacing for same speaker vs different speaker
        string spacing;
        if (firstRow)
            spacing = "";
        else
            spacing = "\n\n";

        if (Mantle)
            StartCoroutine(MantleText(spacing, temp, simultaneous));
        else
            StartCoroutine(TypeLeoText(spacing+temp, firstRow, simultaneous));
        
        MantleLast = Mantle;
        firstRow = false;
    }

    //Type out the text for Leo
    IEnumerator TypeLeoText(string fullText, bool firstLine, bool simultaneous)
    {
        yield return new WaitUntil(() => !connecting);
        yield return new WaitForSeconds(2f);
        //yield return new WaitUntil(() => convoWindow.activeSelf && convoWindow.transform.parent.localScale.x > 0.5f);
        if (firstLine)
            convoTextRight.text += "<color=#202E37>";
        else
            convoTextRight.text += "<color=#202E37>\n";
        fullText += "<align=left>";
        int index = 0;
        bool isAddingTag = false;
        bool addingGlitch = false;
        string glitchText = "";
        AudioManager.Instance.PlayLoop(SFXNAME.Typing);
        AudioManager.Instance.SetSFXVolume(SFXNAME.Typing, 0.2f);
        foreach (char letter in fullText)
        {
            //pause conversation when window minimized or closed
            // yield return new WaitUntil(() => convoWindow.activeSelf && convoWindow.transform.parent.localScale.x >= 1);
            
            if (letter == '<' || isAddingTag)
            {
                isAddingTag = true;
                convoTextLeft.text += letter;
                if (letter == '>')
                {
                    isAddingTag = false;
                }
            }
            else if (letter == '=')
            {
                if (addingGlitch)
                {
                    float time = float.Parse(glitchText);
                    PostProcessingManager.ShowGlitch(time);
                }
                addingGlitch = !addingGlitch;
                glitchText = "";
            }
            else if (addingGlitch)
            {
                glitchText += letter;
            }
            else
            {
                index++;
                convoTextLeft.text += letter;
                if (index > 1) //skip first \n
                    convoTextRight.text += letter;
                if (convoTextLeft.textBounds.extents.y*2f + scrollPadding != goalHeight) //if we go to a new line
                {
                    scrollPadding = 30;
                    ScrollText();
                }
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        AudioManager.Instance.StopLoop(SFXNAME.Typing);
        yield return null;
        if (convoTextLeft.textBounds.extents.y*2f + scrollPadding != goalHeight) //check one more time
        {
            scrollPadding = 30;
            ScrollText();
        }
        convoTextRight.text += "</color>";
        if (simultaneous)
        {
            convoStart++;
            if (convoStart <= convoEnd)
                WriteConvoDialogue(true);
            else
            {
                writingConvo = false;
                if (endOnConvoFinish)
                    StartCoroutine(EndScene(alertManager.alertActive));
            }
        }
        else
            UpdateText();
    }
    
    //Type out text for Mantle, after doing the "..." portion
    IEnumerator MantleText(string spacing, string finalText, bool simultaneous)
    {
        yield return new WaitUntil(() => !connecting);
        //yield return new WaitUntil(() => convoWindow.activeSelf && convoWindow.transform.parent.localScale.x > 0.5f);
        convoTextRight.text += spacing;
        string waitText = "<align=right>...";
        scrollPadding = 100;
        ScrollText();
        bool isAddingTag = false;
        for (int i = 0; i < 3; i++)
        {
            foreach (var c in waitText)
            {
                if (c == '<' || isAddingTag)
                {
                    isAddingTag = true;
                    convoTextRight.text += c;
                    if (c == '>')
                    {
                        isAddingTag = false;
                    }
                }
                else
                {
                    convoTextRight.text += c;
                    yield return new WaitForSeconds(4 * typingSpeed);
                }
            }
            convoTextRight.text = convoTextRight.text.Substring(0, convoTextRight.text.Length - 3);
        }
        if (SequenceManager.Instance.SequenceID != 16)
            convoTextLeft.text += spacing + "<color=#202E37>" + finalText + "</color>";
        convoTextRight.text += "<align=right>" + finalText;
        scrollPadding = 100;
        AudioManager.Instance.PlayOneShot(SFXNAME.MessageAlert, 0.4f);
        ScrollText();
        if (SequenceManager.Instance.SequenceID == 16)
            yield return new WaitForSeconds(2);
        if (simultaneous)
        {
            convoStart++;
            if (convoStart <= convoEnd)
                WriteConvoDialogue(true);
            else
            {
                writingConvo = false;
                if (SequenceManager.Instance.SequenceID == 16)
                    StartCoroutine(Seq16EndScene());
                else if (endOnConvoFinish)
                    StartCoroutine(EndScene(alertManager.alertActive));
            }
        }
        else
            UpdateText();
    }

    private IEnumerator Seq16EndScene()
    {
        yield return new WaitForSeconds(5f);
        screenCrack.CaptureScreenshot();
    }

    public void PlayPauseNewsConvo(bool restart)
    {
        playButton.SetActive(false);
        if (SequenceManager.Instance.SequenceID == 19)
        {
            creditManager.StartCoroutine(creditManager.PlayCredits());
        }
        if (restart || firstPlay)
        {
            currRow = 0;
            timesPlayed++;
            foreach (TextMeshProUGUI caption in newsCaptions)
                caption.text = "";
            captionIndex = 0;
            AudioManager.Instance.StopLoop(SFXNAME.Typing);
        }
        paused = !paused;
        newsHover.transform.GetChild(0).gameObject.SetActive(!paused); //pause symbol
        newsHover.transform.GetChild(1).gameObject.SetActive(paused); //play symbol
        casterAnim.SetBool("isTalking", (!newsWait && !paused));
        if (paused)
        {
            AudioManager.Instance.SetSFXVolume(SFXNAME.mumble, 0.0f);
            if (headlineIN != null)
                headlineIN.Pause();
            if (headlineOUT != null)
                headlineOUT.Pause();
        }
        else
        {
            if (headlineIN != null)
                headlineIN.Play();
            if (headlineOUT != null)
                headlineOUT.Play();
        }
        if (restart || firstPlay)
        {
            UpdateText();
            firstPlay = false;
        }
        ignoreHover = false;
        HoverNews(paused);
        ignoreHover = !paused; //hide hover on upause, show on pause
    }

    private void ScrollText()
    {
        if (SequenceManager.Instance.SequenceID == 16)
            scrollPadding += 50;
        float maxExtents = Mathf.Max(convoTextLeft.textBounds.extents.y, convoTextRight.textBounds.extents.y);
        float diff = Mathf.Max(scrollHeight, maxExtents*2 + scrollPadding) - goalHeight; 
        goalHeight = Mathf.Max(scrollHeight, maxExtents*2 + scrollPadding);
        if (goalHeight > scrollHeight && (scrollRect.verticalNormalizedPosition < 0.01f || !firstScroll))
        {
            firstScroll = true;
            scrollArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (goalHeight-scrollHeight)/2);
        }
        else
        {
            scrollArea.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -diff/2);
        }
        scrollArea.GetComponent<RectTransform>().sizeDelta = new Vector3(740, goalHeight, 1);
        //Auto scroll text as it hits bottom of screen
        RectTransform txt1 = scrollArea.transform.GetChild(0).GetComponent<RectTransform>();
        txt1.anchoredPosition = new Vector2(txt1.anchoredPosition.x, Mathf.Max(0, (goalHeight-scrollHeight)/2) - 10);
        RectTransform txt2 = scrollArea.transform.GetChild(1).GetComponent<RectTransform>();
        txt2.anchoredPosition = new Vector2(txt2.anchoredPosition.x, Mathf.Max(0, (goalHeight-scrollHeight)/2) - 10);
    }

    private IEnumerator SendEmail(bool notif)
    {
        if (notif && SequenceManager.Instance.SequenceID != 19)
            yield return new WaitUntil(() => convoStart == convoEnd);
        else
            yield return null;
        if (SequenceManager.Instance.SequenceID == 7 && notif)
            yield return new WaitForSeconds(9);
        if (SequenceManager.Instance.SequenceID == 10 && notif)
            yield return new WaitForSeconds(5f);

        if (emailIndex >= aceEmails.Length)
        {
            Debug.LogError("Email index out of range!");
        }
        else
        {
            int visibleEmails = 1;
            foreach (Transform child in emailSidebar.transform)
            {
                if (child.gameObject.activeSelf)
                    visibleEmails++;
            }
            emailSidebar.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(242, Mathf.Max(520, visibleEmails*80));
            RectTransform rect = emailSidebar.GetComponent<RectTransform>();
            if (visibleEmails*80 > 520)
            {
                if (firstEmailScroll && visibleEmails*80 < 600)
                {
                    firstEmailScroll = false;
                    rect.anchoredPosition += new Vector2(0, -60);
                    aceEmails[emailIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -38 + 40*visibleEmails - rect.anchoredPosition.y);
                }
                else
                {
                    rect.anchoredPosition += new Vector2(0, -40);
                    aceEmails[emailIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -38 + 40*visibleEmails - rect.anchoredPosition.y);
                }
                emailSidebar.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20 - 40*(visibleEmails-6));
            }
            else
            {
                rect.anchoredPosition += new Vector2(0, -80);
                aceEmails[emailIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 222 - rect.anchoredPosition.y);
            }
            aceEmails[emailIndex].gameObject.SetActive(true);
            EmailObject script = aceEmails[emailIndex].GetComponent<EmailObject>();
            if (!notif)
            {
                script.read = true;
                aceEmails[emailIndex].GetComponent<Image>().color = script.readColor;
            }
            emailIndex++;

            if (notif)
            {
                newEmailAlert.SetActive(true);
                //show additional alert to make sure players see? (like windows popup at bottom right?)
                if (SequenceManager.Instance.SequenceID != 19)
                    AudioManager.Instance.PlayOneShot(SFXNAME.Email);
                newsWait = true;
            }
        }
    }

    public void EmailResponse()
    {
        StartCoroutine(EmailResponseCor());
    }

    public IEnumerator EmailResponseCor()
    {
        yield return new WaitForSeconds(7);
        int row = 1;
        while (desktopSequence.sheetData[row.ToString()]["Name"] != "Email")
            row++;
        CheckConvoTriggers(row, true);
        newsWait = false;
    }

    private void SetUpNewsDetails()
    {
        newsTitle.text = desktopSequence.newsDetailsData["1"]["Dialogue"];

        for (int i = 0; i < articleTexts.Length; i++)
        {
            articleTexts[i].text = desktopSequence.newsDetailsData[(i + 2).ToString()]["Dialogue"];
        }
    }

    public void HoverNews(bool showHover)
    {
        if (!firstPlay && SequenceManager.Instance.SequenceID != 16)
        {
            if (showHover && !ignoreHover)
            {
                newsHover.SetActive(true);
                captionObj.transform.DOLocalMove(new Vector2(-100, -25), 0.1f).SetEase(Ease.Linear);
            }
            else if (!showHover && !paused)
            {
                newsHover.SetActive(false);
                captionObj.transform.DOLocalMove(new Vector2(-100, -75), 0.1f).SetEase(Ease.Linear);
                ignoreHover = false;
            }
        } 
    }

    private void SetHeadline()
    {
        bool seq16 = (SequenceManager.Instance.SequenceID == 16);
        Vector2 leftPos = seq16 ? new Vector2(-1010, 0) : new Vector2(-400, 0);
        Vector2 rightPos = seq16 ? new Vector2(890, 0) : new Vector2(450, 0);
        Vector2 midPos = seq16 ? new Vector2(-110, 0) : new Vector2(4, 0);
        float time = seq16 ? 3f : 2f;
        headlineOUT = headlineText.transform.DOLocalMove(leftPos, time).SetEase(Ease.Linear);
        headlineOUT.onComplete += () =>
        {
            Destroy(headlineText);
        };
        GameObject newHeadline = Instantiate(headlineTextPrefab, Vector3.zero, Quaternion.identity, headlineText.transform.parent);
        newHeadline.GetComponent<TextMeshProUGUI>().text = desktopSequence.sheetData[(currRow).ToString()]["NewsDialogue"];
        newHeadline.GetComponent<RectTransform>().anchoredPosition = rightPos;
        headlineIN = newHeadline.transform.DOLocalMove(midPos, time+0.25f).SetEase(Ease.Linear);
        headlineIN.onComplete += () =>
        {
            headlineText = newHeadline;
        };
        headlineText.transform.parent.parent.gameObject.SetActive(true);
        UpdateText();
    }

    private void SetPictures(char c)
    {
        Debug.Log("in set pictures");
        if (c == '(')
        {
            acePic.SetActive(true);
            breakNewsPic.SetActive(false);
        }
        else if(c == '{')
        {
            harbPic.SetActive(true);
            breakNewsPic.SetActive(false);
        }
        else if(c == ')')
        {
            acePic.SetActive(false);
        }
        else if(c == '}')
        {
            harbPic.SetActive(false);
        }
        else if(c == '~')
        {
            newsFrame.SetActive(false);
            hospitalPic.SetActive(true);
        }
        else if (c == '@')
        {
            newsFrame.SetActive(true);
            hospitalPic.SetActive(false);
        }

        if (!acePic.activeSelf && !harbPic.activeSelf)
        {
            breakNewsPic.SetActive(true);
        }

        if (acePic.activeSelf && !harbPic.activeSelf)
        {
            acePic.transform.localPosition = new Vector3(-102, 19, 0);
        }
        else if (!acePic.activeSelf && harbPic.activeSelf)
        {
            harbPic.transform.localPosition = new Vector3(-102, 19, 0);
        }
        else if (acePic.activeSelf && harbPic.activeSelf)
        {
            acePic.transform.localPosition = new Vector3(-201, 19, 0);
            harbPic.transform.localPosition = new Vector3(-14, 19, 0);
        }
    }
}