using System.Collections.Generic;
using UnityEngine;

public class Corridor
{
    public Vector2Int start;
    public Vector2Int end;
    public List<Vector2Int> path = new List<Vector2Int>();
    public int width = 1; // 廊下の幅
    public int bendCount = 1; // 曲がり角の数

    public Room roomA; // 接続元の部屋
    public Room roomB; // 接続先の部屋

    public bool isSecret = false; // 秘密の通路かどうか

    public List<TileData> tiles = new List<TileData>(); // 廊下のタイルデータ

    public Corridor(Vector2Int start, Vector2Int end, int width = 1, int bendCount = 1, bool isSecret = false)
    {
        this.start = start;
        this.end = end;
        this.width = width;
        this.bendCount = bendCount;
        this.isSecret = isSecret;
    }

    public void GenerateTiles()
    {
        // 廊下の経路を計算
        CalculatePath();

        // タイルデータを生成
        foreach (var position in path)
        {
            for (int dx = -width / 2; dx <= width / 2; dx++)
            {
                for (int dy = -width / 2; dy <= width / 2; dy++)
                {
                    Vector2Int tilePos = new Vector2Int(position.x + dx, position.y + dy);
                    TileData tile = new TileData();
                    tile.isWalkable = !isSecret; // 秘密の通路はデフォルトでは通行不可
                    tile.SetDesignType(isSecret ? DesignCategory.SecretCorridor : DesignCategory.Corridor); // デザインカテゴリを設定
                    tile.x = tilePos.x;
                    tile.y = tilePos.y;
                    tiles.Add(tile);
                }
            }
        }
    }

    private void CalculatePath()
    {
        Vector2Int current = start;
        path.Add(current);

        if (bendCount == 0)
        {
            // 直線的な経路
            while (current != end)
            {
                if (current.x != end.x)
                    current.x += (end.x > current.x) ? 1 : -1;
                else if (current.y != end.y)
                    current.y += (end.y > current.y) ? 1 : -1;
                path.Add(current);
            }
            return;
        }
        else if (bendCount == 1)
        {
            // L 字型の経路
            bool horizontalFirst = Random.value > 0.5f;

            if (horizontalFirst)
            {
                // 水平方向に移動
                while (current.x != end.x)
                {
                    current.x += (end.x > current.x) ? 1 : -1;
                    path.Add(current);
                }
                // 垂直方向に移動
                while (current.y != end.y)
                {
                    current.y += (end.y > current.y) ? 1 : -1;
                    path.Add(current);
                }
            }
            else
            {
                // 垂直方向に移動
                while (current.y != end.y)
                {
                    current.y += (end.y > current.y) ? 1 : -1;
                    path.Add(current);
                }
                // 水平方向に移動
                while (current.x != end.x)
                {
                    current.x += (end.x > current.x) ? 1 : -1;
                    path.Add(current);
                }
            }
            return;
        }
        else
        {
            // 複数の曲がり角を持つ経路
            // 中間点の数を bendCount - 1 とする
            int waypointCount = bendCount - 1;
            List<Vector2Int> waypoints = new List<Vector2Int>();

            // 中間点を計算
            for (int i = 0; i < waypointCount; i++)
            {
                Vector2Int waypoint = new Vector2Int(
                    Random.Range(Mathf.Min(start.x, end.x), Mathf.Max(start.x, end.x) + 1),
                    Random.Range(Mathf.Min(start.y, end.y), Mathf.Max(start.y, end.y) + 1)
                );
                waypoints.Add(waypoint);
            }
            waypoints.Add(end);

            // 経路を生成
            foreach (var waypoint in waypoints)
            {
                while (current != waypoint)
                {
                    if (current.x != waypoint.x)
                    {
                        current.x += (waypoint.x > current.x) ? 1 : -1;
                    }
                    else if (current.y != waypoint.y)
                    {
                        current.y += (waypoint.y > current.y) ? 1 : -1;
                    }
                    path.Add(current);
                }
            }
        }
    }
}
