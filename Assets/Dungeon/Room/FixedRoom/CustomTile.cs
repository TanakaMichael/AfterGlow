using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
[CreateAssetMenu(fileName = "newCustomTile", menuName = "Dungeon/Room/CustomTile")]
public class CustomTile : Tile
{
    public bool isWalkable = true;
    public SpawnSpecialObjectSettings spawnSpecialObjectSettings = new SpawnSpecialObjectSettings();
    public SpawnEnemySettings spawnEnemySettings = new SpawnEnemySettings();
    public SpawnNPCSettings spawnNPCSettings = new SpawnNPCSettings();
    public bool isExit = false;
    public bool isEntrance = false;
    public int priority = 0; // 優先度
}
