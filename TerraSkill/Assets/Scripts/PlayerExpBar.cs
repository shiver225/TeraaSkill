using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] LevelController controller;
    private int LevelCap = 10;
    private int Level = 0;
    public float expGainMultiplier = 0.01f;

    private void Awake()
    {
        controller = FindObjectOfType<LevelController>();
    }

    public int UpdateExpBar(int currentExp, int expGained) 
    {
        currentExp = currentExp + expGained;

        if(Level == LevelCap){
            slider.value = 1;
            currentExp = 0;
        }
        else{
            if(currentExp  == 100) {
                slider.value = 0;
                currentExp = 0;
                if(Level < LevelCap) {
                    Level = Level + 1;
                    controller.UpgradeLevel();
                }
            }
            if (currentExp >= 100) {
                currentExp = currentExp - 100;
                slider.value = currentExp * expGainMultiplier;
                if(Level < LevelCap){
                    Level = Level + 1;
                    controller.UpgradeLevel();
                }
            }
            else
            {
                slider.value = currentExp * expGainMultiplier;
            }
        }
        Debug.Log("CURRENT LEVEL: " + Level);
        Debug.Log("CURRENT EXP: " + currentExp);
        return currentExp;
    }
    

    // private void upgradeLevel()
    // {

    // }
}
