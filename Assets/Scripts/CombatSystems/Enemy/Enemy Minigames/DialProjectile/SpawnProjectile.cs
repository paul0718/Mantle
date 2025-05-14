using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    [SerializeField] private GameObject attackForDial;
    
    [SerializeField] private GameObject attackForClick;

    [SerializeField] private GameObject attackForSword;

    [SerializeField] private AudioSource dialGunAudio;
    
    
    public IEnumerator SpawnProjectilesForDial()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate (attackForDial, new Vector3(Random.Range(-7, 7), 6f, 0f), Quaternion.identity);
            yield return new WaitForSeconds(2);
        }
    }
    
    public IEnumerator SpawnProjectilesForClick()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 8; i++)
        {
            Instantiate (attackForClick, new Vector3(Random.Range(-4.5f, 4.5f), Random.Range(1f, 4f), 0f), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator SpawnProjectilesForSword()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 8; i++)
        {
            Instantiate (attackForSword, GameObject.Find("SwordProjectileSpawn").transform.position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }
}
