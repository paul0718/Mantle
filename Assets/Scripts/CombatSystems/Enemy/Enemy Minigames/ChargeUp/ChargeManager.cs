using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChargeManager : MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    private Transform boxes;
    [SerializeField] private Color boxColor;
    
    //Enaga
    [Header("Enaga")]
    [SerializeField] private GameObject soundwavePrefab;
    [SerializeField] private Transform soundwaveParent;

    //Quincy
    [Header("Quincy")]
    [SerializeField] private GameObject heartbeatDot;
    [SerializeField] private Transform heartbeatParent;
    
    //Vyzzar
    [Header("Vyzzar")]
    [SerializeField] private GameObject sunMoonCircle;

    //Cecil
    [Header("Cecil")]
    [SerializeField] private GameObject cecilRing;
    private GameObject cecilBox;

    //Harbinger
    [Header("Harbinger")]
    [SerializeField] private GameObject ptBoxPrefab;
    [SerializeField] private GameObject movingPoint;
    [SerializeField] private float ptSpeed;
    private int ptIndex;
    private bool ptMove;
    private bool donePtMove;

    //Ace
    [Header("Ace")]
    [SerializeField] private GameObject aceSymbol;
    [SerializeField] private Vector3 aceFillEnd;
    private float aceFails;
    [SerializeField] private GameObject aceRing;
    private GameObject aceBox;
    public bool canClick;
    private float clickDelay;

    [SerializeField] private GameObject scannerBG;
    
    private bool gameDone = true;
    enum enemy
    {
        NONE,
        ENAGA,
        QUINCY,
        VYZZAR,
        CECIL,
        HARBINGER,
        ACE
    }
    private enemy currentEnemy;
    [SerializeField] private GameObject[] enemyPatterns;


    private void OnEnable()
    {
        if (SequenceManager.Instance.SequenceID == 3)
        {
            currentEnemy = enemy.ENAGA;
            boxes = enemyPatterns[0].transform.GetChild(0);
            foreach (Transform child in boxes)
                Destroy(child.gameObject);
        }
        else if (SequenceManager.Instance.SequenceID == 6)
        {
            boxes = enemyPatterns[1].transform.GetChild(0);
            currentEnemy = enemy.QUINCY;
            foreach (Transform child in boxes)
                Destroy(child.gameObject);
        }
        else if (SequenceManager.Instance.SequenceID == 9)
        {
            boxes = enemyPatterns[2].transform.GetChild(0);
            foreach (Transform child in boxes)
            {
                child.GetComponent<SpriteRenderer>().color = Color.red;
            }
            currentEnemy = enemy.VYZZAR;
        }
        else if (SequenceManager.Instance.SequenceID == 12)
        {
            boxes = enemyPatterns[3].transform.GetChild(0);
            foreach (Transform child in boxes)
                child.GetComponent<SpriteRenderer>().color = boxColor;
            currentEnemy = enemy.CECIL;
        }
        else if (SequenceManager.Instance.SequenceID == 14)
        {
            boxes = enemyPatterns[4].transform.GetChild(0);
            foreach (Transform child in boxes)
                Destroy(child.gameObject);
            movingPoint.SetActive(false);
            donePtMove = false;
            currentEnemy = enemy.HARBINGER;
        }
        else if (SequenceManager.Instance.SequenceID == 17)
        {
            boxes = enemyPatterns[5].transform.GetChild(0);
            Vector3 currScale = aceSymbol.transform.GetChild(0).GetChild(0).transform.localScale;
            aceSymbol.transform.GetChild(0).GetChild(0).transform.localScale = new Vector3(currScale.x, 0, currScale.z);
            aceFails = 0;
            foreach (Transform child in boxes)
                child.GetComponent<SpriteRenderer>().color = boxColor;
            currentEnemy = enemy.ACE;
        }

        if (currentEnemy != enemy.NONE)
        {
            scannerBG.transform.position = new Vector3(0, -2.5f, 0);
            scannerBG.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            scannerBG.transform.DOLocalMove(new Vector3(0, 2.4f, 0), 1f).SetEase(Ease.InFlash).onComplete = Setup;
            foreach (GameObject g in enemyPatterns)
                g.SetActive(false);
        }
        else
        {
            Debug.LogError("Sequence number mismatch. No enemy minigame found!");
        }
    }

    private void Setup()
    {
        if (currentEnemy == enemy.ENAGA)
            enemyPatterns[0].SetActive(true);
        else if (currentEnemy == enemy.QUINCY)
            enemyPatterns[1].SetActive(true);
        else if (currentEnemy == enemy.VYZZAR)
            enemyPatterns[2].SetActive(true);
        else if (currentEnemy == enemy.CECIL)
            enemyPatterns[3].SetActive(true);
        else if (currentEnemy == enemy.HARBINGER)
            enemyPatterns[4].SetActive(true);
        else if (currentEnemy == enemy.ACE)
            enemyPatterns[5].SetActive(true);
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentEnemy == enemy.ENAGA)
        {
            float[] randomX = new float[]{Random.Range(-4.5f, 4.5f), Random.Range(-4.5f, 4.5f), Random.Range(-4.5f, 4.5f)};
            while (Mathf.Abs(randomX[0]) < 1.5f)
                randomX[0] = Random.Range(-4.5f, 4.5f);
            while (Mathf.Abs(randomX[1]-randomX[0]) < 1 || Mathf.Abs(randomX[1]) < 1.5f)
                randomX[1] = Random.Range(-4.5f, 4.5f);
            while (Mathf.Abs(randomX[2]-randomX[1]) < 1 || Mathf.Abs(randomX[2]-randomX[0]) < 1 || Mathf.Abs(randomX[2]) < 1.5f)
                randomX[2] = Random.Range(-4.5f, 4.5f);
            //Sort list
            for (int i = 0; i < 3; i++)
            {
                for (int j = i+1; j < 3; j++)
                {
                    if (Mathf.Abs(randomX[i]) > Mathf.Abs(randomX[j]))
                    {
                        float temp = randomX[i];
                        randomX[i] = randomX[j];
                        randomX[j] = temp;
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                GameObject box = Instantiate(boxPrefab, Vector2.zero, Quaternion.identity, boxes);
                box.transform.localPosition = new Vector3(randomX[i], 0, 0);
                box.GetComponent<SpriteRenderer>().color = boxColor;
                box.name = "Box";
                yield return new WaitForSeconds(0.8f);
            }
        }
        else if (currentEnemy == enemy.QUINCY)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject box = Instantiate(boxPrefab, Vector2.zero, Quaternion.identity, boxes);
                box.transform.localPosition = new Vector3(-4 + 4*i, 0, 0);
                box.transform.localEulerAngles = new Vector3(0, 0, 90);
                boxes.GetChild(i).GetComponent<SpriteRenderer>().color = boxColor;
                box.name = "Box";
                yield return new WaitForSeconds(0.8f);
            }
        }
        else if (currentEnemy == enemy.VYZZAR)
        {
            sunMoonCircle.SetActive(true);
            sunMoonCircle.GetComponent<VyzzarArc>().Reset();
            //TODO: countdown by having ring close in 3 times?
            yield return new WaitForSeconds(1);
        }
        else if (currentEnemy == enemy.HARBINGER)
        {
            ptIndex = 0;
            Transform points = enemyPatterns[4].transform.GetChild(1);
            int r = Random.Range(0, points.childCount);
            movingPoint.transform.position = points.transform.GetChild(r).position;
            movingPoint.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            for (int i = 1; i < 5; i++)
            {
                GameObject box = Instantiate(ptBoxPrefab, points.GetChild((r+i)%points.childCount).position, Quaternion.identity, boxes.transform);
                box.GetComponent<SpriteRenderer>().color = boxColor;
                yield return new WaitForSeconds(0.3f);
            }
        }
        StartGame();
    }

    private void StartGame()
    {
        if (currentEnemy == enemy.ENAGA)
        {
            GameObject wave1 = Instantiate(soundwavePrefab, soundwaveParent.position, Quaternion.identity, soundwaveParent);
            wave1.GetComponent<Soundwave>().speed = -2.5f;
            wave1.transform.rotation = Quaternion.Euler(0, 0, 180);
            GameObject wave2 = Instantiate(soundwavePrefab, soundwaveParent.position, Quaternion.identity, soundwaveParent);
            wave2.GetComponent<Soundwave>().speed = 2.5f;
        }
        else if (currentEnemy == enemy.QUINCY)
        {
            heartbeatDot.SetActive(true);
        }
        else if (currentEnemy == enemy.VYZZAR)
        {
            sunMoonCircle.GetComponent<VyzzarArc>().paused = false;
        }
        else if (currentEnemy == enemy.CECIL)
        {
            StartCoroutine(CecilCircles());
        }
        else if (currentEnemy == enemy.HARBINGER)
        {
            ptMove = true;
        }
        else if (currentEnemy == enemy.ACE)
        {
            StartCoroutine(AceSpin());
        }
        gameDone = false;
    }

    void Update()
    {
        if (currentEnemy == enemy.ENAGA)
        {
            if (soundwaveParent.childCount == 0 && !gameDone)
                GameDone();
        }
        else if (currentEnemy == enemy.QUINCY)        
        {
            if (!heartbeatDot.activeSelf && !gameDone)
                GameDone();
        }
        else if (currentEnemy == enemy.VYZZAR)
        {
            if (sunMoonCircle.GetComponent<VyzzarArc>().currentAngle > sunMoonCircle.GetComponent<VyzzarArc>().endAngle && !gameDone)
            {
                sunMoonCircle.GetComponent<VyzzarArc>().paused = true;
                sunMoonCircle.SetActive(false);
                GameDone();
            }
        }
        else if (currentEnemy == enemy.CECIL && Input.GetMouseButtonDown(0))
        {
            if (canClick && clickDelay <= 0)
            {
                cecilBox.GetComponent<SpriteRenderer>().color = Color.green;
                cecilBox.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.1f)
                        .SetLoops(2, LoopType.Yoyo);
                AudioManager.Instance.PlayOneShot(SFXNAME.HitMark);
            }
            else
            {
                clickDelay = 0.2f;
                AudioManager.Instance.PlayOneShot(SFXNAME.MissMark);
            }
        }
        else if (currentEnemy == enemy.HARBINGER && ptMove)
        {
            Transform target = boxes.transform.GetChild(ptIndex);
            Vector3 ptPos = movingPoint.transform.position;
            movingPoint.transform.position += (target.position-ptPos).normalized * ptSpeed * Time.deltaTime;
            if (Vector3.Distance(target.position, ptPos) < 0.01f)
            {
                if (ptIndex == 3)
                {
                    if (!donePtMove)
                    {
                        donePtMove = true;
                        StartCoroutine(DelayedGameDone(1));
                    }
                }
                else
                {
                    ptIndex++;
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (Vector3.Distance(target.position, ptPos) < 0.5f && clickDelay <= 0) //hit box
                {
                    target.GetComponent<SpriteRenderer>().color = Color.green;
                    target.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.1f).SetLoops(2, LoopType.Yoyo);
                    AudioManager.Instance.PlayOneShot(SFXNAME.HitMark);
                }
                else
                {
                    clickDelay = 0.2f;
                    AudioManager.Instance.PlayOneShot(SFXNAME.MissMark);
                }
            }
        }
        else if (currentEnemy == enemy.ACE && Input.GetMouseButtonDown(0))
        {
            if (canClick && clickDelay <= 0)
            {
                aceBox.GetComponent<SpriteRenderer>().color = Color.green;
                aceBox.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.1f)
                        .SetLoops(2, LoopType.Yoyo);
                AudioManager.Instance.PlayOneShot(SFXNAME.HitMark);
            }
            else
            {
                clickDelay = 0.2f;
                AudioManager.Instance.PlayOneShot(SFXNAME.MissMark);
            }
        }
        clickDelay -= Time.deltaTime;
    }


    private IEnumerator CecilCircles()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.5f);
            do
            {
                cecilBox = boxes.GetChild(Random.Range(0, boxes.childCount)).gameObject;
            }
            while (cecilBox.GetComponent<SpriteRenderer>().color != boxColor);
            cecilRing.transform.position = cecilBox.transform.position;
            cecilRing.SetActive(true);
            for (float j = 0; j < 1; j += 0.02f)
            {
                cecilRing.transform.localScale = new Vector3(1, 1, 1) * Mathf.Lerp(0.5f, 0.05f, j);
                yield return new WaitForSeconds(0.01f);
                if (j >= 0.8f)
                    canClick = true;
            }
            cecilRing.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            canClick = false;
        }
        yield return new WaitForSeconds(0.5f);
        GameDone();
    }


    private IEnumerator AceSpin()
    {
        for (int i = 0; i < 4; i++)
        {
            do
            {
                aceBox = boxes.GetChild(Random.Range(0, boxes.childCount)).gameObject;
            }
            while (aceBox.GetComponent<SpriteRenderer>().color != boxColor);

            for (float j = 0; j < 1; j += 0.025f)
            {
                aceRing.transform.localScale = new Vector3(1, 1, 1) * (0.3f-(0.2f*j));
                yield return new WaitForSeconds(0.01f);
                if (j > 0.8f)
                    canClick = true;
            }
            //rotate to point
            float startAngle = aceSymbol.transform.eulerAngles.z;
            float targetAngle = Mathf.Atan2(aceBox.transform.position.y - aceSymbol.transform.position.y, aceBox.transform.position.x - aceSymbol.transform.position.x) * Mathf.Rad2Deg - 90;
            float totalRotation = targetAngle - startAngle;
            while (totalRotation < 360)
                totalRotation += 360;
            for (float j = 0; j < 0.5f; j += 0.01f)
            {
                float newAngle = startAngle + totalRotation * Mathf.SmoothStep(0f, 1f, j/0.5f);
                aceSymbol.transform.rotation = Quaternion.Euler(0, 0, newAngle);
                yield return new WaitForSeconds(0.01f);
            }
            aceSymbol.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            aceRing.transform.position = aceBox.transform.position;
            aceRing.SetActive(true);
            for (float j = 0; j < 1; j += 0.025f)
            {
                aceRing.transform.localScale = new Vector3(1, 1, 1) * Mathf.Lerp(0.5f, 0.1f, j);
                yield return new WaitForSeconds(0.01f);
                if (j > 0.8f)
                    canClick = true;
            }
            aceRing.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            canClick = false;
            if (aceBox.GetComponent<SpriteRenderer>().color != Color.green)
            {
                aceFails++;
                Vector3 currScale = aceSymbol.transform.GetChild(0).GetChild(0).transform.localScale;
                for (float j = 0; j < 0.5f; j += 0.01f)
                {
                    aceSymbol.transform.GetChild(0).GetChild(0).transform.localScale = Vector3.Lerp(currScale, new Vector3(aceFillEnd.x, aceFillEnd.y*aceFails/4f, 1), j/0.5f);
                    yield return new WaitForSeconds(0.01f);
                }
                if (i == 3)
                    yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
        GameDone();
    }


    private IEnumerator DelayedGameDone(float delay)
    {
        yield return new WaitForSeconds(delay/2);
        if (currentEnemy == enemy.HARBINGER)
        {
            movingPoint.SetActive(false);
            ptMove = false;
        }
        yield return new WaitForSeconds(delay/2);
        GameDone();
    }

    private void GameDone()
    {
        gameDone = true;
        bool playerWin = true;
        var battle = SequenceManager.Instance.CurrentBattle;
        int missed = 0;
        for (int i = 0; i < boxes.childCount; i++)
        {
            if (boxes.GetChild(i).GetComponent<SpriteRenderer>().color == boxColor)
            {
                missed++;
            }
        }
        if ((currentEnemy == enemy.CECIL && missed <= 1) || (currentEnemy == enemy.ACE && missed < 8) || missed == 0)
        {
            GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(true);
            scannerBG.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
            GridManager.Instance.UpdateDotPosition(Vector2.zero, GridManager.MiniGame.Defend); //change to actual values
            BarkManager.Instance.ShowGameBark("Charge Interrupt", true);
        }
        else
        {
            GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(false);
            scannerBG.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
            GridManager.Instance.UpdateDotPosition(battle.EnemyMinigames[1].LoseEffect, GridManager.MiniGame.Defend); //change to actual values
            BarkManager.Instance.ShowGameBark("Charge Interrupt", false);
        }
        foreach (GameObject g in enemyPatterns)
            g.SetActive(false);
        Sequence s = DOTween.Sequence();
        s.AppendInterval(1.0f);
        s.Append(scannerBG.transform.DOLocalMove(new Vector3(0, -2.5f, 0), 1f));
    }
}