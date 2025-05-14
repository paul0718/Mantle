using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtensions
{
    private static readonly System.Collections.Generic.Dictionary<Button, bool> ButtonTriggers =
        new System.Collections.Generic.Dictionary<Button, bool>();

    public static void SetTrigger(this Button button)
    {
        ButtonTriggers[button] = true;
    }

    public static bool GetAndResetTrigger(this Button button)
    {
        if (ButtonTriggers.TryGetValue(button, out bool isTriggered) && isTriggered)
        {
            ButtonTriggers[button] = false; 
            return true;
        }
        return false;
    }
}
