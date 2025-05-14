using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorPipe : MonoBehaviour
{
    public Transform nodeA;
    public Transform nodeB;
    public Transform body;

    private Vector2 pointA;
    private Vector2 pointB;

    public void SetPoint(Vector2 pointA, Vector2 pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
    }
}
