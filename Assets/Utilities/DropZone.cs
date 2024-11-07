// DropZone.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public SlotType slotType;  // このスロットで受け入れるアイテムの種類

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        Debug.Log("hello");
        if (draggableItem != null)
        {
            Item item = draggableItem.item;
            Debug.Log($"Dropping item: {item.name}");

            if (slotType == SlotType.Inventory)
            {
                // インベントリにアイテムを戻す処理
                Inventory.Instance.AddItem(item);
                EquipmentManager.Instance.UnEquipItem();
            }
            else if (IsAcceptable(item))
            {
                // 装備スロットにアイテムを装備
                EquipmentManager.Instance.EquipItem(item);
                Inventory.Instance.RemoveItem(item);  // 元のインベントリから削除
            }
        }
    }

    private bool IsAcceptable(Item item)
    {
        return item.itemType == ItemType.Weapon || item.itemType == ItemType.Accessory;
    }
}
