using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Soundwave : MonoBehaviour
{
    public float speed; //use negative speed to move left
    public float delay; //so player can't spam click
    [SerializeField] private Sprite boxSprite;

    void FixedUpdate()
    {
        transform.position += Vector3.right*speed/60;
        if (Mathf.Abs(transform.position.x) > 5)
            Destroy(gameObject);
    }

    void Update()
    {
        delay = Mathf.Max(0, delay-Time.deltaTime);
        if (Input.GetMouseButtonDown(0) && delay <= 0)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, LayerMask.GetMask("ChargeBox"));

            foreach (Collider2D col in colliders)
            {
                if (col.GetComponent<SpriteRenderer>().color != Color.green)
                {
                    AudioManager.Instance.PlayOneShot(SFXNAME.LaserSound);
                    col.GetComponent<SpriteRenderer>().color = Color.green;
                    col.GetComponent<SpriteRenderer>().sprite = boxSprite;
                    col.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.1f)
                        .SetLoops(2, LoopType.Yoyo);
                    AudioManager.Instance.PlayOneShot(SFXNAME.HitMark);
                }
            }

            bool misclick = true;
            foreach (Transform child in transform.parent)
            {
                if (Physics2D.OverlapPointAll(child.position, LayerMask.GetMask("ChargeBox")).Length > 0)
                    misclick = false;
            }
            if (misclick)
            {
                delay = 0.3f;
                AudioManager.Instance.PlayOneShot(SFXNAME.MissMark);
            }
        }
    }
}
