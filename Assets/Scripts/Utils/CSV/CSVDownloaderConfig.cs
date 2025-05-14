using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CSVDownloaderConfig", menuName = "ScriptableObjects/CSV Downloader Config")]
public class CSVDownloaderConfig : ScriptableObject
{
    [System.Serializable]
    public class DialogConfig
    {
        public string name;
        public string sheetID;
        public List<string> sheetNames;
    }

    public List<DialogConfig> dialogConfigs;
}