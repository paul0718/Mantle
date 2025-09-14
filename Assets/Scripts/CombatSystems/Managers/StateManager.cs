using DG.Tweening;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class StateManager : MonoBehaviour
{
    [SerializeField] private GameObject endReticle;
    [SerializeField] private GameObject lowBatteryText;

    [SerializeField] private GameObject destructButton;
    //intro dialogue
    //[SerializeField] private GoogleSheetsDB googleSheetsDB;
    [HideInInspector] public bool playingDialogue;

    [SerializeField] private EndFightCutscene endSequenceManager;

    public bool debugGame = false;
    [SerializeField] private int gameChoice;
    


    //disarm vars
    [HideInInspector] public int enemyChoice;
    
    //temp fix for volume set to 0 for battle
    //fixed! ;)

    //bools for when ending battle to know if killing or capturing
    private bool killWeapon = true;
    public static StateManager Instance { get; private set; }
    public enum BattleState
    {
        BeforeStart,
        Start,
        Player,
        Enemy,
        Win,
        Lose,
        AfterEnd,
    };

    public BattleState currentState = BattleState.BeforeStart;

    [SerializeField] private bool playIntro = true;


    public Animator coverAnimator;
    private float bgmTime;
    private void Start()
    {
        Instance = this;
        bgmTime = SequenceManager.Instance.SequenceID == 14 ? 3 : 1;
        GridManager.Instance.dot.gameObject.transform.localPosition = BattleSequenceManager.Instance.startPos;
        
        if (playIntro && !debugGame)
        {
            playingDialogue = true;
            StartCoroutine(BarkManager.Instance.ShowIntroOutroBark(BarkManager.MultipleBarkType.Intro));
        }
        else
        {
            coverAnimator.enabled = true;
            AudioManager.Instance.PlayNextBGM();
            Sequence s = DOTween.Sequence();
            s.AppendInterval(bgmTime);
            s.AppendCallback(() =>
            {
                AudioManager.Instance.PlayNextBGM();
            });
            s.SetUpdate(true);
            if (SequenceManager.Instance.SequenceID == 17)
                AceMusicManager.Instance.OnFightStart();
        }
    }

    public void DoneIntroDialogue()
    {
        //uiElements.GetComponent<CanvasGroup>().alpha = 1;
        playingDialogue = false;
        coverAnimator.enabled = true;
        AudioManager.Instance.PlayNextBGM();
        currentState = BattleState.Start;
        Sequence s = DOTween.Sequence();
        s.AppendInterval(bgmTime);//important
        s.AppendCallback(() =>
        {
            AudioManager.Instance.PlayNextBGM();
        });
        s.SetUpdate(true);
        if (SequenceManager.Instance.SequenceID == 17)
            AceMusicManager.Instance.OnFightStart();
    }
    private bool[] gamePlayed = new bool[4]; // Array to track if each game has been played

    private void ChooseGame()
    {
        // If all games have been played, reset the gamePlayed array
        if (gamePlayed.All(played => played))
        {
            for (int i = 0; i < 4; i++)
            {
                gamePlayed[i] = false; // Reset all games to not played
            }
        }

        do
        {
            enemyChoice = Random.Range(0, 4); // Randomly select a game
        }
        while (gamePlayed[enemyChoice]); // Repeat until an unplayed game is selected

        gamePlayed[enemyChoice] = true; // Mark the selected game as played
    }

    public void LoseDialogue()
    {
        playingDialogue = true;
        //EnemyInfo.Instance.ChangePose(false);
        StartCoroutine(BarkManager.Instance.ShowIntroOutroBark(BarkManager.MultipleBarkType.Lose));
    }

    //change turn state
    public void UpdateState()
    {
        if (currentState == BattleState.Start)
        {
            currentState = BattleState.Enemy;
        }
        else if (currentState == BattleState.Enemy )
        {
            //actionBlocker.SetActive(false);
            currentState = BattleState.Player;
            CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.LockIn);
            if (GridManager.Instance.selfDestruct)
            {
                Sequence s = DOTween.Sequence();
                s.AppendInterval(2.5f);
                s.Append(destructButton.transform.DOLocalMoveY(-88, 1f, true));
            }
        }
        else if(currentState == BattleState.Player)
        {
            currentState = BattleState.Enemy;
        }

        if (currentState == BattleState.Enemy) //randomly choose an enemy minigame to play
        {
            if (!debugGame)
                ChooseGame();
            else
                enemyChoice = gameChoice;
            //enemyChoice = 1;
            CoverManager.Instance.SetDefendButton(true);
            CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.Respond);

            var choice = StateManager.Instance.enemyChoice;
            var battle = SequenceManager.Instance.CurrentBattle.EnemyMinigames;
            GridManager.Instance.StartDotAnimation(choice, true);
        }
    }

    private void Update()
    {
        //display the targeting reticle at mouse position when ending the game
        if (currentState == BattleState.Win) 
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition;
            RectTransform endRect = endReticle.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                endRect.parent as RectTransform, 
                mousePosition,                     
                endRect.parent.GetComponent<Canvas>().worldCamera,     
                out worldPosition 
            );
            endRect.position = worldPosition;
        }

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            WinBattle(false);
        }
    }

    public void CheckEndBattle()
    {
        if (Input.GetMouseButton(0) && currentState == BattleState.Win) 
        {
            endReticle.gameObject.SetActive(false);
            Cursor.visible = true;
            if (killWeapon)
            {
                CallAchievement(true);
                AudioManager.Instance.PlayOneShot(SFXNAME.swordstabpushmeleeweapon236206);
                EnemyInfo.Instance.ChangePose(true);
                if (SequenceManager.Instance.SequenceID != 9)
                {
                    if (SequenceManager.Instance.SequenceID == 17)
                    {
                        SequenceManager.Instance.aceIsDead = true;
                        AceMusicManager.Instance.OnPostBattleDialogueStart();
                    }
                    CoverManager.Instance.EndGameAnimaton();
                    StartCoroutine(BarkManager.Instance.ShowIntroOutroBark(BarkManager.MultipleBarkType.Win, true));
                }
                else
                {
                    CoverManager.Instance.EndGameAnimaton();
                    endSequenceManager.StartCoroutine(endSequenceManager.PlayEndSequence());
                }
                //endSequenceManager.StartCoroutine(endSequenceManager.PlayEndSequence());
            }
            else 
            {
                CallAchievement(false);
                AudioManager.Instance.PlayOneShot(SFXNAME.TaserGunSFX);
                EnemyInfo.Instance.ChangePose(true);
                if (SequenceManager.Instance.SequenceID != 9)
                {
                    if (SequenceManager.Instance.SequenceID == 17)
                    {
                        SequenceManager.Instance.aceIsDead = false;
                        AceMusicManager.Instance.OnPostBattleDialogueStart();
                    }
                    CoverManager.Instance.EndGameAnimaton();
                    StartCoroutine(BarkManager.Instance.ShowIntroOutroBark(BarkManager.MultipleBarkType.Win, false));
                }
                else
                {
                    CoverManager.Instance.EndGameAnimaton();
                    endSequenceManager.StartCoroutine(endSequenceManager.PlayEndSequence());
                }
                //endSequenceManager.StartCoroutine(endSequenceManager.PlayEndSequence());
            }
            currentState = BattleState.AfterEnd;
        }
    }
    public IEnumerator PlayerLose()
    {
        MasterMinigames.Instance.DisableAllButtons();
        currentState = BattleState.Lose;
        yield return new WaitForSeconds(1);
        //lowBatteryText.SetActive(true);
        yield return new WaitUntil(() => !playingDialogue);
        if (SequenceManager.Instance.SequenceID == 17)
        {
            AceMusicManager.Instance.OnPostBattleDialogueStart();
            SequenceManager.Instance.aceIsDead = false;
            SteamIntegration.Instance.UnlockAchievement("ACH_LOSE_ACE");
            SceneTransition.Instance.FadeToBlack();
        }
        else
        {
            SceneTransition.Instance.SwitchScene("LoseBattle");
        }
    }

    //Battle is in end state. Open the panels on the right and show either the gun or taser
    public void WinBattle(bool kill)
    {
        currentState = BattleState.Win;

        CoverManager.Instance.SetDefendButton(false);
        CoverManager.Instance.SetEndButton(kill);
    }

    public void LoseBattle()
    {
        LoseDialogue();
        StartCoroutine(PlayerLose());
    }

    public void WeaponBattle(bool kill)
    {
        EnemyInfo.Instance.SetCollider(true);
        Cursor.visible = false;
        endReticle.SetActive(true);
        killWeapon = kill;
    }

    public void CallAchievement(bool kill)
    {
        if (kill)
        {
            switch (SequenceManager.Instance.SequenceID)
            {
                case 3:
                    SteamIntegration.Instance.UnlockAchievement("ACH_KILL_ENAGA");
                    break;
                case 6:
                    SteamIntegration.Instance.UnlockAchievement("ACH_KILL_QUINCY");
                    break;
                case 12:
                    SteamIntegration.Instance.UnlockAchievement("ACH_KILL_CECIL");
                    break;
                case 17:
                    SteamIntegration.Instance.UnlockAchievement("ACH_KILL_ACE");
                    break;
            }
        }
        else
        {
            switch (SequenceManager.Instance.SequenceID)
            {
                case 3:
                    SteamIntegration.Instance.UnlockAchievement("ACH_CAPTURE_ENAGA");
                    break;
                case 6:
                    SteamIntegration.Instance.UnlockAchievement("ACH_CAPTURE_QUINCY");
                    break;
            }
        }
    }
}
