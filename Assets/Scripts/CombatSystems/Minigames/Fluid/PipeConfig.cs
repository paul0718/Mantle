using UnityEngine;

[CreateAssetMenu(fileName = "PipeConfig", menuName = "ScriptableObjects/Pipe Config")]
public class PipeConfig : ScriptableObject
{
    public PipeData[] sections;
    public ValveData[] valves;
    public Vector2 sourcePoint;
    public Vector2 targetPoint;
    public Vector2[] sinkPoints;
}

[System.Serializable]
public class ValveData
{
    public Vector2 point;
    public bool open;
}

[System.Serializable]
public class PipeData
{
    public Vector2 startPoint;
    public Vector2 endPoint;
}
