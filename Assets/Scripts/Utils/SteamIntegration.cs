using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamIntegration : MonoBehaviour
{
    public static SteamIntegration Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        try
        {
            Steamworks.SteamClient.Init(3714810);
            PrintName();
        }
        catch (Exception e)
        {
            Debug.Log("failed to connect to steam");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Steamworks.SteamClient.RunCallbacks();

        if (Input.GetKeyDown(KeyCode.I))
        {
            IsThisAchievementUnlocked("ACH_DEFEAT_ENAGA");
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            UnlockAchievement("ACH_DEFEAT_ENAGA");
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            ClearAchievementStatus("ACH_DEFEAT_ENAGA");
        }
    }

    private void PrintName()
    {
        Debug.Log(Steamworks.SteamClient.Name);
    }

    void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }
    
    public void IsThisAchievementUnlocked(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        Debug.Log($"Achievement {id} status: " + ach.State);
    }

    public void UnlockAchievement(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        ach.Trigger();
        Debug.Log($"Achievement {id} unlocked");
    }

    public void ClearAchievementStatus(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        ach.Clear();
        Debug.Log($"Achievement {id} cleared");
    }
}
