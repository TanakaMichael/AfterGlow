// DraggableItem.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;  // ドラッグ中は Raycast を無効化
        transform.SetParent(transform.root);  // 親を一時的に Canvas の直下に移動
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;  // ドロップ後に Raycast を有効化

        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<DropZone>() == null)
        {
            // ドロップ先が無効な場合、元の位置に戻す
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
    }
}
