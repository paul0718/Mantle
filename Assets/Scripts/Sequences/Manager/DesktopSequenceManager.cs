using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using JsonModel;
using UnityEngine.UI;
using TMPro;

public class DesktopSequenceManager : MonoBehaviour
{
    public static DesktopSequenceManager Instance { get; private set; }
    
    //convo info
    public Dictionary<string, Dictionary<string, string>> sheetData = new Dictionary<string, Dictionary<string, string>>();
    
    //alert info
    public bool isAlertActive;
    public string currentEnemy;
    public string currentLocation;
    [SerializeField] private Transform enemyImage;
    [SerializeField] private TMP_Text enemyText;
    [SerializeField] private GameObject dotInfo;
    
    //news details info;
    [HideInInspector]
    public Dictionary<string, Dictionary<string, string>> newsDetailsData =
        new Dictionary<string, Dictionary<string, string>>();

    [SerializeField] private TextMeshProUGUI visionText;
    [SerializeField] private RectTransform visionLine;
    [SerializeField] private GameObject visionPanel;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private CurrentTime timeScript;
    
    private void Start()
    {
        Instance = this;
        var data = SequenceManager.Instance.CurrentDesktop;
        isAlertActive = SequenceManager.Instance.IsAlert();
        Debug.Log(isAlertActive + " isAlertScene in DesktopMan");
        if (isAlertActive)
        {
            currentEnemy = SequenceManager.Instance.battles[SequenceManager.Instance.SequenceID+1].Enemy;
            currentLocation = SequenceManager.Instance.battles[SequenceManager.Instance.SequenceID+1].Background;
            if (SequenceManager.Instance.SequenceID < 10)
            {
                enemyImage.parent.gameObject.SetActive(true);
                dotInfo.SetActive(true);
            }
        }
        
        var res = Resources.Load<TextAsset>(data.GoogleSheetPath).text;
        if(SequenceManager.Instance.SequenceID == 19 && !SequenceManager.Instance.aceIsDead)
            res = Resources.Load<TextAsset>(data.AceKillMantlePath).text;
        sheetData = DialogUtils.ParseJsonToDictionary(res);

        if (data.NewsDetailsPath != null)
        {
            var detailsRes = Resources.Load<TextAsset>(data.NewsDetailsPath).text;
            newsDetailsData = DialogUtils.ParseJsonToDictionary(detailsRes);
        }
        
        visionText.text = "HeroRobot.Ver.1.0.23 Vision";
        if (SequenceManager.Instance.SequenceID > 4)
        {
            if (visionText != null)
            {
                //visionText.rectTransform.sizeDelta = new Vector2(390, 50);
                visionPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 100);
                visionText.text = "Mantle Vision";
            }
            
            if (SequenceManager.Instance.SequenceID > 10)
            {
                if (visionText != null)
                {
                    visionText.color = Color.red; 
                }
            }
        }
        
        SetDateTime();

        string tempText = visionText.text;
        visionText.text = "";
        if(SequenceManager.Instance.SequenceID != 19)
            StartCoroutine(TypeVisionText(tempText));
    }

    IEnumerator TypeVisionText(string fullText)
    {
        visionPanel.SetActive(true);
        foreach (char letter in fullText)
        {
            visionText.text += letter; // Append each letter to the text
            yield return new WaitForSeconds(0.08f); // Wait for the specified duration
        }
        visionPanel.SetActive(false);
        Sequence s = DOTween.Sequence();
        s.Append(visionText.gameObject.transform.DOLocalMove(new Vector3(0, 500, 0), 1f));
        s.Append(DOTween.To(() => visionLine.sizeDelta, value => visionLine.sizeDelta = value,
            new Vector2(549.6f, 8.93f), 0.8f));
    }

    private void SetDateTime()
    {
        if (SequenceManager.Instance.dateNum - 1 >= 0)
        {
            timeText.text = ChangeDate.newTimes[SequenceManager.Instance.dateNum - 1];
            timeScript.currentTime.x = float.Parse(timeText.text.Substring(0, 1));
            timeScript.currentTime.y = float.Parse(timeText.text.Substring(2, 2));
        }
        
        string tempMonth = ChangeDate.month == "March" ? "3" : "4";
        string tempDay = ChangeDate.currentDay.ToString();

        dateText.text = tempMonth + "/" + tempDay + "/" + "3001";
    }

    public void SetEnemyImageDotInfo()
    {
        var enemy = Instantiate(Resources.Load<GameObject>("Prefab/DesktopEnemy/" +
                                                           SequenceManager.Instance.battles[SequenceManager.Instance.SequenceID+1].Enemy), enemyImage);
        enemyText.text = "Enemy: " + SequenceManager.Instance.battles[SequenceManager.Instance.SequenceID + 1].Enemy + "\n";
        enemyText.text += "Weapon: ";
        switch (SequenceManager.Instance.SequenceID)
        {
            case 2:
                enemyText.text += "Speaker Cannon";
                break;
            case 5:
                enemyText.text += "Bombs";
                break;
            case 8:
                enemyText.text += "Battle Axe";
                break;
        }
        
        dotInfo.transform.GetChild(1).GetComponent<Animator>().SetTrigger("PlayTutorial");
    }
}
