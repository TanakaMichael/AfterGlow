using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    public static ItemInfoUI Instance { get; private set; }

    public GameObject itemInfoPanel; // パネル自体
    public Text itemNameText;        // アイテムの名前を表示するテキスト
    public Text itemDescriptionText; // アイテムの説明を表示するテキスト

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // イベントのリスニング設定
        EventManager.Instance.StartListening(EventNames.OnPlayerNearItem, OnPlayerNearItem);
        EventManager.Instance.StartListening(EventNames.OnPlayerExitItem, OnPlayerExitItem);
    }

    private void OnDestroy()
    {
        // イベントのリスニング解除
        EventManager.Instance.StopListening(EventNames.OnPlayerNearItem, OnPlayerNearItem);
        EventManager.Instance.StopListening(EventNames.OnPlayerExitItem, OnPlayerExitItem);
    }

    private void OnPlayerNearItem(object[] parameters)
    {
        if (parameters.Length > 0 && parameters[0] is Item item)
        {
            ShowItemInfo(item);
        }
    }

    private void OnPlayerExitItem(object[] parameters)
    {
        HideItemInfo();
    }

    public void ShowItemInfo(Item item)
    {
        if (item != null)
        {
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;
            itemInfoPanel.SetActive(true);
        }
    }

    public void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
    }
}
