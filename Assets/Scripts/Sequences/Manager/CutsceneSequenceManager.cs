using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneSequenceManager : MonoBehaviour
{
    [SerializeField] private GameObject aceDeadComic;
    [SerializeField] private GameObject mantleDeadComic;
    private void Start()
    {
        if (SequenceManager.Instance.SequenceID == 18)
        {
            if (SequenceManager.Instance.aceIsDead)
            {
                AudioManager.Instance.SetBGMLibrary("S18MantleWinBGM");
                mantleDeadComic.SetActive(false);
                aceDeadComic.SetActive(true);
            }
            else
            {
                AudioManager.Instance.SetBGMLibrary("S18MantleLoseBGM");
                aceDeadComic.SetActive(false);
                mantleDeadComic.SetActive(true);
            }
        }
    }
}
