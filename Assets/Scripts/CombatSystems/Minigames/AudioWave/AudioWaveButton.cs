using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioWaveButton : MonoBehaviour
{
    public Button button;
    public Image buttonImage;
    public Image handler;

    public Sprite[] waveSprites;

    public int state = 1;
    [HideInInspector]
    public int way = 1;
    private static readonly float[] buttonPositionY = new float[3] { -20, 0, 20 };
    private static readonly float[] handlePositionY = new float[3] { -50, 0, 50 };
    private void Start()
    {
        button.onClick.AddListener(()=>HandleFunction(state));
    }
    private void HandleFunction()
    {
        state += way;
        if (state == 2) way = -1;
        if (state == 0) way = 1;
        //AudioWave.Instance.PlayOneShot(state);
        handler.sprite = waveSprites[state];
        button.transform.DOLocalMoveY(buttonPositionY[state], 0.2f) ;
        handler.transform.DOLocalMoveY(handlePositionY[state], 0.2f);
    }
    public void ResetHandle()
    {
        state = 1;
        handler.sprite = waveSprites[state];
        button.transform.DOLocalMoveY(buttonPositionY[state], 0.2f);
        handler.transform.DOLocalMoveY(handlePositionY[state], 0.2f);
    }
    public void HandleFunction(int state)
    {
        this.state = state;
        int n = int.Parse(this.name.Substring(6, 1));
        AudioWave.Instance.audioWaveButtons[n-1].state = state;
        if (AudioWave.Instance.tuneButton.interactable)
        {
            AudioWave.Instance.PlayOneShot(state);
        }
        
        handler.sprite = waveSprites[state];
        buttonImage.transform.DOLocalMoveY(buttonPositionY[state], 0.2f);
        handler.transform.DOLocalMoveY(handlePositionY[state], 0.2f);
    }
}
