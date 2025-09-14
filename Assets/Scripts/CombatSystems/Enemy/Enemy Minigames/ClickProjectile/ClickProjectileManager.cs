using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClickProjectileManager : MonoBehaviour
{
    [HideInInspector] public List<GameObject> projectiles = new List<GameObject>();
    [SerializeField] private float projectileDelay;
    [SerializeField] private float numProjectiles;
    private float timer;

    [SerializeField] private Transform scannerBG;
    [SerializeField] private AudioSource hitSound;

    [SerializeField] private GameObject[] circleReticles;
    [SerializeField] private GameObject bombReticle;
    [SerializeField] private GameObject slashReticle;
    [SerializeField] private GameObject taserReticle;
    [SerializeField] private GameObject aceSlash;

    //Harbinger
    [SerializeField] private GameObject[] shadowCircles;
    [SerializeField] private GameObject shadowBomb;
    [SerializeField] private GameObject shadowSlash;

    private enum enemy{
        NONE,
        ENAGA,
        QUINCY,
        VYZZAR,
        CECIL,
        HARBINGER,
        ACE
    }
    private enemy currentEnemy;

    private void OnEnable()
    {
        if (SequenceManager.Instance.SequenceID == 3)
        {
            currentEnemy = enemy.ENAGA;
            numProjectiles = 6;
            projectileDelay = 1.5f;
            timer = numProjectiles*projectileDelay + 2.5f;
        }
        else if (SequenceManager.Instance.SequenceID == 6)
        {
            currentEnemy = enemy.QUINCY;
            numProjectiles = 6;
            projectileDelay = 1.5f;
            timer = numProjectiles*projectileDelay + 1f;
        }
        else if (SequenceManager.Instance.SequenceID == 9)
        {
            currentEnemy = enemy.VYZZAR;
            numProjectiles = 6;
            projectileDelay = 1.5f;
            timer = numProjectiles*projectileDelay + 1.5f;
        }
        else if (SequenceManager.Instance.SequenceID == 12)
        {

            currentEnemy = enemy.CECIL;
            numProjectiles = 4;
            projectileDelay = 2f;
            timer = numProjectiles*projectileDelay + 1f;
        }
        else if (SequenceManager.Instance.SequenceID == 14)
        {
            currentEnemy = enemy.HARBINGER;
            numProjectiles = 8;
            projectileDelay = 1.5f;
            timer = numProjectiles*projectileDelay + 1f;
        }
        else if (SequenceManager.Instance.SequenceID == 17)
        {
            currentEnemy = enemy.ACE;
            numProjectiles = 6;
            projectileDelay = 1.2f;
            timer = numProjectiles*projectileDelay + 1.5f;
        }

        if (currentEnemy != enemy.NONE)
        {
            scannerBG.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            scannerBG.position = new Vector3(0, -2.5f, 0);
            scannerBG.DOLocalMove(new Vector3(0, 2.4f, 0), 1f).SetEase(Ease.InFlash)
                .onComplete = StartGame;
        }
        else
        {
            Debug.LogError("Sequence number mismatch. No enemy minigame found!");
        }
    }

    void StartGame()
    {
        StartCoroutine(SpawnProjectiles());
        StartCoroutine(GameTimer());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            foreach (Collider2D hit in hits)
            {   
                if (hit.transform.CompareTag("EnemyProjectile"))
                {
                    hitSound.Play();
                    hit.GetComponent<ProjectileReticle>().ClickedOn();
                }
            }
        }
    }

    public IEnumerator SpawnProjectiles()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < numProjectiles; i++)
        {
            if (currentEnemy == enemy.ENAGA)
            {
                StartCoroutine(EnagaAttack());
            }
            else if (currentEnemy == enemy.QUINCY)
            {
                QuincyAttack();
            }
            else if (currentEnemy == enemy.VYZZAR)
            {
                VyzzarAttack();
            }
            else if (currentEnemy == enemy.CECIL)
            {
                CecilAttack();
            }
            else if (currentEnemy == enemy.HARBINGER)
            {
                int attackType = UnityEngine.Random.Range(0, 3);
                if (attackType == 0)
                    StartCoroutine(EnagaAttack(true));
                else if (attackType == 1)
                    QuincyAttack(true);
                else
                    VyzzarAttack(true);
            }
            else if (currentEnemy == enemy.ACE)
            {
                AceAttack();
            }
            yield return new WaitForSeconds(projectileDelay);
        }
    }

    private IEnumerator EnagaAttack(bool harbinger=false)
    {
        GameObject[] reticlePrefabs = (harbinger) ? shadowCircles : circleReticles;
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-4.2f, 4.2f), UnityEngine.Random.Range(1f, 3.5f), 0);
        for (int j = 1; j <= 3; j++)
        {
            GameObject circle = Instantiate(reticlePrefabs[j-1], randomPos + new Vector3(0, 0, j-3), Quaternion.identity);
            if (j == 2)
                circle.GetComponent<ProjectileReticle>().sfx2 = SFXNAME.MidRingProjectile;
            else if (j == 3)
                circle.GetComponent<ProjectileReticle>().sfx3 = SFXNAME.LargeRingProjectile;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void QuincyAttack(bool harbinger=false)
    {
        GameObject reticlePrefab = (harbinger) ? shadowBomb : bombReticle;
        GameObject bomb = Instantiate(reticlePrefab, new Vector3(UnityEngine.Random.Range(-4.2f, 4.2f), UnityEngine.Random.Range(1f, 3f), 0f), Quaternion.identity);
        Vector3 randomPoint = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
        bomb.transform.GetChild(0).transform.localPosition = randomPoint.normalized*0.2f;
    }

    private void VyzzarAttack(bool harbinger=false)
    {
        GameObject reticlePrefab = (harbinger) ? shadowSlash : slashReticle;
        GameObject slash = Instantiate(reticlePrefab, new Vector3(UnityEngine.Random.Range(-4.2f, 4.2f), UnityEngine.Random.Range(1f, 3f), 0f), Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)));
    }

    private void CecilAttack()
    {
        float randomRot = UnityEngine.Random.Range(-45, 45);
        float offset = Mathf.Abs(randomRot)/120f;
        GameObject taser = Instantiate(taserReticle, new Vector3(UnityEngine.Random.Range(-3.1f - offset, 3.1f + offset), UnityEngine.Random.Range(0.5f + 2*offset, 3.9f - 2*offset), 0f), Quaternion.Euler(0, 0, randomRot));
    }

    private void AceAttack()
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-4.2f, 4.2f), UnityEngine.Random.Range(1f, 3f), 0f);
        int rot = UnityEngine.Random.Range(0, 360);
        GameObject slash1 = Instantiate(aceSlash, pos, Quaternion.Euler(0, 0, rot));
        GameObject slash2 = Instantiate(aceSlash, pos, Quaternion.Euler(0, 0, (rot + 90)%360));
    }


    IEnumerator GameTimer()
    {
        yield return new WaitForSeconds(timer);
        var battle = SequenceManager.Instance.CurrentBattle;
        if (projectiles.Count == 0)
        {
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Intercept Projectile", "Win");
            }
            GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(true);
            scannerBG.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
            GridManager.Instance.UpdateDotPosition(Vector2.zero, GridManager.MiniGame.Defend);
            BarkManager.Instance.ShowGameBark("Intercept Projectile", true);
        }
        else
        {
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Intercept Projectile", "Lose");
            }
            GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(false);
            scannerBG.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
            foreach (var p in projectiles)
            {
                if (SequenceManager.Instance.SequenceID == 12)
                {
                    if (p != null)
                        Destroy(p.transform.parent.gameObject);
                }
                else
                    Destroy(p);
            }
            projectiles.Clear();
            GridManager.Instance.UpdateDotPosition(battle.EnemyMinigames[0].LoseEffect, GridManager.MiniGame.Defend);
            BarkManager.Instance.ShowGameBark("Intercept Projectile", false);
        }

        Sequence s = DOTween.Sequence();
        s.AppendInterval(1.0f);
        s.Append(scannerBG.DOLocalMove(new Vector3(0, -2.5f, 0), 1f));
    }
}
