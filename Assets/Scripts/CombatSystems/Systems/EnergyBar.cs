using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core.Easing;

public class EnergyBar : MonoBehaviour
{
    private Slider energySlider;
    [SerializeField] GameObject[] energyMarkers;
    [SerializeField] private int energyUsed = 0;

    public bool energyBlink = false;

    void Start()
    {
        energySlider = GetComponent<Slider>();
        if (energyUsed > 0)
        {
            int j = 0;
            while (j < energyUsed)
            {
                energyMarkers[j].transform.GetChild(1).gameObject.SetActive(false);
                j++;
            }
        }
    }

    public void Update()
    {
        if (energyBlink)
        {
            float duration = 0.7f;
            float alpha = 0;
            int j = energyUsed;
            float lerp = Mathf.PingPong (Time.time, duration) / duration;
            alpha = Mathf.Lerp(0.1f, 1f, Mathf.SmoothStep(0.1f, 1f, lerp));
            energyMarkers[j].transform.GetChild(1).GetComponent<Image>().color = new Color(1,1,1, alpha);
            energyMarkers[j+1].transform.GetChild(1).GetComponent<Image>().color = new Color(1,1,1, alpha);
            energyMarkers[j+2].transform.GetChild(1).GetComponent<Image>().color = new Color(1,1,1, alpha);
        }
    }

    public void UpdateEnergy(float changeVal)
    {
        StartCoroutine(LowerEnergy());
    }

    private IEnumerator LowerEnergy()
    {
        for (int j = energyUsed; j <= energyUsed + 2; j++)
        {
            energyMarkers[j].transform.GetChild(1).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        energyUsed += 3;
    }

    public void CheckIfLost()
    {
        if (energyUsed >= 24)
        {
            Debug.Log("You lost!");
            StateManager.Instance.LoseBattle();
        }
    }

    public string CheckEnergyLevel()
    {
        if (energyUsed < 6)
            return "high";
        else if (energyUsed < 12)
            return "medium";
        else if (energyUsed < 18)
            return "low";
        else
            return "critical";
    }

    public void EnergyBlink()
    {
        
    }
}
