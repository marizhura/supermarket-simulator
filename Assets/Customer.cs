using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    public Shelf targetShelf;
    public Transform exitPoint; // касса / выход
    public Transform cashPoint;
    public QueueManager queueManager;

    public static Customer WaitingCustomer;

    private NavMeshAgent agent;
    private Animator animator;

    private Item carriedItem;
    private Item placedItem;

    private bool goingToCash = false;
    private bool waitingForCashier = false;

    public void MoveToQueuePoint(Transform point)
    {
        if (point == null) return;

        agent.isStopped = false;
        agent.SetDestination(point.position);
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

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
            Debug.LogWarning("Customer: CashRegister not found");

        if (targetShelf != null)
        {
            agent.SetDestination(targetShelf.transform.position);
            Debug.Log("Customer: going to shelf");
        }
    }

    void Update()
    {
        // 🎬 АНИМАЦИЯ (Idle / Walk)
        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        // 1️⃣ Пришёл к полке
        if (!goingToCash && carriedItem == null && targetShelf != null)
        {
            if (!agent.pathPending && agent.remainingDistance <= 1.2f)
            {
                Debug.Log("Customer: reached shelf");
                TakeItem();
            }
        }

        // 2️⃣ Пришёл к кассе → кладёт товар и ждёт
        if (goingToCash && !waitingForCashier)
        {
            if (!agent.pathPending && agent.remainingDistance <= 1.2f)
            {
                PlaceItemOnCash();
                waitingForCashier = true;
                agent.isStopped = true;
                WaitingCustomer = this;

                Debug.Log("Customer: waiting for cashier");
            }
        }
    }

    void TakeItem()
    {
        Item item = targetShelf.TakeItemFromShelf();
        if (item == null)
        {
            Debug.Log("Customer: no item on shelf");
            return;
        }

        carriedItem = item;

        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.forward * 0.5f;

        Debug.Log("Customer: took item from shelf");

        goingToCash = true;

        if (queueManager != null)
        {
            queueManager.JoinQueue(this);
        }
    }

    void PlaceItemOnCash()
    {
        if (carriedItem == null || placedItem != null)
        {
            Debug.Log("Customer: nothing to place on cash");
            return;
        }

        Transform targetPoint = cashPoint != null ? cashPoint : exitPoint;
        if (targetPoint == null)
        {
            Debug.LogWarning("Customer: no cash point or exit point");
            return;
        }

        placedItem = carriedItem;
        carriedItem = null;

        placedItem.transform.SetParent(targetPoint);
        placedItem.transform.localPosition = Vector3.zero;
        placedItem.transform.localRotation = Quaternion.identity;

        Debug.Log("Customer: placed item on cash");
    }

    // 👇 ИГРОК ЗАБИРАЕТ ТОВАР
    public Item TakeItemFromCustomer()
    {
        if (!waitingForCashier || placedItem == null)
            return null;

        Item item = placedItem;
        placedItem = null;

        return item;
    }

    // 👇 ПОСЛЕ ОПЛАТЫ
    public void Leave()
    {
        if (queueManager != null)
        {
            queueManager.LeaveQueue(this);
        }

        waitingForCashier = false;
        agent.isStopped = false;

        if (WaitingCustomer == this)
            WaitingCustomer = null;

        Destroy(gameObject, 3f);
        Debug.Log("Customer: leaving");
    }

    public bool IsWaiting()
    {
        return waitingForCashier;
    }
}