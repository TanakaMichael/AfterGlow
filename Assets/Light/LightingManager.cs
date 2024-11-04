// LightingManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public DungeonMapManager dungeonMapManager;
    public List<LightSource> lightSources = new List<LightSource>();

    private Dictionary<LightSource, List<DungeonTile>> staticLightCache = new Dictionary<LightSource, List<DungeonTile>>();
    private bool isCalculating = false;
    
    // 遅延追加用のリスト
    private List<LightSource> pendingLightSources = new List<LightSource>();

    public void Init()
    {
        if (dungeonMapManager == null)
        {
            dungeonMapManager = FindObjectOfType<DungeonMapManager>();
        }

        CalculateLighting();
    }

    void Update()
    {
        bool needsRecalculation = false;

        foreach (var light in lightSources)
        {
            if (light.isDynamic)
            {
                needsRecalculation = true;
                break;
            }
        }

        if (needsRecalculation)
        {
            CalculateLighting();
        }
    }

    public void AddLightSource(LightSource light)
    {
        if (!lightSources.Contains(light) && !pendingLightSources.Contains(light))
        {
            pendingLightSources.Add(light);
        }
    }

    public void CalculateLighting()
    {
        if (isCalculating)
            return;

        StartCoroutine(CalculateLightingCoroutine());
    }

    private IEnumerator CalculateLightingCoroutine()
    {
        isCalculating = true;

        foreach (var column in dungeonMapManager.dungeonTiles)
        {
            foreach (var tile in column)
            {
                tile.accumulatedColor = Color.black;
            }
        }

        // ここで遅延追加されたLightSourceをlightSourcesリストに追加
        if (pendingLightSources.Count > 0)
        {
            lightSources.AddRange(pendingLightSources);
            pendingLightSources.Clear();
        }

        foreach (var light in lightSources)
        {
            if (light.isDynamic)
            {
                ApplyLight(light);
            }
            else
            {
                if (!staticLightCache.ContainsKey(light))
                {
                    staticLightCache[light] = GetAffectedTiles(light);
                }

                foreach (var tile in staticLightCache[light])
                {
                    ApplyLightToTile(light, tile);
                }
            }

            yield return null;
        }

        foreach (var column in dungeonMapManager.dungeonTiles)
        {
            foreach (var tile in column)
            {
                tile.UpdateBrightness();
            }
        }

        isCalculating = false;
    }


    private void ApplyLight(LightSource light)
    {
        int lightRange = Mathf.CeilToInt(light.range);
        for (int x = -lightRange; x <= lightRange; x++)
        {
            for (int y = -lightRange; y <= lightRange; y++)
            {
                int tileX = light.position.x + x;
                int tileY = light.position.y + y;
    
                if (dungeonMapManager.IsWithinBounds(tileX, tileY))
                {
                    DungeonTile targetTile = dungeonMapManager.dungeonTiles[tileX][tileY];
                    List<Vector2Int> line = GetLine(light.position, new Vector2Int(tileX, tileY));
                    float accumulatedTransparency = 1.0f; // 初期値は完全な光
                    foreach (var point in line)
                    {
                        if (point == new Vector2Int(tileX, tileY))
                        {
                            // 光の最終的な減衰率を適用
                            Color lightContribution = light.color * light.intensity * accumulatedTransparency;
                            targetTile.accumulatedColor = Color.black;
                            targetTile.accumulatedColor += new Color(lightContribution.r, lightContribution.g, lightContribution.b, 0);
                            break;
                        }
                        DungeonTile currentTile = dungeonMapManager.dungeonTiles[point.x][point.y];
                        
                        accumulatedTransparency *= currentTile.transparency; // 各タイルの透過率を掛け合わせて減衰率を計算
                    }
                }
            }
        }
        
    }



    private void ApplyLightToTile(LightSource light, DungeonTile tile)
    {
        float distance = Vector2Int.Distance(light.position, new Vector2Int(tile.x, tile.y));
        float attenuation = 1 - (distance / light.range);
        Color lightContribution = light.color * light.intensity * attenuation;

        // 透過率を考慮して光を蓄積
        // RGBのみを蓄積
        if(tile.GetTileType() == TileType.Empty) Debug.Log($"transparency is :{tile.transparency}");
        tile.accumulatedColor += new Color(lightContribution.r, lightContribution.g, lightContribution.b, 0) * tile.transparency;
    }

    private List<DungeonTile> GetAffectedTiles(LightSource light)
    {
        List<DungeonTile> tiles = new List<DungeonTile>();
        int lightRange = Mathf.CeilToInt(light.range);
        for (int x = -lightRange; x <= lightRange; x++)
        {
            for (int y = -lightRange; y <= lightRange; y++)
            {
                int tileX = light.position.x + x;
                int tileY = light.position.y + y;

                if (dungeonMapManager.IsWithinBounds(tileX, tileY))
                {
                    DungeonTile tile = dungeonMapManager.dungeonTiles[tileX][tileY];
                    float distance = Vector2Int.Distance(light.position, new Vector2Int(tileX, tileY));

                    if (distance <= light.range && !IsTileBlocked(light.position, new Vector2Int(tileX, tileY)))
                    {
                        tiles.Add(tile);
                    }
                }
            }
        }
        return tiles;
    }

    private bool IsTileBlocked(Vector2Int from, Vector2Int to)
    {
        // 光の伝播経路に障害物があるかをチェック（Bresenhamの直線アルゴリズムを使用）
        List<Vector2Int> line = GetLine(from, to);
        foreach (var point in line)
        {
            if (point == to)
                break;

            if (!dungeonMapManager.IsWithinBounds(point.x, point.y))
                return true;

            DungeonTile tile = dungeonMapManager.dungeonTiles[point.x][point.y];
            if (tile.GetTileType() == TileType.Wall)
                return true;
        }
        return false;
    }

    private List<Vector2Int> GetLine(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> line = new List<Vector2Int>();

        int x0 = from.x;
        int y0 = from.y;
        int x1 = to.x;
        int y1 = to.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            line.Add(new Vector2Int(x0, y0));

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return line;
    }
}
