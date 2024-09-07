using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnergyBar : MonoBehaviour
{
    private Slider _energySlider;
    // Start is called before the first frame update
    void Start()
    {
        _energySlider = GetComponent<Slider>();
    }

    public void UpdateEnergy(float changeVal)//Update energy levels 
    {
        float temp = _energySlider.value - changeVal;
        _energySlider.DOValue(temp, 1.0f);
    }
}
