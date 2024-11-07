// Item.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;           // アイテムの名前
    public string description;        // アイテムの説明
    public Sprite icon;               // アイテムのアイコン
    public bool isUnique;             // ユニークアイテムかどうか
    public ItemType itemType;         // アイテムの種類（武器、アクセサリー、消耗品など）

    // 視覚的フィードバック用
    public Sprite visualSprite;       // 装備時に表示するスプライト

    // 使用時の動作
    public virtual void Use()
    {
        Debug.Log($"Used {itemName}");
    }
}