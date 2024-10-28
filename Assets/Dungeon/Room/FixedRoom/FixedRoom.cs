using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 固定部屋を表すクラス
/// </summary>
[CreateAssetMenu(fileName = "FixedRoom", menuName = "Dungeon/Room/FixedRoom")]
public class FixedRoom : ScriptableObject
{
    public Vector2Int size; // 部屋のサイズ (幅, 高さ)
    public Vector2Int baseSize;
    public Vector2Int basePosition;

    public RoomType roomType; // 部屋の種類
    public int priority = 1; // 挿入の優先度（高いほど優先的に選ばれる）

    public List<List<CustomTile>> tiles = new List<List<CustomTile>>(); // タイル情報を保持

    // プレハブ情報
    public GameObject prefab;

    // 入り口と出口のリスト (x, y, priority)
    public List<(int x, int y, int priority)> entrances = new List<(int, int, int)>();
    public List<(int x, int y, int priority)> exits = new List<(int, int, int)>();

    /// <summary>
    /// Tilemap からデータを抽出するメソッド
    /// </summary>
    public void ExtractTileData()
    {
        if (prefab == null)
        {
            Debug.LogError("FixedRoom: Prefab が割り当てられていません。");
            return;
        }

        // プレハブを一時的にインスタンス化して Tilemap データを取得
        GameObject tempPrefab = Instantiate(prefab);
        Tilemap tilemap = tempPrefab.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("FixedRoom: Prefab に Tilemap コンポーネントが見つかりません。");
            DestroyImmediate(tempPrefab);
            return;
        }

        // データをクリア
        tiles.Clear();
        entrances.Clear();
        exits.Clear();

        // Tilemap の範囲を取得し、サイズを設定
        BoundsInt bounds = tilemap.cellBounds;
        size = new Vector2Int(bounds.size.x, bounds.size.y);

        // タイル情報を抽出
        for (int y = 0; y < size.y; y++)
        {
            List<CustomTile> rowTiles = new List<CustomTile>();

            for (int x = 0; x < size.x; x++)
            {
                Vector3Int tilePos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                // ジェネリック型を使用してタイルを取得
                CustomTile customTile = tilemap.GetTile<CustomTile>(tilePos);

                if (customTile != null)
                {
                    rowTiles.Add(customTile);
                    // 入口と出口の情報を取得
                    if (customTile.isEntrance)
                    {
                        entrances.Add((x, y, customTile.priority));
                    }
                    if (customTile.isExit)
                    {
                        exits.Add((x, y, customTile.priority));
                    }
                }
                else
                {
                    // タイルが存在しないか、CustomTileでない場合はデフォルト値
                    rowTiles.Add(null);
                }
            }
            tiles.Add(rowTiles);
        }

        // 一時的に生成したプレハブを削除
        DestroyImmediate(tempPrefab);
    }

    /// <summary>
    /// FixedRoom から Room クラスに変換するメソッド
    /// </summary>
    /// <param name="roomId">生成する Room の ID</param>
    /// <param name="areaId">関連付けるエリアの ID</param>
    /// <param name="position">エリア内での配置位置</param>
    /// <returns>生成された Room インスタンス</returns>
    public Room ToRoom(int roomId, Area area, Vector2Int position)
    {
        // Tile データがまだ抽出されていない場合は抽出する
        if (!isDataExtracted)
        {
            ExtractTileData();
            isDataExtracted = true;
        }
    
        // 新しい Room を作成
        Room room = new Room(roomId, position.x, position.y, size.x, size.y, area);
    
        // FixedRoom のデータを Room に適用
        room.ApplyFixedRoomData(this);
    
        return room;
    }

    // フラグを追加
    [HideInInspector]
    public bool isDataExtracted = false;

    /// <summary>
    /// FixedRoom の一覧から Room を生成する静的メソッド
    /// </summary>
    /// <param name="fixedRooms">すべての FixedRoom のリスト</param>
    /// <param name="desiredType">生成したい RoomType</param>
    /// <param name="area">部屋を配置するエリア</param>
    /// <param name="roomId">生成する Room の ID</param>
    /// <returns>生成された Room インスタンス、該当するものがない場合は null</returns>
    public static Room CreateRoomFromFixedRooms(List<FixedRoom> fixedRooms, RoomType desiredType, Area area, int roomId)
    {
        // 1. isFixed が true で RoomType が desiredType の FixedRoom をフィルタリング
        List<FixedRoom> filteredRooms = fixedRooms.FindAll(fr => fr.roomType == desiredType);

        if (filteredRooms.Count == 0)
        {
            Debug.LogWarning($"No FixedRoom found with RoomType {desiredType} and isFixed=true.");
            return null;
        }

        // 2. エリアのサイズに最も近い FixedRoom を選択
        FixedRoom selectedFixedRoom = null;
        float smallestSizeDifference = Mathf.Infinity;
        foreach (var fr in filteredRooms)
        {
            float sizeDifference = Mathf.Abs(fr.size.x * fr.size.y - area.width * area.height);
            if (sizeDifference < smallestSizeDifference)
            {
                smallestSizeDifference = sizeDifference;
                selectedFixedRoom = fr;
            }
        }

        if (selectedFixedRoom == null)
        {
            Debug.LogWarning("No suitable FixedRoom found based on size.");
            return null;
        }

        // 3. エリア内での配置位置を決定（ここではエリアの中心に配置）
        int minX = area.x;
        int maxX = area.x + area.width - selectedFixedRoom.size.x;
        int minY = area.y;
        int maxY = area.y + area.height - selectedFixedRoom.size.y;

        // 配置位置がエリア内に収まるように調整
        int roomX = Random.Range(minX, maxX + 1);
        int roomY = Random.Range(minY, maxY + 1);

        Vector2Int position = new Vector2Int(roomX, roomY);

        // 4. FixedRoom から Room を生成
        Room newRoom = selectedFixedRoom.ToRoom(roomId, area, position);


        return newRoom;
    }
}
