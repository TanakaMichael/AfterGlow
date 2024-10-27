using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SpawnEnemySettings
{
    public  EnemySpawnPattern enemySpawnPattern = new EnemySpawnPattern();
    [Tooltip("Specificを選択時のみ有効")]
    public List<Enemy> enemyList = new List<Enemy>(); // enemySpawnPatternでSpecificを選択した場合
    public bool isEnable = false;
}

