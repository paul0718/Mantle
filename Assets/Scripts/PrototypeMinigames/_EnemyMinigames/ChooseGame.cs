using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseGame : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    
    [SerializeField] private GameObject game;

    public void OpenGame()
    {
        game.SetActive(true);
        canvas.SetActive(false);
    }
}
