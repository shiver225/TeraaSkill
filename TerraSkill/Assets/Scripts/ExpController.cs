using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ExpController : MonoBehaviour
{
    [SerializeField] public PlayerExpBar expBar;
    [SerializeField] SwordCombat Scombat;

    public int currentExp;

    // Start is called before the first frame update
    
    void Start()
    {
        currentExp = 0;
        expBar = GetComponent<MainPlayerController>().inventoryPanel.gameObject.transform.parent.GetComponentInChildren<PlayerExpBar>();
        Scombat = GetComponentInChildren<SwordCombat>();
        expBar.UpdateExpBar(currentExp, 0);
    }

}