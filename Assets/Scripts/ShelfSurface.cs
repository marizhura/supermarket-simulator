using UnityEngine;

public class ShelfSurface : MonoBehaviour
{
    [Header("Slot points")]
    public Transform[] slotPoints;

    public int SlotCount
    {
        get { return slotPoints != null ? slotPoints.Length : 0; }
    }

    public Transform GetSlot(int index)
    {
        if (slotPoints == null || index < 0 || index >= slotPoints.Length)
            return null;

        return slotPoints[index];
    }
}
