using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    public Item CurrentItem;
    public Transform holdPoint;

    public void TakeItem(Item item)
    {
        if (item == null || CurrentItem != null)
            return;

        CurrentItem = item;

        // выключаем физику
        if (item.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;

        if (item.TryGetComponent(out Collider col))
            col.enabled = false;

        // кладём в руки
        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log("Item in hands");
    }

    public void DropItem()
    {
        CurrentItem = null;
    }
}