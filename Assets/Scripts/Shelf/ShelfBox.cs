using UnityEngine;
using System.Collections.Generic;

public class ShelfBox : MonoBehaviour
{
    [Header("Setup")]
    public Transform spawnPoint;
    public Transform itemsRoot;

    [Header("Capacity")]
    public int maxSlots = 16;

    private List<Item> items = new List<Item>();
    private int usedSlots = 0;

    // сколько слотов занимает товар
    int GetSizeSlots(Item item)
    {
        switch (item.size)
        {
            case ItemSize.Small: return 1;
            case ItemSize.Medium: return 2;
            case ItemSize.Large: return 4;
            default: return 1;
        }
    }

    public bool HasItems()
    {
        return items.Count > 0;
    }

    public bool HasSpaceFor(Item item)
    {
        return usedSlots + GetSizeSlots(item) <= maxSlots;
    }

    public void AddItem(Item item)
    {
        int size = GetSizeSlots(item);

        items.Add(item);
        usedSlots += size;

        item.transform.SetParent(itemsRoot);
        item.transform.position = spawnPoint.position;
        item.gameObject.SetActive(true);
    }

    public Item TakeItem()
    {
        if (items.Count == 0)
            return null;

        Item item = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        usedSlots -= GetSizeSlots(item);

        return item;
    }
}