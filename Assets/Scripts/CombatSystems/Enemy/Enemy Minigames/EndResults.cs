using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndResults : MonoBehaviour
{
    [SerializeField] private GameObject textBox;

    [SerializeField] private TMP_Text text;

    [SerializeField] private GameObject optionCanvas;

    [SerializeField] private GameObject[] games;
    public void UpdateResult(bool win)
    {
        textBox.SetActive(true);
        if (win)
        {
            text.text = "Nooo!";
        }
        else
        {
            text.text = "Loser";
        }

        StartCoroutine(CloseGame());
    }

    IEnumerator CloseGame()
    {
        yield return new WaitForSeconds(2f);
        textBox.SetActive(false);
        foreach (var g in games)
        {
            g.SetActive(false);
        }
        optionCanvas.SetActive(true);
    }
}
