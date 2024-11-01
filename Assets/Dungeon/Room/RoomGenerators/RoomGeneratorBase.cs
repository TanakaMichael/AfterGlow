using System.Collections.Generic;
using UnityEngine;
public abstract class RoomGeneratorBase : ScriptableObject, IRoomGenerator
{
    public EntranceExitOptions entranceOptions;
    public EntranceExitOptions exitOptions;

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

    [Header("Entrance")]
    public int entranceCount = 2;
    [Header("Exit")]
    public int exitCount = 2;

    protected RoomGeneratorBase()
    {
        entranceOptions = new EntranceExitOptions(SpawnPosition.Random);
        exitOptions = new EntranceExitOptions(SpawnPosition.Random);
    }

    public abstract void GenerateRoom(Room room);

    /// <summary>
    /// 部屋のサイズと位置を決定するメソッド
    /// </summary>
    protected virtual void DetermineRoomSizeAndPosition(Room room)
    {
        Area area = room.area;
        if (area == null)
        {
            Debug.LogError("Area is not assigned to the room.");
            return;
        }

        // 部屋の幅と高さをエリアサイズの割合でランダムに決定
        int minWidth = Mathf.CeilToInt(area.width * minWidthRatio);
        int maxWidth = Mathf.FloorToInt(area.width * maxWidthRatio);
        int minHeight = Mathf.CeilToInt(area.height * minHeightRatio);
        int maxHeight = Mathf.FloorToInt(area.height * maxHeightRatio);

        room.width = Random.Range(minWidth, maxWidth + 1);
        room.height = Random.Range(minHeight, maxHeight + 1);

        // 部屋の位置をエリア内でランダムに決定
        int maxX = area.x + area.width - room.width;
        int maxY = area.y + area.height - room.height;

        room.x = Random.Range(area.x, maxX + 1);
        room.y = Random.Range(area.y, maxY + 1);
    }

    /// <summary>
    /// 部屋の形状に基づいて出入り口のウェイトを設定するメソッド
    /// </summary>
    protected void SetEntranceExitWeights(Room room, bool isEntrance = true)
    {
        List<WeightedPosition> weightedPositions = isEntrance ? entranceOptions.weightedPositions : exitOptions.weightedPositions;

        foreach (Vector2Int position in GetRelevantWallTiles(room))
        {
            float weight = CalculateWeightBasedOnShape(room, position);
            weightedPositions.Add(new WeightedPosition(position, weight));
        }
    }

    /// <summary>
    /// 出入り口を配置する候補となる壁のタイルを取得するメソッド
    /// </summary>
    protected abstract List<Vector2Int> GetRelevantWallTiles(Room room);

    /// <summary>
    /// タイルの位置に基づいてウェイトを計算するメソッド
    /// </summary>
    protected abstract float CalculateWeightBasedOnShape(Room room, Vector2Int position);

    /// <summary>
    /// 部屋の輪郭を壁で覆うメソッド
    /// </summary>
    protected void SetRoomOutlineAsWalls(Room room)
    {
        for (int y = 0; y < room.height; y++)
        {
            for (int x = 0; x < room.width; x++)
            {
                if (x == 0 || x == room.width - 1 || y == 0 || y == room.height - 1)
                {
                    room.tiles[y][x].isWalkable = false;
                    room.tiles[y][x].designCategory = DesignCategory.Wall;
                }
            }
        }
    }
}
