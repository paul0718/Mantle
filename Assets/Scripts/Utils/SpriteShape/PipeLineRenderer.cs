using UnityEngine;

public class PipeLineRenderer : MonoBehaviour
{
    public Vector2[] points; 
    public float width = 0.2f; 

    private LineRenderer leftLineRenderer;
    private LineRenderer rightLineRenderer;

    void Start()
    {
        // 初始化左右LineRenderer
        leftLineRenderer = CreateLineRenderer("LeftLineRenderer", Color.blue);
        rightLineRenderer = CreateLineRenderer("RightLineRenderer", Color.red);

        // 计算并设置左右偏移的点
        UpdatePipeLine();
    }

    LineRenderer CreateLineRenderer(string name, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();

        // 配置LineRenderer属性
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        return lineRenderer;
    }

    void UpdatePipeLine()
    {
        int totalSegments = points.Length;
        Vector3[] leftPoints = new Vector3[totalSegments];
        Vector3[] rightPoints = new Vector3[totalSegments];

        for (int i = 0; i < totalSegments; i++)
        {
            Vector2 forward;
            Vector2 current = points[i];
            if (i != totalSegments - 1)
            {
                
                Vector2 next = points[i + 1];

                forward = (next - current).normalized;
            }
            else
            {
                forward = (current - points[i - 1]).normalized;
            }
           
            Vector2 normal;
            float b = 1;
            if (i == 0 || i == totalSegments - 1)  
            {
                normal = new Vector2(-forward.y, forward.x);
            }
            else
            {
                Vector2 l1 = (points[i - 1] - points[i]).normalized;
                Vector2 l2 = (points[i + 1] - points[i]).normalized;
                Vector2 a = ((l1 + l2) / 2).normalized;
                normal = a * (IsVectorL2RightOfL1(l2, -l1) ? 1 : -1);
                float dotProduct = Vector2.Dot(l1, l2);

                float angleInRadians = Mathf.Acos(dotProduct);
                float an = angleInRadians * Mathf.Rad2Deg;
                an /= 2;
                b = 1.0f / Mathf.Sin(an * Mathf.Deg2Rad);
            }

            leftPoints[i] = (Vector3)(current + normal * width * b);
            rightPoints[i] = (Vector3)(current - normal * width * b);
        }
        
        leftLineRenderer.positionCount = leftPoints.Length;
        leftLineRenderer.SetPositions(leftPoints);

        rightLineRenderer.positionCount = rightPoints.Length;
        rightLineRenderer.SetPositions(rightPoints);
    }
    bool IsVectorL2RightOfL1(Vector2 l1, Vector2 l2)
    {
        float crossProduct = l1.x * l2.y - l1.y * l2.x;
        return crossProduct < 0; 
    }
}
