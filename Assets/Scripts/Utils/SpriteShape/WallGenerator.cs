using Unity.VisualScripting;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject squarePrefab;   
    public GameObject circlePrefab;  
    public Vector2[] points;          
    public float lineWidth = 0.2f;    

    void Start()
    {
        GeneratePolylineWallWithJoints();
    }

    void GeneratePolylineWallWithJoints()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];
            Vector2 centerPosition = (start + end) / 2;
            Vector2 direction = (end - start).normalized;
            float length = Vector2.Distance(start, end);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject lineSegment = Instantiate(squarePrefab, centerPosition, Quaternion.Euler(0, 0, angle));
            lineSegment.transform.localScale = new Vector3(length, lineWidth, 1); 
        }

        for (int i = 1; i < points.Length - 1; i++)
        {
            Vector2 jointPosition = points[i];

            GameObject jointCircle = Instantiate(circlePrefab, jointPosition, Quaternion.identity);
            jointCircle.transform.localScale = new Vector3(lineWidth, lineWidth, 1); 
        }
    }
}
