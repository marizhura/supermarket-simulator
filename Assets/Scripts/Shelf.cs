using UnityEngine;
using System.Collections.Generic;

public class Shelf : MonoBehaviour
{
    [Header("Surface")]
    public Transform approachPoint;
    public Transform surface;
    public ShelfSurface shelfSurface;

    private List<Item> items = new List<Item>();
    private bool hasLoggedMissingSlots = false;

    void Awake()
    {
        if (shelfSurface != null)
        {
            if (surface != null && surface != shelfSurface.transform)
                Debug.LogWarning("Shelf: surface differs from shelfSurface, using shelfSurface");
            surface = shelfSurface.transform;
            hasLoggedMissingSlots = false;
        }

        if (surface == null)
        {
            surface = transform;
            Debug.LogWarning("Shelf: surface not set, using shelf transform");
        }

        if (shelfSurface == null && surface != null)
        {
            shelfSurface = surface.GetComponent<ShelfSurface>();
            if (shelfSurface != null)
                hasLoggedMissingSlots = false;
        }
    }

    public bool CanPlaceItem()
    {
        int capacity = GetSlotCount();
        if (capacity == 0)
            return false;

        return items.Count < capacity;
    }

    public void PlaceItem(Item item)
    {
        if (item == null || !CanPlaceItem())
            return;

        int index = items.Count;
        items.Add(item);

        Transform slot = GetSlotTransform(index);
        if (slot != null)
        {
            item.transform.SetParent(slot);
            item.transform.localPosition = Vector3.zero;
        }
        item.transform.localRotation = Quaternion.identity;

        if (item.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;

        if (item.TryGetComponent(out Collider col))
            col.enabled = true;

        item.gameObject.SetActive(true);
    }

    public Item TakeItemFromShelf()
    {
        if (items.Count == 0)
            return null;

        Item item = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);

        item.transform.SetParent(null);

        if (item.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = false;

        return item;
    }

    private int GetSlotCount()
    {
        if (shelfSurface != null && shelfSurface.SlotCount > 0)
            return shelfSurface.SlotCount;

        if (!hasLoggedMissingSlots)
        {
            Debug.LogWarning("Shelf: ShelfSurface not set or has no slots");
            hasLoggedMissingSlots = true;
        }
        return 0;
    }

    private Transform GetSlotTransform(int index)
    {
        if (shelfSurface == null)
            return null;

        return shelfSurface.GetSlot(index);
    }

    public bool HasItems()
{
    return items != null && items.Count > 0;
}
}
