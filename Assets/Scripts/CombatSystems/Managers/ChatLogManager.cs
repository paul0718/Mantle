using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatLogManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _chatText;

    [SerializeField] private float _typingSpeed;

    [SerializeField] private ScrollRect _scroll;

    [SerializeField] private Slider _energySlider;

    [SerializeField] private StateManager _stateManager;

    //check what the current turn state is
    public void UpdateState()
    {
        if (_stateManager.currState == StateManager.BattleState.Player)
        {
            StartCoroutine(TypeText("Energy at " + _energySlider.value + "%." + "\n"));
        }
        else if(_stateManager.currState == StateManager.BattleState.Enemy)
        {
            string temp = "";
            if (_stateManager.enemyChoice == 0)//display if enemy is attacking minigame
            {
                temp = "<color=red>Enemy is attacking...</color>" + "\n";
            }
            else if (_stateManager.enemyChoice == 1)
            {
                temp = "<color=red>Enemy is charging up...</color>" + "\n";
            }
            StartCoroutine(TypeText(temp));//display if enemy is charging up minigame
        }
        else if(_stateManager.currState == StateManager.BattleState.End)
        {
            StartCoroutine(TypeText("Enemy is weakened. Time to win." + "\n"));
        }
    }
    
    public void ChoosingAction(string action)
    {
        StartCoroutine(TypeText("Activating " + action + "\n"));
    }

    void AddDivider()//chat log divider
    {
        _chatText.text = _chatText.text + "----------------------------------------------\n";
    }

    private string CalcEnergy()//calculate current player energy
    {
        return ((1 - _energySlider.value) * 100).ToString();
    }

    IEnumerator TypeText(string fullText)//type out text in the chat log
    {
        foreach (char letter in fullText)
        {
            _chatText.text += letter; // Append each letter to the text
            yield return new WaitForSeconds(_typingSpeed); // Wait for the specified duration
        }
        
        AddDivider();
        _scroll.velocity = new Vector2 (0f, 1000f);
    }
}
