using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    // ================= BASKET =================
    [Header("Basket")]
    [SerializeField] private Transform basketHoldPoint;   // точка в руке
    private GameObject basketInstance;
    private bool hasBasket = false;

    private List<Item> basketItems = new List<Item>();

    // ================= TARGETS =================
    [Header("Targets")]
    [SerializeField] private Transform basketStandPoint;  // КУДА ИДЁТ ЗА КОРЗИНОЙ
    [SerializeField] private Shelf targetShelf;
    [SerializeField] public Transform cashPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private QueueManager queueManager;

    public static Customer WaitingCustomer;

    private NavMeshAgent agent;
    private Animator animator;

    // ================= CASH =================
    private List<Item> placedItems = new List<Item>();
    private bool goingToCash = false;
    private bool waitingForCashier = false;

    // ================= DISTANCES =================
    [Header("Distances")]
    public float basketArriveDistance = 1.2f;
    public float shelfArriveDistance = 2f;
    public float cashArriveDistance = 1.2f;
    public float exitArriveDistance = 1.2f;
    public float exitMaxWaitSeconds = 10f;

    private float defaultStoppingDistance;
    private Coroutine leaveRoutine;

    // ================= START =================
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (agent != null)
            defaultStoppingDistance = agent.stoppingDistance;

        // 👉 СНАЧАЛА ИДЁМ ЗА КОРЗИНОЙ
        if (basketStandPoint != null)
        {
            agent.stoppingDistance = basketArriveDistance;
            agent.SetDestination(basketStandPoint.position);
        }
        else if (targetShelf != null)
        {
            agent.stoppingDistance = shelfArriveDistance;
            agent.SetDestination(targetShelf.approachPoint.position);
        }
    }

    // ================= UPDATE =================
    void Update()
    {
        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("HasBasket", hasBasket);

            bool noItemsAtShelf =
                hasBasket &&
                !goingToCash &&
                targetShelf != null &&
                HasReachedDestination(shelfArriveDistance) &&
                !targetShelf.HasItems();

            animator.SetBool("LookAround", waitingForCashier || noItemsAtShelf);
        }

        // У ПОЛКИ
        if (hasBasket && !goingToCash && targetShelf != null && HasReachedDestination(shelfArriveDistance))
        {
            agent.isStopped = true;

            if (targetShelf.HasItems())
                TakeItemFromShelf();
        }

        // К КАССЕ
        if (goingToCash && !waitingForCashier)
        {
            if (IsFirstInQueue())
                MoveToCashPoint();

            if (IsFirstInQueue() && HasReachedDestination(cashArriveDistance))
                PlaceAllItemsOnCash();
        }
    }

    // ================= BASKET PICKUP =================
    public void TakeBasket(GameObject basketPrefab)
    {
        if (hasBasket) return;

        if (basketPrefab == null || basketHoldPoint == null)
        {
            Debug.LogWarning("Customer: basket prefab or hold point missing");
            return;
        }

        basketInstance = Instantiate(
            basketPrefab,
            basketHoldPoint.position,
            basketHoldPoint.rotation,
            basketHoldPoint
        );

        hasBasket = true;

        // 👉 ПОСЛЕ КОРЗИНЫ ИДЁМ К ПОЛКЕ
        if (targetShelf != null)
        {
            agent.isStopped = false;
            agent.stoppingDistance = shelfArriveDistance;
            agent.SetDestination(targetShelf.approachPoint.position);
        }
    }

    // ================= SHELF =================
    void TakeItemFromShelf()
    {
        Item item = targetShelf.TakeItemFromShelf();
        if (item == null)
            return;

        basketItems.Add(item);
        item.gameObject.SetActive(false); // предмет "в корзине"

        goingToCash = true;

        agent.isStopped = false;
        agent.stoppingDistance = cashArriveDistance;

        if (queueManager != null)
            queueManager.JoinQueue(this);
        else
            MoveToCashPoint();
    }

    // ================= CASH =================
    void PlaceAllItemsOnCash()
    {
        if (!IsFirstInQueue() || basketItems.Count == 0)
            return;

        Transform target = cashPoint != null ? cashPoint : exitPoint;

        foreach (Item item in basketItems)
        {
            item.gameObject.SetActive(true);
            item.transform.SetParent(target);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;

            if (item.TryGetComponent(out Collider col))
                col.enabled = true;

            placedItems.Add(item);
        }

        basketItems.Clear();

        waitingForCashier = true;
        WaitingCustomer = this;
        agent.isStopped = true;
    }

    // 👇 КАССИР ЗАБИРАЕТ ПО ОДНОМУ
    public Item TakeItemFromCustomer()
    {
        if (!waitingForCashier || placedItems.Count == 0)
            return null;

        Item item = placedItems[0];
        placedItems.RemoveAt(0);
        return item;
    }

    // ================= LEAVE =================
    public void Leave()
    {
        if (queueManager != null)
            queueManager.LeaveQueue(this);

        waitingForCashier = false;
        goingToCash = false;
        agent.isStopped = false;

        if (WaitingCustomer == this)
            WaitingCustomer = null;

        if (exitPoint != null)
        {
            agent.stoppingDistance = exitArriveDistance;
            agent.SetDestination(exitPoint.position);

            if (leaveRoutine != null)
                StopCoroutine(leaveRoutine);

            leaveRoutine = StartCoroutine(LeaveWhenReachedExit());
        }
        else
        {
            Destroy(gameObject, 3f);
        }
    }

    // ================= QUEUE =================
    public void MoveToQueuePoint(Transform point)
    {
        if (point == null || agent == null)
            return;

        agent.isStopped = false;
        agent.stoppingDistance = defaultStoppingDistance;
        agent.SetDestination(point.position);
    }

    private bool IsFirstInQueue()
    {
        return queueManager == null || queueManager.GetFirstCustomer() == this;
    }

    private bool HasReachedDestination(float distance)
    {
        return agent != null &&
               !agent.pathPending &&
               agent.remainingDistance <= distance;
    }

    private void MoveToCashPoint()
    {
        Transform target = cashPoint != null ? cashPoint : exitPoint;
        if (target == null)
            return;

        agent.isStopped = false;
        agent.stoppingDistance = cashArriveDistance;
        agent.SetDestination(target.position);
    }

    private System.Collections.IEnumerator LeaveWhenReachedExit()
    {
        float t = 0f;
        while (agent != null && !HasReachedDestination(exitArriveDistance) && t < exitMaxWaitSeconds)
        {
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}