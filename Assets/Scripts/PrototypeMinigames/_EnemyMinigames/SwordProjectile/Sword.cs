using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private AudioSource hitSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            GameObject.Find("SwordProjectileGame").GetComponent<SwordGameManager>().count++;
            GameObject.Find("SwordProjectileGame").GetComponent<SwordGameManager>().countHit++;
            hitSound.Play();
            Destroy(other.gameObject);
        }
    }
}
