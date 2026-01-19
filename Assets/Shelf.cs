using UnityEngine;
using System.Collections.Generic;

public class Shelf : MonoBehaviour
{
    public Transform[] slots;
    private List<Item> items = new List<Item>();

    // Проверка — можно ли поставить предмет
    public bool CanPlaceItem()
    {
        return items.Count < slots.Length;
    }

    // Игрок кладёт предмет на полку
    public void PlaceItem(Item item)
    {
        if (item == null)
            return;

        if (!CanPlaceItem())
        {
            Debug.Log("Shelf is full");
            return;
        }

        int index = items.Count;
        items.Add(item);

        item.transform.SetParent(slots[index]);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        if (item.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;

        if (item.TryGetComponent(out Collider col))
            col.enabled = true;

        item.gameObject.SetActive(true);

        Debug.Log("Item placed on shelf");
    }

    // 👇 ПОКУПАТЕЛЬ БЕРЁТ ТОВАР С ПОЛКИ
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

    // Проверка — есть ли товар
    public bool HasItems()
    {
        return items.Count > 0;
    }
}