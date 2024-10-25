using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "FixedRoom", menuName = "Dungeon/Room/FixedRoom")]
public class FixedRoom : ScriptableObject
{
    public Vector2Int size;
    public RoomType roomType;
    public int priority = 1;

    [HideInInspector]
    public List<List<Tile>> tiles = new List<List<Tile>>();
    [HideInInspector]
    public GameObject prefab;

    // 入り口と出口
    public List<(int x, int y, int priority)> entrances = new List<(int, int, int)>();
    public List<(int x, int y, int priority)> exits = new List<(int, int, int)>();

    public void ExtractTileData(){
        // プレハブを一時的にインスタンス化して Tilemap データを取得
        GameObject tempPrefab = Instantiate(prefab);
        Tilemap tilemap = tempPrefab.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("FixedRoom: Prefab に Tilemap コンポーネントが見つかりません。");
            DestroyImmediate(tempPrefab);
            return;
        }

        tiles.Clear();
        entrances.Clear();
        exits.Clear();

        // Tilemapの範囲を取得
        BoundsInt bounds = tilemap.cellBounds;
        size = new Vector2Int(bounds.size.x, bounds.size.y);
        for (int y = 0; y < size.y; y++){
            for (int x = 0; x < size.x; x++){
                Vector3Int tilePos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase tileBase = tilemap.GetTile(tilePos);
                Tile tile = tileBase as Tile;
            }
        }

    }
}
