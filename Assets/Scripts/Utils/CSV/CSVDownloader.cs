using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public class CSVDownloaderEditor : EditorWindow
{
    private string pastedConfig = "MyDialog, sheetID1: Alert, NewsConvo, BattleLoss\nMyDialog2, sheetID2: Alert2, NewsConvo2";
    private CSVDownloaderConfig config;
    [MenuItem("Tools/Download Dialog")]
    public static void ShowWindow()
    {
        GetWindow<CSVDownloaderEditor>("CSV Downloader");
    }

    private void OnEnable()
    {
        minSize = new Vector2(400, 300);
    }

    private void OnGUI()
    {
        
        GUILayout.Label("Google Sheets Downloader Settings", EditorStyles.boldLabel);

        //GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
        //{
        //    wordWrap = true,
        //    fixedWidth = position.width - 20 
        //};

        //GUILayout.Label("Paste your configuration (Format: name, sheetID: sheetName1, sheetName2), use line breaks between different entries:", labelStyle);
        //pastedConfig = EditorGUILayout.TextArea(pastedConfig, GUILayout.Height(100));

        //if (GUILayout.Button("Pack and Save Configuration"))
        //{
        //    ParseAndSaveConfig();
        //}

        //GUILayout.Space(10);

        var newConfig = (CSVDownloaderConfig)EditorGUILayout.ObjectField("Config File", config, typeof(CSVDownloaderConfig), false);
        if (newConfig != config)
        {
            config = newConfig;
            UpdateConfigData();
        }
        if (GUILayout.Button("Download CSV Files"))
        {
            if (config != null)
            {
                DownloadCSVFiles();
            }
            else
            {
                Debug.LogError("Please assign a CSVDownloaderConfig file.");
            }
        }
        
    }

    private void ParseAndSaveConfig()
    {
        string[] entries = pastedConfig.Split('\n');
        List<CSVDownloaderConfig.DialogConfig> parsedConfigs = new List<CSVDownloaderConfig.DialogConfig>();

        foreach (string entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry)) continue;

            string[] parts = entry.Split(new[] { ',' }, 2); 
            if (parts.Length == 2)
            {
                string name = parts[0].Trim(); 
                string[] sheetParts = parts[1].Split(':');

                if (sheetParts.Length == 2)
                {
                    string sheetID = sheetParts[0].Trim(); 
                    string[] sheetNames = sheetParts[1].Split(',');

                    CSVDownloaderConfig.DialogConfig dialogConfig = new CSVDownloaderConfig.DialogConfig
                    {
                        name = name,
                        sheetID = sheetID,
                        sheetNames = new List<string>()
                    };

                    foreach (string sheetName in sheetNames)
                    {
                        dialogConfig.sheetNames.Add(sheetName.Trim());
                    }

                    parsedConfigs.Add(dialogConfig);
                }
                else
                {
                    Debug.LogError("Invalid format. Each entry must be in the format 'name, sheetID: sheetName1, sheetName2'.");
                }
            }
            else
            {
                Debug.LogError("Invalid format. Each entry must contain a name and sheet configuration.");
            }
        }

        if (parsedConfigs.Count > 0)
        {
            CSVDownloaderConfig newConfig = CreateInstance<CSVDownloaderConfig>();
            newConfig.dialogConfigs = parsedConfigs;
            if (config != null)
            {
                config.dialogConfigs = newConfig.dialogConfigs;
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Configuration updated successfully!");
                return;
            }
            string path = EditorUtility.SaveFilePanelInProject("Save Config", "CSVDownloaderConfig", "asset", "Please enter a file name to save the config");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newConfig, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Configuration saved successfully!");
            }
        }
        else
        {
            Debug.LogError("No valid configuration parsed.");
        }
    }

    private void DownloadCSVFiles()
    {
        string urlTemplate = "https://script.google.com/macros/s/{0}/exec?sheetNameString={1}";

        foreach (var configEntry in config.dialogConfigs)
        {
            foreach (var name in configEntry.sheetNames)
            {
                string url = string.Format(urlTemplate, configEntry.sheetID, name);
                string csvContent = DownloadCsvFile(url);

                if (!string.IsNullOrEmpty(csvContent))
                {
                    string folderPath = Path.Combine("Assets/Resources/Dialog", configEntry.name);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    SaveToResources(csvContent, Path.Combine(folderPath, name + ".json")); 
                    Debug.Log($"Dialog for {name} downloaded and saved to {folderPath} successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to download the dialog file for {name}.");
                }
            }
        }
    }

    private string DownloadCsvFile(string url)
    {
        try
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
        catch (System.Net.WebException ex)
        {
            Debug.LogError("Error downloading dialog file: " + ex.Message);
            return null;
        }
    }
    private void UpdateConfigData()
    {
        pastedConfig = "";
        if (EditorGUIUtility.keyboardControl != 0) 
            GUI.FocusControl(null);
        foreach (var dialogConfig in config.dialogConfigs)
        {
            pastedConfig += $"{dialogConfig.name}, {dialogConfig.sheetID}: {string.Join(", ", dialogConfig.sheetNames)}\n";
        }
    }
    private void SaveToResources(string csvContent, string filePath)
    {
        File.WriteAllText(filePath, csvContent);
        AssetDatabase.Refresh();
    }
}
#endif
