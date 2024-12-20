using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// RoomManagerとCorridorManagerから取得した情報をGizmosで表示するクラス
/// </summary>
[ExecuteAlways]
public class MapGizmosDrawer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("マップを管理するRoomManagerへの参照")]
    public RoomManager roomManager;
    [Tooltip("廊下を管理するCorridorManagerへの参照")]
    public CorridorManager corridorManager;

    [Header("Gizmos Settings")]
    public Color roomColor = Color.green;
    public Color corridorColor = Color.cyan;
    public Color entranceColor = Color.blue;
    public Color exitColor = Color.red;
    public Color wallColor = Color.gray;
    public Color floorColor = Color.white;

    public float roomLineWidth = 0.1f;
    public float entranceSize = 0.2f;
    public float exitSize = 0.2f;
    public float tileSize = 1f; // タイルのサイズ
    public bool draw = false; // デバッグ情報を表示するかどうか

    private void OnDrawGizmos()
    {
        if (!draw) return;
        if (roomManager == null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 5); // デバッグ用
            return;
        }

        // RoomManagerが初期化されていることを確認
        if (roomManager.rooms == null || roomManager.rooms.Count == 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 5); // デバッグ用
            return;
        }

        // 各部屋を描画
        foreach (Room room in roomManager.rooms)
        {
            DrawRoom(room);
            DrawTiles(room);
            DrawEntrances(room);
            DrawExits(room);
            DrawRoomInfo(room);
        }

        // CorridorManagerが設定されている場合、廊下を描画
        if (corridorManager != null && corridorManager.corridors != null)
        {
            foreach (Corridor corridor in corridorManager.corridors)
            {
                DrawCorridor(corridor);
            }
        }
    }

    /// <summary>
    /// 部屋の輪郭をGizmosで描画する
    /// </summary>
    /// <param name="room">対象のRoom</param>
    private void DrawRoom(Room room)
    {
        // RoomTypeに応じた色を設定
        switch (room.roomType)
        {
            case RoomType.Standard:
                Gizmos.color = roomColor;
                break;
            case RoomType.Treasure:
                Gizmos.color = Color.yellow;
                break;
            case RoomType.Boss:
                Gizmos.color = Color.magenta;
                break;
            default:
                Gizmos.color = roomColor;
                break;
        }

        // 部屋の中心とサイズを計算
        Vector3 roomCenter = new Vector3(room.x + room.width / 2f, room.y + room.height / 2f, 0);
        Vector3 roomSize = new Vector3(room.width, room.height, 0.1f);

        // ワイヤーフレームキューブで部屋の輪郭を描画
        Gizmos.DrawWireCube(roomCenter, roomSize);
    }

    /// <summary>
    /// 部屋内のタイルをGizmosで描画する
    /// </summary>
    /// <param name="room">対象のRoom</param>
    private void DrawTiles(Room room)
    {
        for (int rowIndex = 0; rowIndex < room.tiles.Count; rowIndex++)
        {
            List<TileData> row = room.tiles[rowIndex];
            for (int colIndex = 0; colIndex < row.Count; colIndex++)
            {
                TileData tile = row[colIndex];
                Vector2Int tilePosition = new Vector2Int(colIndex, rowIndex); // インデックスから位置を計算

                Vector3 worldPosition = new Vector3(room.x + tilePosition.x, room.y + tilePosition.y, 0);

                // タイルの種類やプロパティに応じた色を設定
                if (tile.isWalkable)
                {
                    Gizmos.color = floorColor;
                }
                else
                {
                    Gizmos.color = wallColor;
                }

                // タイルを四角形で描画
                Gizmos.DrawCube(worldPosition + new Vector3(tileSize / 2, tileSize / 2, 0), Vector3.one * tileSize * 0.9f);

                // SpecialObjectが存在する場合、アイコンを表示
                if (tile.spawnSpecialObjectSettings != null && tile.spawnSpecialObjectSettings.isEnable)
                {
                    #if UNITY_EDITOR
                    Handles.Label(worldPosition + new Vector3(tileSize / 2, tileSize / 2, 0), "S");
                    #endif
                }

                // Enemyが存在する場合、別のアイコンを表示
                if (tile.spawnEnemySettings != null && tile.spawnEnemySettings.isEnable)
                {
                    #if UNITY_EDITOR
                    Handles.Label(worldPosition + new Vector3(tileSize / 2, tileSize / 2, 0), "E");
                    #endif
                }

                // NPCが存在する場合、さらに別のアイコンを表示
                if (tile.spawnNPCSettings != null && tile.spawnNPCSettings.isEnable)
                {
                    #if UNITY_EDITOR
                    Handles.Label(worldPosition + new Vector3(tileSize / 2, tileSize / 2, 0), "N");
                    #endif
                }
            }
        }
    }

    /// <summary>
    /// 部屋の入口をGizmosで描画する
    /// </summary>
    /// <param name="room">対象のRoom</param>
    private void DrawEntrances(Room room)
    {
        Gizmos.color = entranceColor;
        foreach (var entrance in room.entrances)
        {
            Vector3 entrancePosition = new Vector3(room.x + entrance.x + 0.5f, room.y + entrance.y + 0.5f, 0);
            Gizmos.DrawSphere(entrancePosition, entranceSize);
        }
    }

    /// <summary>
    /// 部屋の出口をGizmosで描画する
    /// </summary>
    /// <param name="room">対象のRoom</param>
    private void DrawExits(Room room)
    {
        Gizmos.color = exitColor;
        foreach (var exit in room.exits)
        {
            Vector3 exitPosition = new Vector3(room.x + exit.x + 0.5f, room.y + exit.y + 0.5f, 0);
            Gizmos.DrawCube(exitPosition, Vector3.one * exitSize);
        }
    }

    /// <summary>
    /// 部屋の情報をGizmosで表示する（テキストなど）
    /// </summary>
    /// <param name="room">対象のRoom</param>
    private void DrawRoomInfo(Room room)
    {
        #if UNITY_EDITOR
        // 部屋の中心にラベルを表示
        Vector3 roomCenter = new Vector3(room.x + room.width / 2f, room.y + room.height / 2f, 0);
        Handles.Label(roomCenter, $"ID: {room.id}\nType: {room.roomType}");
        #endif
    }

    /// <summary>
    /// 廊下をGizmosで描画する
    /// </summary>
    /// <param name="corridor">対象のCorridor</param>
    private void DrawCorridor(Corridor corridor)
    {
        if (corridor.tiles == null || corridor.tiles.Count == 0)
            return;

        // 廊下のタイルを描画
        foreach (TileData tile in corridor.tiles)
        {
            Vector3 worldPosition = new Vector3(tile.x, tile.y, 0);

            // タイルの種類やプロパティに応じた色を設定
            if (tile.isWalkable)
            {
                Gizmos.color = corridorColor;
            }
            else
            {
                Gizmos.color = wallColor;
            }

            // タイルを四角形で描画
            Gizmos.DrawCube(worldPosition + new Vector3(tileSize / 2, tileSize / 2, 0), Vector3.one * tileSize * 0.9f);
        }

        #if UNITY_EDITOR
        // 廊下の経路を線で描画
        Gizmos.color = corridorColor;
        for (int i = 0; i < corridor.path.Count - 1; i++)
        {
            Vector3 start = new Vector3(corridor.path[i].x + 0.5f, corridor.path[i].y + 0.5f, 0);
            Vector3 end = new Vector3(corridor.path[i + 1].x + 0.5f, corridor.path[i + 1].y + 0.5f, 0);
            Gizmos.DrawLine(start, end);
        }
        #endif
    }
}
