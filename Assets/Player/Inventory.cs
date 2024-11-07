// Inventory.cs
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public List<Item> items = new List<Item>(); // 所持アイテムのリスト
    public int maxCapacity = 20;                // インベントリの最大容量

    private void Awake()
    {
        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで存在させる
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(Item item)
    {
        if (items.Count >= maxCapacity)
        {
            Debug.Log("インベントリが満杯です！");
            return false;
        }

        if (item.isUnique && items.Contains(item))
        {
            Debug.Log("このアイテムは既にインベントリに存在します！");
            return false;
        }

        items.Add(item);
        Debug.Log($"アイテムを追加しました: {item.itemName}");
        // イベントを発行
        EventManager.Instance.TriggerEvent(EventNames.OnItemAdded, item);
        EventManager.Instance.TriggerEvent(EventNames.OnInventoryUpdated);
        return true;
    }

    public bool RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
            Debug.Log($"アイテムを削除しました: {item.itemName}");
            // イベントを発行
            EventManager.Instance.TriggerEvent(EventNames.OnItemRemoved, item);
            EventManager.Instance.TriggerEvent(EventNames.OnInventoryUpdated);
            return true;
        }
        Debug.Log("削除しようとしたアイテムはインベントリに存在しません！");
        return false;
    }

    public bool Contains(Item item)
    {
        return items.Contains(item);
    }

    public void SortItemsByName()
    {
        items.Sort((a, b) => a.itemName.CompareTo(b.itemName));
        EventManager.Instance.TriggerEvent(EventNames.OnInventoryUpdated);
    }

    public void SortItemsByType()
    {
        items.Sort((a, b) => a.itemType.CompareTo(b.itemType));
        EventManager.Instance.TriggerEvent(EventNames.OnInventoryUpdated);
    }
}
