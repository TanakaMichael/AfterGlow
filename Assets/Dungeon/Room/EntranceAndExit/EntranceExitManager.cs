using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntranceExitManager
{
    /// <summary>
    /// 部屋に出入り口を配置するメソッド
    /// </summary>
    public void AssignEntrancesAndExits(Room room, EntranceExitOptions entranceOptions, EntranceExitOptions exitOptions, int entranceCount = 1, int exitCount = 1)
    {
        // 入口の配置
        AssignEntrances(room, entranceOptions, entranceCount);

        // 出口の配置
        AssignExits(room, exitOptions, exitCount);
    }

    private void AssignEntrances(Room room, EntranceExitOptions options, int count)
    {
        List<WeightedPosition> weightedPositions = options.weightedPositions;

        List<Vector2Int> positions = new List<Vector2Int>();

        if (weightedPositions != null && weightedPositions.Count > 0)
        {
            // ウェイトに基づいて入口を配置
            for (int i = 0; i < count; i++)
            {
                Vector2Int position = GetWeightedRandomPosition(weightedPositions);

                // 位置が有効であるか確認
                if (IsValidPosition(room, position))
                {
                    positions.Add(position);

                    // 選択された位置をウェイトリストから削除して重複を避ける
                    weightedPositions.RemoveAll(wp => wp.position == position);
                }
            }
        }
        else
        {
            // ウェイトが設定されていない場合はランダムに配置
            List<Vector2Int> perimeterPositions = GetPerimeterPositions(room);

            for (int i = 0; i < count && perimeterPositions.Count > 0; i++)
            {
                int index = Random.Range(0, perimeterPositions.Count);
                Vector2Int position = perimeterPositions[index];

                if (IsValidPosition(room, position))
                {
                    positions.Add(position);
                    perimeterPositions.RemoveAt(index); // 重複を避けるために削除
                }
            }
        }

        // 入口を部屋に設定
        foreach (var position in positions)
        {
            room.entrances.Add((position.x, position.y, 1));
            room.tiles[position.y][position.x].isEntrance = true;
            room.tiles[position.y][position.x].designCategory = options.designCategory;
        }
    }

    private void AssignExits(Room room, EntranceExitOptions options, int count)
    {
        List<WeightedPosition> weightedPositions = options.weightedPositions;

        List<Vector2Int> positions = new List<Vector2Int>();

        if (weightedPositions != null && weightedPositions.Count > 0)
        {
            // ウェイトに基づいて出口を配置
            for (int i = 0; i < count; i++)
            {
                Vector2Int position = GetWeightedRandomPosition(weightedPositions);

                // 位置が有効であるか確認
                if (IsValidPosition(room, position))
                {
                    positions.Add(position);

                    // 選択された位置をウェイトリストから削除して重複を避ける
                    weightedPositions.RemoveAll(wp => wp.position == position);
                }
            }
        }
        else
        {
            // ウェイトが設定されていない場合はランダムに配置
            List<Vector2Int> perimeterPositions = GetPerimeterPositions(room);

            for (int i = 0; i < count && perimeterPositions.Count > 0; i++)
            {
                int index = Random.Range(0, perimeterPositions.Count);
                Vector2Int position = perimeterPositions[index];

                if (IsValidPosition(room, position))
                {
                    positions.Add(position);
                    perimeterPositions.RemoveAt(index); // 重複を避けるために削除
                }
            }
        }

        // 出口を部屋に設定
        foreach (var position in positions)
        {
            room.exits.Add((position.x, position.y, 1));
            room.tiles[position.y][position.x].isExit = true;
            room.tiles[position.y][position.x].designCategory = options.designCategory;
        }
    }
    private List<Vector2Int> GetPerimeterPositions(Room room)
    {
        List<Vector2Int> perimeterPositions = new List<Vector2Int>();
    
        for (int y = 0; y < room.height; y++)
        {
            for (int x = 0; x < room.width; x++)
            {
                if (IsPerimeterTile(room, x, y))
                {
                    perimeterPositions.Add(new Vector2Int(x, y));
                }
            }
        }
    
        return perimeterPositions;
    }

    /// <summary>
    /// ウェイトに基づいてランダムな位置を取得する
    /// </summary>
    private Vector2Int GetWeightedRandomPosition(List<WeightedPosition> weightedPositions)
    {
        float totalWeight = weightedPositions.Sum(wp => wp.weight);
        if (totalWeight <= 0)
        {
            // ウェイトが全て0の場合、ランダムに選択
            return weightedPositions[Random.Range(0, weightedPositions.Count)].position;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var wp in weightedPositions)
        {
            cumulativeWeight += wp.weight;
            if (randomValue <= cumulativeWeight)
            {
                return wp.position;
            }
        }

        // 万が一の場合、最後の位置を返す
        return weightedPositions.Last().position;
    }

    /// <summary>
    /// 部屋の輪郭（壁）のタイルからランダムに位置を取得する
    /// </summary>
    private Vector2Int GetRandomPerimeterPosition(Room room)
    {
        List<Vector2Int> perimeterPositions = new List<Vector2Int>();

        for (int y = 0; y < room.height; y++)
        {
            for (int x = 0; x < room.width; x++)
            {
                if (IsPerimeterTile(room, x, y))
                {
                    perimeterPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (perimeterPositions.Count == 0)
        {
            Debug.LogWarning("No perimeter positions found for room.");
            return new Vector2Int(0, 0); // デフォルトの位置
        }

        return perimeterPositions[Random.Range(0, perimeterPositions.Count)];
    }

    /// <summary>
    /// 指定した位置が部屋の輪郭（壁）のタイルかどうかを判定する
    /// </summary>
    private bool IsPerimeterTile(Room room, int x, int y)
    {
        return x == 0 || x == room.width - 1 || y == 0 || y == room.height - 1;
    }

    /// <summary>
    /// 指定した位置が有効かどうかを判定する
    /// </summary>
    private bool IsValidPosition(Room room, Vector2Int position)
    {
        if (position.x < 0 || position.x >= room.width || position.y < 0 || position.y >= room.height)
        {
            return false;
        }

        // 既に出入り口がある場合は無効
        if (room.tiles[position.y][position.x].isEntrance || room.tiles[position.y][position.x].isExit)
        {
            return false;
        }

        // 壁のタイルである必要がある
        if (IsPerimeterTile(room, position.x, position.y))
        {
            return true;
        }

        return false;
    }
}
