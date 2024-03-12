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

public class DisplayInventory : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();
    public GameObject inventoryPrefab;
    public InventoryObject inventory;
    public int X_Start;
    public int Y_Start;

    public int X_Space_Between_Items;
    public int Number_of_column;
    public int y_Space_Between_Items;
    Dictionary<GameObject,InventorySlot> itemDisplay = new Dictionary<GameObject,InventorySlot> ();
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }
    public void CreateDisplay()
    {
        itemDisplay = new Dictionary<GameObject, InventorySlot>();

        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container.Items[i].amount.ToString("n0");


            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnBeginDrag(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnEndDrag(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            itemDisplay.Add(obj, inventory.Container.Items[i]);
        }
    }
    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_Start + (X_Space_Between_Items * (i % Number_of_column)), Y_Start + (-y_Space_Between_Items * (i / Number_of_column)), 0f);
    }
    public void UpdateDisplay()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> item in itemDisplay)
        {
            if(item.Value.ID >= 0)
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
    private void AddEvent(GameObject obj,EventTriggerType type,UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent <EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
    public void OnEnter(GameObject obj)
    {
        mouseItem.hoverObj = obj;
        if (itemDisplay.ContainsKey(obj)) 
        {
            mouseItem.hoverItem = itemDisplay[obj];    
        }
    }
    public void OnExit(GameObject obj)
    {
        mouseItem.hoverObj = null;
        mouseItem.hoverItem = null;
        
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
        mouseItem.obj = mouseObject;
        mouseItem.item = itemDisplay[obj];
    }
    public void OnEndDrag(GameObject obj)
    {
        if (mouseItem.hoverObj)
        {
            inventory.MoveItem(itemDisplay[obj], itemDisplay[mouseItem.hoverObj]);
        }
        else
        {
            inventory.RemoveItem(itemDisplay[obj].item);
        }
        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }
    public void OnDrag(GameObject obj)
    {
        if (mouseItem.obj != null)
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
}