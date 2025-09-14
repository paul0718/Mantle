using DG.Tweening;
using JsonModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Fluid : MonoBehaviour
{
    public static Fluid Instance {  get; private set; }
    public TextMeshPro score;
    public Image backgroundImage;
    public float liquidSpeed = 1.0f;
    public int failCondition = 500;
    public int successCondition = 500;
    private int lastFailCount = 0;
    public int failCount = 0;
    public int successCount = 0;
    public ParticleSystem winParticle;
    private bool end = false;
    private bool showAnimation = false;
    private Sequence sequence;
    private Material targetMaterial;
    private float minValue = 0;
    private float maxValue = 0.1f;
    private float duration = 0.5f;
    [SerializeField] private GameObject[] goopPos;
    [SerializeField] private GameObject goopObject;
    [SerializeField] private GameObject acidObject;
    public List<GameObject> goopObjectsSpawned = new List<GameObject>();

    private Battle battle;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // if (SequenceManager.Instance.SequenceID > 10)
        // {
        //     liquidSpeed = 2f;
        // }
        goopObjectsSpawned.Clear();
        targetMaterial = new Material(backgroundImage.material);
        backgroundImage.material = targetMaterial;
        score.text = "Success: 0" + "\nFail: 0";
        battle = SequenceManager.Instance.CurrentBattle;
        sequence = DOTween.Sequence();
    }
    public void AddFail()
    {
        StartOutlineAnimation();
        failCount++;
        RefreshScore();
    }
    public void AddSuccess()
    {
        successCount++;
        RefreshScore();
    }
    private void RefreshScore()
    {
        if (successCount > successCondition)
        {
            score.text = "Succeeded!";
        }
        else if (failCount > failCondition) 
        {
            score.text = "Failed!";
        }
        else
        {
            score.text = "Success: " + successCount + "\nFail: " + failCount;
        }
    }
    public void ResetGame()
    {
        PipeGenerator.Instance.ResetGame();
        RefreshScore();
        end = false;
        showAnimation = false;
    }
    private void Update()
    {

        if (!PipeGenerator.Instance.HasPath())
        {
            AddFail();
        }
        lastFailCount = failCount;
        if (!end)
        {
            if (failCount > 1000)
            {
                AudioManager.Instance.StopLoop(SFXNAME.StickyGoo);
                foreach (var g in goopPos)
                {
                    GameObject tempGoop;
                    if (SequenceManager.Instance.SequenceID < 10)
                        tempGoop = goopObject;
                    else
                        tempGoop = acidObject;
                    GameObject currGoopSpawned = Instantiate(tempGoop, g.transform.position, quaternion.identity, g.transform);
                    goopObjectsSpawned.Add(currGoopSpawned);
                    currGoopSpawned.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
                }
                if (MetricManagerScript.instance != null)
                { 
                    MetricManagerScript.instance.LogString("Sticky Goo", "Lose");
                }

                foreach (var a in goopObjectsSpawned)
                {
                    Destroy(a, 5.5f);
                }
                GridManager.Instance.UpdateDotPosition(battle.Minigames[3].LoseEffect, GridManager.MiniGame.Fluid, false);
                BarkManager.Instance.ShowGameBark("Sticky Goo", false);
                EnemyInfo.Instance.ChangePose(false);
                end = true;
            }
            if (successCount > 500)
            {
                AudioManager.Instance.StopLoop(SFXNAME.StickyGoo);
                if (MetricManagerScript.instance != null)
                { 
                    MetricManagerScript.instance.LogString("Sticky Goo", "Win");
                }
                GridManager.Instance.UpdateDotPosition(battle.Minigames[3].WinEffect, GridManager.MiniGame.Fluid, true);
                BarkManager.Instance.ShowGameBark("Sticky Goo", true);
                EnemyInfo.Instance.ChangePose(true);
                winParticle.Play();
                end = true;
            }
        }
        
    }
    private void StartOutlineAnimation()
    {
        if (!sequence.IsActive())  
        {
            sequence = DOTween.Sequence();
            sequence.Append(DOVirtual.Float(minValue, maxValue, duration, value =>
            {
                targetMaterial.SetFloat("_FadeWidth", value);
            }));
            sequence.Append(DOVirtual.Float(maxValue, minValue, duration, value =>
            {
                targetMaterial.SetFloat("_FadeWidth", value);
            }));
        }
        
    }
}
