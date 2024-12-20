using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
[CreateAssetMenu(fileName = "newCustomTile", menuName = "Dungeon/Room/CustomTile")]
public class CustomTile : Tile
{
    public bool isWalkable = true;
    public SpawnSpecialObjectSettings spawnSpecialObjectSettings;
    public SpawnEnemySettings spawnEnemySettings;
    public SpawnNPCSettings spawnNPCSettings;
    public DesignType designType = DesignType.None;
    public DesignCategory designCategory = DesignCategory.None;
    public bool isExit = false;
    public bool isEntrance = false;
    public int priority = 0; // 優先度
    public void SetDesignType(DesignCategory designCategory){
        if(designCategory == DesignCategory.Floor){
            if(Random.value < 0.1f) designType = DesignType.CrackedStoneFloor;
            else designType = DesignType.StoneFloor; 
        }
        else if(designCategory == DesignCategory.Wall){
            if(Random.value < 0.1f) designType = DesignType.CrackedStoneWall;
            else designType = DesignType.StoneWall;
        }
        this.designCategory = designCategory;
    }

}
