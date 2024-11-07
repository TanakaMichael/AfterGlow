using UnityEngine;

public class FlameAbility : MonoBehaviour, ISpecialAbility
{
    public float duration = 5f;
    public float damagePerSecond = 10f;

    private bool isActive = false;
    private float timer = 0f;

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            timer = duration;
            Debug.Log("Flame Ability Activated!");
            // 火炎エフェクトの開始やダメージ処理の開始
        }
    }

    public void Deactivate()
    {
        if (isActive)
        {
            isActive = false;
            Debug.Log("Flame Ability Deactivated!");
            // 火炎エフェクトの停止やダメージ処理の停止
        }
    }

    private void Update()
    {
        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Deactivate();
            }

            // ダメージ処理の例
            // ApplyFlameDamage();
        }
    }

    private void ApplyFlameDamage()
    {
        // 火炎ダメージの適用ロジック
    }
}
