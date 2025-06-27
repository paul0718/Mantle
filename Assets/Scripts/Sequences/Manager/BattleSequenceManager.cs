using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSequenceManager : MonoBehaviour
{
    public static BattleSequenceManager Instance { get; private set; }
    public Transform backgroundContainer;
    public Transform enemyContainer;
    public Dictionary<int,JsonModel.Minigame> minigames;
    public Dictionary<int,JsonModel.EnemyMinigame> enemyMinigames;
    public Dictionary<int,JsonModel.EndStates> endStates;
    public Vector2 startPos;
    public Vector2 disarmPos;

    [HideInInspector]
    public Dictionary<string, Dictionary<string, string>> introData= new Dictionary<string, Dictionary<string, string>>();
    [HideInInspector]
    public Dictionary<string, Dictionary<string, string>> specialData= new Dictionary<string, Dictionary<string, string>>();
    [HideInInspector]
    public Dictionary<string, Dictionary<string, string>> barkData = new Dictionary<string, Dictionary<string, string>>();
    [HideInInspector]
    public Dictionary<string, Dictionary<string, string>> outroData = new Dictionary<string, Dictionary<string, string>>();
    [HideInInspector]
    public Dictionary<string, Dictionary<string, string>> destructData= new Dictionary<string, Dictionary<string, string>>();
    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        var enemy = Instantiate(Resources.Load<GameObject>("Prefab/Enemy/" +
             SequenceManager.Instance.CurrentBattle.Enemy), enemyContainer);
        var background = Instantiate(Resources.Load<GameObject>("Prefab/Background/" +
            SequenceManager.Instance.CurrentBattle.Background), backgroundContainer);
        minigames = SequenceManager.Instance.CurrentBattle.Minigames;
        enemyMinigames = SequenceManager.Instance.CurrentBattle.EnemyMinigames;
        endStates = SequenceManager.Instance.CurrentBattle.endStates;
        startPos = SequenceManager.Instance.CurrentBattle.StartPos;
        disarmPos = SequenceManager.Instance.CurrentBattle.DisarmPos;

        AudioManager.Instance.SetBGMLibrary(SequenceManager.Instance.CurrentBattle.Enemy + "BGM");
        
        var data = SequenceManager.Instance.CurrentBattle;
        var intro = Resources.Load<TextAsset>(data.IntroDialoguePath).text;
        introData = DialogUtils.ParseJsonToDictionary(intro);

        var bark = Resources.Load<TextAsset>(data.BarkDialoguePath).text;
        barkData = DialogUtils.ParseJsonToDictionary(bark);

        var outro = Resources.Load<TextAsset>(data.OutroDialoguePath).text;
        outroData = DialogUtils.ParseJsonToDictionary(outro);

        if (SequenceManager.Instance.IsSpecialSequence)
        {
            var special = Resources.Load<TextAsset>("Dialog/Special").text;
            specialData = DialogUtils.ParseJsonToDictionary(special);
            
            var destruct = Resources.Load<TextAsset>("Dialog/SpecialDestruct").text;
            destructData = DialogUtils.ParseJsonToDictionary(destruct);
        }
    }
}
