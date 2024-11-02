using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    [Header("References")]
    public DungeonMapManager dungeonMapManager; // DungeonMapManager への参照

    [Header("Tile Sets")]
    public List<TileSet> tileSets; // 利用可能なタイルセットのリスト

    [Header("Selected Tile Set")]
    public int selectedTileSetIndex = 0; // 選択されたタイルセットのインデックス

    private Dictionary<DesignType, GameObject> designTypeToPrefab;

    public void GenerateLayout()
    {
        if (dungeonMapManager == null)
        {
            Debug.LogError("DungeonMapManager is not assigned.");
            return;
        }

        if (tileSets == null || tileSets.Count == 0)
        {
            Debug.LogError("No tile sets available.");
            return;
        }

        if (selectedTileSetIndex < 0 || selectedTileSetIndex >= tileSets.Count)
        {
            Debug.LogError("Selected tile set index is out of range.");
            return;
        }

        // 選択されたタイルセットを取得
        TileSet selectedTileSet = tileSets[selectedTileSetIndex];

        // DesignType とプレハブのマッピングを作成
        designTypeToPrefab = new Dictionary<DesignType, GameObject>();

        foreach (var tilePrefab in selectedTileSet.tilePrefabs)
        {
            if (!designTypeToPrefab.ContainsKey(tilePrefab.designType))
            {
                designTypeToPrefab.Add(tilePrefab.designType, tilePrefab.prefab);
            }
            else
            {
                GameManager.Log($"Duplicate DesignType {tilePrefab.designType} in tile set {selectedTileSet.setName}");
            }
        }
        GameManager.Log($"dungeonMap: {dungeonMapManager.dungeonTiles.Count}");
        // タイルの配置を開始
        foreach (var column in dungeonMapManager.dungeonTiles)
        {
            foreach (var tile in column)
            {
                PlaceTile(tile);
            }
        }
    }

    private void PlaceTile(DungeonTile tile)
    {
        if (tile == null)
            return;

        // タイルの DesignType に対応するプレハブを取得
        if (designTypeToPrefab.TryGetValue(tile.designType, out GameObject prefab))
        {
            // タイルの位置にプレハブをインスタンス化
            Vector3 position = new Vector3(tile.x, tile.y, 0);
            GameObject tileObj = Instantiate(prefab, position, Quaternion.identity, transform);
            tile.tileGameObject = tileObj; // タイルのゲームオブジェクトを設定
        }
        else
        {
            // 対応するプレハブが見つからない場合、警告を表示
            Debug.LogWarning($"No prefab found for DesignType {tile.designType}");
        }
    }

}
