using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPlayerController : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject inventoryEquipment;
    public GameObject inventoryPanel;
    public GameObject actionBoxPanel;
    public GameObject deathPanel;
    public GameObject[] AllWeapons;
    public ItemDatabaseObject database;
    public MouseItem mouseItem = new MouseItem();
    // Start is called before the first frame update
    void Start()
    {
        inventoryPanel.SetActive(false);
        actionBoxPanel.SetActive(false);
        deathPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.SaveInventory();
            inventoryEquipment.SaveInventory();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.LoadInventory();
            inventoryEquipment.LoadInventory();
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
        inventory.Container.Items = new InventorySlot[GlobalOptions.Inventory_SlotCount];
        inventoryEquipment.ClearInventory();
    }

    #region Action Button
    public void SetActionText(string text)
    {
        if (actionBoxPanel != null)
        {
            actionBoxPanel.SetActive(!actionBoxPanel.active);
            TextMeshProUGUI textMeshPro = actionBoxPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textMeshPro.text = text;
            }
            else
                Debug.LogWarning("TextMeshPro component not found in actionBoxPanel!!!");

        }
        else
            Debug.LogWarning("ActionPanel is not defined and not added to players main script!!!");

    }
    public void ActionPanelDisplay(bool display)
    {
        if (actionBoxPanel != null)
            actionBoxPanel.SetActive(display);
        else
            Debug.LogWarning("ActionPanel is not defined and not added to players main script!!!");
    }

    #endregion

    #region Inventory and Item Controls
    public void SpawnItemObject(GameObject weaponObject)
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        Vector3 spawnPosition = playerObject.transform.position + new Vector3(0f, 0.4f, 0f);
        GameObject newWeaponInstance = Instantiate(weaponObject);
        newWeaponInstance.transform.position = spawnPosition;
        newWeaponInstance.transform.rotation = Quaternion.identity;
        var collider = newWeaponInstance.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
            StartCoroutine(DelayEnableCollider(collider));
        }
    }
    private IEnumerator DelayEnableCollider(Collider collider)
    {
        yield return new WaitForSeconds(2f);
        if (collider != null)
        {
            collider.enabled = true;
        }
    }
    #endregion

    public void Respawn()
    {
        SceneManager.LoadScene(1);
    }

}
