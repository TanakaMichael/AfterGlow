// EquipmentManager.cs
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    // 装備スロットの定義
    public EquipmentSlot weaponSlot;
    public EquipmentSlot accessorySlot1;
    public EquipmentSlot accessorySlot2;
    public Button EquipBtn;
    // 必要に応じてスロットを追加

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        EquipBtn.onClick.AddListener(() => EquipItem(null));
    }



    /// <summary>
    /// アイテムを装備する
    /// </summary>
    /// <param name="item">装備するアイテム</param>
    public void EquipItem(Item item)
    {
        if (item == null){
            if (InventoryUI.Instance.selectedItem == null){
                return;
            }
            item = InventoryUI.Instance.selectedItem;
        } 
        else if (item != null) return;

        switch (item.itemType)
        {
            case ItemType.Weapon:
                EquipWeapon(item as Weapon);
                break;
            case ItemType.Accessory:
                EquipAccessory(item as AccessoryItem);
                break;
            // 他のアイテムタイプに対応する場合は追加
            default:
                Debug.LogWarning($"Cannot equip item of type {item.itemType}");
                break;
        }
    }



    /// <summary>
    /// アイテムを装備する
    /// </summary>
    /// <param name="item">装備するアイテム</param>
    public void UnEquipItem()
    {
        if (weaponSlot.equippedItem != null){
            weaponSlot.Unequip();
            PlayerStats.Instance.ResetStats();
        }
        else if (accessorySlot1.equippedItem != null){
            accessorySlot1.Unequip();
            PlayerStats.Instance.ResetStats();
        }
        else if (accessorySlot2.equippedItem!= null){
            accessorySlot2.Unequip();
            PlayerStats.Instance.ResetStats();
        }
    }
    /// <summary>
    /// 武器を装備する
    /// </summary>
    /// <param name="weapon">装備する武器</param>
    public void EquipWeapon(Weapon weapon)
    {
        if (weapon == null) return;

        if (weaponSlot.equippedItem != null)
        {
            // 既に武器が装備されている場合、インベントリに戻す
            Debug.Log($"Unequipping current weapon: {weaponSlot.equippedItem.itemName}");
            // Inventory.Instance.AddItem(weaponSlot.equippedItem);
            // ステータスを解除
            ApplyWeaponStats(weaponSlot.equippedItem as Weapon, false);
            // 特殊能力を解除
            UnequipSpecialAbility(weaponSlot.equippedItem);
            // スロットを空に
            weaponSlot.Unequip();
        }

        // 新しい武器を装備
        weaponSlot.Equip(weapon);
        Debug.Log($"Equipped weapon: {weapon.itemName}");

        // ステータスを適用
        ApplyWeaponStats(weapon, true);
        // 特殊能力を装備
        EquipSpecialAbility(weapon);
        // 視覚的な更新
        PlayerVisualManager.Instance.UpdateVisuals();

        // イベントの発行
        EventManager.Instance.TriggerEvent(EventNames.OnItemEquipped, weapon);
    }

    /// <summary>
    /// アクセサリーを装備する
    /// </summary>
    /// <param name="accessory">装備するアクセサリー</param>
    public void EquipAccessory(AccessoryItem accessory)
    {
        if (accessory == null) return;

        // まず空きスロットを探す
        if (accessorySlot1.equippedItem == null)
        {
            accessorySlot1.Equip(accessory);
            Debug.Log($"Equipped accessory: {accessory.itemName} to slot 1");
        }
        else if (accessorySlot2.equippedItem == null)
        {
            accessorySlot2.Equip(accessory);
            Debug.Log($"Equipped accessory: {accessory.itemName} to slot 2");
        }
        else
        {
            Debug.LogWarning("All accessory slots are already equipped.");
            return;
        }

        // ステータスを適用
        ApplyAccessoryStats(accessory, true);
        // 特殊能力を装備
        EquipSpecialAbility(accessory);
        // 視覚的な更新
        PlayerVisualManager.Instance.UpdateVisuals();

        // イベントの発行
        EventManager.Instance.TriggerEvent(EventNames.OnItemEquipped, accessory);
    }

    /// <summary>
    /// 武器のステータスを適用・解除する
    /// </summary>
    private void ApplyWeaponStats(Weapon weapon, bool isEquipping)
    {
        if (weapon == null) return;

        PlayerStats stats = PlayerStats.Instance;
        if (stats != null)
        {
            if (isEquipping)
            {
                weapon.SetStatus();
                Debug.Log($"Applied weapon stats: +{weapon.attackPower} Attack Power, +{weapon.attackSpeed} Attack Speed");
            }
            else
            {
                weapon.SetStatus();
                Debug.Log($"Removed weapon stats: -{weapon.attackPower} Attack Power, -{weapon.attackSpeed} Attack Speed");
            }
        }
    }

    /// <summary>
    /// アクセサリーのステータスを適用・解除する
    /// </summary>
    private void ApplyAccessoryStats(AccessoryItem accessory, bool isEquipping)
    {
        if (accessory == null) return;

        PlayerStats stats = PlayerStats.Instance;
        if (stats != null)
        {
            if (isEquipping)
            {
                // アクセサリーのステータスを追加
                accessory.SetStatus();
                Debug.Log($"Applied accessory stats: +{accessory.defense} Defense");
            }
            else
            {
                // アクセサリーのステータスを削除
                accessory.SetStatus();
                Debug.Log($"Removed accessory stats: -{accessory.defense} Defense");
            }
        }
    }

    /// <summary>
    /// 特殊能力を装備する
    /// </summary>
    private void EquipSpecialAbility(Item item)
    {
        if (item == null) return;

        if (item is Weapon weapon && weapon.abilityPrefab != null)
        {
            GameObject abilityInstance = Instantiate(weapon.abilityPrefab, PlayerStats.Instance.transform);
            ISpecialAbility ability = abilityInstance.GetComponent<ISpecialAbility>();
            if (ability != null)
            {
                ability.Activate();
                Debug.Log($"Activated special ability for weapon: {weapon.abilityPrefab.name}");
            }
        }
        else if (item is AccessoryItem accessory && accessory.abilityPrefab != null)
        {
            GameObject abilityInstance = Instantiate(accessory.abilityPrefab, PlayerStats.Instance.transform);
            ISpecialAbility ability = abilityInstance.GetComponent<ISpecialAbility>();
            if (ability != null)
            {
                ability.Activate();
                Debug.Log($"Activated special ability for accessory: {accessory.abilityPrefab.name}");
            }
        }
    }

    /// <summary>
    /// 特殊能力を装備解除する
    /// </summary>
    private void UnequipSpecialAbility(Item item)
    {
        if (item == null) return;

        // 装備解除時に特殊能力を無効化
        foreach (Transform child in PlayerStats.Instance.transform)
        {
            ISpecialAbility ability = child.GetComponent<ISpecialAbility>();
            if (ability != null)
            {
                ability.Deactivate();
                Destroy(child.gameObject);
                Debug.Log($"Deactivated special ability: {ability.GetType().Name}");
            }
        }
    }

    public void OnClickEquipment(){
        EquipItem(null);
    }
}
