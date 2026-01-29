using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public int money = 100; // стартовые деньги

    public bool CanAfford(int price)
    {
        return money >= price;
    }

    public void SpendMoney(int price)
    {
        money -= price;
        Debug.Log("💸 Потрачено: " + price + " | Осталось: " + money);
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("💰 Получено: " + amount + " | Теперь: " + money);
    }
}