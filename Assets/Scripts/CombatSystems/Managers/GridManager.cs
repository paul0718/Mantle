using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private RectTransform _grid;
    [SerializeField] private Vector3 _gridBasePos;
    
    [SerializeField] private RectTransform _dot;

    [SerializeField] private StateManager _stateManager;
    [SerializeField] private UITransitionManager _uiManager;

    private float x, y;
    
    //Middle point is (430, 270);
    //Min Y = -203, Max y = 203, Min X = -208, Max X = 208

    public void UpdateDotPosition(float a, float b) //Open grid in the UI
    {
        x = a;
        y = b;
        _grid.DOLocalMove(_gridBasePos, 1f);
        _grid.DOScale(new Vector3(1, 1, 1), 1f).onComplete = MoveDot;
    }
    
    public void MoveDot() //Get the destination of the dot
    {
        float newX = _dot.localPosition.x + x;
        float newY = _dot.localPosition.y + y;

        if (newX > 208)
        {
            newX = 208;
        }
        else if (newX < -208)
        {
            newX = -208;
        }

        if (newY > 203)
        {
            newY = 203;
        }
        else if (newY < -203)
        {
            newY = -203;
        }
        
        
        Vector3 end = new Vector3(newX, newY, 0);
        StartCoroutine(MoveOverSeconds(end, 3, 0));
    }
    
    public IEnumerator MoveOverSeconds (Vector3 end, float seconds, int m)//Move the dot
    {
        float elapsedTime = 0;
        Vector3 startingPos = _dot.transform.localPosition;
        while (elapsedTime < seconds)
        {
            _dot.transform.localPosition = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        CheckIfWin();
        _dot.transform.localPosition = end;
        _uiManager.Transition();
        _stateManager.UpdateState();
    }

    void CheckIfWin()//Check if the dot is in either win state
    {
        if (_dot.transform.localPosition.y >= 200)
        {
            if (_dot.transform.localPosition.x >= -200 && _dot.transform.localPosition.x < -105)
            {
                _stateManager.EndBattle(true);
                Debug.Log("win, kill");
            }
        }
        
        if (_dot.transform.localPosition.x >= 200)
        {
            if (_dot.transform.localPosition.y > -200 && _dot.transform.localPosition.y < -110)
            {
                _stateManager.EndBattle(false);
                Debug.Log("win, capture");
            }
        }
    }
    
}
