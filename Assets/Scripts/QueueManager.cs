using UnityEngine;
using System.Collections.Generic;

public class QueueManager : MonoBehaviour
{
    public Transform[] queuePoints;

    private List<Customer> customers = new List<Customer>();

    public void JoinQueue(Customer customer)
    {
        if (!customers.Contains(customer))
        {
            customers.Add(customer);
            UpdateQueue();
        }
    }

    public void LeaveQueue(Customer customer)
    {
        if (customers.Contains(customer))
        {
            customers.Remove(customer);
            UpdateQueue();
        }
    }

    void UpdateQueue()
    {
        if (queuePoints == null || queuePoints.Length == 0)
        {
            Debug.LogWarning("QueueManager: no queue points set");
            return;
        }

        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i] != null)
            {
                if (i >= queuePoints.Length)
                {
                    Debug.LogWarning("QueueManager: not enough queue points");
                    break;
                }
                customers[i].MoveToQueuePoint(queuePoints[i]);
            }
        }
    }

    public Customer GetFirstCustomer()
    {
        if (customers.Count == 0)
            return null;

        return customers[0];
    }
}
