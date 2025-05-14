using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using DG.Tweening;

// Attaches to the background of the indicator
public class Indicator : MonoBehaviour
{
    protected float attackTimeTotal; // unit: seconds
    [SerializeField] protected GameObject mainObject;
    [SerializeField] protected GameObject outline;
    [SerializeField] protected GameObject filler;
    [SerializeField] protected Vector3 fillerStartPosition;
    [SerializeField] protected Vector3 fillerEndPosition;
    [SerializeField] protected bool isRadial = false;
    [SerializeField] protected Vector3 fillerStartScale;
    [SerializeField] protected Vector3 fillerEndScale;

    /*[SerializeField] private GameObject mask;
    [SerializeField] private Sprite maskSprite;*/
    
    
    protected float progress;
    protected bool isBlocked;
    protected Vector3 defaultLocalPosition;
    protected Vector3 defaultLocalScale;
    protected PolygonCollider2D collider;

    protected ContactFilter2D filter;
    
    public virtual void Start()
    {
        // Set Contact Filter
        filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Ignore Raycast"));
        filter.useLayerMask = true;
        filter.useTriggers = true;
    }

    public virtual void InitIndicator(float newAttackTime)
    {
        InitFiller();
        // Make sure that the physics shape for the sprite has been set
        collider = mainObject.GetComponent<PolygonCollider2D>();
        // Set Default values
        defaultLocalScale = mainObject.transform.localScale;
        defaultLocalPosition = transform.localPosition;
        attackTimeTotal = newAttackTime;
        progress = 0;
        isBlocked = false;
        HideIndicator();
    }

    public Sprite GetMainSprite()
    {
        return mainObject.GetComponent<SpriteRenderer>().sprite;
    }
    
    protected void InitFiller()
    {
        ResetFiller();
        var fillerSpriteRenderer = filler.GetComponentInChildren<SpriteRenderer>();
        fillerSpriteRenderer.material.shader = Shader.Find("GUI/Text Shader"); // Default = Shader.Find("Sprites/Default")
        fillerSpriteRenderer.color = new Color(1f, 0f, 0f, 0.8f);
        fillerSpriteRenderer.sortingOrder = mainObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        /*if (isRadial)
        {
            fillerSpriteRenderer.sprite = maskSprite;
        }*/
    }
    
    protected void ResetFiller()
    {
        if (isRadial)
        {
            filler.transform.localScale = fillerStartScale;
        }
        else
        {
            filler.transform.localPosition = fillerStartPosition;
        }
        filler.SetActive(false);
    }
    
    public virtual void StartAttack()
    {
        ShowIndicator();
        filler.SetActive(true);
    }
    
    public virtual void Update()
    {
        UpdateFiller();
        UpdateOutline();
    }
    
    protected void UpdateFiller()
    {
        if (!filler.activeSelf)
        {
            return;
        }
        progress += Time.deltaTime / attackTimeTotal;
        progress = Mathf.Clamp01(progress);
        if (isRadial)
        {
            filler.transform.localScale = Vector3.Lerp(fillerStartScale, fillerEndScale, progress);
        }
        else
        {
            filler.transform.localPosition = Vector3.Lerp(fillerStartPosition, fillerEndPosition, progress);
        }
    }

    protected void UpdateOutline()
    {
        int count = 0;
        var result = new List<Collider2D>();
        if (collider != null)
        {
            // count = collider.OverlapCollider(new ContactFilter2D().NoFilter(), result);
            count = collider.OverlapCollider(filter, result);
        }
        isBlocked = (count >= 1);
        outline.SetActive(isBlocked);
    }
    
    public virtual bool IsBlocked()
    {
        return isBlocked;
    }

    public virtual void Resetting()
    {
        progress = 0;
        isBlocked = false;
        transform.position = defaultLocalPosition;
        ResetFiller();
        HideIndicator();
    }

    public virtual void Randomize()
    {
        var newPositionX = defaultLocalPosition.x + Random.Range(-1f, 1f);
        transform.localPosition = new Vector3(newPositionX, defaultLocalPosition.y, defaultLocalPosition.z);
    }
    
    protected virtual void HideIndicator()
    {
        mainObject.transform.localScale = Vector3.zero;
        collider.enabled = false;
        outline.SetActive(false);
    }

    protected virtual void ShowIndicator()
    {
        mainObject.transform.localScale = defaultLocalScale;
        collider.enabled = true;
    }
    
}
