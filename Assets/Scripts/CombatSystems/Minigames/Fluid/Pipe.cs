using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public Transform square;
    public Transform startCircle;
    public Transform endCircle;

    public SpriteRenderer liquidA;
    public SpriteRenderer liquidB;
    public SpriteRenderer circleA;
    public SpriteRenderer circleB;

    public Vector2 pointA;
    public Vector2 pointB;

    public SpriteRenderer ditherA;
    public SpriteRenderer ditherB;

    public GameObject sinkA;
    public GameObject sinkB;

    private Color redColor = new Color(255 / 255f, 87 / 255f, 87 / 255f);
    private Color greenColor = new Color(132 / 255f, 255 / 255f, 87 / 255f);
    private Color pipeColor = new Color(124 / 255f, 124 / 255f, 124 / 255f);
    private Color gooColor2= new Color(106 / 255f, 190 / 255f, 48 / 255f);
    private Color gooColor1 = new Color(126 / 255f, 0 / 255f, 180 / 255f);

    public float capacity;

    public Sprite acidSprite;
    private void Start()
    {
        if (SequenceManager.Instance.SequenceID > 10)
        {
            liquidA.sprite = acidSprite;
            liquidB.sprite = acidSprite;
        }
        Material matA = new Material(liquidA.material);
        Material matB = new Material(liquidB.material);
        liquidA.material = matA;
        liquidB.material = matB;
    }
    public void SetPosition(Vector2 startPoint, Vector2 endPoint)
    {
        this.pointA = startPoint;
        this.pointB = endPoint;
        float length = (endPoint - startPoint).magnitude;
        capacity = length;
        square.localScale = new Vector3(length, 0.5f, 0);
        endCircle.localPosition = new Vector3(length / 2, 0, 0f);
        startCircle.localPosition = new Vector3(-length / 2, 0, 0f);

        var mid = (startPoint + endPoint) / 2f;
        var direction = endPoint - startPoint;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localPosition = mid;
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }
    public void SetSinkDither(Vector2 pos)
    {
        if (pos == pointA) 
        {
            ditherA.gameObject.SetActive(true);
            ditherA.color = redColor;
            SetDitherLength(ditherA);
        }
        if (pos== pointB)
        {
            ditherB.gameObject.SetActive(true);
            ditherB.color = redColor;
            //sinkB.SetActive(true);
            SetDitherLength(ditherB);
        }
        
    }
    public void SetGoalDither(Vector2 pos)
    {
        if (pos == pointA)
        {
            ditherA.gameObject.SetActive(true);
            ditherA.GetComponent<SpriteRenderer>().color = greenColor;
            SetDitherLength(ditherA);
        }
        if (pos == pointB)
        {
            ditherB.gameObject.SetActive(true);
            ditherB.GetComponent<SpriteRenderer>().color = greenColor;
            //sinkB.SetActive(true);
            SetDitherLength(ditherB);
        }
    }
    public void SetCircleA()
    {

        circleA.color = SequenceManager.Instance.SequenceID < 10 ? gooColor1 : gooColor2;
    }
    public void SetCircleB()
    {
        circleB.color = SequenceManager.Instance.SequenceID < 10 ? gooColor1 : gooColor2;
    }
    public void ResetCircle()
    {
        circleA.color = pipeColor;
        circleB.color = pipeColor;
    }
    private void SetDitherLength(SpriteRenderer spriteRenderer)
    {
        float l = 1.2f;
        l = Mathf.Max(l, square.localScale.x / 2);
        l = Mathf.Min(l, 1.4f);
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, l);
    }
    public void SetMask(SpriteMaskInteraction mask)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var renderer in renderers)
        {
            renderer.maskInteraction = mask;
        }
    }
}
