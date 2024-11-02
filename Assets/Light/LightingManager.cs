using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [Header("References")]
    public DungeonMapManager dungeonMapManager;  // DungeonMapManagerへの参照
    public Transform player;  // プレイヤーのTransform
    public SpecialObjectManager specialObjectManager;  // SpecialObjectManagerへの参照

    [Header("Player Light Settings")]
    public float playerLightRadius = 5f;    // プレイヤーの光の半径
    public float playerLightIntensity = 1f; // プレイヤーの光の強さ
    public bool playerCanOverbright = false; // プレイヤーの光がオブジェクトを元の色より明るくできるか
    public Color playerLightColor = Color.white; // プレイヤーの光の色

    [Header("Static Objects")]
    public List<GameObject> staticObjects; // 静的なオブジェクト

    [Header("Dynamic Objects")]
    public List<GameObject> dynamicObjects; // 動的なオブジェクト（NPCや敵など）

    // 光源のリスト
    private List<LightSource> lightSources = new List<LightSource>();
    private List<LightSource> staticLightSources = new List<LightSource>();
    private List<LightSource> dynamicLightSources = new List<LightSource>();

    // 前回のプレイヤー光パラメータ
    private float previousPlayerLightRadius;
    private float previousPlayerLightIntensity;

    public void init()
    {
        // プレイヤーを光源リストに追加
        LightSource playerLight = new LightSource(
            player,
            playerLightRadius,
            playerLightIntensity,
            playerCanOverbright,
            playerLightColor
        );
        lightSources.Add(playerLight);
        dynamicLightSources.Add(playerLight);

        previousPlayerLightRadius = playerLightRadius;
        previousPlayerLightIntensity = playerLightIntensity;

        // SpecialObjectManagerの参照を取得
        if (specialObjectManager == null)
        {
            specialObjectManager = FindObjectOfType<SpecialObjectManager>();
            if (specialObjectManager == null)
            {
                Debug.LogError("LightingManager: SpecialObjectManagerがシーン内に見つかりませんでした。");
            }
        }

        // 特殊オブジェクトの光源を追加
        if (specialObjectManager != null)
        {
            foreach (var specialObject in specialObjectManager.specialObjectInstances)
            {
                AddLightSource(specialObject);
            }
        }

        // 静的オブジェクトのライティングを初期化
        UpdateStaticLighting();

        // 動的オブジェクトのライティングを定期的に更新
        StartCoroutine(UpdateDynamicLightingRoutine());
    }

    void Update()
    {
        // プレイヤーの光パラメータが変更された場合に更新
        if (playerLightRadius != previousPlayerLightRadius || playerLightIntensity != previousPlayerLightIntensity)
        {
            UpdatePlayerLightSource();
            UpdateStaticLighting(); // プレイヤー光源の変更時には静的ライティングも更新
            previousPlayerLightRadius = playerLightRadius;
            previousPlayerLightIntensity = playerLightIntensity;
        }
    }

    // プレイヤーの光源情報を更新するメソッド
    private void UpdatePlayerLightSource()
    {
        // プレイヤーの光源を更新
        LightSource playerLight = lightSources.Find(ls => ls.transform == player);
        if (playerLight != null)
        {
            playerLight.lightRadius = playerLightRadius;
            playerLight.lightIntensity = playerLightIntensity;
            playerLight.canOverbright = playerCanOverbright;
            playerLight.lightColor = playerLightColor;
        }
    }

    // 特定の光源を追加するメソッド
    public void AddLightSource(SpecialObjectInstance specialObjectInstance)
    {
        if (specialObjectInstance.lightIntensity > 0f)
        {
            LightSource lightSource = new LightSource(
                specialObjectInstance.gameObject.transform,
                specialObjectInstance.lightRadius,
                specialObjectInstance.lightIntensity,
                specialObjectInstance.canOverbright,
                specialObjectInstance.lightColor
            );

            lightSources.Add(lightSource);

            // 静的か動的かでリストを分ける（ここでは静的として扱う）
            staticLightSources.Add(lightSource);
        }
    }

    // 静的オブジェクトのライティングを更新するメソッド
    private void UpdateStaticLighting()
    {
        if (dungeonMapManager == null || dungeonMapManager.dungeonTiles == null)
        {
            Debug.LogError("LightingManager: DungeonMapManagerまたはdungeonTilesが設定されていません。");
            return;
        }
        ResetAllTileLighting();

        // タイルの明るさを初期化
        foreach (var column in dungeonMapManager.dungeonTiles)
        {
            foreach (var tile in column)
            {
                tile.lightIntensity = 0f;
                tile.accumulatedColor = Color.black;
            }
        }

        // 静的光源ごとに影響範囲を計算
        foreach (var lightSource in staticLightSources)
        {
            ApplyLightingFromSource(lightSource);
        }

        // タイルの明るさを更新
        foreach (var column in dungeonMapManager.dungeonTiles)
        {
            foreach (var tile in column)
            {
                SetTileBrightness(tile);
            }
        }

        // 静的オブジェクトの明るさを更新
        foreach (var obj in staticObjects)
        {
            SetObjectBrightness(obj);
        }
    }

    // 動的オブジェクトのライティングを定期的に更新するコルーチン
    private IEnumerator UpdateDynamicLightingRoutine()
    {
        while (true)
        {
            Debug.Log("asdfasdfasdfasdfasdfasdf");
            UpdateDynamicLighting();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void UpdateDynamicLighting()
{
    if (dungeonMapManager == null || dungeonMapManager.dungeonTiles == null)
    {
        Debug.LogError("LightingManager: DungeonMapManagerまたはdungeonTilesが設定されていません。");
        return;
    }
    ResetAllTileLighting();

    // 動的光源によって影響を受けるタイルの明るさをリセット
    foreach (var lightSource in dynamicLightSources)
    {
        ResetTileLightingInRange(lightSource);
    }

    // 動的光源からのライティングをタイルに適用
    foreach (var lightSource in dynamicLightSources)
    {
        ApplyLightingFromSource(lightSource);
    }

    // タイルの明るさを更新
    foreach (var lightSource in dynamicLightSources)
    {
        UpdateTileBrightnessInRange(lightSource);
    }

    // 動的オブジェクトの明るさを更新
    foreach (var obj in dynamicObjects)
    {
        SetObjectBrightness(obj);
    }

    // プレイヤーの明るさを更新
    SetObjectBrightness(player.gameObject);
}


    private void ApplyLightingFromSource(LightSource lightSource)
{
    int width = dungeonMapManager.mapWidth;
    int height = dungeonMapManager.mapHeight;

    int minX = Mathf.Max(0, Mathf.FloorToInt(lightSource.transform.position.x - lightSource.lightRadius));
    int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(lightSource.transform.position.x + lightSource.lightRadius));
    int minY = Mathf.Max(0, Mathf.FloorToInt(lightSource.transform.position.y - lightSource.lightRadius));
    int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(lightSource.transform.position.y + lightSource.lightRadius));

    Vector2 lightPosition = new Vector2(lightSource.transform.position.x, lightSource.transform.position.y);

    for (int x = minX; x <= maxX; x++)
    {
        for (int y = minY; y <= maxY; y++)
        {
            DungeonTile tile = dungeonMapManager.dungeonTiles[x][y];
            Vector2 tilePosition = new Vector2(tile.x, tile.y);
            float distance = Vector2.Distance(lightPosition, tilePosition);

            if (distance <= lightSource.lightRadius)
            {
                // 距離に基づいて減衰率を計算
                float attenuation = CalculateAttenuation(lightSource.transform.position, new Vector3(tile.x, tile.y, 0));
                if (attenuation > 0f)
                {
                    float distanceFactor = 1f - (distance / lightSource.lightRadius);
                    float intensity = lightSource.lightIntensity * attenuation * distanceFactor;

                    // 明るさを0以上に保つ
                    intensity = Mathf.Max(0f, intensity);

                    tile.lightIntensity += intensity;
                    tile.accumulatedColor += lightSource.lightColor * intensity;
                }
            }
        }
    }
}


    // 光の減衰を計算するメソッド
    private float CalculateAttenuation(Vector3 fromPosition, Vector3 toPosition)
    {
        float attenuation = 1f; // 初期光強度
        float minIntensityThreshold = 0.01f; // 最小強度の閾値

        // Bresenhamのアルゴリズムで経路上のタイルを取得
        int x0 = Mathf.RoundToInt(fromPosition.x);
        int y0 = Mathf.RoundToInt(fromPosition.y);
        int x1 = Mathf.RoundToInt(toPosition.x);
        int y1 = Mathf.RoundToInt(toPosition.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (IsWithinBounds(x0, y0))
            {
                DungeonTile currentTile = dungeonMapManager.dungeonTiles[x0][y0];
                attenuation *= currentTile.transparency;

                if (attenuation <= minIntensityThreshold)
                {
                    return 0f;
                }
            }
            else
            {
                return 0f; // マップ外の場合、光が届かない
            }

            if (x0 == x1 && y0 == y1)
            {
                break;
            }

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

        return attenuation;
    }

    /* タイルの明るさを設定するメソッド
    private void SetTileBrightness(DungeonTile tile)
    {
        if (tile.tileGameObject != null)
        {
            SpriteRenderer sr = tile.tileGameObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 明るさと色を調整して設定
                Color originalColor = sr.color;

                // タイルの最大明るさを定義（必要に応じて調整）
                float maxTileLightIntensity = 2f;

                // 明るさを 0 ～ 1 の範囲に正規化
                float normalizedIntensity = Mathf.Clamp01(tile.lightIntensity / maxTileLightIntensity);

                // 累積色を正規化
                Color finalColor = tile.accumulatedColor / tile.lightIntensity;

                Color newColor = Color.Lerp(originalColor * normalizedIntensity, finalColor, normalizedIntensity);
                newColor.a = originalColor.a; // 元のアルファ値を保持
                sr.color = newColor;


            }
        }
    }*/
    private void SetTileBrightness(DungeonTile tile)
    {
        if (tile.tileGameObject != null)
        {
            SpriteRenderer sr = tile.tileGameObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 明るさと累積色を適用
                float maxTileLightIntensity = 1f;
                float normalizedIntensity = Mathf.Clamp01(tile.lightIntensity / maxTileLightIntensity);

                // 累積された色を正規化
                Color finalColor;
                if (tile.lightIntensity > 0f)
                {
                    finalColor = tile.accumulatedColor / tile.lightIntensity;
                }
                else
                {
                    finalColor = Color.black;
                }

                // 初期色と光の色をブレンド
                Color newColor = Color.Lerp(tile.initialColor, finalColor, normalizedIntensity);
                newColor.a = tile.initialColor.a; // アルファ値を保持
                sr.color = newColor;
            }
        }
    }



    // オブジェクトの明るさを設定するメソッド
    private void SetObjectBrightness(GameObject obj)
    {
        if (obj == null)
            return;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float intensity = CalculateLightIntensityAtPosition(obj.transform.position, out Color accumulatedColor);

            Color originalColor = sr.color;

            // 元の色より明るくできるかどうか
            bool canOverbright = CanOverbrightAtPosition(obj.transform.position);

            // オブジェクトの最大明るさを定義（必要に応じて調整）
            float maxObjectLightIntensity = 2f;

            // 明るさを 0 ～ 1 の範囲に正規化
            float normalizedIntensity = Mathf.Clamp01(intensity / maxObjectLightIntensity);

            if (canOverbright)
            {
                // 明るさをそのまま適用
                Color newColor = accumulatedColor * normalizedIntensity;
                newColor.a = originalColor.a;
                sr.color = newColor;
            }
            else
            {
                // オブジェクトの元の色と光源の色をブレンド
                Color newColor = Color.Lerp(originalColor, accumulatedColor, normalizedIntensity);
                newColor.a = originalColor.a;
                sr.color = newColor;
            }

        }
    }

    // 指定座標での光の強度を計算するメソッド
    private float CalculateLightIntensityAtPosition(Vector3 position, out Color accumulatedColor)
    {
        float totalIntensity = 0f;
        accumulatedColor = Color.black;

        foreach (var lightSource in lightSources)
        {
            Vector3 lightPosition = lightSource.transform.position;
            float distance = Vector3.Distance(lightPosition, position);

            if (distance <= lightSource.lightRadius)
            {
                float attenuation = CalculateAttenuation(lightPosition, position);
                if (attenuation > 0f)
                {
                    float distanceFactor = 1f - (distance / lightSource.lightRadius);
                    float intensity = lightSource.lightIntensity * attenuation * distanceFactor;

                    // 明るさが負になるのを防ぐ
                    intensity = Mathf.Max(0f, intensity);

                    accumulatedColor += lightSource.lightColor * intensity;
                    totalIntensity += intensity;
                }
            }

        }

        if (totalIntensity > 0f)
        {
            accumulatedColor /= totalIntensity;
        }
        else
        {
            accumulatedColor = Color.black;
        }

        return totalIntensity;
    }

    // 指定座標での光源がオーバーブライト可能か判定するメソッド
    private bool CanOverbrightAtPosition(Vector3 position)
    {
        foreach (var lightSource in lightSources)
        {
            Vector3 lightPosition = lightSource.transform.position;
            float distance = Vector3.Distance(lightPosition, position);

            if (distance <= lightSource.lightRadius)
            {
                float attenuation = CalculateAttenuation(lightPosition, position);
                if (attenuation > 0f && lightSource.canOverbright)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 指定座標がマップ範囲内か確認するメソッド
    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < dungeonMapManager.mapWidth && y >= 0 && y < dungeonMapManager.mapHeight;
    }
    private void ResetTileLightingInRange(LightSource lightSource)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(lightSource.transform.position.x - lightSource.lightRadius));
        int maxX = Mathf.Min(dungeonMapManager.mapWidth - 1, Mathf.CeilToInt(lightSource.transform.position.x + lightSource.lightRadius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(lightSource.transform.position.y - lightSource.lightRadius));
        int maxY = Mathf.Min(dungeonMapManager.mapHeight - 1, Mathf.CeilToInt(lightSource.transform.position.y + lightSource.lightRadius));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                DungeonTile tile = dungeonMapManager.dungeonTiles[x][y];
                tile.lightIntensity = 0f;
                tile.accumulatedColor = Color.black;
            }
        }
    }
    private void ResetAllTileLighting()
    {
        foreach (var column in dungeonMapManager.dungeonTiles)
        {
            foreach (var tile in column)
            {
                tile.lightIntensity = 0f;
                tile.accumulatedColor = Color.black;
            }
        }
    }

    private void UpdateTileBrightnessInRange(LightSource lightSource)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(lightSource.transform.position.x - lightSource.lightRadius));
        int maxX = Mathf.Min(dungeonMapManager.mapWidth - 1, Mathf.CeilToInt(lightSource.transform.position.x + lightSource.lightRadius));
        int minY = Mathf.Max(0, Mathf.FloorToInt(lightSource.transform.position.y - lightSource.lightRadius));
        int maxY = Mathf.Min(dungeonMapManager.mapHeight - 1, Mathf.CeilToInt(lightSource.transform.position.y + lightSource.lightRadius));

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                DungeonTile tile = dungeonMapManager.dungeonTiles[x][y];
                SetTileBrightness(tile);
            }
        }
    }


}

// 光源を表すクラス
public class LightSource
{
    public Transform transform;
    public float lightRadius;
    public float lightIntensity;
    public bool canOverbright;
    public Color lightColor;

    public LightSource(Transform transform, float lightRadius, float lightIntensity, bool canOverbright, Color lightColor)
    {
        this.transform = transform;
        this.lightRadius = lightRadius;
        this.lightIntensity = lightIntensity;
        this.canOverbright = canOverbright;
        this.lightColor = lightColor;
    }
}
