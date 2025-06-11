using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
[RequireComponent(typeof(CompositeToLineRenderer))]
public class PipeGenerator : MonoBehaviour
{
    public GameObject pipePrefab;
    public GameObject forceAreaPrefab;
    public GameObject liquidPrefab;
    public GameObject nodePrefab;
    public GameObject valvePrefab;
    public GameObject arrowPrefab;
    public GameObject sinkPrefab;

    public Transform pipeContainer;
    public Transform forceAreaContainer;
    public Transform liquidContainer;
    public Transform nodeContainer;
    public Transform valveContainer;
    public Transform arrowContainer;
    public Transform sinkContainer;

    public PipeConfig[] pipeConfigs;
    private PipeConfig pipeConfig;

    public static PipeGenerator Instance {  get; private set; }
    private PipeData[] segments;
    private List<Vector2> crossPoints = new List<Vector2>();
    private List<Vector2> forcePoints = new List<Vector2>();
    private List<Vector2> nodePoints = new List<Vector2>();
    private List<(Vector2, Vector2)> splitSegments = new List<(Vector2, Vector2)>();
    private List<Pipe> pipeInstances = new List<Pipe>();

    private PipeNode sourceNode;
    private PipeNode targetNode;

    private Dictionary<Vector2, PipeNode> pipeNodeDict = new Dictionary<Vector2, PipeNode>();
    private Dictionary<Vector2, Valve> valveDict = new Dictionary<Vector2, Valve>();

    HashSet<PipeNode> hashset = new HashSet<PipeNode>();

