using UnityEngine;

[System.Serializable]
public class SpawnSettings
{
    [Range(0f, 1f)]
    public float spawnProbability = 1f; // 出現確率

    public int minSpawnCount = 1; // 最小出現数
    public int maxSpawnCount = 3; // 最大出現数

    public SpawnPosition spawnPosition = SpawnPosition.Random; // 配置場所

    // それぞれの設定
    public SpawnSpecialObjectSettings specialObjectSettings;
    public SpawnEnemySettings enemySettings;
    public SpawnNPCSettings npcSettings;
}

