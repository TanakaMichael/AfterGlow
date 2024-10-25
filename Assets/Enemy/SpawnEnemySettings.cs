using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SpawnEnemySettings", menuName = "Dungeon/Enemies/SpawnSettings")]
public class SpawnEnemySettings : ScriptableObject
{
    public List<Enemy> enemies = new List<Enemy>();

    public Enemy GetEnemy(){
        // 総優先度を計算
        float totalPriority = 0f;
        foreach (var enemy in enemies)
        {
            totalPriority += enemy.priority;
        }

        // ランダムなポイントを選択
        float randomPoint = Random.value * totalPriority;

        // ランダムポイントに基づいて敵を選択
        foreach (var enemy in enemies)
        {
            if (randomPoint < enemy.priority)
            {
                return enemy;
            }
            randomPoint -= enemy.priority;
        }

        // 万が一選択に失敗した場合、最後の敵を返す
        return enemies[enemies.Count - 1];
    }
}

