using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestColiderScript : MonoBehaviour
{
    public Animator chestAnim;
    public ChestState State;
    private Collider currentColider;
    public void Awake()
    {
        State = ChestState.New;
    }
    public enum ChestState
    {
        New,
        Opened,
        Locked
    }
    private void Update()
    {
        OnTriggerStay(currentColider);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            MainPlayerController controler = other.gameObject.GetComponent<MainPlayerController>();
            currentColider = other;
            if (State == ChestState.New)
            {
                controler.SetActionText("Open chest?");
                controler.ActionPanelDisplay(true);
            }
        }   
    }
    private void OnTriggerStay(Collider other)
    {
        if (other != null && other.gameObject.name == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            MainPlayerController controler = other.gameObject.GetComponent<MainPlayerController>();
            if (State == ChestState.New)
            {
                OpenChest(controler);
                State = ChestState.Opened;
            }
            else
            {
                if(State == ChestState.Opened)
                {
                    //rodom pranešima kad atidarytas;
                }
                else
                {
                    //Rodom kad uzrakintas
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            MainPlayerController controler = other.gameObject.GetComponent<MainPlayerController>();
            controler.SetActionText("-");
            controler.ActionPanelDisplay(false);
        }
        currentColider = null;
    }
    public void OpenChest(MainPlayerController controller)
    {
        int maxCnt = controller.database.Items.Count();
        int rnd = Random.Range(0, maxCnt);

        chestAnim.Play("ChestOpen");
        controller.SpawnItemObject(controller.database.Items[rnd].phisicalItemObject);
    }
}
