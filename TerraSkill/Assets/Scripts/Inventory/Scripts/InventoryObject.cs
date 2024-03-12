using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName ="New Inventory", menuName ="Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDatabaseObject database;
    public Inventory Container;

    public void AddItem(Item item,int amount)
    {
       
        for (int i = 0; i < Container.Items.Length; i++)
        {
            InventorySlot slot = Container.Items[i];
            if (slot.item != null &&slot.item.ID == item.ID)
            {
                slot.AddAmount(amount);
                return;
            }
        }
        SetEmtpySlot(item, amount);
    }
    public InventorySlot SetEmtpySlot(Item item,int amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            InventorySlot slot = Container.Items[i];
            if(slot.ID <= -1)
            {
                slot.UpdateSlot(item.ID, item, amount);
                return slot;
            }
        }
        //Kai inventorius pilnas kazka daryti
        return null;
    }

    [ContextMenu("Save Inventory")]
    public void SaveInventory()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }
    [ContextMenu("Load Inventory")]
    public void LoadInventory()
    {
        if(File.Exists(savePath))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(savePath, FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < Container.Items.Length; i++)
                Container.Items[i].UpdateSlot(newContainer.Items[i].ID, newContainer.Items[i].item, newContainer.Items[i].amount);
            
            stream.Close();
        }
    }
    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        Container = new Inventory();
    }
    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.ID, item2.item, item2.amount);
        item2.UpdateSlot(item1.ID, item1.item, item1.amount);
        item1.UpdateSlot(temp.ID, temp.item, temp.amount);
    }
    public void RemoveItem(Item item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == item)
            {
                Container.Items[i].UpdateSlot(-1, null, 0);
            }
        }
    }
}
[System.Serializable]
public class InventorySlot
{
    public int ID = -1;
    public Item item;
    public int amount;
    public InventorySlot(int _id, Item _item, int _amount)
    {
        amount = _amount;
        item = _item;
        ID = _id;
    }
    public InventorySlot()
    {
        amount = 0;
        item = new Item();
        ID = -1;
    }
    public void UpdateSlot(int _id, Item _item, int _amount)
    {
        amount = _amount;
        item = _item;
        ID = _id;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[GlobalOptions.Inventory_SlotCount];
}