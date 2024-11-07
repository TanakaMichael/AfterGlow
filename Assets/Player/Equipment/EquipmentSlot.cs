// EquipmentSlot.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item equippedItem;
    public Image itemIcon;
    public Image backgroundIcon; // 背景画像の参照
    private CanvasGroup canvasGroup;

    public SlotType slotType; // このスロットのアイテムタイプ（Weapon, Accessoryなど）

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {

    }

    public void Equip(Item item)
    {
        equippedItem = item;
        if(itemIcon == null){
            GameObject newImageObject = new GameObject("Image");
            newImageObject.transform.SetParent(this.transform); // 親オブジェクトを設定
            itemIcon = newImageObject.AddComponent<Image>();
            // RectTransformの位置を親と同じ位置に設定
            RectTransform rectTransform = itemIcon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero; // 親と同じ位置に配置
            rectTransform.localPosition = new Vector3(-50f, 50f); // ローカル位置をリセット
            rectTransform.sizeDelta = new Vector2(80, 80); // 親のサイズに合わせる
            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
        itemIcon.sprite = item.icon;
        UpdateUI();
        UpdatePlayerVisual();
    }

    public void Unequip()
    {
        if (equippedItem != null)
        {
            Debug.Log($"{equippedItem.itemName} を装備解除しました。");
            Item itemToUnequip = equippedItem;
            equippedItem = null;
            UpdateUI();
            UpdatePlayerVisual();
            // Inventory.Instance.AddItem(itemToUnequip); DraggableItem.csに同じ記述あり
            EventManager.Instance.TriggerEvent(EventNames.OnItemRemoved, itemToUnequip);
        }
    }

    private void UpdateUI()
    {
        if (equippedItem != null)
        {
            itemIcon.sprite = equippedItem.icon;
            itemIcon.enabled = true;
            // 背景画像は常に表示されるため、変更しません
        }
        else if(itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            // 背景画像は常に表示されるため、変更しません
        }
    }

    private void UpdatePlayerVisual()
    {
        PlayerVisualManager.Instance.UpdateVisuals();
    }

    public void SetEffect(GameObject effect)
    {
        // エフェクト設定の実装（必要に応じて）
    }

    public void RemoveEffect()
    {
        // エフェクト削除の実装（必要に応じて）
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggableItem != null && IsAcceptable(draggableItem.item))
        {
            if(equippedItem != null){
                Inventory.Instance.AddItem(equippedItem);
    
                // 新しいアイテムを装備
                Equip(draggableItem.item);
                // インベントリからアイテムを削除
                Inventory.Instance.RemoveItem(draggableItem.item);
            }
        }
    }

    private bool IsAcceptable(Item item)
    {
        return (slotType == SlotType.Weapon && item.itemType == ItemType.Weapon) ||
               (slotType == SlotType.Accessory && item.itemType == ItemType.Accessory);
    }

    // マウスがスロットに入ったときのハイライト
    public void OnPointerEnter(PointerEventData eventData)
    {
        // アイテムがドロップ可能かどうかをチェック
        DraggableItem draggableItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggableItem != null && IsAcceptable(draggableItem.item))
        {
            backgroundIcon.color = Color.green; // 受け入れ可能な場合は緑色にハイライト
        }
    }

    // マウスがスロットから離れたときのハイライト解除
    public void OnPointerExit(PointerEventData eventData)
    {
        backgroundIcon.color = Color.white; // デフォルト色に戻す
    }
}
