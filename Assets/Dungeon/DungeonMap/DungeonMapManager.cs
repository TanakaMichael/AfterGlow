using System.Collections.Generic;
using UnityEngine;

public class DungeonMapManager : MonoBehaviour
{
    public int mapWidth = 100; // マップの幅
    public int mapHeight = 100; // マップの高さ

    public List<List<DungeonTile>> dungeonTiles; // ダンジョンマップの二次元リスト

    public RoomManager roomManager;
    public CorridorManager corridorManager;

    public void GenerateDungeonMap()
    {
        // マップの初期化
        dungeonTiles = new List<List<DungeonTile>>();
        for (int x = 0; x < mapWidth; x++)
        {
            List<DungeonTile> column = new List<DungeonTile>();
            for (int y = 0; y < mapHeight; y++)
            {
                DungeonTile tile = new DungeonTile(x, y);
                column.Add(tile);
                // タイルの初期色を設定
                if (tile.tileGameObject != null)
                {
                    SpriteRenderer sr = tile.tileGameObject.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = tile.initialColor;
                    }
                }
            }
            dungeonTiles.Add(column);
        }

        // 部屋のタイルをマップに適用
        foreach (Room room in roomManager.rooms)
        {
            ApplyRoomToMap(room);
        }

        // 廊下のタイルをマップに適用
        foreach (Corridor corridor in corridorManager.corridors)
        {
            ApplyCorridorToMap(corridor);
        }
        foreach (var column in dungeonTiles)
        {
            foreach (var tile in column)
            {
                tile.SetInitialColor(); // タイルの初期色を保存
            }
        }

        // 壁の分類を実行
        ClassifyWalls();
    }

    private void ApplyRoomToMap(Room room)
    {
        for (int rowIndex = 0; rowIndex < room.tiles.Count; rowIndex++)
        {
            List<TileData> row = room.tiles[rowIndex];
            for (int colIndex = 0; colIndex < row.Count; colIndex++)
            {
                TileData tileData = row[colIndex];
                int x = room.x + colIndex;
                int y = room.y + rowIndex;

                if (IsWithinBounds(x, y))
                {
                    DungeonTile dungeonTile = dungeonTiles[x][y];

                    // TileData の情報を Tile にコピー
                    dungeonTile.isWalkable = tileData.isWalkable;
                    dungeonTile.isEntrance = tileData.isEntrance;
                    dungeonTile.isExit = tileData.isExit;
                    dungeonTile.priority = tileData.priority;
                    dungeonTile.spawnSpecialObjectSettings = tileData.spawnSpecialObjectSettings;
                    dungeonTile.spawnEnemySettings = tileData.spawnEnemySettings;
                    dungeonTile.spawnNPCSettings = tileData.spawnNPCSettings;
                    dungeonTile.designType = tileData.designType;
                    dungeonTile.designCategory = tileData.designCategory;
                    dungeonTile.room = room;

                    // タイルタイプを設定
                    dungeonTile.tileType = GetTileTypeFromDesignCategory(tileData.designCategory);
                }
            }
        }
    }

    private void ApplyCorridorToMap(Corridor corridor)
    {
        foreach (TileData tileData in corridor.tiles)
        {
            int x = tileData.x;
            int y = tileData.y;

            if (IsWithinBounds(x, y))
            {
                DungeonTile dungeonTile = dungeonTiles[x][y];

                // 既存のタイルが床であり、部屋に属している場合は上書きしない
                if (dungeonTile.tileType == TileType.Floor && dungeonTile.room != null)
                {
                    continue; // 上書きせず次のタイルへ
                }

                // TileData の情報を Tile にコピー
                dungeonTile.isWalkable = tileData.isWalkable;
                dungeonTile.isEntrance = tileData.isEntrance;
                dungeonTile.isExit = tileData.isExit;
                dungeonTile.priority = tileData.priority;
                dungeonTile.spawnSpecialObjectSettings = tileData.spawnSpecialObjectSettings;
                dungeonTile.spawnEnemySettings = tileData.spawnEnemySettings;
                dungeonTile.spawnNPCSettings = tileData.spawnNPCSettings;
                dungeonTile.designType = tileData.designType;
                dungeonTile.designCategory = tileData.designCategory;
                dungeonTile.room = null; // 廊下は特定の部屋に属さない

                // タイルタイプを設定
                dungeonTile.tileType = corridor.isSecret ? TileType.SecretCorridor : TileType.Corridor;
            }
        }
    }

    private TileType GetTileTypeFromDesignCategory(DesignCategory designCategory)
    {
        switch (designCategory)
        {
            case DesignCategory.Floor:
                return TileType.Floor;
            case DesignCategory.Wall:
                return TileType.Wall;
            case DesignCategory.Corridor:
                return TileType.Corridor;
            case DesignCategory.SecretCorridor:
                return TileType.SecretCorridor;
            default:
                return TileType.Empty;
        }
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
    }

    // 壁の分類を行うメソッドを追加
    private void ClassifyWalls()
    {
        for (int x = 0; x < dungeonTiles.Count; x++)
        {
            for (int y = 0; y < dungeonTiles[x].Count; y++)
            {
                DungeonTile tile = dungeonTiles[x][y];

                // 壁タイルのみを対象
                if (tile.tileType == TileType.Wall)
                {
                    // 上下のタイルを取得
                    DungeonTile tileAbove = GetTile(x, y + 1);
                    DungeonTile tileBelow = GetTile(x, y - 1);

                    // 床が下にあり、上が空のタイルの場合
                    //if (tileBelow != null && tileBelow.tileType == TileType.Floor &&
                    //    (tileAbove == null || tileAbove.tileType == TileType.Empty))
                    //{
                        // デザインタイプを外部の壁に変更
                    //    tile.designType = DesignType.OuterWall;
                    //}
                    if(tileBelow != null && tileBelow.tileType == TileType.Floor){
                        tile.designType = DesignType.OuterWall;
                    }
                    else
                    {
                        // デザインタイプを内部の壁に設定（必要に応じて変更）
                        tile.designType = DesignType.StoneWall; // または他の内部壁のデザインタイプ
                    }
                }
            }
        }
    }

    // 指定座標のタイルを取得するヘルパーメソッド
    private DungeonTile GetTile(int x, int y)
    {
        if (IsWithinBounds(x, y))
        {
            return dungeonTiles[x][y];
        }
        return null;
    }
}
