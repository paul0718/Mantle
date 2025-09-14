using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ProjectileReticle : MonoBehaviour
{
    [SerializeField] private bool destroyParent;
    public SFXNAME sfx2;
    public SFXNAME sfx3;

    void Start()
    {
        if (destroyParent)
            GameObject.Find("ClickProjectile EnemyGame").GetComponent<ClickProjectileManager>().projectiles.Add(gameObject.transform.parent.gameObject);
        else
            GameObject.Find("ClickProjectile EnemyGame").GetComponent<ClickProjectileManager>().projectiles.Add(gameObject);
    }

    void OnMouseDown()
    {
        ClickedOn();
    }

    public void ClickedOn()
    {
        PlayProjectileAudio();
        if (SequenceManager.Instance.SequenceID == 12 && transform.parent.childCount > 2)
        {
            if (transform.GetSiblingIndex() == 0)
                transform.parent.GetChild(1).GetComponent<ProjectileReticle>().destroyParent = true;
            else
                transform.parent.GetChild(0).GetComponent<ProjectileReticle>().destroyParent = true;
        }
        if (destroyParent)
        {
            GameObject.Find("ClickProjectile EnemyGame").GetComponent<ClickProjectileManager>().projectiles.Remove(gameObject);
            GameObject.Find("ClickProjectile EnemyGame").GetComponent<ClickProjectileManager>().projectiles.Remove(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
        }
        else
        {
            GameObject.Find("ClickProjectile EnemyGame").GetComponent<ClickProjectileManager>().projectiles.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    public void PlayProjectileAudio()
    {
        if(sfx3 != SFXNAME.LargeRingProjectile)
            AudioManager.Instance.PlayOneShot(SFXNAME.LargeRingProjectile);
        else if(sfx2 != SFXNAME.MidRingProjectile)
            AudioManager.Instance.PlayOneShot(SFXNAME.MidRingProjectile);
        else
            AudioManager.Instance.PlayOneShot(SFXNAME.MainProjectile);
    }
}
