using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInteractions : MonoBehaviour
{
    [SerializeField] private GameObject iconHighlight;

    public void CloseWindow(bool minimize)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        iconHighlight.SetActive(minimize);
        /*if (minimize)
        {
            //TODO: play animation
        }*/

        //TODO: pause dialogue
    }

    public void OpenWindow()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.SetSiblingIndex(transform.parent.childCount); //bring window to front
        /*if (iconHighlight.activeSelf) //if window minimized
        {
            //TODO: play animation
        }
        else //if window closed
        {
            //TODO: play startup animation?
        }*/
        iconHighlight.SetActive(true);

        //TODO: resume dialogue
        //TODO: button click anim
    }
}
