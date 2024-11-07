// PlayerStats.cs
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public int baseAttackPower = 10;
    public int baseDefense = 10;
    public float baseAttackSpeed = 1.0f;
    public int maxHealth = 100;
    public int currentHealth;

    public int attackPower;
    public float attackSpeed;
    public int defense;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStats()
    {
        attackPower = baseAttackPower;
        attackSpeed = baseAttackSpeed;
        currentHealth = maxHealth;
    }
    public void ResetStats(){
        InitializeStats();
    }

    /// <summary>
    /// ヘルスを回復する
    /// </summary>
    /// <param name="amount">回復量</param>
    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        Debug.Log($"Player restored {amount} health. Current Health: {currentHealth}");
        // ヘルスUIを更新するイベントを発行可能
    }

    /// <summary>
    /// ヘルスを減少させる
    /// </summary>
    /// <param name="amount">減少量</param>
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;
        Debug.Log($"Player took {amount} damage. Current Health: {currentHealth}");
        // ダメージUIを更新するイベントを発行可能
    }
}
