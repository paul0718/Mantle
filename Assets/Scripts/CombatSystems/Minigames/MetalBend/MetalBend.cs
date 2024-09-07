using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.EventSystems;

public class MetalBend : MonoBehaviour
{
    [SerializeField] private GridManager gridScript;
    
    [SerializeField] SpriteShapeController shape;
    [SerializeField] private SpriteShapeController goalShape;

    [SerializeField] private GameObject equal;
    [SerializeField] private GameObject cross;

    [SerializeField] private GameObject border;
    [SerializeField] private GameObject scannerBG;
    [SerializeField] private GameObject goalMagnet;

    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource loseSound;
    
    private void OnEnable()
    {
        SetMinigame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!border.activeSelf)
            {
                border.SetActive(true);
            }
            else
            {
                border.SetActive(false);
            }
        }
    }

    public void LeftSide()
    {
        Vector3 temp = shape.spline.GetPosition(0);
        float newY = temp.y - 0.008f;
        shape.spline.SetPosition(0, new Vector3(temp.x, newY, 0));
    }

    public void RightSide()
    {
        Vector3 temp2 = shape.spline.GetPosition(2);
        float newY2 = temp2.y - 0.008f;
        shape.spline.SetPosition(2, new Vector3(temp2.x, newY2, 0));
    }

    public void CheckResults()
    {
        scannerBG.transform.DOLocalMove(new Vector3(0, 0.55f, 0), 1f);
        Vector3 goalLeftPt = goalShape.spline.GetPosition(0);
        Vector3 playerLeftPt = shape.spline.GetPosition(0);
        Vector3 goalRightPt = goalShape.spline.GetPosition(2);
        Vector3 playerRightPt = shape.spline.GetPosition(2);
        if ((Mathf.Abs(goalLeftPt.y - playerLeftPt.y) < 0.3f))
        {
            if ((Mathf.Abs(goalRightPt.y - playerRightPt.y) < 0.3f))
            {
                winSound.Play();
                equal.SetActive(true);
                gridScript.UpdateDotPosition(0, -40);
                GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Calming Wave", "W");
            }
        }
        else
        {
            loseSound.Play();
            cross.SetActive(true);
            gridScript.UpdateDotPosition(0, 40);
            GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Calming Wave", "L");
        }
    }

    void SetMinigame()
    {
        goalMagnet.SetActive(false);
        scannerBG.transform.DOLocalMove(new Vector3(0, 4.5f, 0), 1.5f).onComplete = SetMagnets;
        equal.SetActive(false);
        cross.SetActive(false);
        Vector3 tempLeft = shape.spline.GetPosition(0);
        Vector3 tempRight = shape.spline.GetPosition(2);
        shape.spline.SetPosition(0, new Vector3(tempLeft.x, 0.3585606f, tempLeft.z));
        shape.spline.SetPosition(2, new Vector3(tempRight.x, 0.3585606f, tempLeft.z));
    }

    void SetMagnets()
    {
        goalMagnet.SetActive(true);
    }
}
