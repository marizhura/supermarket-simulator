using UnityEngine;

public class CashRegister : MonoBehaviour
{
    public PlayerMoney playerMoney;

    void OnTriggerEnter(Collider other)
    {
        Customer customer = other.GetComponent<Customer>();
        if (customer == null) return;

        Item item = customer.GetItem();
        if (item == null) return;

        playerMoney.AddMoney(item.sellPrice);

        Destroy(item.gameObject);
        customer.PayAndLeave();
    }
}