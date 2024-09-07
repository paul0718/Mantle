using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespondEnemyButtons : MonoBehaviour
{
   [SerializeField] private GameObject[] _blockGames;
   [SerializeField] private GameObject[] _interruptGames;

   [SerializeField] private GameObject _actionBlocker;

   [SerializeField] private UITransitionManager _uiManager;
   
   public void BlockButton()//Call UI transition and display enemy block minigame
   {
      _actionBlocker.SetActive(true);
      _uiManager._currGame = _blockGames[0];
      _uiManager.Transition();
      _uiManager.CloseGrid();
      _uiManager.CloseRightPanels(true);
   }

   public void InterruptButton()//Call UI transition and display enemy charging up minigame
   {
      _actionBlocker.SetActive(true);
      _uiManager._currGame = _interruptGames[0];
      _uiManager.Transition();
      _uiManager.CloseGrid();
      _uiManager.CloseRightPanels(false);
   }
}
