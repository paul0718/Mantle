using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelfDestruct : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image fillUpImg;
    private Image fillImage; // Assign your UI Image in the Inspector
    private float fillDuration = 1f; // How long it takes to fully fill the image

    private float startTime;
    private float fillAmountValue = 0f;
    private bool isHolding = false;
    [SerializeField] private float fillUpScale;
    void Start()
    {
        ShowSelfDestructButton();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        fillUpImg.fillAmount = 0;
        isHolding = false;
    }

    public void OnPointerDown (PointerEventData eventData) 
    {
        Debug.Log (this.gameObject.name + " Was Clicked.");
        isHolding = true;
    }

    public void Update()
    {
        if (isHolding)
        {
            Debug.Log("is holding");
            fillUpImg.fillAmount += (1 / fillUpScale) * Time.deltaTime;
        }
    }

    public void ShowSelfDestructButton()
    {
        fillImage = GetComponent<Image>();
        transform.DOLocalMove(new Vector3(0, 0, 0), 1f);
    }
}
