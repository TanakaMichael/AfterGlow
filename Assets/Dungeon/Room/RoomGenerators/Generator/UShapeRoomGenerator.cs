using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UShapeRoomGenerator", menuName = "Room Generators/UShapeRoomGenerator")]
public class UShapeRoomGenerator : RoomGeneratorBase
{
    public override void GenerateRoom(Room room)
    {
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

        room.tiles = new List<List<TileData>>();

        // U字型の部屋を生成
        int direction = Random.Range(0, 4); // 0: 下向き, 1: 上向き, 2: 左向き, 3: 右向き
        for (int y = 0; y < room.height; y++)
        {
            List<TileData> row = new List<TileData>();
            for (int x = 0; x < room.width; x++)
            {
                TileData tile = new TileData();

                switch (direction)
                {
                    case 0: // 下向き U 字
                        if (y < room.height / 2 || (x < room.width / 4 || x > 3 * room.width / 4))
                        {
                            tile.isWalkable = true;
                            tile.SetDesignType(DesignCategory.Floor);
                        }
                        else
                        {
                            tile.isWalkable = false;
                            tile.SetDesignType(DesignCategory.Wall);
                        }
                        break;
                    case 1: // 上向き U 字
                        if (y >= room.height / 2 || (x < room.width / 4 || x > 3 * room.width / 4))
                        {
                            tile.isWalkable = true;
                            tile.SetDesignType(DesignCategory.Floor);
                        }
                        else
                        {
                            tile.isWalkable = false;
                            tile.SetDesignType(DesignCategory.Wall);
                        }
                        break;
                    case 2: // 左向き U 字
                        if (x < room.width / 2 || (y < room.height / 4 || y > 3 * room.height / 4))
                        {
                            tile.isWalkable = true;
                            tile.SetDesignType(DesignCategory.Floor);
                        }
                        else
                        {
                            tile.isWalkable = false;
                            tile.SetDesignType(DesignCategory.Wall);
                        }
                        break;
                    case 3: // 右向き U 字
                        if (x >= room.width / 2 || (y < room.height / 4 || y > 3 * room.height / 4))
                        {
                            tile.isWalkable = true;
                            tile.SetDesignType(DesignCategory.Floor);
                        }
                        else
                        {
                            tile.isWalkable = false;
                            tile.SetDesignType(DesignCategory.Wall);
                        }
                        break;
                }

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

        int direction = Random.Range(0, 4); // 0: 下向き, 1: 上向き, 2: 左向き, 3: 右向き
        for (int y = 0; y < room.height; y++)
        {
            for (int x = 0; x < room.width; x++)
            {
                switch (direction)
                {
                    case 0: // 下向き U 字の外周部分
                        if (y == 0 || (y == room.height - 1 && (x < room.width / 4 || x > 3 * room.width / 4)) ||
                            x == 0 || x == room.width - 1)
                        {
                            wallTiles.Add(new Vector2Int(x, y));
                        }
                        break;
                    case 1: // 上向き U 字の外周部分
                        if (y == room.height - 1 || (y == 0 && (x < room.width / 4 || x > 3 * room.width / 4)) ||
                            x == 0 || x == room.width - 1)
                        {
                            wallTiles.Add(new Vector2Int(x, y));
                        }
                        break;
                    case 2: // 左向き U 字の外周部分
                        if (x == 0 || (x == room.width - 1 && (y < room.height / 4 || y > 3 * room.height / 4)) ||
                            y == 0 || y == room.height - 1)
                        {
                            wallTiles.Add(new Vector2Int(x, y));
                        }
                        break;
                    case 3: // 右向き U 字の外周部分
                        if (x == room.width - 1 || (x == 0 && (y < room.height / 4 || y > 3 * room.height / 4)) ||
                            y == 0 || y == room.height - 1)
                        {
                            wallTiles.Add(new Vector2Int(x, y));
                        }
                        break;
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

        // 内側の壁は低いウェイトを設定
        if (!room.tiles[position.y][position.x].isWalkable)
        {
            return 0.2f; // 内側の壁のウェイト
        }

        // 外周の壁には標準のウェイトを設定
        return 1.0f; // 標準のウェイト
    }
}
