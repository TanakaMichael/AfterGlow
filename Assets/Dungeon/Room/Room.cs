using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int id;
    public int areaId;
    public int x, y;
    public int width, height;
    public RoomType roomType;
    public bool isFixed = false;

    public List<List<TileData>> tiles = new List<List<TileData>>();
    public List<(int x, int y, int priority)> entrances = new List<(int, int, int)>();
    public List<(int x, int y, int priority)> exits = new List<(int, int, int)>();

    public Room(int id, int x, int y, int width, int height, int areaId = 0)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.roomType = RoomType.Empty;
        this.areaId = areaId;
        this.isFixed = false;
    }

    public Vector2Int GetCenterInt()
    {
        return new Vector2Int(x + width / 2, y + height / 2);
    }

    public Vector2 GetCenterFloat()
    {
        return new Vector2(x + width / 2f, y + height / 2f);
    }

    /// <summary>
    /// 固定部屋のデータを適用する
    /// </summary>
    public void ApplyFixedRoomData(FixedRoom fixedRoom)
    {
        this.x = fixedRoom.basePosition.x + Random.Range(0, fixedRoom.baseSize.x - fixedRoom.size.x);
        this.y = fixedRoom.basePosition.y + Random.Range(0, fixedRoom.baseSize.y - fixedRoom.size.y);
        this.width = fixedRoom.size.x;
        this.height = fixedRoom.size.y;
        ConvertCustomTileToTileData(fixedRoom.tiles, this.tiles);
        this.entrances = new List<(int, int, int)>(fixedRoom.entrances);
        this.exits = new List<(int, int, int)>(fixedRoom.exits);
        this.roomType = fixedRoom.roomType;
        this.isFixed = true;
    }
    private void ConvertCustomTileToTileData(List<List<CustomTile>> customTiles, List<List<TileData>> tiles)
    {
        tiles.Clear();
        Debug.Log($"customTile: {customTiles.Count}");
        for (int y = 0; y < height; y++)
        {
            List<TileData> row = new List<TileData>();
            for (int x = 0; x < width; x++)
            {
                CustomTile customTile = customTiles[y][x];

                TileData tileData = new TileData
                {
                    isWalkable = customTile.isWalkable,
                    spawnSpecialObjectSettings = customTile.spawnSpecialObjectSettings,
                    spawnEnemySettings = customTile.spawnEnemySettings,
                    spawnNPCSettings = customTile.spawnNPCSettings,
                    isExit = customTile.isExit,
                    isEntrance = customTile.isEntrance,
                    priority = customTile.priority
                };

                // DesignTypeの設定（必要に応じて）
                // tileData.SetDesignType(customTile.designCategory); // DesignCategoryが存在する場合

                row.Add(tileData);

                // デバッグログ
                Debug.Log($"Converted CustomTile at ({x}, {y}) to TileData.");
            }
            tiles.Add(row);
        }
    }
    public void ApplyRandomRoomData(Room room){
        this.width = room.width;
        this.height = room.height;
        this.tiles = room.tiles;
        this.isFixed = room.isFixed;
    }
    public RoomType GetRoomType(){
        return roomType;
    }
}
