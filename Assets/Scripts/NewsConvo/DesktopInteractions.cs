using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInteractions : MonoBehaviour
{
    [SerializeField] private GameObject taskbarIcon;
    private Vector3 startingPos;

    void Start()
    {
        startingPos = GetComponent<RectTransform>().anchoredPosition;
    }

    public void CloseWindow(bool minimize)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        taskbarIcon.transform.GetChild(0).gameObject.SetActive(minimize);
        if (minimize)
        {
            //TODO: play animation
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = startingPos; //reset position on close
        }

        //TODO: pause dialogue
    }

    public void OpenWindow()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        BringToFront();
        /*if (iconHighlight.activeSelf) //if window minimized
        {
            //TODO: play animation
        }
        else //if window closed
        {
            //TODO: play startup animation?
        }*/
        taskbarIcon.transform.GetChild(0).gameObject.SetActive(true);
        taskbarIcon.GetComponent<Animator>().Play("TaskbarIconClick");

        //TODO: resume dialogue
        //TODO: button click anim
    }

    public void BringToFront()
    {
        transform.SetSiblingIndex(transform.parent.childCount);
    }
}
