using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BasicRoomGenerator", menuName = "Room Generators/BasicRoomGenerator")]
public class BasicRoomGenerator : RoomGeneratorBase
{
    public override void GenerateRoom(Room room)
    {
        if (room.isFixed)
        {
            // 固定部屋の場合、サイズと座標は既に設定されていると仮定
            return;
        }

        // エリアから部屋のサイズを決定
        int areaWidth = room.area.width;
        int areaHeight = room.area.height;

        int minWidth = Mathf.RoundToInt(areaWidth * minWidthRatio);
        int maxWidth = Mathf.RoundToInt(areaWidth * maxWidthRatio);
        int minHeight = Mathf.RoundToInt(areaHeight * minHeightRatio);
        int maxHeight = Mathf.RoundToInt(areaHeight * maxHeightRatio);

        room.width = Random.Range(minWidth, maxWidth + 1);
        room.height = Random.Range(minHeight, maxHeight + 1);

        // 部屋の位置をエリア内でランダムに決定
        int maxX = room.area.x + room.area.width - room.width;
        int maxY = room.area.y + room.area.height - room.height;

        room.x = Random.Range(room.area.x, maxX + 1);
        room.y = Random.Range(room.area.y, maxY + 1);

        // タイルの初期化
        room.tiles = new List<List<TileData>>();

        for (int y = 0; y < room.height; y++)
        {
            List<TileData> row = new List<TileData>();
            for (int x = 0; x < room.width; x++)
            {
                TileData tile = new TileData
                {
                    isWalkable = true,
                    designCategory = DesignCategory.Floor
                };
                tile.SetDesignType(DesignCategory.Floor);
                row.Add(tile);
            }
            room.tiles.Add(row);
        }
        // 部屋の輪郭を壁で覆う
        SetRoomOutlineAsWalls(room);

        // 出入り口のウェイト設定
        SetEntranceExitWeights(room, isEntrance); // Entrance
        SetEntranceExitWeights(room, isExit); // Exit
    }

    protected override List<Vector2Int> GetRelevantWallTiles(Room room)
    {
        List<Vector2Int> wallTiles = new List<Vector2Int>();

        for (int y = 0; y < room.height; y++)
        {
            for (int x = 0; x < room.width; x++)
            {
                if (x == 0 || x == room.width - 1 || y == 0 || y == room.height - 1)
                {
                    wallTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        return wallTiles;
    }

    protected override float CalculateWeightBasedOnShape(Room room, Vector2Int position)
    {
        // 角はウェイトを低くする
        if ((position.x == 0 && position.y == 0) ||
            (position.x == 0 && position.y == room.height - 1) ||
            (position.x == room.width - 1 && position.y == 0) ||
            (position.x == room.width - 1 && position.y == room.height - 1))
        {
            return 0.1f; // 低いウェイト
        }

        return 1.0f; // 標準のウェイト
    }
}
