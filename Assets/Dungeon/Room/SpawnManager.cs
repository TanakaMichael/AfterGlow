using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float nearWallWeight = 0.7f; // 壁際の配置確率
    [Range(0f, 1f)]
    public float centralWeight = 0.3f; // 中央部分の配置確率

    public void AssignNPCData(Room room, SpawnNPCSettings settings, int count = 1)
    {
        List<Vector2Int> positions = GetSetablePositions(room.tiles);
        AssignDataToTiles(room, positions, count, (tile) => tile.spawnNPCSettings = settings);
    }

    public void AssignSpecialObjectData(Room room, SpawnSpecialObjectSettings settings, int count = 1)
    {
        List<Vector2Int> positions = GetSetablePositions(room.tiles);
        AssignDataToTiles(room, positions, count, (tile) => tile.spawnSpecialObjectSettings = settings);
    }

    public void AssignEnemyData(Room room, SpawnEnemySettings settings, int count = 1)
    {
        List<Vector2Int> positions = GetSetablePositions(room.tiles);
        AssignDataToTiles(room, positions, count, (tile) => tile.spawnEnemySettings = settings);
    }

    private List<Vector2Int> GetSetablePositions(List<List<TileData>> tiles)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int y = 0; y < tiles.Count; y++)
        {
            for (int x = 0; x < tiles[y].Count; x++)
            {
                if (tiles[y][x].isWalkable && !tiles[y][x].isEntrance && !tiles[y][x].isExit && tiles[y][x].spawnSpecialObjectSettings == null && tiles[y][x].spawnEnemySettings == null && tiles[y][x].spawnNPCSettings == null)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
        }
        return positions;
    }

    private void AssignDataToTiles(Room room, List<Vector2Int> positions, int count, System.Action<TileData> assignAction)
    {
        int assignedCount = 0;
        List<Vector2Int> shuffledPositions = ShuffleList(positions);

        foreach (var position in shuffledPositions)
        {
            if (assignedCount >= count)
                break;

            TileData tile = room.tiles[position.y][position.x];

            // 壁際の判定を利用して配置確率を調整（ここでは例として壁際に配置しやすくしています）
            if (IsNearWall(room.tiles, position.x, position.y))
            {
                if (Random.value <= nearWallWeight) // nearWallWeightの確率で壁際に配置
                {
                    assignAction(tile);
                    assignedCount++;
                }
            }
            else
            {
                if (Random.value <= centralWeight) // centralWeightの確率で中央部分に配置
                {
                    assignAction(tile);
                    assignedCount++;
                }
            }
        }
    }

    /// <summary>
    /// タイルが壁際かどうかを判定するメソッド
    /// </summary>
    private bool IsNearWall(List<List<TileData>> tiles, int x, int y)
    {
        int height = tiles.Count;
        int width = tiles[0].Count;

        // 現在のタイルが床であることを確認
        if (!tiles[y][x].isWalkable)
        {
            return false;
        }

        // 上下左右に壁があるかを確認する
        bool nearWall = false;

        // 上のタイルの確認
        if (y - 1 >= 0 && !tiles[y - 1][x].isWalkable)
        {
            nearWall = true;
        }

        // 下のタイルの確認
        if (y + 1 < height && !tiles[y + 1][x].isWalkable)
        {
            nearWall = true;
        }

        // 左のタイルの確認
        if (x - 1 >= 0 && !tiles[y][x - 1].isWalkable)
        {
            nearWall = true;
        }

        // 右のタイルの確認
        if (x + 1 < width && !tiles[y][x + 1].isWalkable)
        {
            nearWall = true;
        }

        return nearWall;
    }

    /// <summary>
    /// リストをシャッフルするメソッド
    /// </summary>
    private List<Vector2Int> ShuffleList(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
}
