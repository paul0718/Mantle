using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TestForLoop : MonoBehaviour
{
    // Start is called before the first frame update
    public Button button;
    private int number = 0;
    void Start()
    {
        // //Example 1: Trigger and stop by key.
        // AudioManager.Instance.PlayLoop(SFXNAME.Email, () => Input.GetKeyDown(KeyCode.T));
        //
        // //Example 2: Trigger and stop by button.
        // button.onClick.AddListener(button.SetTrigger);
        // AudioManager.Instance.PlayLoop(SFXNAME.Alert, () => button.GetAndResetTrigger());
        //
        // //Example 3: Trigger and stop by value.
        // AudioManager.Instance.PlayLoop(SFXNAME.TaserGunSFX, () => number > 100);
    }
}
