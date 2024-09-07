using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    [SerializeField] private Material material;
    private LaserBeam beam;

    private List<GameObject> allBeams = new List<GameObject>();

    private bool on = true;

    private void OnEnable()
    {
        on = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            Destroy(GameObject.Find("Laser Beam"));
            beam = new LaserBeam(gameObject.transform.position, gameObject.transform.right, material);
        }
    }

    public void AddLaser(GameObject laser)
    {
        allBeams.Add(laser);
    }
    public void DestroyLasers()
    {
        on = false;
        Debug.Log("destroying" + allBeams.Count.ToString());
        foreach (GameObject b in allBeams)
        {
            Destroy(b);
        }
    }
}
