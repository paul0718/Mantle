using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PipeNode : MonoBehaviour
{
    public string flag = "";
    public Vector2 point;
    public Pipe pipe;

    public List<(PipeNode, SpriteRenderer)> adjacentInfo = new List<(PipeNode, SpriteRenderer)>();
    public Valve valve;
    public bool open = true;

    public bool hasLiquid = false;

    public List<Tween> tweens = new List<Tween>();

    private int clogCount = 0;
    public void Switch()
    {
        if (open)
        {
            if (hasLiquid)
                Send();
        }
        else
        {
            Stop();
        }
    }
    public void Stop()
    {
        foreach (var t in tweens) t.Kill(false);
        tweens.Clear();
        foreach (var info in adjacentInfo)
        {
            var i = info;
            if (i.Item2.size.x > 0)
            {
                i.Item2.material.SetVector("_ScrollSpeed", new Vector4(0, 0, 0, 0));
                if (i.Item1.hasLiquid)
                {
                    i.Item1.Stop();
                }
            }
            
        }
    }
    public void ResetPipe()
    {
        hasLiquid = false;
        foreach (var t in tweens) t.Kill();
        tweens.Clear();
        foreach (var info in adjacentInfo)
        {
            info.Item2.size = new Vector2(0, 1);
            info.Item2.material.SetVector("_ScrollSpeed", new Vector4(-1f, 0, 0, 0));
        }
    }
    public void Send()
    {
        if (!PipeGenerator.Instance.Reachable(this)) return;
        hasLiquid = true;
        if (!open)
            return;
        foreach(var info in adjacentInfo)
        {
            var i = info;
            if (i.Item1.hasLiquid) 
            {
                if (i.Item2.size.x > 0)
                {
                    i.Item2.material.SetVector("_ScrollSpeed", new Vector4(-1, 0, 0, 0));
                    i.Item1.Send();
                }
                
                continue;
            }
            float distance = Vector2.Distance(point, i.Item1.point);
            Vector2 startSize = i.Item2.size;
            i.Item2.material.SetVector("_ScrollSpeed", new Vector4(-1, 0, 0, 0));
            var t = DOTween.To(
                () => i.Item2.size.x,
                x => i.Item2.size = new Vector2(x, startSize.y),
                distance * 2,
                (distance * 2 - startSize.x) / 2f/Fluid.Instance.liquidSpeed
            ).SetEase(Ease.Linear);
            t.onComplete += () =>
            {
                i.Item1.Send();
                PipeGenerator.Instance.SetCircle(i.Item1.point);
            };
            tweens.Add(t);
        }
    }
    private void Update()
    {
        foreach(var info in adjacentInfo)
        {
            if (info.Item1.hasLiquid && info.Item1.flag == "Sink" && (info.Item2.material.GetVector("_ScrollSpeed").x!=0))
            {
                Fluid.Instance.AddFail();
            }
            if (info.Item1.hasLiquid && info.Item1.flag == "Target" && (info.Item2.material.GetVector("_ScrollSpeed").x != 0))
            {
                Fluid.Instance.AddSuccess();
            }
        }
        //if (open)
        //{
        //    clogCount = 0;
        //    if (valve != null)
        //        valve.StopFlashing();
        //}
        //if (hasLiquid && !open)
        //{
            
        //    clogCount++;
        //}
        //if (clogCount > 500)
        //{
        //    if (valve != null)
        //        valve.StartFlashing();
        //}
        //if (clogCount > 1000)
        //{
        //    Fluid.Instance.AddFail();
        //}
    }
}
