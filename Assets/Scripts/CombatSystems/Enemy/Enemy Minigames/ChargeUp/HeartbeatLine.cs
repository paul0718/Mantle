using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeartbeatLine : MonoBehaviour
{
    [SerializeField] private Transform boxes;
    [SerializeField] private Sprite boxSprite;

    private int sign = 1;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float speed;
    [SerializeField] private float maxY;
    [SerializeField] private float minY;

    private float randomChance;

    private float delay;

    void Start()
    {
        GetComponent<TrailRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
    }

    void FixedUpdate()
    {
        Vector2 normalizedDir = direction.normalized;
        transform.position += new Vector3(normalizedDir.x, normalizedDir.y*sign, 0)*speed/60;

        bool lockedOntoBox = false;
        foreach (Transform child in boxes)
        {
            if (child.transform.position.x > transform.position.x)
            {
                Vector3 diff = child.transform.position - transform.position;
                if (Mathf.Abs(diff.y/direction.y) > diff.x)
                {
                    lockedOntoBox = true;
                    sign = (int)(diff.y/Mathf.Abs(diff.y));
                }
                break;
            }
        }

        if (!lockedOntoBox)
        {   
            if (Random.Range(0, 300) <= randomChance || transform.position.y > maxY || transform.position.y < minY)
            {
                randomChance = 0;
                sign *= -1;
            }
            else
            {
                randomChance++;
            }
        }
    }

    void Update()
    {
        if (transform.localPosition.x > 12)
        {
            gameObject.SetActive(false);
            transform.localPosition = Vector3.zero;
        }

        delay = Mathf.Max(0, delay-Time.deltaTime);
        if (Input.GetMouseButtonDown(0) && delay <= 0)
        {
            Collider2D collider = Physics2D.OverlapPoint(transform.position);
            if (collider != null)
            {
                if (collider.name != "Box")
                    collider = null;
                else
                {
                    AudioManager.Instance.PlayOneShot(SFXNAME.LaserSound);
                    collider.GetComponent<SpriteRenderer>().color = Color.green;
                    collider.GetComponent<SpriteRenderer>().sprite = boxSprite;
                    collider.gameObject.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.1f)
                        .SetLoops(2, LoopType.Yoyo);
                    AudioManager.Instance.PlayOneShot(SFXNAME.HitMark);
                }
            }

            if (collider == null)
            {
                delay = 0.3f;
                AudioManager.Instance.PlayOneShot(SFXNAME.MissMark);
            }
        }
    }
}
