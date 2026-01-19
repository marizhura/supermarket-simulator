using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    public Transform targetShelf;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (targetShelf != null)
        {
            agent.SetDestination(targetShelf.position);
        }
    }
}
