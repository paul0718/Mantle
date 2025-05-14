using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JsonModel;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
public class SequenceManager : MonoBehaviour
{
    public static SequenceManager Instance { get; private set; }

    [HideInInspector] public Dictionary<int,Battle> battles;
    [HideInInspector] public Dictionary<int,Desktop> desktops;
    [HideInInspector] public Dictionary<int,Cutscene> cutscenes;
    [HideInInspector] public Battle CurrentBattle { get => battles[SequenceID]; }
    [HideInInspector] public Desktop CurrentDesktop { get => desktops[SequenceID]; }
    [HideInInspector] public Cutscene CurrentCutscene { get => cutscenes[SequenceID]; }

    private int[] alertSeq = new[] { 2, 5, 8, 11};

    public int dateNum = 0;
    
    public int SequenceID;

    [HideInInspector] public int specialSequence = 17;
    public bool aceIsDead = false;
    public bool IsSpecialSequence { get => specialSequence == SequenceID; }
    public int[] desktopSeq = new[] { 2, 4, 5, 7, 8, 10, 11, 16, 19};
    [HideInInspector] public int[] battleSeq = new[] { 3, 6, 9, 12, 14, 17};
    public int[] cutsceneSeq = new[] { 1, 13, 15, 18 };

    [HideInInspector] public bool steamPageOn = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("SequenceID: "+PlayerPrefs.GetInt("SequenceID"));
        Debug.Log("DateNum: " + PlayerPrefs.GetInt("DateNum"));
        battles = LoadJson<Dictionary<int, Battle>>("Json/Battle");
        desktops = LoadJson<Dictionary<int, Desktop>>("Json/Desktop");
        cutscenes = LoadJson<Dictionary<int, Cutscene>>("Json/Cutscene");
    }
    private void Start()
    {
        SceneManager.sceneLoaded += (a, b) =>
        {
            SaveData();
        };
    }

    private T LoadJson<T>(string path) 
    {
        var jsonText = Resources.Load<TextAsset>(path);
        if (jsonText == null)
        {
            Debug.LogError("No JSON file found at path: " + path);
            return default;
        }
        try
        {
            return JsonConvert.DeserializeObject<T>(jsonText.text);
        }
        catch (JsonException ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
            return default;
        }
        
    }
    private void OnApplicationQuit()
    {
        SaveData();
    }
    
    private void SaveData()
    {
        Debug.Log("SequenceId and DataNum saved.");
        PlayerPrefs.SetInt("SequenceID", SequenceID);
        PlayerPrefs.SetInt("DateNum", dateNum);
    }
    public bool IsAlert()
    {
        return alertSeq.Contains(SequenceID);
    }
}
