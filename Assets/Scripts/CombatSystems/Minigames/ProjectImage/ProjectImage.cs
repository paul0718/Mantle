using DG.Tweening;
using JsonModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class ProjectImage : MonoBehaviour
{
    public GameObject gameRoot;
    public Button redValveButton;
    public Button yellowValveButton;
    public Button blueValveButton;
    public Button flipXValveButton;
    public Button flipYValveButton;
    public Image leftImage;
    public Image rightImage;
    public Image leftHoloImage;
    public Image rightHoloImage;
    public RectTransform resultTransform;
    public TextMeshProUGUI signText;
    public Sprite bunnySkull;

    public Image redFill;
    public Image yellowFill;
    public Image blueFill;

    private bool redValve = false;
    private bool yellowValve = false;
    private bool blueValve = false;

    private bool flipXValve = false;
    private bool flipYValve = false;

    private int attemps = 0;

    private readonly Color[] COLORS = new Color[] { Color.red, Color.yellow, Color.blue, Color.green, Color.black, new Color(1, 0.5f, 0), new Color(0.5f, 0, 1) };
    private readonly int MAX_ATTEMPS = 6;

    private Vector3 leftStartPosition = Vector3.zero;
    private Vector3 rightStartPosition = Vector3.zero;

    private Material leftHoloMaterial;
    private Material rightHoloMaterial;
    
    //#region Aqua
    public RectTransform upLiquid;
    private void Awake()
    {
        if (SequenceManager.Instance.SequenceID > 10)
        {
            leftImage.sprite = bunnySkull;
            rightImage.sprite = bunnySkull;
            leftHoloImage.sprite = bunnySkull;
            rightHoloImage.sprite = bunnySkull;
        }
        leftStartPosition = leftImage.transform.position;
        rightStartPosition = rightImage.transform.position;
    }
    private void Start()
    {
        leftHoloMaterial = new Material(leftHoloImage.material);
        rightHoloMaterial = new Material(rightHoloImage.material);
    }
    public void RenderAqua()
    {
        upLiquid.DOSizeDelta(new Vector2(32, 32 * attemps), 0.4f);

    }

    private void OnEnable()
    {
        ResetHoloMaterial();

        leftImage.color = Color.white;
        leftImage.transform.localEulerAngles = Vector3.zero;

        redValve = yellowValve = blueValve = flipXValve = flipYValve = false;

        redValveButton.transform.eulerAngles = Vector3.zero;
        yellowValveButton.transform.eulerAngles = Vector3.zero;
        blueValveButton.transform.eulerAngles = Vector3.zero;
        flipXValveButton.transform.eulerAngles = Vector3.zero;
        flipYValveButton.transform.eulerAngles = Vector3.zero;

        redFill.transform.localScale = new Vector3(0, 4, 4);
        yellowFill.transform.localScale = new Vector3(0, 4, 4);
        blueFill.transform.localScale = new Vector3(0, 4, 4);

        rightImage.color = COLORS[Random.Range(0, COLORS.Length)];
        int rx = Random.Range(0, 2);
        int ry = Random.Range(0, 2);
        var e = rightImage.transform.localEulerAngles;
        e.x = (rx == 0) ? 0 : 180;
        e.y = (ry == 0) ? 0 : 180;
        e.z = 0;
        rightImage.transform.localEulerAngles = e;

        ClearEvent();

        redValveButton.onClick.AddListener(() => ValveClick(ref redValve, redValveButton.transform, redFill));
        yellowValveButton.onClick.AddListener(() => ValveClick(ref yellowValve, yellowValveButton.transform, yellowFill));
        blueValveButton.onClick.AddListener(() => ValveClick(ref blueValve, blueValveButton.transform, blueFill));

        flipXValveButton.onClick.AddListener(() => ValveClick(ref flipXValve, flipXValveButton.transform));
        flipYValveButton.onClick.AddListener(() => ValveClick(ref flipYValve, flipYValveButton.transform));

        upLiquid.sizeDelta = new Vector2(0, 0);

        if (leftStartPosition != Vector3.zero) leftImage.transform.position = leftStartPosition;
        if (rightStartPosition != Vector3.zero) rightImage.transform.position = rightStartPosition;

        signText.gameObject.SetActive(false); 
        attemps = 0;
    }
    private void SetHoloMaterial(Color leftColor, Color rightColor)
    {
        leftHoloMaterial.SetColor("_MainColor", leftColor);
        rightHoloMaterial.SetColor("_MainColor", rightColor);
        leftHoloImage.material = leftHoloMaterial;
        rightHoloImage.material = rightHoloMaterial;
        leftHoloImage.DOFillAmount(1, 1f);
        rightHoloImage.DOFillAmount(1, 1f);

        leftHoloImage.transform.localEulerAngles = leftImage.transform.localEulerAngles;
        rightHoloImage.transform.localEulerAngles = rightImage.transform.localEulerAngles;

        leftHoloImage.color = leftColor;
        rightHoloImage.color = rightColor;
    }
    private void ResetHoloMaterial()
    {
        leftHoloImage.fillAmount = 0;
        rightHoloImage.fillAmount = 0;
    }
    private void CheckResult()
    {
        var le = leftImage.transform.localEulerAngles;
        var re = rightImage.transform.localEulerAngles;
        var battle = SequenceManager.Instance.CurrentBattle;
        if (leftImage.color == rightImage.color && le == re)
        {
            //GetComponent<AudioSource>().Play();
            //equalSign.SetActive(true);
            GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(true);
            SetSign(win: true);

            SetHoloMaterial(leftImage.color, rightImage.color);

            //leftImage.transform.DOMove(resultTransform.position,0.5f);
            //rightImage.transform.DOMove(resultTransform.position, 0.5f);
            for (; attemps <= MAX_ATTEMPS; attemps++)
            {
                RenderAqua();
            }
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Image Projection", "Win");
            }
            GridManager.Instance.UpdateDotPosition(battle.Minigames[1].WinEffect, GridManager.MiniGame.Project, true);
            BarkManager.Instance.ShowGameBark("Project Image", true);
            ClearEvent();
            //Invoke("HideGame", 6f);
        }
        if ((leftImage.color != rightImage.color || le != re) && attemps > MAX_ATTEMPS) 
        {
            GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(false);
            SetSign(win: false);

            SetHoloMaterial(leftImage.color, rightImage.color);

            //leftImage.transform.DOMove(resultTransform.position, 0.5f);
            //rightImage.transform.DOMove(resultTransform.position, 0.5f);
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Image Projection", "Lose");
            }
            GridManager.Instance.UpdateDotPosition(battle.Minigames[1].LoseEffect, GridManager.MiniGame.Project, false);
            BarkManager.Instance.ShowGameBark("Project Image", false);
            ClearEvent();
            //Invoke("HideGame", 6f);
        }
        RenderAqua();

    }
    private void ValveClick(ref bool valveStatus, Transform valveTransform, Image fill = null)
    {
        if (valveStatus)
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.Minigame_Project_Image_LightValveOFF);
        }
        valveStatus = !valveStatus;
        if (fill != null)
        {
            if (!valveStatus)
                fill.transform.DOScale(new Vector3(0, 4, 4), 0.2f);
            else
                fill.transform.DOScale(new Vector3(4, 4, 4), 0.2f);
        }
        RenderLeftImage();
        ValveAnimation(valveStatus, valveTransform);
        attemps++;
        CheckResult();
    }
    private void RenderLeftImage()
    {
        if (!redValve && !yellowValve && !blueValve)
            leftImage.color = Color.white;

        if (redValve && !yellowValve && !blueValve)
            leftImage.color = Color.red;

        if (!redValve && yellowValve && !blueValve)
            leftImage.color = Color.yellow;

        if (!redValve && !yellowValve && blueValve)
            leftImage.color = Color.blue;

        if (redValve && yellowValve && !blueValve)
            leftImage.color = new Color(1.0f, 0.5f, 0f);

        if (redValve && !yellowValve && blueValve)
            leftImage.color = new Color(0.5f, 0f, 1.0f);

        if (!redValve && yellowValve && blueValve)
            leftImage.color = Color.green;

        if (redValve && yellowValve && blueValve)
            leftImage.color = Color.black;

        var e = leftImage.transform.localEulerAngles;
        e.x = flipXValve ? 180 : 0;
        e.y = flipYValve ? 180 : 0;
        e.z = 0;
        leftImage.transform.localEulerAngles = e;
    }
    private void ValveAnimation(bool open,Transform valve)
    {
        valve.DORotate(new Vector3(0, 0, open ? 90 : 0), 0.4f, RotateMode.Fast).SetEase(Ease.Linear);
    }
    private void ClearEvent()
    {
        redValveButton.onClick.RemoveAllListeners();
        yellowValveButton.onClick.RemoveAllListeners();
        blueValveButton.onClick.RemoveAllListeners();

        flipXValveButton.onClick.RemoveAllListeners();
        flipYValveButton.onClick.RemoveAllListeners();
    }
    private void SetSign(bool win = true)
    {
        if (win)
        {
            signText.color = Color.green;
            signText.text = "=";
        }
        else
        {
            signText.color = Color.red;
            signText.text = "X";
        }
        signText.gameObject.SetActive(true);
    }
    private void HideGame()
    {
        Debug.Log("hiding game");
        gameRoot.SetActive(false);
    }
}
