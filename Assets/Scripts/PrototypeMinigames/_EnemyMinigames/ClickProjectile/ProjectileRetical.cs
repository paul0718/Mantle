using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ProjectileRetical : MonoBehaviour
{
    [SerializeField] private GameObject managerGO;

    private ClickProjectileManager managerScript;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("ClickProjectileGame").GetComponent<ClickProjectileManager>().projectiles.Add(this.gameObject);
        transform.DOScale(new Vector3(1, 1, 1), 0.3f);
    }

    private void OnDestroy()
    {
        if (GameObject.Find("ClickProjectileGame") != null)
        {
            GameObject.Find("ClickProjectileGame").GetComponent<ClickProjectileManager>().projectiles.Remove(this.gameObject);
        }
    }
}
