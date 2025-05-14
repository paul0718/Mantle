using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceArea : MonoBehaviour
{
    public List<Vector2> directions = new List<Vector2>();
    private readonly float force = 5;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() == null || directions.Count == 0)  return;
        Rigidbody2D r = collision.GetComponent<Rigidbody2D>();
        Vector2 v = r.velocity.normalized;
        List<Vector2> realForce = new List<Vector2>();
        foreach(var f in directions)
        {
            if (Vector2.Dot(f, v) < -0.9)
                continue;
            else
                realForce.Add(f);
        }
        if (realForce.Count == 0) return;
        r.velocity = realForce[Random.Range(0, realForce.Count)] * force;
    }
}
