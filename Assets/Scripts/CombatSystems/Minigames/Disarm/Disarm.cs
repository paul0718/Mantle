using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Disarm : MonoBehaviour
{
    [HideInInspector] public bool gameDone = false;
    private bool moving = true;
    private bool isHorizontal = true;
    
    [SerializeField] private Transform reticalTransform;
    [SerializeField] private float reticalSpeed;
    
    [SerializeField] Vector3 initalPos;
    [SerializeField] Vector3 initalEndPos;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 targetPos;
    private Vector3 currentPos;
    
    [SerializeField] private Image lockInButton;
    public GameObject scannerBG;
    [SerializeField] private float verticalLimit;

    [SerializeField] private AudioSource shootAudio;
    [SerializeField] private AudioSource reloadAudio;
    
    private void OnEnable()
    {
        SetMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = reticalTransform.position;
 
        if(currentPos == startPos) {
            targetPos = endPos;
        }
        else if (currentPos == endPos) {
            targetPos = startPos;
        }

        if (moving)
        {
            reticalTransform.position = Vector3.MoveTowards(reticalTransform.position, targetPos,
                reticalSpeed * Time.deltaTime);
        }
        
    }

    public void LockIn()
    {
        if (isHorizontal)
        {
            isHorizontal = false;
            reloadAudio.Play();
            moving = false;
        
            startPos = currentPos;
            endPos = new Vector3(startPos.x, startPos.y + verticalLimit, 0);
            targetPos = endPos;
        
            lockInButton.color = Color.gray;
            reticalSpeed -= 2;

            StartCoroutine(StartVerticalMode());
        }
        else
        {
            shootAudio.Play();
            moving = false;
            gameDone = true;
        }
    }

    IEnumerator StartVerticalMode()
    {
        yield return new WaitForSeconds(1.0f);
        lockInButton.color = Color.white;
        moving = true;
    }

    private void SetMinigame()
    {
        reticalTransform.gameObject.SetActive(false);
        isHorizontal = true;
        scannerBG.transform.position = new Vector3(0, -2.5f, 0);
        scannerBG.transform.DOMove(new Vector3(0, 2.5f, 0), 1.5f).onComplete = SetReticals;
    }

    void SetReticals()
    {
        reticalSpeed += 2;
        reticalTransform.position = initalPos;
        reticalTransform.gameObject.SetActive(true);
        endPos = initalEndPos;
        startPos = initalPos;
        targetPos = endPos;
        lockInButton.color = Color.white;
        moving = true;
    }
}
