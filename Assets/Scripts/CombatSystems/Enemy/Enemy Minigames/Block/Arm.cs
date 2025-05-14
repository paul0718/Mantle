using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Arm : MonoBehaviour
{
    [SerializeField] private Transform root; // R
    [SerializeField] private Transform mid;  // M, always on perpendicular bisector of RE
    [SerializeField] private Transform end;  // E, tip of the arm
    [SerializeField] private Transform target; // T, where the arm is trying to reach, RE isn't always RT
    [SerializeField] private int maxLength = 30;
    private float newVertexCount = 12;
    [SerializeField] private float maxAngle = 45;
    
    private LineRenderer line;
    private PolygonCollider2D armCollider;
    
    private Vector2 rootLocalPos;
    private Vector2 armBaseVector;
    private float armBaseLength;

    private Vector2 endLocalPos;

    private bool isDragging = false;
    

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        armCollider = GetComponent<PolygonCollider2D>();
        armCollider.enabled = true;
        rootLocalPos = root.localPosition;
        endLocalPos = end.localPosition;
    }
    
    void LateUpdate()
    {
        UpdateArmCurve();
        if (isDragging)
        {
            UpdateCollider();
        }
    }
    
    public void StartAttack()
    {
        UpdateArmCurve();
        UpdateCollider();
    }
    
    public void Resetting()
    {
        root.localPosition = rootLocalPos;
        // reset Target position and update collider afterwards
        target.gameObject.transform.DOLocalMove(endLocalPos, 1f).SetEase(Ease.InFlash).OnComplete(() =>
        {
            end.localPosition = endLocalPos;
            target.localPosition = end.localPosition;
            UpdateCollider();
        });
    }
    
    public void DisableArmControl()
    {
        target.gameObject.SetActive(false);
    }

    public void EnableArmControl()
    {
        target.gameObject.SetActive(true);
    }
    
    private void UpdateArmCurve()
    {
        armBaseVector = GetBaseVector();
        armBaseLength = GetBaseLength();
        end.localPosition= rootLocalPos + armBaseVector * armBaseLength;
        mid.localPosition = GetCurveMidPt();
        var pointList = new List<Vector3>();
        for(float ratio = 0; ratio <= 1; ratio += 1/newVertexCount)
        {
            Vector3 tangent1 = Vector3.Lerp(root.position, mid.position, ratio);
            Vector3 tangent2 = Vector3.Lerp(mid.position, end.position, ratio);
            Vector3 curveIntersect = Vector3.Lerp(tangent1, tangent2, ratio);

            pointList.Add(curveIntersect);
        }

        line.positionCount = pointList.Count;
        line.SetPositions(pointList.ToArray());

        // Rotate art of the tip of arm
        Vector3 awayVectorWorld = end.position - pointList[^2];
        float angle = Vector3.SignedAngle(Vector3.up, awayVectorWorld, Vector3.forward);
        end.localRotation = Quaternion.Euler(0f, 0f, angle);
    }


    // Updates the polygon 2D collider, creates multiple seperated rectangles
    public void UpdateCollider()
    {
        // Author: BLANKdev
        // https://theblankdev.itch.io/linerenderseries

        Vector3[] positions = new Vector3[line.positionCount];
        line.GetPositions(positions);

        //If we have enough points to draw a line
        if (positions.Length >= 2) {
            int numberOfLines = positions.Length - 1;

            //Make as many paths for each different line as we have lines
            armCollider.pathCount = numberOfLines;

            //Get Collider points between two consecutive points
            for (int i = 0; i < numberOfLines; i++) {
                //Get the two next points
                List<Vector2> currentPositions = new List<Vector2> {
                    positions[i],
                    positions[i+1]
                };

                List<Vector2> currentColliderPoints = CalculateColliderPoints(currentPositions);
                // Convert From World to Local as Line Renderer uses Local
                armCollider.SetPath(i, currentColliderPoints.ConvertAll(p => (Vector2)transform.InverseTransformPoint(p)));
            }
        }
        else {
            armCollider.pathCount = 0;
        }
    }

    // Get base/direction vector (normalized) of arm given the angle constraint
    // Examples (vectors should all be normalized):
    // Let R being root of the arm, T being the target, E being the end/tip of arm),
    //      RT = (1,1), maxAngle = 45, RE = (1,1)
    //      RT = (-1,1), maxAngle = 45, RE = (-1,1)
    //      RT = (1,0), maxAngle = 45, RE = (1,1)
    private Vector2 GetBaseVector()
    {
        Vector2 targetPos = target.localPosition;
        Vector2 rawDirection = (targetPos - rootLocalPos).normalized;
        float rawAngle = Vector2.SignedAngle(Vector2.up, rawDirection);
        if (Mathf.Abs(rawAngle) <= maxAngle)
        {
            return rawDirection;
        }
        float angleInRadians = maxAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);
        return new Vector2(Mathf.Sign(rawAngle) * -x, y);
    }

    // Get the distance from Root to expected End
    private float GetBaseLength()
    {
        Vector2 targetPos = target.localPosition;
        float rawDistance = Vector2.Distance(rootLocalPos, targetPos);
        if (rawDistance < maxLength)
        {
            return rawDistance;
        }
        return maxLength;
    }
    
    private Vector2 GetCurveMidPt()
    {
        Vector2 endPos = end.localPosition;
        Vector2 midBasePos = (rootLocalPos + endPos) / 2; // Midpoint from start to end
        
        Vector2 midDirectionVector = new Vector2(armBaseVector.y, -armBaseVector.x); // 90 deg CW
        float angle = Vector2.SignedAngle(Vector2.up, armBaseVector);
        midDirectionVector *= Mathf.Sign(angle); // flip if necessary

        // midpoint can't exceed x=0, makes sure bottom always lean towards the correct direction
        // distance increases as |angle| increases
        // distance also increases as |RE| increases
        float distanceAngle = Mathf.Abs(angle) / 10;
        float distanceLen = Vector2.Distance(rootLocalPos, endPos) / 3;
        float distance = Mathf.Min(distanceAngle, distanceLen);
        float maxDistance = GetMaxDistance(midBasePos, midDirectionVector);
        if (maxDistance != 0)
        {
            distance = Mathf.Min(distance, maxDistance);
        }
        // Calculate the result point
        Vector2 resultPoint = midBasePos + midDirectionVector * distance;

        return resultPoint;
    }
    
    private float GetMaxDistance(Vector2 point, Vector2 direction)
    {
        // Check if the direction vector is horizontal or zero (no unique intersection)
        if (Mathf.Abs(direction.x) < Mathf.Epsilon)
        {
            return 0; // The line is parallel to the vertical line x = 0
        }

        // Find the equation of the line passing through the point with the same slope as the direction vector
        // Slope of the line: m = vy / vx
        float m = direction.y / direction.x;

        // Line equation: y = m*x + (y1 - m*x1)
        // At x = 0: y = y1 - m*x1
        float x = 0;
        float y = point.y - m * point.x;

        return Vector2.Distance(new Vector2(x, y), point);
    }
    
    // Fixed Infinite Slope Problem and Improved by Paul Liu
    // Original Author: BLANKdev
    // https://theblankdev.itch.io/linerenderseries
    private List<Vector2> CalculateColliderPoints(List<Vector2> positions) {
        float width = line.startWidth;

        Vector2 p0 = positions[0];
        Vector2 p1 = positions[1];

        Vector2 direction = (p1 - p0).normalized;
        Vector2 normal;

        // Check for vertical line to avoid division by zero
        if (Mathf.Approximately(p1.x, p0.x)) {
            // Vertical line: use horizontal offset
            normal = new Vector2(1, 0);
        } else if (Mathf.Approximately(p1.y, p0.y)) {
            // Horizontal line: use vertical offset
            normal = new Vector2(0, 1);
        } else {
            // General case
            normal = new Vector2(-direction.y, direction.x); // Perpendicular to direction
        }

        normal *= (width / 2f);

        List<Vector2> colliderPoints = new List<Vector2> {
            p0 + normal,
            p1 + normal,
            p1 - normal,
            p0 - normal
        };

        return colliderPoints;
    }

    public void SetDragging(bool dragging)
    {
        isDragging = dragging;
    }
    
}
