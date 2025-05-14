using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]
public class CompositeToLineRenderer : MonoBehaviour
{
    private CompositeCollider2D compositeCollider;
    public Material material;
    public Transform lineContainer;
    public void RegenerateLineRenderer(SpriteMaskInteraction mask = SpriteMaskInteraction.None)
    {
        for (int i = lineContainer.childCount - 1; i >= 0; i--) 
        {
            Destroy(lineContainer.GetChild(i).gameObject);
        }
        GenerateLineRenderer(mask);
    }
    public void ClearLineRenderer()
    {
        int len = lineContainer.childCount;
        for (int i = len - 1; i >= 0; i--)
            Destroy(lineContainer.GetChild(i).gameObject);
    }
    public void GenerateLineRenderer(SpriteMaskInteraction mask = SpriteMaskInteraction.VisibleInsideMask)
    {
        compositeCollider = GetComponent<CompositeCollider2D>();

        int outerPathCount = compositeCollider.pathCount;
        if (outerPathCount > 0)
        {
            Vector2[] outerPath = new Vector2[compositeCollider.GetPathPointCount(0)];
            compositeCollider.GetPath(0, outerPath);
            CreateLineRenderer(outerPath, "OuterBoundary", mask);
        }

        for (int i = 1; i < outerPathCount; i++) 
        {
            Vector2[] holePath = new Vector2[compositeCollider.GetPathPointCount(i)];
            compositeCollider.GetPath(i, holePath);

            CreateLineRenderer(holePath, $"Hole_{i}", mask);
        }
    }

    private void CreateLineRenderer(Vector2[] points, string name, SpriteMaskInteraction mask)
    {
        GameObject line = new GameObject(name);
        line.transform.SetParent(lineContainer);

        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = points.Length;

        for (int j = 0; j < points.Length; j++)
        {
            lineRenderer.SetPosition(j, points[j]);
        }

        lineRenderer.loop = true; 
        lineRenderer.startWidth = 0.12f;
        lineRenderer.endWidth = 0.12f;
        
        lineRenderer.material = material;
        lineRenderer.transform.position = transform.parent.position;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.sortingOrder = 2;
        lineRenderer.numCornerVertices = 5;
        lineRenderer.maskInteraction = mask;
        //lineRenderer.transform.localPosition = new Vector3(lineRenderer.transform.localPosition.x, lineRenderer.transform.localPosition.y, -2);
    }
}
