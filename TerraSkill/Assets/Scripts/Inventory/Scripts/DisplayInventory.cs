using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class DisplayInventory : MonoBehaviour
{
    public InventoryObject inventory;
    public int X_Start;
    public int Y_Start;

    public int X_Space_Between_Items;
    public int Number_of_column;
    public int y_Space_Between_Items;
    Dictionary<InventorySlot,GameObject> itemDisplay = new Dictionary<InventorySlot, GameObject> ();
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
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
            itemDisplay.Add(inventory.Container[i], obj);
        }
    }
    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_Start + (X_Space_Between_Items * (i % Number_of_column)),Y_Start + (-y_Space_Between_Items * (i / Number_of_column)), 0f);
    }
    public void UpdateDisplay()
    {
        for(int i = 0;i < inventory.Container.Count; i++)
        {
            if (itemDisplay.ContainsKey(inventory.Container[i]))
            {
                itemDisplay[inventory.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
            }
            else
            {
                var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
                itemDisplay.Add(inventory.Container[i], obj);
            }
        }
    }
}
