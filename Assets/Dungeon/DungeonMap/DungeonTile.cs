using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile
{
    public int x, y; // タイルの座標
    public bool isWalkable = false;
    public TileType tileType = TileType.Empty;
    public bool isEntrance = false;
    public bool isExit = false;
    public Room room; // このタイルが属する部屋（廊下の場合は null）


    // スポーン情報
    public SpawnSpecialObjectSettings spawnSpecialObjectSettings;
    public SpawnEnemySettings spawnEnemySettings;
    public SpawnNPCSettings spawnNPCSettings;

    // デザイン情報
    public DesignType designType = DesignType.None;
    public DesignCategory designCategory = DesignCategory.None;

    [Range(0f, 1f)]
    public int priority = 0; // 優先度

    public DungeonTile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // デザインタイプを設定するメソッド
    public void SetDesignType(DesignCategory designCategory)
    {
        this.designCategory = designCategory;

        switch (designCategory)
        {
            case DesignCategory.Floor:
                if (Random.value < 0.1f)
                    designType = DesignType.CrackedStoneFloor;
                else
                    designType = DesignType.StoneFloor;
                break;
            case DesignCategory.Wall:
                if (Random.value < 0.1f)
                    designType = DesignType.CrackedStoneWall;
                else
                    designType = DesignType.StoneWall;
                break;
            case DesignCategory.Corridor:
                if (Random.value < 0.1f)
                    designType = DesignType.CrackedStoneCorridor;
                else
                    designType = DesignType.StoneCorridor;
                break;
            case DesignCategory.SecretCorridor:
                designType = DesignType.SecretPassage;
                break;
            // その他のカテゴリ
            default:
                designType = DesignType.None;
                break;
        }
    }
}