// Weapon.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon")]
public class Weapon : Item
{
    public int attackPower;           // 攻撃力
    public float attackSpeed;         // 攻撃速度
    public GameObject abilityPrefab;  // 特殊能力のプレハブ（オプション）
    public GameObject effectPrefab;   // エフェクトのプレハブ

    public override void Use()
    {
        base.Use();
        EquipmentManager.Instance.EquipWeapon(this);
    }
    public void SetStatus(){
        // 武器のステータスを設定する
        PlayerStats.Instance.attackSpeed += attackSpeed;
        PlayerStats.Instance.attackPower += attackPower;
    }
}
