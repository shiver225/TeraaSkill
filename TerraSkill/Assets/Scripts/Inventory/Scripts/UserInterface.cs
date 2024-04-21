using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public abstract class UserInterface : MonoBehaviour
{
    public MainPlayerController player;
    public InventoryObject inventory;

    public Dictionary<GameObject, InventorySlot> itemDisplay = new Dictionary<GameObject, InventorySlot>();
    void Start()
    {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
            inventory.Container.Items[i].parent = this;
        
        CreateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
    }
    public abstract void CreateDisplay();

    public void UpdateDisplay()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> item in itemDisplay)
        {
            if (item.Value.ID >= 0)
            {
                item.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[item.Value.item.ID].prefab;
                item.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                item.Key.GetComponentInChildren<TextMeshProUGUI>().text = item.Value.amount == 1 ? "" : item.Value.amount.ToString("n0");
            }
            else
            {
                item.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                item.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                item.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }

    }
    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
    public void OnEnter(GameObject obj)
    {
        player.mouseItem.hoverObj = obj;
        if (itemDisplay.ContainsKey(obj))
        {
            player.mouseItem.hoverItem = itemDisplay[obj];
        }
    }
    public void OnExit(GameObject obj)
    {
        player.mouseItem.hoverObj = null;
        player.mouseItem.hoverItem = null;

    }
    public void OnBeginDrag(GameObject obj)
    {
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);
        if (itemDisplay[obj].ID >= 0)
        {
            var image = mouseObject.AddComponent<Image>();
            image.sprite = inventory.database.GetItem[itemDisplay[obj].ID].prefab;
            image.raycastTarget = false;
        }
        player.mouseItem.obj = mouseObject;
        player.mouseItem.item = itemDisplay[obj];
    }
    public void OnEndDrag(GameObject obj)
    {
        var itemOnMouse = player.mouseItem;
        var mouseHoverItem = itemOnMouse.hoverItem;
        var mouseHoverObj = itemOnMouse.hoverObj;
        var getITemObject = inventory.database.GetItem;

        if (mouseHoverObj)
        {
            if (mouseHoverItem.CanPlaceInSlot(getITemObject[itemDisplay[obj].ID]) && (mouseHoverItem.item.ID <= -1 || (mouseHoverItem.item.ID >= 0 && itemDisplay[obj].CanPlaceInSlot(getITemObject[mouseHoverItem.item.ID]))))
                inventory.MoveItem(itemDisplay[obj], mouseHoverItem.parent.itemDisplay[mouseHoverObj]);
        }
        else
        {
           
            foreach (GameObject weaponObject in player.AllWeapons)
            {
                GroundItem groundItem = weaponObject.GetComponent<GroundItem>();

                if (groundItem != null && itemDisplay[obj].item.ID == groundItem.item.ID)
                {
                    SpawnWeaponObject(weaponObject);
                    break;
                }
                else
                {
                    Debug.LogWarning($"GroundItem script not found on {weaponObject.name}.");
                }
            }
            inventory.RemoveItem(itemDisplay[obj].item);
        }
        Destroy(itemOnMouse.obj);
        itemOnMouse.item = null;
    }
    public void OnDrag(GameObject obj)
    {
        if (player.mouseItem.obj != null)
        {
            player.mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    private void SpawnWeaponObject(GameObject weaponObject)
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        Vector3 spawnPosition = playerObject.transform.position + new Vector3(0f, 1f, 0f);
        GameObject newWeaponInstance = Instantiate(weaponObject);
        newWeaponInstance.transform.position = spawnPosition;
        newWeaponInstance.transform.rotation = Quaternion.identity;
        var collider = newWeaponInstance.GetComponent<Collider>();
        if(collider != null)
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
}
public class MouseItem
{
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}