using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelfDestruct()
    {
        CoreButton.Instance.RegisterFunction(CoreButton.FunctionType.Destruct, () =>
        {
            Debug.Log("self destructed good bye");
        });
        CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.Destruct);
        ChatLogManager.Instance.ShowText("<color=orange>Self-Destruct?");
    }
}
