using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemDatabaseObject database;
    public List<InventorySlot> container = new List<InventorySlot>();

    public bool invChange = false;
    public bool invLoad = false;

    private void OnEnable()
    {
        database = Resources.Load<ItemDatabaseObject>("Database");
    }

    public void AddItem(ItemObject item, int amount)
    {
        invChange = true;
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].item == item)
            {
                container[i].AddAmount(amount);
                return;
            }
        }
        container.Add(new InventorySlot(database.getID[item], item, amount));
    }

    public void Load(string inventory)
    {
        JsonUtility.FromJsonOverwrite(inventory, this);
        database = Resources.Load<ItemDatabaseObject>("Database");
        for (int i = 0; i < container.Count; i++)
        {
            container[i].item = database.getItem[container[i].ID];
        }
        invLoad = true;
    }

    public void OnAfterDeserialize()
    {
        
    }

    public void OnBeforeSerialize()
    {
        
    }
}

[System.Serializable]
public class InventorySlot
{
    public int ID;
    public ItemObject item;
    public int amount;
    public InventorySlot(int ID, ItemObject item, int amount)
    {
        this.ID = ID;
        this.item = item;
        this.amount = amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
        if (amount > item.stackLimit && item.stackLimit != 0) amount = item.stackLimit;
    }
}