    private bool inited = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
       Init();
    }
    private void DeleteChildGameObjects(Transform parent)
    {
        int len = parent.childCount;
        for (int i = len - 1; i >= 0; i--) 
            Destroy(parent.GetChild(i).gameObject);
    }
    private void Init()
    {

        crossPoints.Clear();
        forcePoints.Clear();
        nodePoints.Clear();
        splitSegments.Clear();
        pipeInstances.Clear();
        valveDict.Clear();
        pipeNodeDict.Clear();
        hashset.Clear();

        DeleteChildGameObjects(pipeContainer);
        DeleteChildGameObjects(valveContainer);
        GetComponent<CompositeToLineRenderer>().ClearLineRenderer();

        pipeConfig = pipeConfigs[SequenceManager.Instance.SequenceID < 10 ? 0 : 1];

        ProcessSegments();
        InstantiatePipes();
        ConnectPipes();
        //InstantiateForceAreas();
        InstantiateNodes();
        InstantiateValves();
        InstantiateSink();
        transform.parent.position = new Vector3(-10.39f, -3.79f, 0);
        inited = true;

        AudioManager.Instance.PlayLoop(SFXNAME.StickyGoo);
    }
    private void OnEnable()
    {
        if (inited) Init();
        sourceNode?.Send();
        //Init();
    }
    public void ResetGame()
    {
        
        foreach (var v in pipeConfig.valves)
        {
            bool r = Random.Range(0, 2) != 0;
            valveDict[v.point].Init(r);
        }
        Fluid.Instance.successCount = 0;
        Fluid.Instance.failCount = 0;
        foreach (var p in pipeNodeDict)
        {
            p.Value.ResetPipe();
        }
        foreach(var p in pipeInstances)
        {
            p.ResetCircle();
        }
    }
    private void ProcessSegments()
    {
        FindIntersections();
        SplitSegments();
    }
    public void SetCircle(Vector2 point)
    {
        foreach(var p in pipeInstances)
        {
            if (p.pointA == point)
                p.SetCircleA();
            if (p.pointB == point)
                p.SetCircleB();
        }
    }
    private void FindIntersections()
    {
        segments = pipeConfig.sections;
        for (int i = 0; i < segments.Length; i++)
        {
            for (int j = i + 1; j < segments.Length; j++)
            {
                Vector2? intersection = GetIntersection(segments[i].startPoint, segments[i].endPoint, segments[j].startPoint, segments[j].endPoint);
                if (intersection.HasValue)
                {
                    if (!forcePoints.Contains(intersection.Value))
                    {
                        forcePoints.Add(intersection.Value);
                    }
                    if (!nodePoints.Contains(intersection.Value))
                    {
                        nodePoints.Add(intersection.Value);
                    }
                    if ((intersection.Value == segments[i].startPoint || intersection.Value == segments[i].endPoint) &&
                        (intersection.Value == segments[j].startPoint || intersection.Value == segments[j].endPoint))
                    {
                        continue; 
                    }
                    if (!crossPoints.Contains(intersection.Value))
                    {
                        crossPoints.Add(intersection.Value);
                    }

                }
            }
        }
        foreach(var v in pipeConfig.valves)
        {
            crossPoints.Add(v.point);
        }
    }


    private void SplitSegments()
    {
        foreach (var segment in segments)
        {
            List<Vector2> points = new List<Vector2> { segment.startPoint, segment.endPoint };
            foreach (var cross in crossPoints)
            {
                if (IsPointOnSegment(cross, segment.startPoint, segment.endPoint)) 
                {
                    points.Add(cross);
                }
            }
            points.Sort((a, b) => a.x.CompareTo(b.x));
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i] != points[i + 1])
                    splitSegments.Add((points[i], points[i + 1]));
            }
        }
    }

    private Vector2? GetIntersection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        float A1 = p2.y - p1.y;
        float B1 = p1.x - p2.x;
        float C1 = A1 * p1.x + B1 * p1.y;

        float A2 = q2.y - q1.y;
        float B2 = q1.x - q2.x;
        float C2 = A2 * q1.x + B2 * q1.y;

        float det = A1 * B2 - A2 * B1;
        if (Mathf.Abs(det) < 1e-6) return null;

        float x = (B2 * C1 - B1 * C2) / det;
        float y = (A1 * C2 - A2 * C1) / det;
        Vector2 intersection = new Vector2(x, y);

        if (IsPointOnSegment(intersection, p1, p2) && IsPointOnSegment(intersection, q1, q2))
            return intersection;
        return null;
    }

    private bool IsPointOnSegment(Vector2 point, Vector2 p1, Vector2 p2)
    {
        return Mathf.Min(p1.x, p2.x) <= point.x && point.x <= Mathf.Max(p1.x, p2.x) &&
               Mathf.Min(p1.y, p2.y) <= point.y && point.y <= Mathf.Max(p1.y, p2.y);
    }

    private void InstantiatePipes()
    {
        foreach (var segment in splitSegments)
        {
            var p0 = segment.Item1;
            var p1 = segment.Item2;

            int v = 0;
            foreach(var va in pipeConfig.valves)
            {
                if (va.point == p0)
                {
                    v++;
                }
                if (va.point == p1)
                {
                    v++;
                }
                if (v == 2)
                {
                    break;
                }
            }
            if (v == 2)
                continue;

            Pipe pipe = Instantiate(pipePrefab, pipeContainer).GetComponent<Pipe>();
            pipe.SetPosition(p0, p1);
            pipeInstances.Add(pipe);
        }
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.2f);
        sequence.AppendCallback(() => GetComponent<CompositeToLineRenderer>().GenerateLineRenderer());
    }

    private void ConnectPipes()
    {

    }
    private void InstantiateForceAreas()
    {
        foreach (var forcePoint in forcePoints)
        {
            GameObject areaObj = Instantiate(forceAreaPrefab, forcePoint, Quaternion.identity, forceAreaContainer);
            ForceArea forceArea = areaObj.GetComponent<ForceArea>();

            foreach (var segment in splitSegments)
            {
                if (segment.Item1 == forcePoint)
                {
                    forceArea.directions.Add((segment.Item2 - segment.Item1).normalized);
                }
                else if (segment.Item2 == forcePoint)
                {
                    forceArea.directions.Add((segment.Item1 - segment.Item2).normalized);
                }
            }
        }
    }
    
    private void InstantiateNodes()
    {
        foreach(var segment in splitSegments)
        {
            if (!pipeNodeDict.ContainsKey(segment.Item1))
            {
                var nodePoint = segment.Item1;
                var nodeObject = Instantiate(nodePrefab, nodePoint, Quaternion.identity, nodeContainer);
                var pipeNode = nodeObject.GetComponent<PipeNode>();
                pipeNode.point = nodePoint;
               
                pipeNodeDict.Add(nodePoint, pipeNode);
            }
            if (!pipeNodeDict.ContainsKey(segment.Item2))
            {
                var nodePoint = segment.Item2;
                var nodeObject = Instantiate(nodePrefab, nodePoint, Quaternion.identity, nodeContainer);
                var pipeNode = nodeObject.GetComponent<PipeNode>();
                pipeNode.point = nodePoint;

                pipeNodeDict.Add(nodePoint, pipeNode);
            }
        }
        foreach (var pair in pipeNodeDict)
        {
            foreach (var pipe in pipeInstances)
            {
                if (pair.Key == pipe.pointA)
                {
                    pair.Value.adjacentInfo.Add((pipeNodeDict[pipe.pointB], pipe.liquidA));
                }
                else if (pair.Key == pipe.pointB)
                {
                    pair.Value.adjacentInfo.Add((pipeNodeDict[pipe.pointA], pipe.liquidB));
                }
            }
        }
        
        foreach (var pair in pipeNodeDict)
        {
            if (pair.Value.point.x == pipeConfig.sourcePoint.x && pair.Value.point.y == pipeConfig.sourcePoint.y)
            {
                pair.Value.Send();
                pair.Value.flag = "Source";
                sourceNode = pair.Value;
            }
            if (pair.Value.point.x == pipeConfig.targetPoint.x && pair.Value.point.y == pipeConfig.targetPoint.y)
            {
                pair.Value.flag = "Target";
                targetNode = pair.Value;
            }
            foreach (var sink in pipeConfig.sinkPoints)
            {
                if (pair.Value.point.x == sink.x && pair.Value.point.y == sink.y) 
                {
                    pair.Value.flag = "Sink";
                }
            }
        }
    }

    private void InstantiateValves()
    {
        foreach(var v in pipeConfig.valves)
        {
            if (pipeNodeDict.ContainsKey(v.point))
            {
                float angle = 0;
                foreach(var instance in pipeInstances)
                {
                    if (v.point == instance.pointA || v.point == instance.pointB)
                    {
                        var direction= instance.pointA-instance.pointB;
                        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        break;
                    }
                }
                var valve = Instantiate(valvePrefab, new Vector3(v.point.x, v.point.y, -3), Quaternion.Euler(0, 0, angle + 90), valveContainer).GetComponent<Valve>();
                valve.pipeNode = pipeNodeDict[v.point];
                bool r = Random.Range(0, 2) != 0;                
                valve.pipeNode.open = r;
                valve.Init(r);
                valve.transform.localPosition = new Vector3(v.point.x, v.point.y, -3);
                valveDict.Add(v.point, valve);
                pipeNodeDict[v.point].valve = valve;
            }
        }
    }
    private void InstantiateSink()
    {
        foreach(var p in pipeInstances)
        {
            foreach(var s in pipeConfig.sinkPoints)
            {
                p.SetSinkDither(s);
            }
            p.SetGoalDither(pipeConfig.targetPoint);
        }
    }
    

    public bool Reachable(PipeNode p)
    {
        if (sourceNode == null)
        {
            return true;
        }
        Queue<PipeNode> queue = new Queue<PipeNode>();
        queue.Enqueue(sourceNode);
        hashset.Clear();

        while(queue.Count > 0)
        {
            var top = queue.Dequeue();
            hashset.Add(top);
            foreach (var adj in top.adjacentInfo)
            {
                if (adj.Item1 == p) return true;
                if (!hashset.Contains(adj.Item1) && adj.Item1.open)  
                {
                    queue.Enqueue(adj.Item1);
                }
            }
        }
        return false;
    }
    public bool HasPath()
    {
        if (sourceNode == null || targetNode == null) 
        {
            return true;
        }
        Queue<PipeNode> queue = new Queue<PipeNode>();
        queue.Enqueue(sourceNode);
        hashset.Clear();

        while (queue.Count > 0)
        {
            var top = queue.Dequeue();
            hashset.Add(top);
            foreach (var adj in top.adjacentInfo)
            {
                if (adj.Item1 == targetNode) return true;
                if (!adj.Item1.hasLiquid) return true;
                if (!hashset.Contains(adj.Item1) && adj.Item1.open)
                {
                    queue.Enqueue(adj.Item1);
                }
            }
        }
        return false;
    }
}
