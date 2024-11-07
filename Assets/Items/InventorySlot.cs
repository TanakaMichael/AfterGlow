// InventorySlot.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Text itemNameText; // アイテム名テキスト
    public Item item;

    // 選択状態の視覚的フィードバック
    private CanvasGroup canvasGroup;
    public GameObject selectionOutline; // 選択枠のオブジェクト

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Setup(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;

        // アイテム名を設定
        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }

        // ドラッグアンドドロップのための設定
        DraggableItem draggableItem = GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            draggableItem = gameObject.AddComponent<DraggableItem>();
        }
        draggableItem.item = item;

        // 初期状態では未選択
        Deselect();
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;

        // アイテム名をクリア
        if (itemNameText != null)
        {
            itemNameText.text = "";
        }

        // 選択状態をリセット
        Deselect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InventoryUI.Instance.OnItemSlotClicked(this);
    }

    // 選択状態を設定するメソッド
    public void Select()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f; // 明るさを通常に戻す
        }
        if (selectionOutline != null)
        {
            selectionOutline.SetActive(true); // 選択枠を表示
        }
    }

    public void Deselect()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f; // 明るさを暗くする
        }
        if (selectionOutline != null)
        {
            selectionOutline.SetActive(false); // 選択枠を非表示
        }
    }
}
