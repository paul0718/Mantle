using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoverManager : MonoBehaviour
{
    public static CoverManager Instance { get; private set; }
    
    public Image coverImage;
    public Image blockerImage;

    public Animator coverAnimator;
    public Animator killAnimator;
    public Animator captureAnimator;

    public GameObject Pipes;
    public GameObject Core;

    private bool init = false;
    [HideInInspector]
    public bool showDefendButton = false;
    public bool showKillButton = false;
    public bool showCaptureButton = false;
    private void Start()
    {
        Instance = this;

        CoreButton.Instance.RegisterFunction(CoreButton.FunctionType.Scan, () =>
        {
            StartCoroutine(ScanEnemy());
            CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.LockIn);
        });
        CoreButton.Instance.RegisterFunction(CoreButton.FunctionType.Kill, ()=>
        {
            killAnimator.gameObject.SetActive(true);
            StateManager.Instance.WeaponBattle(true);
            coverAnimator.SetTrigger("WireClose");
            coverAnimator.SetTrigger("PanelClose");
            GridManager.Instance.GridHiding();
            CoreButton.Instance.ClearCoreButton();
        });
        CoreButton.Instance.RegisterFunction(CoreButton.FunctionType.Capture, () => {
            captureAnimator.gameObject.SetActive(true);
            StateManager.Instance.WeaponBattle(false);
            coverAnimator.SetTrigger("WireClose");
            coverAnimator.SetTrigger("PanelClose");
            GridManager.Instance.GridHiding();
            CoreButton.Instance.ClearCoreButton();
        });

        CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.Scan);
    }
    public void PanelOpenSFX()
    {
        AudioManager.Instance.PlayOneShot(SFXNAME.PanelOpen);
    }
    public void PanelCloseSFX()
    {
        AudioManager.Instance.PlayOneShot(SFXNAME.PanelClose);
    }
    public void WireOpenSFX()
    {
        AudioManager.Instance.PlayOneShot(SFXNAME.WireOpen);
    }
    public void WireCloseSFX()
    {
        AudioManager.Instance.PlayOneShot(SFXNAME.WireClose);
    }
    public void ShowCover()
    {
        coverImage.raycastTarget = true;

    }
    public void HideCover()
    {
        coverImage.raycastTarget = false;
        if (init)
        {
            blockerImage.gameObject.SetActive(showDefendButton || showCaptureButton || showKillButton);
        }
        init = true;
    }
    public void SetDefendButton(bool show = true)
    {
        showDefendButton = show;
    }
    public void SetEndButton(bool kill)
    {
        if (kill)
        {
            showKillButton = true;
            showCaptureButton = false;
            CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.Kill);
        }
        else
        {
            showKillButton = false;
            showCaptureButton = true;
            CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.Capture);
        }
    }
    IEnumerator ScanEnemy()
    {
        if (!StateManager.Instance.debugGame)
        {
            ChatLogManager.Instance.ShowText("Scanning Enemy");
            AudioManager.Instance.PlayOneShot(SFXNAME.Scan);
            yield return new WaitForSeconds(3.5f);
            ChatLogManager.Instance.ShowText("Enemy Scanned");
            yield return new WaitForSeconds(1f);
        }
        
        GridManager.Instance.GridOpening().onComplete += () =>
        {
            blockerImage.gameObject.SetActive(false);
            ChatLogManager.Instance.ShowText("Battle Systems Ready");
        };
    }
    public void Switch(bool game, bool defend = false, bool blockGame = false)
    {
        Sequence sequence = DOTween.Sequence();
        if (!game && defend)
        {
            coverAnimator.SetTrigger("PanelOpen");
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() =>
            {
                MasterMinigames.Instance.SetGame();
                Core.SetActive(true);
                Pipes.SetActive(true);
                Debug.Log(StateManager.Instance.currentState);
                ChatLogManager.Instance.UpdateState();// When do you want me to show this?
                
            });
            CoreButton.Instance.ResetAnimation();
        }
        if (game && defend)
        {
            coverAnimator.SetTrigger("WireClose");
            coverAnimator.SetTrigger("PanelClose");
            sequence.AppendInterval(1.5f);
            sequence.AppendCallback(() =>
            {
                MasterMinigames.Instance.SetGame();
                Core.SetActive(false);
                Pipes.SetActive(false);
            });
        }
        if (!game && !defend)
        {
            coverAnimator.SetTrigger("WireClose");
            sequence.AppendInterval(1.5f);
            sequence.AppendCallback(() =>
            {
                MasterMinigames.Instance.SetGame();
                coverAnimator.SetTrigger("WireOpen");
                Core.SetActive(true);
                Pipes.SetActive(true);
                ChatLogManager.Instance.UpdateState();
            });
            if (blockGame)
            {
                CoreButton.Instance.ResetAnimation();
            }
            else
            if (!(StateManager.Instance.currentState == StateManager.BattleState.Win || StateManager.Instance.currentState == StateManager.BattleState.Lose) ) 
            {
                sequence.AppendInterval(1.55f);
                sequence.AppendCallback(() =>
                {
                    CoreButton.Instance.SetAnimation();
                });
            }
            
            
        }
        if (game && !defend)
        {
            coverAnimator.SetTrigger("WireClose");
            sequence.AppendInterval(1.5f);
            sequence.AppendCallback(() =>
            {
                MasterMinigames.Instance.SetGame();
                coverAnimator.SetTrigger("WireOpen");
                Core.SetActive(false);
                Pipes.SetActive(false);
            });
            
        }
    }
    public void SpecialSwitch(bool game, bool defend = false, bool blockGame = false)
    {
        CoverManager.Instance.coverAnimator.ResetTrigger("WireClose");
        CoverManager.Instance.coverAnimator.ResetTrigger("PanelClose");
        //CoverManager.Instance.coverAnimator.SetTrigger("WireOpen");
        CoverManager.Instance.coverAnimator.SetTrigger("PanelOpen");
        
        Sequence sequence = DOTween.Sequence();
        if (!game && defend)
        {
            //MasterMinigames.Instance.SetGame();
            Core.SetActive(true);
            Pipes.SetActive(true);
            Debug.Log(StateManager.Instance.currentState);
            ChatLogManager.Instance.UpdateState();// When do you want me to show this?
            CoreButton.Instance.ResetAnimation();
        }
        if (!game && !defend)
        {
            sequence.AppendInterval(1.5f);
            //MasterMinigames.Instance.SetGame();
            Core.SetActive(true);
            Pipes.SetActive(true);
            ChatLogManager.Instance.UpdateState();
            if (blockGame)
            {
                CoreButton.Instance.ResetAnimation();
            }
            else
            if (!(StateManager.Instance.currentState == StateManager.BattleState.Win || StateManager.Instance.currentState == StateManager.BattleState.Lose))
            {
                sequence.AppendInterval(1.55f);
                sequence.AppendCallback(() =>
                {
                    CoreButton.Instance.SetAnimation();
                });
            }
        }
    }
    public void EndGameAnimaton()
    {
        if (showKillButton)
        {
            killAnimator.enabled = true;
            AudioManager.Instance.PlayOneShot(SFXNAME.EndAnimStab);
        }
        else
        {
            captureAnimator.enabled = true;
            AudioManager.Instance.PlayOneShot(SFXNAME.EndAnimTaser);
        }
        Invoke("StopAnimation", 2f);
    }
    public void StopAnimation()
    {
        killAnimator.gameObject.SetActive(false);
        captureAnimator.gameObject.SetActive(false);
    }
}
