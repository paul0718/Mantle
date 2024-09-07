using System.Collections;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public enum TurnState
    {
        Player,
        Enemy
    }
    
    //grid items
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject dot;
    
    //player game buttons
    [SerializeField] private GameObject[] gameButtons;
    
    //enemy buttons
    [SerializeField] private GameObject enemyText;
    [SerializeField] private GameObject enemyButton;

    //game items
    [SerializeField] private GameObject game;
    [SerializeField] private GameObject gameCanvas;

    //enemy art
    [SerializeField] private GameObject enemy;

    private TurnState currState = TurnState.Player;

    private float a = 1f;
    private float b = 1f;
    
    public IEnumerator MoveOverSeconds (Vector3 end, float seconds, int m)
    {
        yield return new WaitForSeconds(1.0f);
        
        float elapsedTime = 0;
        Vector3 startingPos = dot.transform.position;
        while (elapsedTime < seconds)
        {
            dot.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        dot.transform.position = end;
        
        UpdateTurnState();
    }

    public void ActivateGrid()
    {
        dot.SetActive(true);
        grid.SetActive(true);
        enemy.SetActive(true);
    }

    public void DeactivateGameEnableButtons()
    {
        game.SetActive(false);
        gameCanvas.SetActive(false);
        if (currState == TurnState.Player)
        {
            foreach(GameObject i in gameButtons)
            {
                i.SetActive(true);
            } 
        }
        else if (currState == TurnState.Enemy)
        {
            enemyText.SetActive(true);
            enemyButton.SetActive(true);
        }
    }

    public void Disarm()
    {
        ActivateGrid();
        Vector3 end = new Vector3(dot.transform.position.x - a, dot.transform.position.y + b, 0);
        StartCoroutine(MoveOverSeconds(end, 3, 0));
    }
    
    public void ProjectImage()
    {
        ActivateGrid();
        Vector3 end = new Vector3(dot.transform.position.x + a, dot.transform.position.y - b, 0);
        StartCoroutine(MoveOverSeconds(end, 3, 1));
    }
    
    public void MetalBend()
    {
        ActivateGrid();
        Vector3 end = new Vector3(dot.transform.position.x, dot.transform.position.y - (2 * b), 0);
        StartCoroutine(MoveOverSeconds(end, 3, 4));
    }

    public void EnemyBlockGame()
    {
        Vector3 end = new Vector3(dot.transform.position.x + a, dot.transform.position.y + (2 * b), 0);
        StartCoroutine(MoveOverSeconds(end, 3, 4));
    }

    public void EnemyChargeUpGame()
    {
        Vector3 end = new Vector3(dot.transform.position.x + (2 * a), dot.transform.position.y + b, 0);
        StartCoroutine(MoveOverSeconds(end, 3, 4));
    }

    public void EnemyGame()
    {
        ActivateGrid();
        Vector3 end = new Vector3(dot.transform.position.x + a, dot.transform.position.y + b, 0);
        StartCoroutine(MoveOverSeconds(end, 3, 4));
    }

    public void NoMove()
    {
        ActivateGrid();
        Vector3 end = new Vector3(dot.transform.position.x, dot.transform.position.y, 0);
        StartCoroutine(MoveOverSeconds(end, 1, 4));
    }
  //-----------------------------------------------------------------------------------------------------------------  
    public void FlashLight(bool win)
    {
        if (win)
        {
            Vector3 end = new Vector3(dot.transform.position.x - (2* a), dot.transform.position.y + b, 0);
            StartCoroutine(MoveOverSeconds(end, 3, 2));
        }
        else
        {
            Vector3 end = new Vector3(dot.transform.position.x + a, dot.transform.position.y - b, 0);
            StartCoroutine(MoveOverSeconds(end, 3, 2));
        }
    }
    
    public void ShootLaser(bool win)
    {
        if (win)
        {
            Vector3 end = new Vector3(dot.transform.position.x + a, dot.transform.position.y + b, 0);
            StartCoroutine(MoveOverSeconds(end, 3, 3));
        }
        else
        {
            Vector3 end = new Vector3(dot.transform.position.x - a, dot.transform.position.y - b, 0);
            StartCoroutine(MoveOverSeconds(end, 3, 3));
        }
    }
    
    public void EyeAttack(bool win)
    {
        if (win)
        {
            Vector3 end = new Vector3(dot.transform.position.x - a, dot.transform.position.y + (2*b), 0);
            StartCoroutine(MoveOverSeconds(end, 3, 5));
        }
        else
        {
            Vector3 end = new Vector3(dot.transform.position.x + (2 * a), dot.transform.position.y, 0);
            StartCoroutine(MoveOverSeconds(end, 3, 5));
        }
    }
    
    public void ElectricPulse(bool win)
    {
        if (win)
        {
            Vector3 end = new Vector3(dot.transform.position.x - (2*a), dot.transform.position.y, 0);
            StartCoroutine(MoveOverSeconds(end, 3, 6));
        }
        else
        {
            Vector3 end = new Vector3(dot.transform.position.x, dot.transform.position.y - b, 0);
            StartCoroutine(MoveOverSeconds(end, 3, 6));
        }
    }
    //--------------------------------------------------------------------------------
    public void TurnOffGrid()
    {
        dot.SetActive(false);
        grid.SetActive(false);
        enemy.SetActive(false);
        
        game.SetActive(true);
        gameCanvas.SetActive(true);
        
        foreach (GameObject b in gameButtons)
        {
            b.SetActive(false);
        }
        
        enemyText.SetActive(false);
        enemyButton.SetActive(false);
    }

    public void DisarmButton()
    {
        TurnOffGrid();
        game.GetComponent<PlayerGame>().currGame = 1;
    }
    
    public void ProjectButton()
    {
        TurnOffGrid();
        game.GetComponent<PlayerGame>().currGame = 2;
    }

    public void MetalButton()
    {
        TurnOffGrid();
        game.GetComponent<PlayerGame>().currGame = 3;
    }

    public void EnemyButton()
    {
        TurnOffGrid();
        game.GetComponent<PlayerGame>().currGame = 4;
    }

    public void UpdateTurnState()
    {
        if (currState == TurnState.Player)
        {
            currState = TurnState.Enemy;
        }
        else
        {
            currState = TurnState.Player;
        }
        DeactivateGameEnableButtons();
    }
}
