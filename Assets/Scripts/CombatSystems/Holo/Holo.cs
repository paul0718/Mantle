using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Holo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.gameObject.SetActive(false);
    }
}
