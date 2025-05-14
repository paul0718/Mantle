using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class ChatLogManager : MonoBehaviour
{
    public static ChatLogManager Instance {  get; private set; }
    [SerializeField] private GameObject chatText;
    [SerializeField] private GameObject newTxt;
    [SerializeField] private GameObject textPrefab;

    private bool alreadyDestroyed;

    //[SerializeField] private float _typingSpeed;
    [SerializeField] private float scrollTime;

    [SerializeField] private Slider energySlider;

    private void Start()
    {
        Instance = this;
    }

    public void ShowText(string content)
    {
        /*if (newTxt != null)
        {
            alreadyDestroyed = true;
            Destroy(chatText);
            chatText = newTxt;
            //speed up currently scrolling txt maybe?
        }
        else
        {
            alreadyDestroyed = false;
        }*/


        Destroy(chatText);
        /*chatText.transform.DOLocalMove(new Vector2(-220, 0), scrollTime).SetEase(Ease.Linear).onComplete += () =>
        {
            if (!alreadyDestroyed)
                Destroy(chatText);
        };*/
        chatText = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, transform);
        chatText.GetComponent<TextMeshProUGUI>().text = content;
        chatText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        chatText.GetComponent<RectTransform>().anchoredPosition = new Vector2(236, 0);
        chatText.transform.DOLocalMove(new Vector2(0, 0), scrollTime+0.15f).SetEase(Ease.Linear).onComplete += () =>
        {
            /*if (!alreadyDestroyed)
            {
                chatText = newTxt;
                newTxt = null;
            }*/
        };
    }

    //check what the current turn state is
    public void UpdateState()
    {
        if (StateManager.Instance.currentState == StateManager.BattleState.Player)
        {
            float tempEnergy = energySlider.value + 20;
            string temp = "Mantle energy level: " + energySlider.GetComponent<EnergyBar>().CheckEnergyLevel();
            ShowText(temp);
        }
        else if(StateManager.Instance.currentState == StateManager.BattleState.Enemy)
        {
            string temp = "";
            if (StateManager.Instance.enemyChoice == 0)//display if enemy is attacking minigame
            {
                temp = "<color=red>Preparing Defense...</color>";
            }
            else if (StateManager.Instance.enemyChoice == 1)//charge up
            {
                temp = "<color=red>Enemy is Charging Up...</color>";
            }
            else if (StateManager.Instance.enemyChoice == 2)//repair
            {
                temp = "<color=red>Enemy is Attacking...</color>";
            }
            else if (StateManager.Instance.enemyChoice == 3)
            {
                temp = "<color=red>Raising Wires...</color>";
            }
            ShowText(temp);
            //display if enemy is charging up minigame
        }
        else if(StateManager.Instance.currentState == StateManager.BattleState.Win)
        {
            ShowText("Enemy is weak.");
        }
        else if (StateManager.Instance.currentState == StateManager.BattleState.Lose)
        {
            ShowText("Out of energy.Systems failure");
        }
    }

    /*private IEnumerator TypeText(string fullText)//type out text in the chat log
    {
        chatText.text = "";
        bool isAddingRichTextTag = false;
        foreach (char letter in fullText)
        {
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                chatText.text += letter;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                chatText.text += letter;

                yield return new WaitForSeconds(_typingSpeed);
            }
        }
        
        //_scroll.velocity = new Vector2 (0f, 1000f);
    }*/
}
