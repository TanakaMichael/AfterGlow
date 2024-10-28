using UnityEngine;

[System.Serializable]
public class EntranceExitSettings
{
    [Range(0, 10)]
    public int minCount = 1; // 最小数
    [Range(0, 10)]
    public int maxCount = 2; // 最大数

    public SpawnPosition spawnPosition = SpawnPosition.Random; // 配置場所
}
