// AccessoryItem.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewAccessory", menuName = "Inventory/Accessory")]
public class AccessoryItem : Item
{
    public int defense; // Defenseフィールドを追加
    public string specialAbility;     // 特殊能力の説明
    public GameObject abilityPrefab;  // 特殊能力のプレハブ（オプション）

    public override void Use()
    {
        base.Use();
        EquipmentManager.Instance.EquipAccessory(this);
    }
    public void SetStatus(){
        // 特殊能力の発動を追加
        PlayerStats.Instance.defense += defense;
    }
}
