using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AlertManager : MonoBehaviour
{
    [SerializeField] private GameObject _alertGO;
    [SerializeField] private TMP_Text _alertImage;
    
    [SerializeField] private AlertDialogue _dialogueScript;
    
    private bool _alertActive = true;
    // Start is called before the first frame update
    void Start()
    {
        if (_alertActive)
        {
            _alertImage.DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlertReceived()
    {
        _dialogueScript.UpdateText();
        _alertGO.SetActive(false);
    }
}
