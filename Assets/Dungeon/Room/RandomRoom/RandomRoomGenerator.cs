using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "new GenerateRandomRoom", menuName = "Dungeon/Room/RandomRoomGenerator")]
public class RandomRoomGenerator : ScriptableObject
{
    public RoomType roomType;

    // 生成アルゴリズムの種類を定義
    public enum GenerationAlgorithmType
    {
        Basic,
        Maze,
        // 他のアルゴリズムを追加可能
    }

    public GenerationAlgorithmType generationAlgorithmType;

    // アルゴリズムごとの設定
    [Header("Size Settings")]
    [Tooltip("部屋の最小幅（エリアサイズに対する割合）")]
    [Range(0f, 1f)]
    public float minWidthRatio = 0.3f; // 幅の最小割合
    [Tooltip("部屋の最小高さ（エリアサイズに対する割合）")]
    [Range(0f, 1f)]
    public float minHeightRatio = 0.3f; // 高さの最小割合

    [Tooltip("部屋の最大幅（エリアサイズに対する割合）")]
    [Range(0f, 1f)]
    public float maxWidthRatio = 0.8f; // 幅の最大割合
    [Tooltip("部屋の最大高さ（エリアサイズに対する割合）")]
    [Range(0f, 1f)]
    public float maxHeightRatio = 0.8f; // 高さの最大割合

    // SpecialObject、Enemy、NPC の設定
    [Header("Spawn Settings")]
    public SpawnSettings SpawnSetting;

    /// <summary>
    /// ランダムな部屋を生成する
    /// </summary>
    public Room GenerateRandomRoom(Room baseRoom)
    {
        // エリアサイズに基づいて部屋のサイズを決定
        int width = Mathf.RoundToInt(Random.Range(minWidthRatio * baseRoom.width, maxWidthRatio * baseRoom.width));
        int height = Mathf.RoundToInt(Random.Range(minHeightRatio * baseRoom.height, maxHeightRatio * baseRoom.height));

        // 部屋のサイズがエリアを超えないように調整
        width = Mathf.Clamp(width, 1, baseRoom.width);
        height = Mathf.Clamp(height, 1, baseRoom.height);

        // 部屋の位置をエリア内でランダムに決定
        int xOffset = Random.Range(0, baseRoom.width - width + 1);
        int yOffset = Random.Range(0, baseRoom.height - height + 1);
        int x = baseRoom.x + xOffset;
        int y = baseRoom.y + yOffset;

        // 新しい Room を作成
        Room newRoom = new Room(baseRoom.id, x, y, width, height, baseRoom.areaId)
        {
            roomType = this.roomType,
            isFixed = false
        };

        // タイル情報を生成
        switch (generationAlgorithmType)
        {
            case GenerationAlgorithmType.Basic:
                GenerateBasicRoom(newRoom);
                break;
            case GenerationAlgorithmType.Maze:
                GenerateMazeRoom(newRoom);
                break;
            // 他のアルゴリズムの場合も追加
            default:
                GenerateBasicRoom(newRoom);
                break;
        }

        // SpecialObject、Enemy、NPC を配置
        PlaceElementsInRoom(newRoom);

        return newRoom;
    }

    /// <summary>
    /// 基本的な部屋を生成
    /// </summary>
    private void GenerateBasicRoom(Room room)
    {
        int width = room.width;
        int height = room.height;

        room.tiles = new List<List<TileData>>();
        for (int y = 0; y < height; y++)
        {
            List<TileData> row = new List<TileData>();
            for (int x = 0; x < width; x++)
            {
                TileData tile = new TileData();
                tile.isWalkable = true;
                tile.SetDesignType(DesignCategory.Floor);
                row.Add(tile);
            }
            room.tiles.Add(row);
        }
        Debug.Log($"Generated Basic Room Tiles: Rows={room.tiles.Count}, Columns per row={(room.tiles.Count > 0 ? room.tiles[0].Count : 0)}");
    }

