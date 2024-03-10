using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerController : MonoBehaviour
{
    public InventoryObject inventory;
    public GameObject inventoryPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.SaveInventory();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.LoadInventory();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.active);
            Cursor.lockState = inventoryPanel.active ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = inventoryPanel.active;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1);
            Destroy(other.gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        inventory.Container.Items = new InventorySlot[24];
    }
}
