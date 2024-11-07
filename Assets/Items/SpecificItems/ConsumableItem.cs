// ConsumableItem.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : Item
{
    public int restoreAmount;         // 回復量

    public override void Use()
    {
        base.Use();
        Consume();
    }

    private void Consume()
    {
        Debug.Log($"{itemName}を消費しました。回復量: {restoreAmount}");
        PlayerStats.Instance.RestoreHealth(restoreAmount);
        Inventory.Instance.RemoveItem(this);
    }
}
