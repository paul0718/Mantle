using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInteractions : MonoBehaviour
{
    [SerializeField] private GameObject taskbarIcon;
    private Vector3 startingPos;

    private float startingScale;
    private Vector3 lastPos;
    private bool maximized;

    void Start()
    {
        startingPos = GetComponent<RectTransform>().anchoredPosition;
        startingScale = transform.localScale.x;
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
            //reset position on close
            if (maximized)
                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 40); //to center if maximized
            else
                GetComponent<RectTransform>().anchoredPosition = startingPos; //to original pos if not maximized
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

    public void Fullscreen()
    {
        BringToFront();
        if (maximized)
        {
            StartCoroutine(Resize(startingScale, lastPos));
        }
        else
        {
            lastPos = GetComponent<RectTransform>().anchoredPosition;
            StartCoroutine(Resize(2, new Vector2(0, 40)));
        }
        maximized = !maximized;
    }

    private IEnumerator Resize(float targetScale, Vector2 targetPos) //TODO: change to FixedUpdate to standardize time?
    {
        float scaleChange = targetScale - transform.localScale.x;
        Vector2 posChange = targetPos - GetComponent<RectTransform>().anchoredPosition;
        for (int i = 0; i < 10; i++)
        {
            transform.localScale += new Vector3(scaleChange, scaleChange, scaleChange)/10;
            GetComponent<RectTransform>().anchoredPosition += posChange/10;
            yield return new WaitForSeconds(0.005f);
        }
    }
}
