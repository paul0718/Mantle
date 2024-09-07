using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerGame : MonoBehaviour
{
    [SerializeField] private GridSystem gridScript;
    
    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject ball;

    [SerializeField] private float time;
    
    public int currGame = 1;
    // Start is called before the first frame update
    void OnEnable()
    {
        float tempX = Random.Range(-7, 7);
        goal.transform.localPosition = new Vector3(tempX, 0, 0);
        ball.transform.localPosition = new Vector3(-6.5f, 0, 0);
        DOTween.Play(ball.transform);
        ball.transform.DOLocalMove(new Vector3(6.5f, 0, 0), time).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBall()
    {
        DOTween.Kill(ball.transform);
        ball.GetComponent<CheckPlayerResults>().CheckResult();
    }

    public void UpdateDot(bool win)
    {
        Debug.Log(currGame);
        if (win)
        {
            if (currGame == 1)
            {
                Debug.Log("calling disarm");
                gridScript.Disarm();
            }
            else if (currGame == 2)
            {
                gridScript.ProjectImage();
            }
            else if (currGame == 3)
            {
                gridScript.MetalBend();
            }
            else if (currGame == 4)
            {
                gridScript.NoMove();
            }
        }
        else
        {
            if (currGame == 4)
            {
                gridScript.EnemyGame();
            }
            else
            {
                gridScript.NoMove();
            }
        }
    }
}
