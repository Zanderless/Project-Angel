using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Wreckless/Items/Default")]
public class Item : ScriptableObject, IComparable<Item>
{

    [Header("Item Info")]
    public string itemName;
    public int itemID;
    [TextArea]
    public string itemDescription;
    public Sprite itemIcon;
    public int itemValue;

    public int CompareTo(Item compareItem)
    {
        // A null value means that this object is greater.
        if (compareItem == null)
            return -1;

        else
            return this.itemID.CompareTo(compareItem.itemID);
    }

}
