using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Disarm : MonoBehaviour
{
    [HideInInspector] public bool gameDone = false;
    private bool moving;
    private bool isHorizontal = true;
    
    [SerializeField] private GameObject reticle;
    [SerializeField] private GameObject goalReticle;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    private float reticleSpeed;
    [SerializeField] private Color initialColor;

    [SerializeField] Transform topLeft;
    [SerializeField] Transform topRight;
    [SerializeField] Transform bottomLeft;
    private Vector3 targetPos;
    private Vector3 currentPos;
    
    [SerializeField] private Image lockInButton;
    public GameObject scannerBG;

    [SerializeField] private AudioSource shootAudio;
    [SerializeField] private AudioSource reloadAudio;
    
    private void OnEnable()
    {
        SetMinigame();
    }

    private void SetMinigame()
    {
        reticle.SetActive(false);
        goalReticle.SetActive(false);
        isHorizontal = true;
        scannerBG.transform.position = new Vector3(0, -2.5f, 0);
        scannerBG.transform.DOMove(new Vector3(0, 2.4f, 0), 1.5f).onComplete = SetReticles;
    }

    void Update()
    {
        currentPos = reticle.transform.position;
 
        if (isHorizontal)
        {
            if (currentPos.x <= topLeft.position.x) {
                targetPos = topRight.position;
            }
            else if (currentPos.x >= topRight.position.x) {
                targetPos = topLeft.position;
            }
        }
        else
        {
            if(currentPos.y <= bottomLeft.position.y) {
                targetPos = new Vector3(currentPos.x, topLeft.position.y, currentPos.z);
            }
            else if (currentPos.y >= topLeft.position.y) {
                targetPos = new Vector3(currentPos.x, bottomLeft.position.y, currentPos.z);
            }
        }

        if (moving)
        {
            reticle.transform.position = Vector3.MoveTowards(reticle.transform.position, targetPos,
                reticleSpeed * Time.deltaTime);
        }
    }

    public void LockIn()
    {
        lockInButton.GetComponent<Button>().interactable = false;
        if (isHorizontal)
        {
            isHorizontal = false;
            AudioManager.Instance.PlayOneShot(SFXNAME.LockOnH, 0.8f);
            moving = false;        
            reticleSpeed = verticalSpeed;
            StartCoroutine(StartVerticalMode());
        }
        else
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.LockOnVver2, 0.8f);
            //TODO: play different audio on hit vs miss
            moving = false;
            gameDone = true;
        }
    }

    IEnumerator StartVerticalMode()
    {
        yield return new WaitForSeconds(1.0f);
        lockInButton.GetComponent<Button>().interactable = true;
        moving = true;
    }

    void SetReticles()
    {
        goalReticle.transform.position = BattleSequenceManager.Instance.disarmPos;
        reticleSpeed = horizontalSpeed;
        reticle.transform.position = topLeft.position;
        reticle.GetComponent<SpriteRenderer>().color = initialColor;
        reticle.SetActive(true);
        goalReticle.SetActive(true);
        reticle.GetComponent<Animator>().Play("ReticleOn");
        lockInButton.GetComponent<Button>().interactable = true;
        moving = true;
    }
}