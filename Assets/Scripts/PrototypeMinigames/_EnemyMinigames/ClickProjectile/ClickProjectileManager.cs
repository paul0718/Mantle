using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ClickProjectileManager : MonoBehaviour
{
    [HideInInspector] public List<GameObject> projectiles = new List<GameObject>();
    
    [SerializeField] private float Timer = 6f;

    [SerializeField] private SpawnProjectile spawnScript;

    [SerializeField] private SpriteRenderer scannerBG;

    [SerializeField] private AudioSource hitSound;
    // Start is called before the first frame update

    private void OnEnable()
    {
        scannerBG.transform.position = new Vector3(0, -1.87f, 0);
        scannerBG.color = new Color(0, 0.8797369f, 1, 0.5f);
        scannerBG.gameObject.transform.DOLocalMove(new Vector3(0, 2.8f, 0), 1f).SetEase(Ease.InFlash)
            .onComplete = StartGame;
    }

    void Start()
    {
        
    }

    void StartGame()
    {
        StartCoroutine(spawnScript.SpawnProjectilesForClick());
        StartCoroutine(GameTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider!= null)
            {
                hitSound.Play();
                if (hit.transform.CompareTag("EnemyProjectile"))
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }

    IEnumerator GameTimer()
    {
        yield return new WaitForSeconds(Timer);
        Debug.Log(projectiles.Count);
        if (projectiles.Count == 0)
        {
            scannerBG.color = new Color(0, 1, 0, 0.5f);
            GameObject.Find("BattleManagers").GetComponent<GridManager>().UpdateDotPosition(0, 0);
            GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Block Projectile", "W");
        }
        else
        {
            scannerBG.color = new Color(1, 0, 0, 0.5f);
            foreach (var p in projectiles)
            {
                Destroy(p);
            }
            projectiles.Clear();
            GameObject.Find("BattleManagers").GetComponent<GridManager>().UpdateDotPosition(50, 0);
            GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Block Projectile", "L");
        }

        Sequence s = DOTween.Sequence();
        s.AppendInterval(1.0f);
        s.Append(scannerBG.transform.DOLocalMove(new Vector3(0, -1.8f, 0), 1f));
    }
}
