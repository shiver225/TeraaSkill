using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateHealthBar(float currValue, float maxValue) 
    {
        slider.value = currValue / maxValue;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
