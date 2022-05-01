using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] items;
    public Dictionary<ItemObject, int> getID = new Dictionary<ItemObject, int>();
    public Dictionary<int, ItemObject> getItem = new Dictionary<int, ItemObject>();

    public void OnAfterDeserialize()
    {
        getID = new Dictionary<ItemObject, int>();
        getItem = new Dictionary<int, ItemObject>();
        for (int i = 0; i < items.Length; i++)
        {
            getID.Add(items[i], i);
            getItem.Add(i, items[i]);
        }
    }

    public void OnBeforeSerialize()
    {

    }
}
