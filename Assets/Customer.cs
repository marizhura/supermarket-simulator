using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    public Shelf targetShelf;
    public Transform exitPoint; // касса / выход

    private NavMeshAgent agent;
    private Item carriedItem;
    private bool goingToCash = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (targetShelf != null)
        {
            agent.SetDestination(targetShelf.transform.position);
        }
    }

    void Update()
    {
        // Пришёл к полке и ещё не взял товар
        if (!goingToCash && carriedItem == null && targetShelf != null)
        {
            if (!agent.pathPending && agent.remainingDistance <= 1.2f)
            {
                TakeItem();
            }
        }
    }

    void TakeItem()
    {
        Item item = targetShelf.TakeItemFromShelf();
        if (item == null)
            return;

        carriedItem = item;

        // Берёт товар
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.forward * 0.5f;

        // 🔥 ИДЁТ К КАССЕ
        if (exitPoint != null)
        {
            goingToCash = true;
            agent.SetDestination(exitPoint.position);
        }
    }

    // Касса забирает товар
    public Item GetItem()
    {
        return carriedItem;
    }

    // Касса вызывает этот метод
    public void PayAndLeave()
    {
        Destroy(gameObject, 2f);
    }
}