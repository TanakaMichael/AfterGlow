using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MazeCheckerboardRoomGenerator", menuName = "Room Generators/MazeCheckerboardRoomGenerator")]
public class MazeCheckerboardRoomGenerator : RoomGeneratorBase
{
    [Header("Checkerboard Settings")]
    [Tooltip("個室の幅（ブロックサイズ）")]
    public int blockWidth = 4;
    [Tooltip("個室の高さ（ブロックサイズ）")]
    public int blockHeight = 4;

    [Header("Corridor Settings")]
    [Tooltip("通路の幅")]
    public int corridorWidth = 1;

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

        // 市松模様の生成
        GenerateCheckerboardPattern(room);

        // 個室をつなぐ迷路風の通路を生成
        GenerateMazeConnections(room);

        // 部屋の輪郭を壁で覆う
        SetRoomOutlineAsWalls(room);

        // 出入り口のウェイト設定
        SetEntranceExitWeights(room, true); // Entrance
        SetEntranceExitWeights(room, false); // Exit
    }

    private void GenerateCheckerboardPattern(Room room)
    {
        for (int y = 0; y < room.height; y++)
        {
            List<TileData> row = new List<TileData>();
            for (int x = 0; x < room.width; x++)
            {
                TileData tile = new TileData();

                // 市松模様の条件
                if (((x / blockWidth) + (y / blockHeight)) % 2 == 0)
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
    }

    private void GenerateMazeConnections(Room room)
    {
        int width = room.width;
        int height = room.height;
        bool[,] visited = new bool[height / blockHeight, width / blockWidth];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        // 開始地点をランダムに選択
        Vector2Int start = new Vector2Int(Random.Range(0, width / blockWidth), Random.Range(0, height / blockHeight));
        stack.Push(start);
        visited[start.y, start.x] = true;

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current, visited);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                CreateCorridorBetweenBlocks(room, current, next);
                visited[next.y, next.x] = true;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int current, bool[,] visited)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        int rows = visited.GetLength(0);
        int cols = visited.GetLength(1);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = current + dir;
            if (neighbor.x >= 0 && neighbor.x < cols && neighbor.y >= 0 && neighbor.y < rows && !visited[neighbor.y, neighbor.x])
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private void CreateCorridorBetweenBlocks(Room room, Vector2Int blockA, Vector2Int blockB)
    {
        int startX = Mathf.Min(blockA.x, blockB.x) * blockWidth;
        int endX = (Mathf.Max(blockA.x, blockB.x) + 1) * blockWidth;
        int startY = Mathf.Min(blockA.y, blockB.y) * blockHeight;
        int endY = (Mathf.Max(blockA.y, blockB.y) + 1) * blockHeight;

        // 通路の幅を考慮して床を設置
        if (blockA.x != blockB.x) // 水平方向の通路
        {
            for (int y = 0; y < blockHeight; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    if (y < corridorWidth || y >= blockHeight - corridorWidth)
                    {
                        room.tiles[startY + y][x].isWalkable = true;
                        room.tiles[startY + y][x].SetDesignType(DesignCategory.Floor);
                    }
                }
            }
        }
        else if (blockA.y != blockB.y) // 垂直方向の通路
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = 0; x < blockWidth; x++)
                {
                    if (x < corridorWidth || x >= blockWidth - corridorWidth)
                    {
                        room.tiles[y][startX + x].isWalkable = true;
                        room.tiles[y][startX + x].SetDesignType(DesignCategory.Floor);
                    }
                }
            }
        }
    }

    protected override List<Vector2Int> GetRelevantWallTiles(Room room)
    {
        List<Vector2Int> wallTiles = new List<Vector2Int>();

        for (int y = 0; y < room.height; y++)
        {
            for (int x = 0; x < room.width; x++)
            {
                // 壁の部分を探す（部屋の外周）
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
