// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject inventoryUI;
    public GameObject equipmentTabContent;
    public GameObject itemsTabContent;

    [Header("Item List")]
    public Transform itemsParent;
    public GameObject inventorySlotPrefab;

    [Header("Context Menu")]
    public GameObject contextMenu;
    public Text itemDetailsText;
    public Button equipButton;
    public Button useButton;
    public Button discardButton;
    public Button viewDetailsButton;

    public Item selectedItem;
    private InventorySlot selectedSlot; // 選択されたスロット

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inventoryUI.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventNames.OnInventoryUpdated, _ => UpdateUI());
    }

    private void OnDisable()
    {
        EventManager.Instance.StopListening(EventNames.OnInventoryUpdated, _ => UpdateUI());
    }

    private void UpdateUI()
    {
        // アイテムリストをクリア
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }

        // アイテムを表示
        foreach (var item in Inventory.Instance.items)
        {
            GameObject slotObject = Instantiate(inventorySlotPrefab, itemsParent);
            InventorySlot slot = slotObject.GetComponent<InventorySlot>();
            slot.Setup(item);
        }

        // 選択状態をリセット
        selectedSlot = null;
        HideContextMenu();
    }

    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        if (inventoryUI.activeSelf)
        {
            UpdateUI();
        }
        else
        {
            HideContextMenu();
        }
    }

    public void ShowEquipmentTab()
    {
        equipmentTabContent.SetActive(true);
        itemsTabContent.SetActive(false);
    }

    public void ShowItemsTab()
    {
        equipmentTabContent.SetActive(false);
        itemsTabContent.SetActive(true);
    }

    // アイテムスロットがクリックされたときに呼ばれるメソッド
    public void OnItemSlotClicked(InventorySlot slot)
    {
        // 以前の選択を解除
        if (selectedSlot != null)
        {
            selectedSlot.Deselect();
        }

        // 新しいスロットを選択
        selectedSlot = slot;
        selectedSlot.Select();

        // コンテキストメニューを表示
        ShowContextMenu(slot.item);
    }

    private void ShowContextMenu(Item item)
    {
        selectedItem = item;
        contextMenu.SetActive(true);

        //// マウスの位置にオフセットを追加して ContextMenu を表示
        //Vector3 mousePosition = Input.mousePosition;
        //Vector3 offsetPosition = new Vector3(mousePosition.x + 15f, mousePosition.y - 10f, 0f); // 適切に調整

        // 画面端の制約を適用
        RectTransform contextRect = contextMenu.GetComponent<RectTransform>();
        float halfWidth = contextRect.rect.width / 2;
        float halfHeight = contextRect.rect.height / 2;

        //float clampedX = Mathf.Clamp(offsetPosition.x, halfWidth, Screen.width - halfWidth);
        //float clampedY = Mathf.Clamp(offsetPosition.y, halfHeight, Screen.height - halfHeight);
        //contextMenu.transform.position = new Vector3(clampedX, clampedY, 0f);

        // アイテムの種類に応じてボタンを表示・非表示にする
        UpdateContextMenuButtons();

        // アイテム詳細テキストを更新
        UpdateItemDetails();
    }

    private void UpdateItemDetails()
    {
        if (itemDetailsText != null && selectedItem != null)
        {
            itemDetailsText.text = selectedItem.description;
        }
    }

    private void UpdateContextMenuButtons()
    {
        // 「使う」ボタンの表示切り替え
        useButton.gameObject.SetActive(selectedItem.itemType == ItemType.Consumable);

        // 「装備」ボタンの表示切り替え
        bool isEquipable = selectedItem.itemType == ItemType.Weapon || selectedItem.itemType == ItemType.Accessory;
        equipButton.gameObject.SetActive(isEquipable);

        // 「捨てる」と「詳細を見る」ボタンは常に表示
        discardButton.gameObject.SetActive(true);
        viewDetailsButton.gameObject.SetActive(true);
    }

    public void OnEquipButtonClicked()
    {
        if (selectedItem != null)
        {
            EquipmentManager.Instance.EquipItem(selectedItem);
            Inventory.Instance.RemoveItem(selectedItem);
            HideContextMenu();
            UpdateUI();
        }
    }

    public void OnUseButtonClicked()
    {
        if (selectedItem != null)
        {
            selectedItem.Use();
            HideContextMenu();
            UpdateUI();
        }
    }

    public void OnDiscardButtonClicked()
    {
        if (selectedItem != null)
        {
            Inventory.Instance.RemoveItem(selectedItem);
            HideContextMenu();
            UpdateUI();
        }
    }

    public void OnViewDetailsButtonClicked()
    {
        // 必要に応じて詳細表示の拡張
        Debug.Log("OnViewDetailsButtonClicked");
        HideContextMenu();
    }

    // コンテキストメニューを隠すメソッド
    public void HideContextMenu()
    {
        contextMenu.SetActive(!contextMenu.activeSelf);
        DetailPanel.Instance.ShowDetails(selectedItem);


        // 選択状態を解除
        if (selectedSlot != null)
        {
            selectedSlot.Deselect();
            selectedSlot = null;
        }
    }
}
