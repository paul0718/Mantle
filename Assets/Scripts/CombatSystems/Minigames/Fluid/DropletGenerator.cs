using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropletGenerator : MonoBehaviour
{
    public GameObject droplet;
    public Transform dropletContainer;
    private bool flag = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            InvokeRepeating("GenerateDroplet", 0, 0.4f);
        }
    }
    private void GenerateDroplet()
    {
        Debug.Log("Generated one Droplet.");
        //for (int i = 0; i < 4; i++)
        //{
        //    var d = Instantiate(droplet, new Vector3(-2+(i<2?0.2f:-0.2f), (i%2==0?0.2f:-0.2f), 0), Quaternion.identity, dropletContainer);
        //    d.GetComponent<Rigidbody2D>().velocity = Vector3.right * 5;
        //}
        var d = Instantiate(droplet, new Vector3(-2, 0, 0), Quaternion.identity, dropletContainer);
        d.GetComponent<Rigidbody2D>().velocity = Vector3.right * 5;
    }
}
