using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UITransitionManager : MonoBehaviour
{
    [SerializeField] private StateManager _stateManager;
    
    [SerializeField] private RectTransform _grid;
    
    [SerializeField] private RectTransform _leftPanel;
    [SerializeField] private RectTransform _rightPanel;
    [SerializeField] private GameObject _backPanel;

    [SerializeField] private RectTransform _weaponPanelUp;
    [SerializeField] private RectTransform _weaponPanelDown;

    [HideInInspector] public GameObject _currGame;
    
    //---------------------------------------------------------------------
    public void Update()
    {
        
    }

    public void Transition()//Main transition panels when going from default state <-> minigames
    {
        Sequence s = DOTween.Sequence();
        s.Append(_leftPanel.DOLocalMove(new Vector3(-483, _leftPanel.localPosition.y,
            _leftPanel.localPosition.z), 0.5f).SetEase(Ease.OutSine));
        s.Join(_rightPanel.DOLocalMove(new Vector3(483, _rightPanel.localPosition.y,
            _rightPanel.localPosition.z), 0.5f).SetEase(Ease.OutSine));
        s.AppendInterval(0.5f);
        s.SetLoops(2, LoopType.Yoyo);
        if (!_currGame.activeSelf)
        {
            s.AppendCallback(ActivateGame);
        }
        else 
        {
            s.AppendCallback(DeactivateGame);
        }
    }

    void ActivateGame()//activate currently chosen minigame
    {
        _backPanel.GetComponent<SpriteRenderer>().sortingOrder = 1;
        _currGame.SetActive(true);
        CloseGrid();
    }

    public void DeactivateGame()//deactivate currently active minigame
    {
        _backPanel.GetComponent<SpriteRenderer>().sortingOrder = -1;
        _currGame.SetActive(false);
        _currGame = null;
    }

    public void CloseGrid()//close the grid in the UI
    {
        _grid.DOLocalMove(new Vector3(0, _grid.localPosition.y, _grid.localPosition.z), 0.5f);
        _grid.DOScale(new Vector3(0, 0, 0), 0.5f);
    }

    public void OpenRightPanels(bool top)//Open the panels on the right side of the UI for enemy games/end weapons
    {
        Sequence p = DOTween.Sequence();
        p.AppendInterval(1.5f);
        if (top)
        {
            p.Append(_weaponPanelUp.DOLocalMove(new Vector3(412, -248, 0), 1.5f).SetEase(Ease.InOutQuart));
        }
        else
        {
            p.Append(_weaponPanelDown.DOLocalMove(new Vector3(412, -93, 0), 1.5f).SetEase(Ease.InOutQuart));
        }
    }
    
    public void CloseRightPanels(bool top)//Close the panels on the right after playing enemy games
    {
        if (top)
        {
            _weaponPanelUp.DOLocalMove(new Vector3(412, -94, 0), 1f).SetEase(Ease.InOutQuart);
        }
        else
        {
            _weaponPanelDown.DOLocalMove(new Vector3(412, -248, 0), 1f).SetEase(Ease.InOutQuart);
        }
    }
}
