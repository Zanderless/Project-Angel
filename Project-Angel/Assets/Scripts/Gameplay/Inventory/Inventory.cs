using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance;

    public List<Item> items = new List<Item>();
    public List<ItemCount> itemCounts = new List<ItemCount>();

    private void Start()
    {
        ResortInventory();
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        ResortInventory();
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        ResortInventory();
    }

    public List<ItemCount> GetRestorableItems()
    {

        List<ItemCount> restore = new List<ItemCount>();

        foreach(ItemCount count in itemCounts)
        {
            if (count.item is Restorables)
                restore.Add(count);
        }

        return restore;

    }

    private void ResortInventory()
    {
        //Sort Items
        items.Sort();

        //Reinitilize itemCounts List
        itemCounts = new List<ItemCount>();

        Item lastItem = null;
        int count = 0;

        //Recount
        for (int i = 0; i < items.Count; i++)
        {

            if (lastItem == null)
                lastItem = items[i];

            if (lastItem.itemID != items[i].itemID)
            {
                itemCounts.Add(new ItemCount(lastItem, count));
                lastItem = items[i];
                count = 0;
            }

            count++;

            if (i == items.Count - 1)
            {
                itemCounts.Add(new ItemCount(lastItem, count));
            }

        }
    }

    public void Awake()
    {
        Instance = this;
    }


}

[System.Serializable]
public struct ItemCount
{

    public Item item;
    public int count;

    public ItemCount(Item _item, int _count)
    {
        item = _item;
        count = _count;
    }

}
