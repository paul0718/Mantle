using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Collections;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    private readonly Dictionary<string, Enemies> EnemyMap = new()
    {
        { "Enaga", Enemies.Enaga },
        { "Quincy", Enemies.Quincy },
        { "Vyzzar", Enemies.Vyzzar },
        { "Cecil", Enemies.Cecil},
        { "Harbinger", Enemies.Harbinger },
        { "Ace", Enemies.Ace }
    };
    private enum Enemies
    {
        Enaga,
        Quincy,
        Vyzzar,
        Cecil,
        Harbinger,
        Ace
    }
    
    /*
    Enaga, Quincy, Harbinger, Cecil: x = +-1
    Vyzzar:
        Left: -0.43, 2.45
        Right: 0.43, -2.45
    */
    
    [SerializeField] private Enemies enemy;
    
    [SerializeField] private List<GameObject> IndicatorPrefabs;
    [SerializeField] private List<Transform> IndicatorSpawnPositions;
    private List<Indicator> indicators;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float attackTimeTotal; // unit: seconds
    [SerializeField] private Vector3 attackPoint;
    [SerializeField] private GameObject arms;
    private Vector2 armsPos; 
    
    private float timeLeft = -1f;
    private bool ended = true;
    private bool winning = false;
    private int roundsTotal;
    private int roundsRemain;

    [SerializeField] private float effectDuration = 0.25f;
    [SerializeField] private GameObject flashObject;  // Assign in Inspector
    [SerializeField] private float flashDuration = 0.25f;
    [SerializeField] private float startAlpha = 0.8f;

    private float[] spawnArea; // float[4]
    private float[] indicatorExtents; // float[2]
    
    private AudioSource endSoundAudioSource;
    
    void OnEnable() // Reset everything
    {
        if (timeLeft == -1f)
        {
            return;
        }
        StartGame();
    }
    void Start()
    {
        InitBlockGame();
        StartGame();
    }
    void Update()
    {
        if (ended) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft > 0) return;
        
        roundsRemain--;
        winning = IsWinning();
        if (!winning || roundsRemain == 0) // Game ends due to round lost or no rounds remaining
        {
            ended = true;
            DisableCtrl();
            StartCoroutine(ShowEffect(winning));
            ResetAll();
            EndsGame(winning);
        }
        else
        {
            StartCoroutine(ShowEffect(true));
            ResetRound();
            StartGame();
        }
    }
    
    private IEnumerator ShowEffect(bool isWinning)
    {
        const float RotationAngle = 180f;
        const float EndBuffer = 0.1f;

        if (enemy == Enemies.Ace && !isWinning)
        {
            for (int i = 0; i < 2; i++)
            {
                Transform indicatorTrans = indicators[i].transform;
                Transform effectTransform = CreateEffectTransform(indicatorTrans);
                
                Vector3 middleWorld = IndicatorSpawnPositions[1].position;

                Sequence flightSequence = DOTween.Sequence();
                flightSequence.Append(effectTransform.DOMove(middleWorld, effectDuration / 2).SetEase(Ease.OutQuad));
                flightSequence.Join(effectTransform.DORotate(Vector3.forward * RotationAngle, effectDuration / 2));
                flightSequence.Append(effectTransform.DOMove(attackPoint, effectDuration / 2).SetEase(Ease.InQuad));
                flightSequence.OnComplete(() => Destroy(effectTransform.gameObject));
            }

            yield return DOTween.Sequence()
                .AppendInterval(effectDuration + EndBuffer)
                .OnComplete(FlashOnHit)
                .WaitForCompletion();
            yield break;
        }

        foreach (Indicator indicator in indicators)
        {
            Transform indicatorTrans = indicator.transform;
            Transform effectTransform = CreateEffectTransform(indicatorTrans);

            if (indicator.IsBlocked())
            {
                Vector3 targetScale = effectTransform.localScale * 0.1f;

                Sequence blockedSequence = DOTween.Sequence();
                blockedSequence.Join(effectTransform.DOScale(targetScale, effectDuration));
                blockedSequence.Join(effectTransform.DORotate(new Vector3(0f, 0f, 720f), effectDuration, RotateMode.LocalAxisAdd));
                blockedSequence.OnComplete(() => Destroy(effectTransform.gameObject));
            }
            else
            {
                Vector3 targetScale = effectTransform.localScale * 10f;

                Sequence unblockedSequence = DOTween.Sequence();
                unblockedSequence.Join(effectTransform.DOMove(attackPoint, effectDuration));
                unblockedSequence.Join(effectTransform.DOScale(targetScale, effectDuration));
                unblockedSequence.OnComplete(() => Destroy(effectTransform.gameObject));
            }
        }

        yield return DOTween.Sequence()
            .AppendInterval(effectDuration + EndBuffer)
            .OnComplete(() =>
                {
                    if (!isWinning) FlashOnHit();
                })
            .WaitForCompletion();
    }
    private Transform CreateEffectTransform(Transform referenceTransform)
    {
        Transform effect = Instantiate(effectPrefab, referenceTransform.position, referenceTransform.rotation).transform;
        effect.localScale = referenceTransform.lossyScale;
        return effect;
    }
    private void FlashOnHit()
    {
        if (flashObject == null) return;
        flashObject.SetActive(true);
        endSoundAudioSource.Play();
        var flashSprite = flashObject.GetComponent<SpriteRenderer>();

        Color c = flashSprite.color;
        c.a = startAlpha;
        flashSprite.color = c;

        flashSprite
            .DOFade(0f, flashDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                flashObject.SetActive(false);
            });
    }
    private bool IsWinning()
    {
        var result = true;
        for (int i = 0; i < indicators.Count; i++)
        {
            var indicator = indicators[i];
            if (!indicator.IsBlocked())
            {
                result = false;
            }
        }
        Debug.Log(result);
        return result;
    }
    private void ResetAll()
    {
        timeLeft = attackTimeTotal;
        roundsRemain = roundsTotal;
        arms.transform.DOLocalMoveY(armsPos.y, 1f).SetEase(Ease.InFlash);
        BroadcastMessage("Resetting");
        RandomizeIndicators();
    }
    private void ResetRound()
    {
        timeLeft = attackTimeTotal;
        BroadcastMessage("Resetting");
        RandomizeIndicators();
    }
    private void StartGame()
    {
        // Move Arms Up and Start
        arms.transform.DOLocalMoveY(0f, 1f).SetEase(Ease.InFlash)
            .OnComplete(() =>
            {
                BroadcastMessage("StartAttack");
                ended = false;
                EnableCtrl();
            });
    }
    private void DisableCtrl()
    {
        BroadcastMessage("DisableArmControl");
    }
    private void EnableCtrl()
    {
        BroadcastMessage("EnableArmControl");
    }
    private void InitBlockGame()
    {
        // Set Enemy
        enemy = EnemyMap.TryGetValue(SequenceManager.Instance.CurrentBattle.Enemy, out var foundEnemy) 
            ? foundEnemy : Enemies.Enaga;
        roundsTotal = (enemy == Enemies.Ace) ? 3 : 1; // Set rounds based on enemy
        roundsRemain = roundsTotal;
        armsPos = arms.transform.position; // Store default arms position
        timeLeft = attackTimeTotal; // Reset Timer
        ended = true; // Set Game State
        endSoundAudioSource = GetComponent<AudioSource>();
        endSoundAudioSource.playOnAwake = false;
        
        indicators = new List<Indicator>();
        
        Vector3 leftPos = IndicatorSpawnPositions[0].position;
        Vector3 midPos = IndicatorSpawnPositions[1].position;
        Vector3 rightPos = IndicatorSpawnPositions[2].position;
        var prefab = IndicatorPrefabs[(int)enemy];

        switch (enemy)
        {
            case Enemies.Vyzzar:
                leftPos.y += 2.78f;
                rightPos.y += 2.78f;
                indicators.Add(SpawnIndicator(prefab, leftPos, Quaternion.identity));
                indicators.Add(SpawnIndicator(prefab, rightPos, Quaternion.identity));
                break;

            case Enemies.Cecil:
                indicators.Add(SpawnIndicator(prefab, leftPos, Quaternion.Euler(0, 0, 90)));
                indicators.Add(SpawnIndicator(prefab, rightPos, Quaternion.Euler(0, 0, 90)));
                break;

            case Enemies.Harbinger:
                indicators.Add(SpawnIndicator(prefab, leftPos, Quaternion.identity));
                indicators.Add(SpawnIndicator(prefab, rightPos, Quaternion.identity));
                break;

            case Enemies.Ace:
                var left = SpawnIndicator(prefab, leftPos, Quaternion.Euler(0, 0, 135));
                left.transform.localScale = new Vector3(-1 * left.transform.localScale.x, left.transform.localScale.y, left.transform.localScale.z);
                indicators.Add(left);
                indicators.Add(SpawnIndicator(prefab, rightPos, Quaternion.Euler(0, 0, -135)));
                break;

            default:
                for (int i = 0; i < IndicatorSpawnPositions.Count; i++)
                {
                    indicators.Add(SpawnIndicator(prefab, IndicatorSpawnPositions[i].position, Quaternion.identity));
                }
                break;
        }

        effectPrefab.GetComponent<SpriteRenderer>().sprite = indicators[0].GetMainSprite();
        BroadcastMessage("InitIndicator", attackTimeTotal, SendMessageOptions.DontRequireReceiver);
        InitSpawnArea();
        RandomizeIndicators();
        // BroadcastMessage("Randomize");
    }
    private Indicator SpawnIndicator(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instantiate(prefab, position, rotation, transform).GetComponent<Indicator>();
    }
    private void EndsGame(bool playerWins)
    {
        var battle = BattleSequenceManager.Instance.enemyMinigames;
        
        /*endSound.clip = playerWins ? windSound : loseSound;
        endSound.Play();*/

        if (playerWins)
        {
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Block", "Win");
            }
            GridManager.Instance.UpdateDotPosition(Vector2.zero, GridManager.MiniGame.Defend, playerWins);
        }
        else
        {
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Block", "Lose");
            }
            GridManager.Instance.UpdateDotPosition(battle[3].LoseEffect, GridManager.MiniGame.Defend, playerWins);
        }
        BarkManager.Instance.ShowGameBark("Block", playerWins);
        GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(playerWins);
    }
    private void InitSpawnArea()
    {
        switch (enemy)
        {
            case Enemies.Vyzzar:
            case Enemies.Harbinger:
                return;
            default:
                break;
        }
        
        // Set the bottom of the grid to the upper edge of energy bar
        var energyBar = GameObject.Find("EnergyBar").transform.Find("Background").GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        energyBar.GetWorldCorners(corners);
        float yMin = corners[1].y;
        
        // Set the top, left and right of the grid to the edge of the screen
        // Untiy screen space starts at bottom left
        var mainCam = Camera.main;
        var upLeft = mainCam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        var upRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        float xMin = upLeft.x;
        float yMax = upLeft.y;
        float xMax = upRight.x;
        
        // Init extents for later calculation for spawn area
        var outlineSpriteRenderer = indicators[0].gameObject.transform.Find("Outline").GetComponent<SpriteRenderer>();
        indicatorExtents = new float[] {outlineSpriteRenderer.bounds.extents.x, outlineSpriteRenderer.bounds.extents.y};
        /*float extentX = indicatorExtents[0];
        float extentY = indicatorExtents[1];
        
        xMin += extentX;
        xMax -= extentX;
        yMin += extentY;
        yMax -= extentY;*/
        
        spawnArea = new float[] { xMin, xMax, yMin, yMax };
        // TODO: make sure when generating indicators don't touch each other and the arms
    }
    private void RandomizeIndicators()
    {
        switch (enemy)
        {
            case Enemies.Vyzzar:
            case Enemies.Harbinger:
                return;
            default:
                break;
        }
        
        var gridWidth = (spawnArea[1] - spawnArea[0]) / indicators.Count;
        var gridMinY = spawnArea[2] + indicatorExtents[1];
        var gridMaxY = spawnArea[3] - indicatorExtents[1];
        for (int i = 0; i < indicators.Count; i++)
        {
            var indicatorCollider = indicators[i].gameObject.transform.GetComponentInChildren<Collider2D>();
            do
            {
                var gridMinX = spawnArea[0] + gridWidth * i + indicatorExtents[0];
                var gridMaxX = spawnArea[0] + gridWidth * (i + 1) - indicatorExtents[0];
                var newPositionX = Random.Range(gridMinX, gridMaxX);
                var newPositionY = Random.Range(gridMinY, gridMaxY);
                var newPositionZ = indicators[i].gameObject.transform.position.z;
                indicators[i].gameObject.transform.position = new Vector3(newPositionX, newPositionY, newPositionZ);
            } while (IsOverlapping(indicatorCollider));
        }
    }
    private bool IsOverlapping(Collider2D collider)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        int mask = LayerMask.GetMask("Ignore Raycast");
        filter.SetLayerMask(mask);
        filter.useTriggers = true;
        int result = collider.OverlapCollider(filter, new List<Collider2D>());
        return result != 0;
    }
}
