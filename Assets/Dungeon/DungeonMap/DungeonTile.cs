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

    public float lightIntensity = 0f; // タイルの初期の明るさを0に
    public Color accumulatedColor = Color.black; // 累積カラーを黒で初期化
    public float transparency = 1f; // タイルの透過率（1f：完全に透過、0f：完全に遮蔽）
    public GameObject tileGameObject; // タイルの実際のゲームオブジェクト
    public Color initialColor = Color.white; // 初期色を保持

    [Range(0f, 1f)]
    public int priority = 0; // 優先度

    public DungeonTile(int x, int y)
    {
        this.x = x;
        this.y = y;
        // タイルの種類に応じて透明度を設定
        switch (tileType)
        {
            case TileType.Wall:
                transparency = 0.3f; // 壁は光を通しにくくする
                break;
            case TileType.Floor:
                transparency = 0.9f; // 床は光を通しやすくする
                break;
            default:
                transparency = 1f; // その他のタイルの透明度
                break;
        }
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
    // タイルの初期色を設定するメソッド
    public void SetInitialColor()
    {
        if (tileGameObject != null)
        {
            SpriteRenderer sr = tileGameObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                initialColor = sr.color;
            }
        }
    }
}