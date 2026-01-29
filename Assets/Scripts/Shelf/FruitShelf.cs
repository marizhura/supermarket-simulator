using UnityEngine;

public class FruitShelf : MonoBehaviour
{
    private ShelfBox[] boxes;

    void Awake()
    {
        boxes = GetComponentsInChildren<ShelfBox>(true);
        Debug.Log($"FruitShelf: найдено ящиков = {boxes.Length}");
    }

    public bool HasItems()
    {
        foreach (var box in boxes)
            if (box.HasItems())
                return true;

        return false;
    }

    public Item TakeItemFromShelf()
    {
        foreach (var box in boxes)
            if (box.HasItems())
                return box.TakeItem();

        return null;
    }

    public bool AddItemToShelf(Item item)
    {
        foreach (var box in boxes)
        {
            if (box.HasSpaceFor(item))
            {
                box.AddItem(item);
                return true;
            }
        }
        return false;
    }
}