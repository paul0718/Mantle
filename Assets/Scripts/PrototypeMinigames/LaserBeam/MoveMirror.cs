using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveMirror : MonoBehaviour
{
    [HideInInspector]
    public GameObject currMirror;

    [SerializeField] private GameObject upButton;
    [SerializeField] private GameObject downButton;
    
    [SerializeField] private GameObject[] mirrors;

    [SerializeField] private MeshRenderer goalMesh;
    
    [HideInInspector] public bool gameDone = false;

    [SerializeField] private ShootLaser shootScript;
    private void OnEnable()
    {
        SetMinigame();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Mirror") || hit.collider.gameObject.CompareTag("RightMirror"))
                {
                    currMirror = hit.transform.gameObject;
                }
            }
        }
    }

    public void EndGame(GameObject laserObj)
    {
        if (!gameDone)
        {
            gameDone = true;
            goalMesh.material.color = Color.green;
            StartCoroutine(DestroyLaser(laserObj));
            GameObject.Find("Canvas").GetComponent<GridSystem>().ShootLaser(true);  
            SetMinigame();
        }
        
    }

    IEnumerator DestroyLaser(GameObject laser)
    {
        yield return new WaitForSeconds(1.9f);
        shootScript.DestroyLasers();
        goalMesh.material.color = Color.yellow;
    }

    void SetMinigame()
    {
        foreach (GameObject m in mirrors)
        {
            m.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        gameDone = false;
    }
}
