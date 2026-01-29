using UnityEngine;

public class PlayerCashier : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 2f;
    public Camera playerCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryServeCustomer();
        }
    }

    void TryServeCustomer()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerCashier: Camera not assigned");
            return;
        }

        // 🔍 Проверяем, есть ли ожидающий покупатель
        Customer customer = Customer.WaitingCustomer;
        if (customer == null)
        {
            Debug.Log("Cashier: no customer waiting");
            return;
        }

        // 🔍 Проверяем дистанцию до покупателя
        float distance = Vector3.Distance(
            transform.position,
            customer.transform.position
        );

        if (distance > interactDistance)
        {
            Debug.Log("Cashier: customer too far");
            return;
        }

        // 🧾 Забираем товар у покупателя
        Item item = customer.TakeItemFromCustomer();
        if (item == null)
        {
            Debug.Log("Cashier: customer has no item");
            return;
        }

        // 💰 ПРОДАЖА
        Debug.Log("Cashier: item sold");

        Destroy(item.gameObject);

        // 🚶 Покупатель уходит
        customer.Leave();
    }
}