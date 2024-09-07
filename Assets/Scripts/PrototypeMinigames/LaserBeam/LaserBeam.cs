using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LaserBeam  
{
    private Vector3 pos, dir;

    private GameObject laserObj;
    private LineRenderer laser;
    private List<Vector3> laserIndices = new List<Vector3>();
    
    public LaserBeam(Vector3 pos, Vector3 dir, Material material)
    {
        laser = new LineRenderer();
        laserObj = new GameObject();
        laserObj.name = "Laser Beam";
        GameObject.Find("LaserPointer").GetComponent<ShootLaser>().AddLaser(laserObj);
        pos = pos;
        dir = dir;
        
        laser = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        laser.startWidth = 0.1f;
        laser.endWidth = 0.1f;
        laser.material = material;
        laser.startColor = Color.green;
        laser.endColor = Color.green;

        CastRay(pos, dir, laser);
    }

    public void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser)
    {
        laserIndices.Add(pos);

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 30, 1))
        {
            CheckHit(hit, dir, laser, ray);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(30));
            UpdateLaser();
        }
    }

    public void UpdateLaser()
    {
        int count = 0;
        laser.positionCount = laserIndices.Count;

        foreach (var idx in laserIndices)
        {
            laser.SetPosition(count, idx);
            count++;
        }
    }

    public void CheckHit(RaycastHit hitInfo, Vector3 direction, LineRenderer laser, Ray ray)
    {
        if (hitInfo.collider.gameObject.CompareTag("Mirror") || hitInfo.collider.gameObject.CompareTag("StillMirror")
            || hitInfo.collider.gameObject.CompareTag("RightMirror"))
        {
            Vector3 pos = hitInfo.point;
            Vector3 dir = Vector3.Reflect(direction, hitInfo.normal);
            
            CastRay(pos, dir, laser);
        }
        else if (hitInfo.collider.gameObject.CompareTag("LaserGoal"))
        {
            Debug.Log("win");
            GameObject.Find("LaserBeam").GetComponent<MoveMirror>().EndGame(laserObj);
            // laserIndices.Add(ray.GetPoint(30));
            // laserIndices.Add(hitInfo.point);
            // UpdateLaser();
        }
        else
        {
            laserIndices.Add(hitInfo.point);
            UpdateLaser();
        }
    }
}
