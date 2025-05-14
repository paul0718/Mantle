using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IndicatorHarbinger : Indicator
{
    private Tween tween;
    private Quaternion defaultRotation;
    [SerializeField] private Transform pivot;

    public override void InitIndicator(float newAttackTime)
    {
        base.InitIndicator(newAttackTime);
        var fillerSpriteRenderer = filler.GetComponentInChildren<SpriteRenderer>();
        var tailSpriteRenderer = mainObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        fillerSpriteRenderer.sortingOrder = mainObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
        tailSpriteRenderer.sortingOrder = mainObject.GetComponent<SpriteRenderer>().sortingOrder - 2;
    }

    public override void StartAttack()
    {
        tween = pivot.DOLocalRotate(new Vector3(0,0,360), 4f, RotateMode.LocalAxisAdd)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
        base.StartAttack();
    }

    public override void Resetting()
    {
        tween.Kill();
        base.Resetting();
    }
}
