using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public Transform killState;
    public Transform captureState;
    public GameObject specialBlocker;

    [SerializeField] private RectTransform _grid;
    [SerializeField] private Vector3 _gridBasePos;
    
    public RectTransform dot;
    [SerializeField] private GameObject plusSign;
    [SerializeField] private GameObject minusSign;
    private GameObject loseSign;
    private GameObject winSign;

    [SerializeField] private EnergyBar _energyBar;


    private Sequence dotAnimationSequence;
    private Vector2 distance;

    public Transform dotAnimationContainer;
    //Middle point is (430, 270);
    //Min Y = -203, Max y = 203, Min X = -208, Max X = 208

    public readonly int MAX_X = 200;
    public readonly int MAX_Y = 200;

    private bool specialUsed = false;

    public enum MiniGame
    {
        Init,
        Disarm,
        Project,
        Vent,
        Fluid,
        Audio,
        Defend,
    }
    private Dictionary<int, MiniGame> dict = new Dictionary<int, MiniGame>()
    {
        {0, MiniGame.Disarm },
        {1, MiniGame.Project},
        {2, MiniGame.Vent},
        {3, MiniGame.Fluid },
        {4, MiniGame.Audio }
    };
    private MiniGame lastType = MiniGame.Init;
    private bool lastWin = false;
    private int miniGameWinStrike = 0;
    private int miniGameLoseStrike = 0;

    private void Awake()
    {
        Instance = this;
        winSign = dot.gameObject;
        loseSign = dot.gameObject;
    }
    private void Start()
    {
        var battle = SequenceManager.Instance.CurrentBattle;
        killState.transform.localPosition = battle.endStates[0].Pos;
        if (SequenceManager.Instance.SequenceID < 10)
            captureState.transform.localPosition = battle.endStates[1].Pos;
        else
            captureState.gameObject.SetActive(false);
    }
    public void StartDotAnimation(int index, bool isEnemy = false)
    {
        AudioManager.Instance.StopLoop(SFXNAME.GridDotMoving);
        AudioManager.Instance.PlayLoop(SFXNAME.GridDotMoving);
        AudioManager.Instance.SetSFXVolume(SFXNAME.GridDotMoving, 0.6f);
        var g = SequenceManager.Instance.CurrentBattle.Minigames;
        var eg = SequenceManager.Instance.CurrentBattle.EnemyMinigames;
        Vector2 winDistance = Vector2.zero;
        Vector2 loseDistance = Vector2.zero;


        if (!isEnemy)
        {
            winDistance = g[index].WinEffect;
            loseDistance = g[index].LoseEffect;
            var type = dict[index];
            if (type==lastType)
            {
                if (miniGameLoseStrike == 0)
                {
                    loseDistance /= 2;
                }
                if (miniGameLoseStrike >= 1)
                {
                    loseDistance /= 4;
                }
                if (miniGameWinStrike == 0)
                {
                    winDistance /= 2;
                }
                if (miniGameWinStrike >= 1)
                {
                    winDistance = Vector2.zero;
                }
            }
            
        }
        else
        {
            winDistance = Vector2.zero;
            loseDistance = eg[index].LoseEffect;

        }
        //winDistance *= 2;
        //loseDistance *= 2;

        var winTargetLocalPosition = dot.localPosition + new Vector3(winDistance.x, winDistance.y, 0);
        var loseTargetLocalPosition = dot.localPosition + new Vector3(loseDistance.x, loseDistance.y, 0);
        dotAnimationSequence.SetLoops(1);
        dotAnimationSequence.Kill(true);

        dotAnimationSequence = DOTween.Sequence();
        dotAnimationSequence.SetLoops(-1);
        dotAnimationSequence.AppendCallback(() =>
        {
            if (loseDistance.x != 0 || loseDistance.y != 0) 
            {
                var d2 = Instantiate(loseSign.gameObject, dotAnimationContainer);
                d2.GetComponent<Image>().color = Color.red;
                d2.transform.localPosition = dot.localPosition;
                d2.GetComponent<Image>().DOFade(0, 1.9f).SetEase(Ease.Linear);
                d2.transform.DOLocalMove(loseTargetLocalPosition, 2f).SetEase(Ease.Linear).onComplete += () =>
                {
                    Destroy(d2.gameObject);
                };
            }
            
            if (winDistance.x != 0 || winDistance.y != 0)
            {
                var d = Instantiate(winSign.gameObject, dotAnimationContainer);
                d.GetComponent<Image>().color = Color.green;
                d.transform.localPosition = dot.localPosition;
                d.GetComponent<Image>().DOFade(0, 1.9f).SetEase(Ease.Linear);
                d.transform.DOLocalMove(winTargetLocalPosition, 2f).SetEase(Ease.Linear).onComplete += () =>
                {
                    Destroy(d.gameObject);
                };
            }
        });
        dotAnimationSequence.AppendInterval(0.8f);
    }
    public void StopDotAnimation()
    {
        AudioManager.Instance.StopLoop(SFXNAME.GridDotMoving);
        dotAnimationSequence.SetLoops(1);
        dotAnimationSequence.Kill(true);
    }
    public void UpdateDotPosition(Vector2 distance, MiniGame type = MiniGame.Init, bool win = false) //Open grid in the UI
    {
        if (type != MiniGame.Init) 
        {
            if (type == MiniGame.Defend)
            {

            }
            else
            {
                if (win)
                {
                    miniGameLoseStrike = 0;
                    miniGameWinStrike++;
                    if (type == lastType && lastWin)
                    {
                        if (miniGameWinStrike == 1)
                            distance /= 2;
                        if (miniGameWinStrike >= 2)
                            distance = Vector2.zero;
                    }
                    else
                    {
                        miniGameWinStrike = 0;
                    }
                }
                else
                {
                    miniGameWinStrike = 0;
                    miniGameLoseStrike++;
                    if (type == lastType && !lastWin)
                    {
                        if (miniGameLoseStrike == 1) 
                            distance /= 2;
                        if (miniGameLoseStrike >= 2)
                            distance /= 4;
                    }
                    else
                    {
                        miniGameLoseStrike = 0;
                    }
                }
                lastType = type;
                lastWin = win;
            }
        }
        this.distance = distance;
        GridOpening().onComplete = () => MoveDot(type == MiniGame.Defend);
    }
    private Vector3 startPosition;
    private Vector3 endPosition;
    public void MoveDot(bool defend) //Get the destination of the dot
    {
        float newX = dot.localPosition.x + distance.x;
        float newY = dot.localPosition.y + distance.y;

        if (newX > MAX_X)
            newX = MAX_X;
        else if (newX < -MAX_X)
            newX = -MAX_X; 

        if (newY > MAX_Y)
            newY = MAX_Y;
        else if (newY < -MAX_Y)
            newY = -MAX_Y;
        Vector3 startPosition = dot.transform.localPosition;
        Vector3 endPosition = new Vector3(newX, newY, 0);
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        dot.DOLocalMove(endPosition, 3).onComplete += () =>
        {
            _energyBar.transform.parent.DOLocalMove(new Vector3(-146.1f, 234.8f, 1), 1, false);
            if ((CheckIfPass() || CheckIfCaptured() || CheckIfKilled()) && SequenceManager.Instance.IsSpecialSequence && !specialUsed)  
            {
                EnemyInfo.Instance.ResetPose();
                
                StartCoroutine(BarkManager.Instance.ShowIntroOutroBark(BarkManager.MultipleBarkType.Special,false, () =>
                {
                    
                    StateManager.Instance.UpdateState();
                    CoverManager.Instance.SpecialSwitch(false, defend && StateManager.Instance.enemyChoice != 2, StateManager.Instance.currentState == StateManager.BattleState.Enemy && StateManager.Instance.enemyChoice == 2);

                    killState.DOLocalMove(new Vector3(0, 150, 0), 1.5f);//Set the correct position!!!
                    var battle = BattleSequenceManager.Instance.endStates;
                    battle[0].Pos.x = 0;// It is always an issue to sync the UI and data.
                    battle[0].Pos.y = 150;// It is bad to code like this.
                    specialBlocker.SetActive(false);
                    specialUsed = true;
                    
                }));

                CoverManager.Instance.coverAnimator.SetTrigger("WireClose");
                CoverManager.Instance.coverAnimator.SetTrigger("PanelClose");
                Sequence s = DOTween.Sequence();
                s.AppendInterval(1.5f);
                s.AppendCallback(MasterMinigames.Instance.SetGame);
                
            }
            else
            {
                CheckIfWin();
                EnemyInfo.Instance.ResetPose();
                CoverManager.Instance.Switch(false, defend && StateManager.Instance.enemyChoice != 2, StateManager.Instance.currentState == StateManager.BattleState.Enemy && StateManager.Instance.enemyChoice == 2);
                StateManager.Instance.UpdateState();
            }
            
        };
    }
    private bool CheckIfPass()
    {
        var battle = BattleSequenceManager.Instance.endStates;
        Vector3 target = new Vector3(battle[0].Pos.x, battle[0].Pos.y, 0);
        return DoesLineSegmentIntersectSquare(startPosition, endPosition, target);
    }
    private bool DoesLineSegmentIntersectSquare(Vector3 startPosition, Vector3 endPosition, Vector3 target)
    {
        float halfSize = 40f; // Half of the square's side length (square side length is 80)
        float minX = target.x - halfSize, maxX = target.x + halfSize;
        float minY = target.y - halfSize, maxY = target.y + halfSize;

        // 1. Quick rejection: If both points are outside the square on the same side, return false
        if ((startPosition.x < minX && endPosition.x < minX) || (startPosition.x > maxX && endPosition.x > maxX) ||
            (startPosition.y < minY && endPosition.y < minY) || (startPosition.y > maxY && endPosition.y > maxY))
        {
            return false; // The segment is entirely outside and cannot intersect
        }

        // 2. If either endpoint is inside the square, the segment intersects
        if ((startPosition.x >= minX && startPosition.x <= maxX && startPosition.y >= minY && startPosition.y <= maxY) ||
            (endPosition.x >= minX && endPosition.x <= maxX && endPosition.y >= minY && endPosition.y <= maxY))
        {
            return true;
        }

        // 3. Check if the line segment intersects any of the four edges of the square
        return LineIntersectsLine(startPosition, endPosition, new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0)) || // Bottom edge
               LineIntersectsLine(startPosition, endPosition, new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0)) || // Top edge
               LineIntersectsLine(startPosition, endPosition, new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0)) || // Left edge
               LineIntersectsLine(startPosition, endPosition, new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));   // Right edge
    }

    // Helper function to check if two line segments intersect
    private bool LineIntersectsLine(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
    {
        float s1_x = p2.x - p1.x, s1_y = p2.y - p1.y;
        float s2_x = q2.x - q1.x, s2_y = q2.y - q1.y;

        float denom = (-s2_x * s1_y + s1_x * s2_y);
        if (denom == 0) return false; // The lines are parallel or collinear

        float s = (-s1_y * (p1.x - q1.x) + s1_x * (p1.y - q1.y)) / denom;
        float t = (s2_x * (p1.y - q1.y) - s2_y * (p1.x - q1.x)) / denom;

        // Intersection occurs when s and t are both in the range [0, 1]
        return (s >= 0 && s <= 1 && t >= 0 && t <= 1);
    }

    private void CheckIfWin()//Check if the dot is in either win state
    {
        var battle = BattleSequenceManager.Instance.endStates;
        if ((int)dot.transform.localPosition.x >= (battle[0].Pos.x - 40) &&
            (int)dot.transform.localPosition.x <= (battle[0].Pos.x + 40))
        {
            if ((int)dot.transform.localPosition.y >= (battle[0].Pos.y - 40) &&
                (int)dot.transform.localPosition.y <= (battle[0].Pos.y + 40))
            {
                StateManager.Instance.WinBattle(true);
                Debug.Log("Battle win, kill");
                return;
            }
        }

        if ((int)dot.transform.localPosition.x >= (battle[1].Pos.x - 40) &&
            (int)dot.transform.localPosition.x <= (battle[1].Pos.x + 40))
        {
            if((int)dot.transform.localPosition.y >= (battle[1].Pos.y - 40) &&
               (int)dot.transform.localPosition.y <= (battle[1].Pos.y + 40))
            {
                StateManager.Instance.WinBattle(false);
                Debug.Log("Battle win, Capture");
                return; 
            }
        }
        _energyBar.CheckIfLost();
    }
    private bool CheckIfCaptured()
    {
        var battle = BattleSequenceManager.Instance.endStates;
        if ((int)dot.transform.localPosition.x >= (battle[1].Pos.x - 40) &&
            (int)dot.transform.localPosition.x <= (battle[1].Pos.x + 40))
        {
            if ((int)dot.transform.localPosition.y >= (battle[1].Pos.y - 40) &&
               (int)dot.transform.localPosition.y <= (battle[1].Pos.y + 40))
            {
                Debug.Log("Battle win, Capture");
                return true;
            }
        }
        return false;
    }
    private bool CheckIfKilled()
    {
        var battle = BattleSequenceManager.Instance.endStates;
        if ((int)dot.transform.localPosition.x >= (battle[0].Pos.x - 40) &&
            (int)dot.transform.localPosition.x <= (battle[0].Pos.x + 40))
        {
            if ((int)dot.transform.localPosition.y >= (battle[0].Pos.y - 40) &&
                (int)dot.transform.localPosition.y <= (battle[0].Pos.y + 40))
            {
                Debug.Log("Battle win, kill");
                return true;
            }
        }
        return false;
    }
    public Tween GridOpening()
    {
        _grid.DOLocalMove(_gridBasePos, 1f);
        return _grid.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 1f);
    }
    public void GridClosing()
    {
        _grid.DOLocalMoveX(0, 0.5f);
        _grid.DOScale(new Vector3(0, 0, 0), 0.5f);
    }
    public void GridHiding()
    {
        _grid.gameObject.SetActive(false);
       
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            winSign = dot.gameObject;
            loseSign = dot.gameObject;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            winSign = plusSign;
            loseSign = minusSign;
        }
    }
}
