using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SwordProjectile : MonoBehaviour
{
    [SerializeField] private AudioSource hitSound;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(new Vector3(Random.Range(-13, 13), -6, 0), 1.3f).SetEase(Ease.Linear)
            .onComplete = PlayHitSound;
    }

    void PlayHitSound()
    {
        GameObject.Find("SwordProjectileGame").GetComponent<SwordGameManager>().count++;
        hitSound.Play();
    }

    private void OnDestroy()
    {
        DOTween.Kill(this.gameObject);
    }
}
