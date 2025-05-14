using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FluidMouse : MonoBehaviour
{
    public GameObject pipePrefab;
    public Transform pipeContainer;
    public CompositeToLineRenderer compositeToLineRenderer;

    public List<Pipe> allPipes = new List<Pipe>();
    private Vector3 pointA;
    private Vector3 pointB;
    private Pipe currentPipe;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = GetCurrentMouse3DPosition();
            Vector2 closestPoint = GetClosestPerpendicularPoint(mousePosition);

            pointA = closestPoint;
            currentPipe = Instantiate(pipePrefab, pipeContainer).GetComponent<Pipe>();
            currentPipe.SetPosition(pointA, pointA);
            currentPipe.SetMask(SpriteMaskInteraction.None);
            
        }
        if (Input.GetMouseButton(0)) {
            var pointT = GetCurrentMouse3DPosition();
            currentPipe.SetPosition(pointA, pointT);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePosition = GetCurrentMouse3DPosition();
            Vector2 closestPoint = GetClosestPerpendicularPoint(mousePosition);

            pointB = closestPoint;
            currentPipe.SetPosition(pointA, pointB);
            allPipes.Add(currentPipe);
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(0.1f);
            sequence.AppendCallback(() => compositeToLineRenderer.RegenerateLineRenderer());
        }
    }
    private Vector3 GetCurrentMouse3DPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
    Vector2 GetClosestPerpendicularPoint(Vector2 mousePos)
    {
        float minDistance = float.MaxValue;
        Vector2 closestPoint = mousePos;

        foreach (Pipe pipe in allPipes)
        {
            Vector2 projectedPoint = GetClosestPointOnPipe(pipe.pointA, pipe.pointB, mousePos);
            if (projectedPoint != mousePos)
            {
                float distance = Vector2.Distance(mousePos, projectedPoint);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = projectedPoint;
                }
            }
        }
        
        return minDistance > 0.5f ? mousePos : closestPoint;
    }

    Vector2 GetClosestPointOnPipe(Vector2 A, Vector2 B, Vector2 P)
    {
        if (Vector2.Distance(P, A) <= 0.5f) return A;
        if (Vector2.Distance(P, B) <= 0.5f) return B;

        Vector2 AB = B - A;
        float lengthAB = AB.magnitude;
        Vector2 AB_normalized = AB / lengthAB;

        Vector2 A_ext = A + AB_normalized * 0.5f;
        Vector2 B_ext = B - AB_normalized * 0.5f;

        Vector2 AB_ext = B_ext - A_ext;
        Vector2 AP_ext = P - A_ext;

        float t = Vector2.Dot(AP_ext, AB_ext) / Vector2.Dot(AB_ext, AB_ext);
        t = Mathf.Clamp01(t); 

        Vector2 projectedPoint = A_ext + t * AB_ext;

        float distanceToProjected = Vector2.Distance(P, projectedPoint);

        return (distanceToProjected <= 0.5f) ? projectedPoint : P;
    }

}
