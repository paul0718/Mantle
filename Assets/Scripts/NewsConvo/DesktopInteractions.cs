using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInteractions : MonoBehaviour
{
    [SerializeField] private GameObject taskbarIcon;
    public Vector3 minimizeTo; //TODO: replace with position of taskbarIcon?
    private Vector3 startingPos;

    private float startingScale;
    private Vector3 lastPos;
    private bool maximized;

    void Start()
    {
        startingPos = GetComponent<RectTransform>().anchoredPosition;
        lastPos = startingPos;
        startingScale = transform.localScale.x;
    }

    public void CloseWindow(bool minimize)
    {
        taskbarIcon.transform.GetChild(0).gameObject.SetActive(minimize);
        if (minimize)
        {
            lastPos = GetComponent<RectTransform>().anchoredPosition;
            StartCoroutine(Resize(0, minimizeTo));
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            //reset position & scale on close
            transform.localScale = new Vector3(1, 1, 1) * startingScale;
            GetComponent<RectTransform>().anchoredPosition = startingPos;
            maximized = false;
        }
    }

    public void OpenWindow()
    {
        BringToFront();
        if (taskbarIcon.transform.GetChild(0).gameObject.activeSelf) //if window minimized
        {
            if (maximized)
                StartCoroutine(Resize(2, lastPos));
            else
                StartCoroutine(Resize(startingScale, lastPos));
        }
        else //if window closed
        {
            transform.GetChild(0).gameObject.SetActive(true);
            //TODO: play startup animation?
        }
        taskbarIcon.transform.GetChild(0).gameObject.SetActive(true);
        taskbarIcon.GetComponent<Animator>().Play("TaskbarIconClick");
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
            StartCoroutine(Resize(2, new Vector2(0, 40))); //TODO: don't hard code these?
        }
        maximized = !maximized;
    }

    private IEnumerator Resize(float targetScale, Vector2 targetPos) //TODO: change to FixedUpdate to standardize time?
    {
        float scaleChange = targetScale - transform.localScale.x;
        Vector2 posChange = targetPos - GetComponent<RectTransform>().anchoredPosition;
        for (int i = 0; i < 10; i++)
        {
            transform.localScale += (new Vector3(1, 1, 1) * scaleChange)/10;
            GetComponent<RectTransform>().anchoredPosition += posChange/10;
            yield return new WaitForSeconds(0.005f);
        }
    }
}
