using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : MonoBehaviour
{
    public Vector2 point;
    public PipeNode pipeNode;
    public Transform leftValve;
    public Transform rightValve;
    private bool open = false;
    private bool isFlashing = false;
    private Tween leftTween;
    private Tween rightTween;

    private void OnMouseUp()
    {
        if (open)
        {
            leftValve.DOLocalMoveX(-0.22f, 0.5f);
            rightValve.DOLocalMoveX(0.22f, 0.5f);
            AudioManager.Instance.PlayOneShot(SFXNAME.CloseValve, 0.5f);
        }
        else
        {
            leftValve.DOLocalMoveX(-0.5f, 0.5f);
            rightValve.DOLocalMoveX(0.5f, 0.5f);
            AudioManager.Instance.PlayOneShot(SFXNAME.ChoosingMinigame, 0.5f);
        }
        open = !open;
        pipeNode.open = open;
        pipeNode.Switch();
    }
    public void Init(bool open)
    {
        this.open = open;
        if (!open)
        {
            leftValve.DOLocalMoveX(-0.22f, 0.5f);
            rightValve.DOLocalMoveX(0.22f, 0.5f);
        }
        else
        {
            leftValve.DOLocalMoveX(-0.5f, 0.5f);
            rightValve.DOLocalMoveX(0.5f, 0.5f);
        }
        pipeNode.open = open;
    }
    public void StartFlashing()
    {
        if (isFlashing) return;
        isFlashing = true;
        var left = leftValve.GetComponent<SpriteRenderer>();
        var right = rightValve.GetComponent<SpriteRenderer>();

        leftTween?.Kill();
        rightTween?.Kill();

        left.color = Color.white;
        right.color = Color.white;

        leftTween = left.DOColor(Color.red, 0.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        rightTween = right.DOColor(Color.red, 0.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void StopFlashing()
    {
        if (!isFlashing) return;
        isFlashing = false;
        var left = leftValve.GetComponent<SpriteRenderer>();
        var right = rightValve.GetComponent<SpriteRenderer>();

        leftTween?.Kill();
        rightTween?.Kill();

        leftTween = left.DOColor(Color.white, 0.3f);
        rightTween = right.DOColor(Color.white, 0.3f);
    }
}
