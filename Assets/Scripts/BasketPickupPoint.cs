using UnityEngine;

public class BasketPickupPoint : MonoBehaviour
{
    public GameObject basketPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Customer")) return;

        Customer customer = other.GetComponent<Customer>();
        if (customer == null) return;

        customer.TakeBasket(basketPrefab);
    }
}