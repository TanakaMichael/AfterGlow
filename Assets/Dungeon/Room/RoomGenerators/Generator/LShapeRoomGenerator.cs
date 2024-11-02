using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LShapeRoomGenerator", menuName = "Room Generators/LShapeRoomGenerator")]
public class LShapeRoomGenerator : RoomGeneratorBase
{
    public LShapeRoomGenerator()
    {
        // 必要に応じて初期化
    }

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
                TileData tile = new TileData();
                
                // L字型の条件
                if ((y < room.height / 2 && x < room.width / 2) || (y >= room.height / 2 && x < room.width / 2) || (y < room.height / 2 && x >= room.width / 2))
                {
                    tile.isWalkable = true;
                    tile.SetDesignType(DesignCategory.Floor);
                }
                else
                {
                    tile.isWalkable = false;
                    tile.SetDesignType(DesignCategory.Wall);
                }

                row.Add(tile);
            }
            room.tiles.Add(row);
        }

        // 部屋の輪郭を壁で覆う
        SetRoomOutlineAsWalls(room);

        // 出入り口のウェイト設定
        if(entranceCount >= 0) 
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
        int width = room.width;
        int height = room.height;

        // 出っ張っている部分（高いウェイト）
        if ((position.x == 0 && position.y >= height / 2) ||
            (position.y == 0 && position.x >= width / 2))
        {
            return 2.0f;
        }

        // 角部分（低いウェイト）
        if ((position.x == 0 && position.y == 0) ||
            (position.x == 0 && position.y == height - 1) ||
            (position.x == width - 1 && position.y == 0) ||
            (position.x == width - 1 && position.y == height - 1))
        {
            return 0.1f;
        }

        // その他の部分（標準のウェイト）
        return 1.0f;
    }
}