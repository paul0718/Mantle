using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ChangeDate : MonoBehaviour
{
    public static ChangeDate Instance { get; private set; }
    
    [SerializeField] TMP_Text monthText;
    [SerializeField] TMP_Text dayText;
    [SerializeField] private TMP_Text timeText;
    
    public static int currentDay = 20;
    public static string month = "March";
    private static string time = "2:00PM";

    [SerializeField] static int[] amountToAddToDay = new[] { 0, 0, 0, 9, 0, 0, 14, 0, 0, 2, 0, 1};

    public static string[] newTimes = new[]
    {
        "2:00 PM", "2:20 PM", "7:00 PM", "4:00 PM", "4:35 PM", "7:00 PM",
        "4:30 PM", "5:00 PM", "7:00 PM", "3:00 AM", "3:50 AM", "7:00 AM"
    };
    
    private float dateChangeSpeed = 0.7f;

    private int[] battleTransitions = new[] { 1, 4, 7, 9 };
    
    void Start()
    {
        if (SequenceManager.Instance.SequenceID == 1)
        {
            time = "2:00 PM";
            month = "March";
            currentDay = 20;
        }

        if (SequenceManager.Instance.dateNum > 6)
        {
            monthText.text = "April";
        }
        else
        {
            monthText.text = "March";
        }

        dayText.text = currentDay.ToString();
        if (SequenceManager.Instance.SequenceID > 10)
        {
            monthText.color = Color.red;
            dayText.color = Color.red;
        }
        if (time == "7:00PM")
        {
            timeText.gameObject.SetActive(false);
        }
        timeText.text = time;
        StartCoroutine(IncrementDay());
    }

    IEnumerator IncrementDay()
    {
        if (battleTransitions.Contains(SequenceManager.Instance.dateNum))
        {
            AudioManager.Instance.PlayOneShot(SFXNAME.DateFlySFX);
        }
        
        yield return new WaitForSeconds(2f);
        dateChangeSpeed = 0.7f;
        int j = 0;
        while (j < amountToAddToDay[SequenceManager.Instance.dateNum])
        {
            currentDay++;
            if (currentDay > 30)
            {
                month = "April";
                monthText.text = month;
                currentDay = 1;
            }
            dayText.text = currentDay.ToString();
            j++;
            yield return new WaitForSeconds(dateChangeSpeed);
            dateChangeSpeed -= .1f;
            if (dateChangeSpeed < 0.3f)
            {
                dateChangeSpeed = 0.3f;
            }
        }
        
        //time text
        yield return new WaitForSeconds(0.5f);
        time = newTimes[SequenceManager.Instance.dateNum];
        timeText.text = time;
        timeText.gameObject.SetActive(true);
        
        //load next scene
        yield return new WaitForSeconds(1.5f);
        SequenceManager.Instance.dateNum++;
        SceneTransition.Instance.FadeToBlack();
    }
}
