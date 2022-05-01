using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    consumable,
    equipment,
    miscellaneous

}
public abstract class ItemObject : ScriptableObject
{
    public int ID;
    public GameObject prefab;
    public string itemID;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    public int stackLimit;

}
