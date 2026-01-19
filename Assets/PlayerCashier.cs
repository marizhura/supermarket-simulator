using UnityEngine;

public class PlayerCashier : MonoBehaviour
{
    public float interactDistance = 2f;
    public Camera playerCamera;
    public Transform cashPoint;
    public PlayerMoney playerMoney;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryServeCustomer();
        }
    }

    void TryServeCustomer()
    {
        if (cashPoint == null)
        {
            GameObject cashRegister = GameObject.Find("CashRegister");
            if (cashRegister != null)
            {
                Transform point = cashRegister.transform.Find("CashPoint");
                cashPoint = point != null ? point : cashRegister.transform;
            }
        }
        if (cashPoint == null)
        {
            Debug.LogWarning("Cashier: cash point not found");
            return;
        }
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, interactDistance))
        {
            Debug.Log("Cashier: nothing to interact with");
            return;
        }

        Item item = hit.collider.GetComponent<Item>();
        if (item == null)
        {
            Debug.Log("Cashier: no item in sight");
            return;
        }

        if (!item.transform.IsChildOf(cashPoint))
        {
            Debug.Log("Cashier: item is not on cash");
            return;
        }

        if (playerMoney == null)
            playerMoney = FindObjectOfType<PlayerMoney>();

        if (playerMoney == null)
        {
            Debug.LogWarning("Cashier: PlayerMoney not found");
            return;
        }

        playerMoney.AddMoney(item.sellPrice);
        Destroy(item.gameObject);

        Debug.Log("Cashier: item removed from cash");
        if (Customer.WaitingCustomer != null && Customer.WaitingCustomer.IsWaiting())
        {
            Customer.WaitingCustomer.Leave();
            Debug.Log("Cashier served customer");
        }
    }
}
