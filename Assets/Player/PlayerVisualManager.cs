// PlayerVisualManager.cs
using UnityEngine;

public class PlayerVisualManager : MonoBehaviour
{
    public static PlayerVisualManager Instance { get; private set; }

    // プレイヤーのベーススプライト
    public SpriteRenderer baseSpriteRenderer;

    // 装備アイテムごとのスプライトレンダラー
    public SpriteRenderer weaponSpriteRenderer;
    public SpriteRenderer accessory1SpriteRenderer;
    public SpriteRenderer accessory2SpriteRenderer;
    // 必要に応じて追加

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

    /// <summary>
    /// プレイヤーの見た目を更新する
    /// </summary>
    public void UpdateVisuals()
    {
        // 武器の見た目を更新
        if (EquipmentManager.Instance.weaponSlot.equippedItem != null)
        {
            Debug.Log($"Updated weapon slot: {EquipmentManager.Instance.weaponSlot.equippedItem}");
            Debug.Log($"Updated weapon slot: {EquipmentManager.Instance.weaponSlot.equippedItem.visualSprite}");
            weaponSpriteRenderer.sprite = EquipmentManager.Instance.weaponSlot.equippedItem.visualSprite;
            weaponSpriteRenderer.enabled = true;
        }
        else
        {
            weaponSpriteRenderer.sprite = null;
            weaponSpriteRenderer.enabled = false;
        }

        // アクセサリー1の見た目を更新
        if (EquipmentManager.Instance.accessorySlot1.equippedItem != null)
        {
            accessory1SpriteRenderer.sprite = EquipmentManager.Instance.accessorySlot1.equippedItem.visualSprite;
            accessory1SpriteRenderer.enabled = true;
        }
        else
        {
            accessory1SpriteRenderer.sprite = null;
            accessory1SpriteRenderer.enabled = false;
        }

        // アクセサリー2の見た目を更新
        if (EquipmentManager.Instance.accessorySlot2.equippedItem != null)
        {
            accessory2SpriteRenderer.sprite = EquipmentManager.Instance.accessorySlot2.equippedItem.visualSprite;
            accessory2SpriteRenderer.enabled = true;
        }
        else
        {
            accessory2SpriteRenderer.sprite = null;
            accessory2SpriteRenderer.enabled = false;
        }

        // 他の装備スロットも同様に更新
    }
}
