using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ShootJamManager : MonoBehaviour
{
    [SerializeField] private Transform arrow;

    [SerializeField] private GameObject shot;

    [SerializeField] private Slider powerSlider;
    
    [SerializeField] private Slider angleSlider;

    [SerializeField] private float powerValue, heightPower;

    [SerializeField] private GameObject target;
    
    [SerializeField] private GameObject lockInButton;

    [HideInInspector] public bool won = false;

    private GameObject projectile;

    private void OnEnable()
    {
        won = false;
        target.GetComponent<SpriteRenderer>().color = Color.white;
        lockInButton.SetActive(false);
        powerSlider.value = -3;
        angleSlider.value = 0;
    }

    public void ShootObject()
    {
        projectile = Instantiate(shot, new Vector3(arrow.position.x, arrow.position.y, arrow.position.z),
            arrow.rotation);
        Vector2 impulse = new Vector2((4 - Mathf.Abs(powerSlider.value)) * powerValue, heightPower) *
                          projectile.transform.right.normalized;
        projectile.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
        projectile.GetComponent<Rigidbody2D>().gravityScale = 1;
        lockInButton.SetActive(false);
        StartCoroutine(CheckLose());
    }

    IEnumerator CheckLose()
    {
        yield return new WaitForSeconds(5f);
        if (!won)
        {
            Destroy(projectile);
            GameObject.Find("ResultGO").GetComponent<EndResults>().UpdateResult(false);
        }
    }
}
