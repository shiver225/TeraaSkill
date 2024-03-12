using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Equipment,
    Default
}

public abstract class ItemObject : ScriptableObject
{
    public int ID = -1;
    public Sprite prefab;
    public ItemType type;
    [TextArea(15,20)]
    public string description;
    public ItemObject()
    {
        ID = -1;
    }
}

[System.Serializable]
public class Item
{
    public string Name;
    public int ID = -1;
    public Item(ItemObject item)
    {
        Name = item.name;
        ID = item.ID;
    }
    public Item()
    {
        Name = null;
        ID = -1;
    }
}