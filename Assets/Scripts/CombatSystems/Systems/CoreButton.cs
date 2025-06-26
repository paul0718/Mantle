using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoreButton : MonoBehaviour
{
    public Animator coreButtonAnimator;
    public Animator wireAnimator;
    public Animator flashAnimator;
    public static CoreButton Instance { get; private set; }
    public enum FunctionType
    {
        Scan,
        LockIn,
        Respond,
        Kill,
        Capture,
        Destruct
    };

    private Dictionary<FunctionType, Action> functions = new Dictionary<FunctionType, Action>();
    private Button coreButton;
    
    public void RegisterFunction(FunctionType type, Action action)
    {
        functions[type] = action;
    }
    
    public void SetCoreButton(FunctionType type)
    {
        coreButton.onClick.RemoveAllListeners();
        coreButton.onClick.AddListener(functions[type].Invoke);
        coreButton.onClick.AddListener(() => AudioManager.Instance.PlayOneShot(SFXNAME.CoreButton));
    }
    public void ClearCoreButton()
    {
        coreButton.onClick.RemoveAllListeners();
    }
    public void SetAnimation()
    {
        Debug.Log("SetAnimation");
        coreButtonAnimator.SetTrigger("Animation");
        wireAnimator.SetTrigger("Animation");
    }
    public void StartFlash()
    {
        flashAnimator.gameObject.SetActive(true);
    }
    public void ResetAnimation()
    {
        coreButtonAnimator.SetTrigger("Reset");
        wireAnimator.SetTrigger("Reset");
        flashAnimator.gameObject.SetActive(false);
    }
    private void Awake()
    {
        Instance = this;
        coreButton = GetComponent<Button>();
    }
}
