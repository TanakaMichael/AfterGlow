// DungeonMapManager.cs
using System.Collections.Generic;
using UnityEngine;

public class DungeonMapManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 100; // マップの幅
    public int mapHeight = 100; // マップの高さ

    [Header("Tile Prefabs")]
    public List<TileSet> tileSets; // 利用可能なタイルセットのリスト
    public int selectedTileSetIndex = 0; // 選択されたタイルセットのインデックス

    [Header("References")]
    public RoomManager roomManager;
    public CorridorManager corridorManager;
    public LightingManager lightingManager; // LightingManager への参照

    public GameObject tilePrefab; // デフォルトのタイルプレハブ
    public GameObject lightSourcePrefab; // ライトソースのプレハブ

    public List<List<DungeonTile>> dungeonTiles = new List<List<DungeonTile>>(); // ダンジョンマップの二次元リスト

    public PlayerController player;

    private Dictionary<DesignType, GameObject> designTypeToPrefab;

    public void GenerateDungeonMap()
    {
        if (tileSets == null || tileSets.Count == 0)
        {
            Debug.LogError("No tile sets available.");
            return;
        }

        if (selectedTileSetIndex < 0 || selectedTileSetIndex >= tileSets.Count)
        {
            Debug.LogError("Selected tile set index is out of range.");
            return;
        }

        // 選択されたタイルセットを取得
        TileSet selectedTileSet = tileSets[selectedTileSetIndex];

        // DesignType とプレハブのマッピングを作成
        designTypeToPrefab = new Dictionary<DesignType, GameObject>();

        foreach (var tilePrefabData in selectedTileSet.tilePrefabs)
        {
            if (!designTypeToPrefab.ContainsKey(tilePrefabData.designType))
            {
                designTypeToPrefab.Add(tilePrefabData.designType, tilePrefabData.prefab);
            }
            else
            {
                Debug.LogWarning($"Duplicate DesignType {tilePrefabData.designType} in tile set {selectedTileSet.setName}");
            }
        }

        Debug.Log($"Generating dungeon map with width: {mapWidth}, height: {mapHeight}");

        // マップの初期化
        dungeonTiles = new List<List<DungeonTile>>();
        for (int x = 0; x < mapWidth; x++)
        {
            List<DungeonTile> column = new List<DungeonTile>();
            for (int y = 0; y < mapHeight; y++)
            {
                DungeonTile tile = new DungeonTile(x, y, TileType.Wall);

                // タイルのゲームオブジェクトを生成
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                tile.tileGameObject = tileObj;

                // タイルの初期色を設定
                tile.SetInitialColor();

                column.Add(tile);
            }
            dungeonTiles.Add(column);
        }

        // 部屋や廊下をマップに適用
        foreach (Room room in roomManager.rooms)
        {
            ApplyRoomToMap(room);
        }

        foreach (Corridor corridor in corridorManager.corridors)
        {
            ApplyCorridorToMap(corridor);
        }

        // 壁の分類を実行
        ClassifyWalls();

        // タイルのプレハブを配置
        PlaceAllTiles();

        Debug.Log($"Dungeon map generated with {dungeonTiles.Count} columns.");
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
                    dungeonTile.TileType = GetTileTypeFromDesignCategory(tileData.designCategory);
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
                if (dungeonTile.TileType == TileType.Floor && dungeonTile.room != null)
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
                dungeonTile.TileType = corridor.isSecret ? TileType.SecretCorridor : TileType.Corridor;
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

    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
    }

    // 壁の分類を行うメソッド
    private void ClassifyWalls()
    {
        for (int x = 0; x < dungeonTiles.Count; x++)
        {
            for (int y = 0; y < dungeonTiles[x].Count; y++)
            {
                DungeonTile tile = dungeonTiles[x][y];

                // 壁タイルのみを対象
                if (tile.GetTileType() == TileType.Wall || tile.GetTileType() == TileType.Empty)
                {
                    // 下のタイルを取得
                    DungeonTile tileBelow = GetTile(x, y - 1);

                    if (tileBelow != null && tileBelow.GetTileType() == TileType.Floor)
                    {
                        tile.designType = DesignType.OuterWall;
                    }
                    else
                    {
                        // 内部の壁として設定
                        tile.designType = DesignType.StoneWall;
                        tile.TileType = TileType.Wall;
                    }
                }
            }
        }
    }

    private DungeonTile GetTile(int x, int y)
    {
        if (IsWithinBounds(x, y))
        {
            return dungeonTiles[x][y];
        }
        return null;
    }

    // タイルのプレハブを配置
    private void PlaceAllTiles()
    {
        foreach (var column in dungeonTiles)
        {
            foreach (var tile in column)
            {
                PlaceTile(tile);
            }
        }
    }

    private void PlaceTile(DungeonTile tile)
    {
        if (tile == null)
            return;

        GameObject prefab = null;

        // タイルの DesignType に対応するプレハブを取得
        if (!designTypeToPrefab.TryGetValue(tile.designType, out prefab))
        {
            // DesignType が見つからない場合、DesignType.Empty に対応するプレハブを取得
            if (!designTypeToPrefab.TryGetValue(DesignType.None, out prefab))
            {
                // DesignType.Empty も見つからない場合、エラーログを出力
                Debug.LogError($"DesignType {tile.designType} に対応するプレハブが見つかりません。また、DesignType.Empty のプレハブも見つかりません。");
                return;
            }
            else
            {
                Debug.LogWarning($"DesignType {tile.designType} に対応するプレハブが見つかりませんでした。DesignType.Empty のプレハブを使用します。");
            }
        }

        // プレハブのインスタンス化
        Vector3 position = new Vector3(tile.x, tile.y, 0);
        GameObject tileObj = Instantiate(prefab, position, Quaternion.identity, transform);
        tile.tileGameObject = tileObj; // タイルのゲームオブジェクトを設定

        // 初期色を設定
        tile.SetInitialColor();
    }


    // ライトソースを配置するメソッド
    public void PlaceLightSources()
    {
        if (lightingManager == null)
        {
            Debug.LogError("LightingManager reference is missing in DungeonMapManager.");
            return;
        }

        if (player != null && player.gameObject != null)
        {
            LightSource playerLight = player.playerLight;
            if (playerLight == null)
            {
                Debug.LogError("LightSource component is missing on the instantiated lightSourcePrefab.");
            }
            else
            {
                lightingManager.AddLightSource(playerLight);
            }
        }
        else
        {
            Debug.LogWarning("PlayerController not found in the scene.");
        }
    }
}
