using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

public class RepairManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> PieceSprites;
    [SerializeField] private PhysicsMaterial2D pieceMaterial;
    [SerializeField] private SpriteTimer timer;
    [SerializeField] private Transform attackAlert;
    [SerializeField] private float attackInterval;
    [SerializeField] private int knockOffPerAttack = 4;
    [SerializeField] private float knockOffForce;
    [SerializeField] private float gravityFactor;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    
    private List<Part> Parts;
    private List<Part> KnockOffCandidates;
    private int totalFixed = 0;
    private AudioSource audioSource;
    private bool hasEnded;
    private readonly Vector2 dotMovement = Vector2.zero;
    private bool isInitialized = false;

    private float alertDuration = 0.6f;
    private Coroutine attackCoroutine;

    private float delayTime = 0f;

    void OnEnable()
    {
        if (!isInitialized) return;
        attackCoroutine = StartCoroutine(RepeatedAttack());
    }
    
    void OnDisable()
    {
        StopCoroutine(attackCoroutine);
        totalFixed = 0;
        hasEnded = false;
        KnockOffCandidates.Clear();
        foreach (Part part in Parts)
        {
            KnockOffCandidates.Add(part);
        }
        timer.ResetTimer();
    }
    
    void Start()
    {
        GameObject slotObj;
        GameObject pieceObj;
        Part partScript;
        Rigidbody2D pieceRb;
        SpriteRenderer slotRenderer;
        
        Parts = new List<Part>();
        KnockOffCandidates = new List<Part>();
        audioSource = GetComponent<AudioSource>();
        
        totalFixed = 0;
        hasEnded = false;
        
        for (int i = 0; i < PieceSprites.Count; i++)
        {
            slotObj = PieceSprites[i];
            slotObj.AddComponent<PolygonCollider2D>();
            pieceObj = Instantiate(slotObj, slotObj.transform.position, slotObj.transform.rotation, slotObj.transform);
            slotRenderer = slotObj.GetComponent<SpriteRenderer>();
            slotRenderer.color = Color.black;

            pieceObj.transform.localScale = Vector3.one;
            pieceObj.GetComponent<SpriteRenderer>().sortingOrder = slotRenderer.sortingOrder + 1;
            pieceRb = pieceObj.AddComponent<Rigidbody2D>();
            pieceRb.sharedMaterial = pieceMaterial;
            partScript = pieceObj.AddComponent<Part>();
            
            Parts.Add(partScript);
            KnockOffCandidates.Add(partScript);
        }

        for (int i = 0; i < Parts.Count; i++)
        {
            partScript = Parts[i];
            partScript.SetRepairManager(gameObject.GetComponent<RepairManager>());
            partScript.SetForce(knockOffForce);
            partScript.SetGravity(gravityFactor);
        }
        
        attackInterval = SequenceManager.Instance.CurrentBattle.RepairParameters.AttackInterval;
        timer.SetTimer(attackInterval);
        attackCoroutine = StartCoroutine(RepeatedAttack());
        knockOffPerAttack = SequenceManager.Instance.CurrentBattle.RepairParameters.KnockOffPerAttack;
        isInitialized = true;
    }

    void Update()
    {
        if (hasEnded) return;
        
        // Fixed all KnockedOff Pcs AND not beginning of game
        if (totalFixed == (Parts.Count - KnockOffCandidates.Count) && 
            totalFixed != 0)
        {
            EndsGame();
        }
    }
    
    IEnumerator RepeatedAttack()
    {
        yield return WaitForNextAlert();
        while (!hasEnded)
        {
            TriggerAttackAlert();
            yield return new WaitForSeconds(alertDuration);
            if (hasEnded) yield break;
            PerformAttack();
            if (KnockOffCandidates.Count == 0)
            {
                timer.ChangeColor();
                yield return new WaitForSeconds(attackInterval);
                EndsGame();
                yield break;
            }
            yield return WaitForNextAlert();
        }
    }

    private IEnumerator WaitForNextAlert()
    {
        yield return new WaitForSeconds(attackInterval - alertDuration);
    }

    private void TriggerAttackAlert()
    {
        PlayEnemyAttackSound();
        attackAlert
            .DOScale(new Vector3(25, 25, 0), alertDuration)
            .onComplete = ResetAlertScale;
    }

    private void PerformAttack()
    {
        KnocksOff(knockOffPerAttack);
        timer.ResetTimer();
        timer.StartTimer();
    }
    
    private void KnocksOff(int num)
    {
        if (KnockOffCandidates.Count == 0) return;
        if (num > KnockOffCandidates.Count)
        {
            num = KnockOffCandidates.Count;
        }
        int i;
        for (int remain = num; remain > 0; remain--)
        {
            i = Random.Range(0, KnockOffCandidates.Count-1);
            KnockOffCandidates[i].KnockOff();
            KnockOffCandidates.RemoveAt(i);
        }
    }

    private void ResetAlertScale()
    {
        attackAlert.localScale = new Vector3(0, 0, 0);
    }

    private void EndsGame()
    {
        hasEnded = true;
        bool playerWins = (totalFixed == (Parts.Count - KnockOffCandidates.Count));
        StopCoroutine(attackCoroutine);
        timer.StopTimer();
        for (int i = 0; i < Parts.Count; i++)
        {
            Part partScript = Parts[i];
            partScript.DisablePlayerControl();
        }
        
        
        var battle = BattleSequenceManager.Instance.enemyMinigames;
        
        GameObject.Find("Enemy").transform.GetChild(0).GetComponent<EnemyInfo>().ChangePose(playerWins);
        BarkManager.Instance.ShowGameBark("Repair", playerWins);
        if (!playerWins)
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.Overheated, 0.6f);
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Repair", "Lose");
            }
            GridManager.Instance.UpdateDotPosition(battle[2].LoseEffect, GridManager.MiniGame.Defend);
        }
        else
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.RepairSuccess);
            if (MetricManagerScript.instance != null)
            { 
                MetricManagerScript.instance.LogString("Repair", "Win");
            }
            GridManager.Instance.UpdateDotPosition(Vector2.zero, GridManager.MiniGame.Defend);
        }
    }
    
    public void NewPartFixed()
    {
        totalFixed++;
    }

    public void PlayEnemyAttackSound()
    {
        switch (SequenceManager.Instance.SequenceID)
        {
            case 3:
                AudioManager.Instance.PlayOneShot(SFXNAME.EnagaRepair, .2f);
                break;
            case 6:
                AudioManager.Instance.PlayOneShot(SFXNAME.QuincyRepair);
                break;
            case 9:
                AudioManager.Instance.PlayOneShot(SFXNAME.VyzzarRepair, .5f);
                break;
            case 12:
                AudioManager.Instance.PlayOneShot(SFXNAME.CecilRepair, .5f);
                break;
            case 14:
                AudioManager.Instance.PlayOneShot(SFXNAME.HarbingerRepair);
                break;
            case 17:
                AudioManager.Instance.PlayOneShot(SFXNAME.AceRepair);
                break;
        }
    }
}
