using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.name == "JammerTarget")
        {
            other.GetComponent<SpriteRenderer>().color = Color.green;
            Destroy(this.gameObject);
            GameObject.Find("ShootJammerGame").GetComponent<ShootJamManager>().won = true;
            GameObject.Find("ResultGO").GetComponent<EndResults>().UpdateResult(true);
        }
    }
}
