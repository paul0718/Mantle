using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTime : MonoBehaviour
{
    public Vector2 currentTime; //x is hours, y is minutes
    
    void Start()
    {
        StartCoroutine(TrackTime());
    }

    private IEnumerator TrackTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            currentTime.y++;
            if (currentTime.y > 59)
            {
                currentTime.x++;
                currentTime.y -= 60;
                if (currentTime.x > 12)
                    currentTime.x -= 12;
            }
            if (currentTime.y < 10)
                GetComponent<TMPro.TextMeshProUGUI>().text = currentTime.x + ":0" + currentTime.y;
            else
                GetComponent<TMPro.TextMeshProUGUI>().text = currentTime.x + ":" + currentTime.y;
            if (SequenceManager.Instance.SequenceID != 11)
                GetComponent<TMPro.TextMeshProUGUI>().text += " PM";
            else
                GetComponent<TMPro.TextMeshProUGUI>().text += " AM"; 
        }
    }
}
