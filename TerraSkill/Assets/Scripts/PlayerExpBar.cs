using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public int UpdateExpBar(int currentExp, int expGained) 
    {
        currentExp = currentExp + expGained;

        if(slider.value == slider.maxValue) {
            slider.value = 0;
            currentExp = 0;
        }
        if (currentExp >= 100) {
            currentExp = currentExp - 100;
            slider.value = currentExp * 0.01f;
        }
        else
        {
            slider.value = currentExp * 0.01f;
        }
        return currentExp;
    }
    

    // private void upgradeLevel()
    // {

    // }
}
