using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMapData", menuName = "ScriptableObjects/Scene Map", order = 1)]
public class SceneMap : ScriptableObject
{
    [System.Serializable]
    public class SceneEntry
    {
        public int SceneID;
        public string SceneName;
    }

    public List<SceneEntry> SceneEntries = new List<SceneEntry>();
}
