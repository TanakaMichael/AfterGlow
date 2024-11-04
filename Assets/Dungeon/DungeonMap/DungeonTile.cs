// DungeonTile.cs
using Unity.VisualScripting;
using UnityEngine;

public class DungeonTile
{
    public int x, y; // タイルの座標
    public bool isWalkable = false;
    private TileType tileType;
    public TileType TileType
    {
        get { return tileType; }
        set
        {
            tileType = value;
            UpdateTransparency();
        }
    }
    public bool isEntrance = false;
    public bool isExit = false;
    public Room room; // このタイルが属する部屋（廊下の場合は null）

    // スポーン情報
    public SpawnSpecialObjectSettings spawnSpecialObjectSettings;
    public SpawnEnemySettings spawnEnemySettings;
    public SpawnNPCSettings spawnNPCSettings;

    // デザイン情報
    public DesignType designType = DesignType.None;
    public DesignCategory designCategory = DesignCategory.None;

    public float brightness = 0.2f; // タイルの明るさ（0.0(暗黒) ～ 1.0(通常) ～ 2.0(白色)）
    public Color accumulatedColor = Color.black; // タイルが受ける光の合計色
    public float transparency { get; private set; } = 1f; // タイルの透過率（1f：光を完全に通す ~ 0f：光を通さない）
    public Color initialColor = Color.white; // タイルの初期色
    public GameObject tileGameObject; // タイルの実際のゲームオブジェクト

    [Range(0f, 1f)]
    public int priority = 0; // 優先度

    // シェーダー用マテリアル（オプション）
    private Material tileMaterial;

    public DungeonTile(int x, int y, TileType tileType)
    {
        this.x = x;
        this.y = y;
        this.tileType = tileType;

        accumulatedColor = Color.black; // 初期化
    }
     private void UpdateTransparency()
    {
        // タイルの種類に応じて透明度を設定
        switch (tileType)
        {
            case TileType.Empty:
                transparency = 0.2f; // 光を通しにくくする
                break;
            case TileType.Wall:
                transparency = 0.3f; // 壁は光を通しにくくする
                break;
            case TileType.Floor:
                transparency = 0.8f; // 床は光を通しやすくする
                break;
            default:
                transparency = 0.8f; // その他のタイルの透明度
                break;
        }
    }

    // デザインタイプを設定するメソッド
    public void SetDesignType(DesignCategory designCategory)
    {
        this.designCategory = designCategory;

        switch (designCategory)
        {
            case DesignCategory.Floor:
                if (Random.value < 0.1f)
                    designType = DesignType.CrackedStoneFloor;
                else
                    designType = DesignType.StoneFloor;
                break;
            case DesignCategory.Wall:
                if (Random.value < 0.1f)
                    designType = DesignType.CrackedStoneWall;
                else
                    designType = DesignType.StoneWall;
                break;
            case DesignCategory.Corridor:
                if (Random.value < 0.1f)
                    designType = DesignType.CrackedStoneCorridor;
                else
                    designType = DesignType.StoneCorridor;
                break;
            case DesignCategory.SecretCorridor:
                designType = DesignType.SecretPassage;
                break;
            // その他のカテゴリ
            default:
                designType = DesignType.None;
                break;
        }
    }

    // 初期色を設定するメソッド
    public void SetInitialColor()
    {
        if (tileGameObject != null)
        {
            SpriteRenderer sr = tileGameObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                initialColor = sr.color;
                initialColor.a = 1f; // アルファを強制的に1に設定
                sr.color = initialColor; // 変更を適用
            }
        }
    }


     // 明るさを更新するメソッド
    public void UpdateBrightness()
    {
        if (accumulatedColor != Color.black)
        {
            // 明るさレベルの判定
            if (accumulatedColor.maxColorComponent < 0.3f)
                brightness = 0.0f; // 暗い
            else if (accumulatedColor.maxColorComponent < 1.0f)
                brightness = 1.0f; // 通常
            else
                brightness = 2.0f; // 真っ白

            // 明るさに基づく最終色の計算
            Color finalColor = initialColor;

            if (brightness == 0.0f)
            {
                // 暗い状態：タイルを暗くする
                finalColor *= 0.2f; // 例: 20% の明るさ
            }
            else if (brightness == 1.0f)
            {
                // 通常状態：初期色と累積色をブレンド
                finalColor = Color.Lerp(initialColor, accumulatedColor, 0.5f);
            }
            else if (brightness == 2.0f)
            {
                // 真っ白状態：累積色を強調
                finalColor = Color.Lerp(initialColor, accumulatedColor, 1.0f);
                finalColor = Color.Lerp(finalColor, Color.white, 0.5f); // さらに白に近づける
            }

            // RGBのみを計算し、アルファは保持
            finalColor.a = initialColor.a;

            // タイルの色を更新
            if (tileGameObject != null)
            {
                SpriteRenderer sr = tileGameObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = finalColor;
                }
            }
        }
        else
        {
            // 光が届いていない場合：暗く表示
            brightness = 0.0f;
            if (tileGameObject != null)
            {
                SpriteRenderer sr = tileGameObject.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color finalColor = initialColor * 0.2f; // 20% の明るさ
                    sr.color = finalColor;
                }
            }
        }
    }

    public TileType GetTileType(){
        return tileType;
    }

}
