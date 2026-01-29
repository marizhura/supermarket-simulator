using UnityEngine;

public class ShelfTester : MonoBehaviour
{
    public FruitShelf shelf;
    public Item testItem;

    void Start()
    {
        shelf.AddItemToShelf(testItem);
    }
}
