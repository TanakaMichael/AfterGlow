// DetailPanel.cs
using UnityEngine;
using UnityEngine.UI;

public class DetailPanel : MonoBehaviour
{
    public static DetailPanel Instance { get; private set; }

    public GameObject detailPanel;
    public Text itemNameText;
    public Text itemDescriptionText;
    public Text itemStatsText; // アイテムのステータス表示用テキスト

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            detailPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowDetails(Item item)
    {
        if (item != null)
        {
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;

            // アイテムのタイプに応じてステータスを表示
            if (item is Weapon weapon)
            {
                itemStatsText.text = $"攻撃力: {weapon.attackPower}\n攻撃速度: {weapon.attackSpeed}";
            }
            else if (item is AccessoryItem accessory)
            {
                itemStatsText.text = $"防御力: {accessory.defense}\n特殊能力: {accessory.specialAbility}";
            }
            else
            {
                itemStatsText.text = "ステータス情報はありません。";
            }

            detailPanel.SetActive(true);
        }
    }

    public void HideDetails()
    {
        detailPanel.SetActive(false);
    }
}
