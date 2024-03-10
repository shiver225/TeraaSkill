using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory", menuName ="Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public void AddItem(ItemObject _item,int _amount)
    {
        bool hasItems = false;
        foreach (InventorySlot slot in Container) 
        {
            if(slot.item == _item)
            {
                slot.AddAmount(_amount);
                hasItems = true;
                break;
            }
        }

        if (!hasItems) 
        {
            Container.Add(new InventorySlot(_item,_amount));
        }
    }

}
[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    public InventorySlot(ItemObject _item, int _amount)
    {
        amount = _amount;
        item = _item;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}
