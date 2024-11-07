using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldItem : MonoBehaviour
{
    public Item item;                 // このWorldItemが表すアイテム
    public float pickupRadius = 2f;   // プレイヤーが近づく半径
    public KeyCode pickupKey = KeyCode.E; // アイテムを拾うためのキー

    private static List<WorldItem> nearbyItems = new List<WorldItem>(); // 近くのWorldItemを管理

    private void Start()
    {
        // コライダーをトリガーに設定
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }

    private void Update()
    {
        // プレイヤーとの距離をチェック
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= pickupRadius)
            {
                // 近くにいる場合、リストに追加（重複防止）
                if (!nearbyItems.Contains(this))
                {
                    nearbyItems.Add(this);
                }

                // リスト内で最も近いアイテムのみ表示
                UpdateItemInfoUI();

                // Eキーでアイテムを拾う
                if (Input.GetKeyDown(pickupKey) && IsClosestItem())
                {
                    PickupItem();
                }
            }
            else
            {
                // 距離外の場合、リストから削除
                if (nearbyItems.Contains(this))
                {
                    nearbyItems.Remove(this);
                }

                // アイテムが範囲外に出たときに、UIを更新
                UpdateItemInfoUI();
            }
        }
    }

    private bool IsClosestItem()
    {
        // プレイヤーに最も近いアイテムかどうかを確認
        return nearbyItems.Count > 0 && nearbyItems[0] == this;
    }

    private static void UpdateItemInfoUI()
    {
        if (nearbyItems.Count > 0)
        {
            // 距離順に並び替え
            nearbyItems.Sort((a, b) =>
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player == null) return 0;
                float distanceA = Vector3.Distance(a.transform.position, player.transform.position);
                float distanceB = Vector3.Distance(b.transform.position, player.transform.position);
                return distanceA.CompareTo(distanceB);
            });

            // 一番近いアイテムのみ情報を表示
            ItemInfoUI.Instance.ShowItemInfo(nearbyItems[0].item);
        }
        else
        {
            // アイテムが範囲外なら非表示
            ItemInfoUI.Instance.HideItemInfo();
        }
    }

    private void PickupItem()
    {
        bool success = Inventory.Instance.AddItem(item);
        if (success)
        {
            // アイテムをマップから削除
            nearbyItems.Remove(this);
            Destroy(gameObject);
            // アイテム拾得イベントを発行
            EventManager.Instance.TriggerEvent(EventNames.OnItemPickedUp, item);
            UpdateItemInfoUI(); // UIを更新
        }
    }
}
