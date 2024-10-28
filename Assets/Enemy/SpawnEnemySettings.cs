using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new SpawnSpecialObject", menuName = "Dungeon/Room/Spawn/Enemy")]
public class SpawnEnemySettings : ScriptableObject
{
    public  EnemySpawnPattern enemySpawnPattern = new EnemySpawnPattern();
    [Tooltip("Specificを選択時のみ有効")]
    public List<Enemy> enemyList = new List<Enemy>(); // enemySpawnPatternでSpecificを選択した場合
    public bool isEnable = false;
}

