using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SliderControls : MonoBehaviour
{
    [SerializeField] private Slider angleSlider;
    [SerializeField] private Slider powerSlider;

    [SerializeField] private Transform arrow;
    [SerializeField] private RectTransform mask;

    [SerializeField] private ShootJamManager manager;

    [SerializeField] private GameObject lockInButton;

    private bool canLock = false;

    private void OnEnable()
    {
        canLock = false;
    }

    // Update is called once per frame
    void Update()
    {
        arrow.rotation = Quaternion.Euler(0, 0, angleSlider.value);
        mask.anchoredPosition = new Vector3(powerSlider.value, 0, 0);

        if (mask.anchoredPosition.x != -3 && !canLock)
        {
            canLock = true;
            lockInButton.SetActive(true);
        }
    }

    public void LockIn()
    {
        manager.ShootObject();
    }
}
