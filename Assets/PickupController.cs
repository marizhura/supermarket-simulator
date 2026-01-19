using UnityEngine;

public class PickupController : MonoBehaviour
{
    public float pickupDistance = 2f;
    public Camera playerCamera;
    public PlayerHands playerHands;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (playerCamera == null || playerHands == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, pickupDistance))
            return;

        // 🔹 ЕСЛИ В РУКАХ НЕТ — БЕРЁМ
        if (playerHands.CurrentItem == null)
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                playerHands.TakeItem(item);
            }
        }
        // 🔹 ЕСЛИ В РУКАХ ЕСТЬ — КЛАДЁМ
        else
        {
            // 👇 ИЩЕМ Shelf НЕ НА КОЛЛАЙДЕРЕ, А У РОДИТЕЛЯ
            Shelf shelf = hit.collider.GetComponentInParent<Shelf>();

            if (shelf != null && shelf.CanPlaceItem())
            {
                shelf.PlaceItem(playerHands.CurrentItem);
                playerHands.DropItem();
            }
        }
    }
}