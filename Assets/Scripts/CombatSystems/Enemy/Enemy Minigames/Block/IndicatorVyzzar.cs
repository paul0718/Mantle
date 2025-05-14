using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Radial Indicator
// Filler: circle
// Mask: Two seperated axe heads
public class IndicatorVyzzar : Indicator
{
    private Tween tween;
    [SerializeField] private Vector3 swingFrom;
    [SerializeField] private Vector3 swingTo;
    
    public override void StartAttack()
    {
        swingFrom.x = 0;
        swingTo.x = 0;
        transform.localEulerAngles = swingFrom;
        tween = transform.DOLocalRotate(swingTo, 2f, RotateMode.Fast)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutCubic);
        base.StartAttack();
    }

    public override void Resetting()
    {
        tween.Kill();
        base.Resetting();
    }

    public override void Randomize()
    {
        float newPositionX;
        if (transform.localPosition.x <= 0)
        {
            newPositionX = defaultLocalPosition.x + Random.Range(-0.43f, 2.45f);
        }
        else
        {
            newPositionX = defaultLocalPosition.x + Random.Range(0.43f, -2.45f);
        }
        
        transform.localPosition = new Vector3(newPositionX, defaultLocalPosition.y, defaultLocalPosition.z);
    }
}
