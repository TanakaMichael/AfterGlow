using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new BasicRoomGenerator", menuName = "Dungeon/Room/Generators/LShape")]
public class LShapeRoomGenerator : RoomGeneratorBase
{
    public LShapeRoomGenerator()
    {
        // 必要に応じて初期化
    }

    public override void GenerateRoom(Room room)
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
                // L字型の条件
                if ((x < width / 2 && y >= height / 2) || (x >= width / 2 && y < height / 2))
                {
                    tile.isWalkable = true;
                    tile.designCategory = DesignCategory.Floor;
                }
                else
                {
                    tile.isWalkable = false;
                    tile.designCategory = DesignCategory.Wall;
                }
                row.Add(tile);
            }
            room.tiles.Add(row);
        }

        // 出入り口のウェイト設定
        SetEntranceExitWeights(room, true); // Entrance
        SetEntranceExitWeights(room, false); // Exit
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
        if ((position.x == width / 2 && position.y == height - 1) ||
            (position.x == width - 1 && position.y == height / 2))
        {
            return 2.0f;
        }

        // 角部分（低いウェイト）
        if (position.x == width / 2 && position.y == height / 2)
        {
            return 0.5f;
        }

        // その他の部分（標準のウェイト）
        return 1.0f;
    }
}