    /// <summary>
    /// 迷路状の部屋を生成（簡易的な例）
    /// </summary>
    private void GenerateMazeRoom(Room room)
    {
        int width = room.width;
        int height = room.height;

        room.tiles = new List<List<TileData>>();
        for (int y = 0; y < height; y++)
        {
            List<TileData> row = new List<TileData>();
            for (int x = 0; x < width; x++)
            {
                TileData tile = new TileData();
                // 壁と床をランダムに配置（簡易的な迷路生成）
                tile.isWalkable = Random.value > 0.1f;
                if(tile.isWalkable) tile.SetDesignType(DesignCategory.Floor);
                else tile.SetDesignType(DesignCategory.Wall);
                row.Add(tile);
            }
            room.tiles.Add(row);
        }
    }

    /// <summary>
    /// SpecialObject、Enemy、NPC を部屋内に配置
    /// </summary>
    private void PlaceElementsInRoom(Room room)
    {
        // SpecialObject の配置
        PlaceElements(room, SpawnSetting, ElementType.SpecialObject);

        // Enemy の配置
        PlaceElements(room, SpawnSetting, ElementType.Enemy);

        // NPC の配置
        PlaceElements(room, SpawnSetting, ElementType.NPC);
    }

    /// <summary>
    /// 要素を部屋内に配置する汎用メソッド
    /// </summary>
    private void PlaceElements(Room room, SpawnSettings spawnSettings, ElementType elementType)
    {
        // 出現判定
        if (Random.value > spawnSettings.spawnProbability)
        {
            return; // 出現しない
        }

        // 出現数を決定
        int spawnCount = Random.Range(spawnSettings.minSpawnCount, spawnSettings.maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            // 配置場所を決定
            Vector2Int position = GetSpawnPosition(room, spawnSettings.spawnPosition);

            // タイルに要素を設定
            TileData tile = room.tiles[position.y][position.x];

            switch (elementType)
            {
                case ElementType.SpecialObject:
                    tile.spawnSpecialObjectSettings = spawnSettings.specialObjectSettings;
                    break;
                case ElementType.Enemy:
                    tile.spawnEnemySettings = spawnSettings.enemySettings;
                    break;
                case ElementType.NPC:
                    tile.spawnNPCSettings = spawnSettings.npcSettings;
                    break;
            }
        }
    }

    /// <summary>
    /// 配置場所を取得する
    /// </summary>
    private Vector2Int GetSpawnPosition(Room room, SpawnPosition spawnPosition)
    {
        int x = 0;
        int y = 0;
        int width = room.width;
        int height = room.height;

        switch (spawnPosition)
        {
            case SpawnPosition.Center:
                x = width / 2;
                y = height / 2;
                break;
            case SpawnPosition.Edge:
                // 部屋の周囲からランダムに選択
                List<Vector2Int> edgePositions = new List<Vector2Int>();
                for (int i = 0; i < width; i++)
                {
                    edgePositions.Add(new Vector2Int(i, 0)); // 上辺
                    edgePositions.Add(new Vector2Int(i, height - 1)); // 下辺
                }
                for (int i = 1; i < height - 1; i++)
                {
                    edgePositions.Add(new Vector2Int(0, i)); // 左辺
                    edgePositions.Add(new Vector2Int(width - 1, i)); // 右辺
                }
                Vector2Int edgePos = edgePositions[Random.Range(0, edgePositions.Count)];
                x = edgePos.x;
                y = edgePos.y;
                break;
            case SpawnPosition.Random:
                x = Random.Range(0, width);
                y = Random.Range(0, height);
                break;
            // 他の配置方法を追加可能
            default:
                x = Random.Range(0, width);
                y = Random.Range(0, height);
                break;
        }

        return new Vector2Int(x, y);
    }

    // 要素の種類を表す列挙型
    private enum ElementType
    {
        SpecialObject,
        Enemy,
        NPC
    }
}
