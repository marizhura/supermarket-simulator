using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Info")]
    public string itemId;            // "banana", "apple", "watermelon"
    public string displayName;        // Банан, Яблоко, Арбуз

    [Header("Size & Price")]
    public ItemSize size = ItemSize.Small;
    public int price = 1;

    [Header("Runtime")]
    public bool isPicked = false;     // взят игроком
    public bool isInShelf = false;    // лежит на полке
    public bool isInBasket = false;   // у покупателя

    // вызывается, когда предмет кладут в полку
    public void OnPlacedToShelf()
    {
        isInShelf = true;
        isPicked = false;
    }

    // вызывается, когда игрок берет предмет
    public void OnPickedUp()
    {
        isPicked = true;
        isInShelf = false;
    }

    // когда покупатель взял с полки
    public void OnTakenByCustomer()
    {
        isInBasket = true;
        isInShelf = false;
    }
}