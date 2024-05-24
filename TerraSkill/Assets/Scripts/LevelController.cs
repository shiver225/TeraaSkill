using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] GameObject upgradeMenu;
    private float dmgInc = 5f;
    private float speepInc = 2f;
    private float healthInc = 10f;
    [SerializeField] MovementController movement;
    [SerializeField] SwordCombat combat;
    [SerializeField] PlayerHealthContoller health;

    // Start is called before the first frame update
    private void Awake(){
        upgradeMenu = GameObject.Find("AddSkill");
        movement = FindObjectOfType<MovementController>();
        combat = FindObjectOfType<SwordCombat>();
        health = FindObjectOfType<PlayerHealthContoller>();
    }
    
    void Start()
    {
        upgradeMenu.active = false;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.B) && upgradeMenu.active == true){
            Debug.Log("UPGRADED DAMAGE");
            DamageIncrease();
        }
        else if(Input.GetKeyDown(KeyCode.N) && upgradeMenu.active == true){
            Debug.Log("UPGRADED SPEED");
            SpeedIncrease();
        }
        else if(Input.GetKeyDown(KeyCode.M) && upgradeMenu.active == true){
            Debug.Log("UPGRADED HEALTH");
            ExpGainIncrease();
        }
    }

    // Update is called once per frame
    public void UpgradeLevel(){
        upgradeMenu.active = true;
        Cursor.visible = true;
        Debug.Log("You leveled up!");
    }

    public void DamageIncrease(){
        combat.baseDamage += dmgInc;
        upgradeMenu.active = false;
        Cursor.visible = false;
    }
    public void SpeedIncrease(){
        movement.walkSpeed += speepInc;
        movement.sprintSpeed += speepInc;
        upgradeMenu.active = false;
        Cursor.visible = false;
    }
    public void ExpGainIncrease(){
        health.maxHealth += healthInc;
        upgradeMenu.active = false;
        Cursor.visible = false;
    }
}
