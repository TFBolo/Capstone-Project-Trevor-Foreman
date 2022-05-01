using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Miscellaneous Object", menuName = "Inventory System/Items/Miscellaneous")]
public class MiscellaneousObject : ItemObject
{
    public void Awake()
    {
        type = ItemType.miscellaneous;
    }
}
