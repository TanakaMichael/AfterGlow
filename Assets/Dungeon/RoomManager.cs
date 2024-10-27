using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    [Header("Area Selection Settings")]
    public RoomSelector roomSelector = new RoomSelector();
    [Header("Room Type Settings")]
    public RoomType defaultRoomType = RoomType.Standard;
    public List<RoomTypeSetting> roomTypeSettings;
    public RoomTypeSetting defaultSetting;
    [Header("Fixed Room")]
    public List<FixedRoom> fixedRooms = new List<FixedRoom>();
    [Header("Random Room")]
    public List<RandomRoomGenerator> randomRoomGenerators = new List<RandomRoomGenerator>();
    private AssignRoomTypes assignRoomTypes = new AssignRoomTypes();
    private RandomRoomManager randomRoomManager;
    private FixedRoomManager fixedRoomManager;

    public List<Room> rooms;
    public void init(){
        randomRoomManager = new RandomRoomManager(randomRoomGenerators);
        fixedRoomManager = new FixedRoomManager(fixedRooms);
    }

    
    public void AssignRoomType(){
        assignRoomTypes.Assign(rooms, roomTypeSettings, defaultRoomType);
    }
    public void AssignRoomPositions(List<Area> areas){
        List<Area> selectArea = roomSelector.SelectAreasForRooms(areas);
        GameManager.Log($"割り当て可能なエリアの数: {selectArea.Count}");
        rooms = roomSelector.GenerateRoomsFromAreas(selectArea); 
    }

    public void GenerateRoom(){
        init();
        if (rooms == null || rooms.Count == 0){
            Debug.LogError("No Room");
            return;
        }
        foreach(Room room in rooms){
            if (room.isFixed){
                if(!fixedRoomManager.Generate(room)) randomRoomManager.Generate(room);
            }
            else{
                if(randomRoomManager.Generate(room)) AssignEntrancesAndExits(room);
            }
        }


    }
    /// <summary>
    /// 部屋に入口と出口を配置する
    /// </summary>
    /// <param name="room">対象の Room インスタンス</param>
    private void AssignEntrancesAndExits(Room room)
    {
        // 部屋のタイプ設定を取得
        RoomTypeSetting roomTypeSetting = roomTypeSettings.Find(rts => rts.roomType == room.roomType);
        if (roomTypeSetting == null)
        {
            Debug.LogWarning($"No RoomTypeSetting found for RoomType {room.roomType}.");
            if(defaultSetting == null){
                Debug.LogError("No default RoomTypeSetting found.");
                return;
            }
            roomTypeSetting = defaultSetting;
        }
    
        // 入口を配置
        if (roomTypeSetting.entranceSettings != null)
        {
            int entranceCount = Random.Range(roomTypeSetting.entranceSettings.minCount, roomTypeSetting.entranceSettings.maxCount + 1);
            Debug.Log($"Assigning {entranceCount} entrances to Room ID {room.id}");
            for (int i = 0; i < entranceCount; i++)
            {
                List<Vector2Int> positions = GetSpawnPositionsOnWall(room, roomTypeSetting.entranceSettings.spawnPosition);
    
                foreach (var position in positions)
                {
                    if (position.y < 0 || position.y >= room.tiles.Count)
                    {
                        Debug.LogError($"Invalid position.y={position.y} for Room ID {room.id}. room.tiles.Count={room.tiles.Count}");
                        continue;
                    }
    
                    if (position.x < 0 || position.x >= room.tiles[position.y].Count)
                    {
                        Debug.LogError($"Invalid position.x={position.x} for Room ID {room.id}. room.tiles[{position.y}].Count={room.tiles[position.y].Count}");
                        continue;
                    }
    
                    if (IsValidWallPosition(room, position) && !IsOccupied(room, position))
                    {
                        room.entrances.Add((position.x, position.y, 1)); // 優先度を1と仮定
                        // タイルに入口フラグを設定
                        Debug.Log($"Entrance placed at ({position.x}, {position.y}) in Room ID {room.id}");
                        room.tiles[position.y][position.x].isEntrance = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to place entrance at ({position.x}, {position.y}) in Room ID {room.id}");
                    }
                }
            }
        }
    
        // 出口の配置も同様に処理
        if (roomTypeSetting.exitSettings != null)
        {
            int exitCount = Random.Range(roomTypeSetting.exitSettings.minCount, roomTypeSetting.exitSettings.maxCount + 1);
            Debug.Log($"Assigning {exitCount} exits to Room ID {room.id}");
            for (int i = 0; i < exitCount; i++)
            {
                List<Vector2Int> positions = GetSpawnPositionsOnWall(room, roomTypeSetting.exitSettings.spawnPosition);
    
                foreach (var position in positions)
                {
                    if (position.y < 0 || position.y >= room.tiles.Count)
                    {
                        Debug.LogError($"Invalid position.y={position.y} for Room ID {room.id}. room.tiles.Count={room.tiles.Count}");
                        continue;
                    }
    
                    if (position.x < 0 || position.x >= room.tiles[position.y].Count)
                    {
                        Debug.LogError($"Invalid position.x={position.x} for Room ID {room.id}. room.tiles[{position.y}].Count={room.tiles[position.y].Count}");
                        continue;
                    }
    
                    if (IsValidWallPosition(room, position) && !IsOccupied(room, position))
                    {
                        room.exits.Add((position.x, position.y, 1)); // 優先度を1と仮定
                        // タイルに出口フラグを設定
                        Debug.Log($"Exit placed at ({position.x}, {position.y}) in Room ID {room.id}");
                        room.tiles[position.y][position.x].isExit = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to place exit at ({position.x}, {position.y}) in Room ID {room.id}");
                    }
                }
            }
        }
    }


    private List<Vector2Int> GetSpawnPositionsOnWall(Room room, SpawnPosition spawnPosition)
    {
        List<Vector2Int> wallPositions = new List<Vector2Int>();

        // 上辺と下辺
        for (int x = 0; x < room.width; x++)
        {
            wallPositions.Add(new Vector2Int(x, 0)); // 上辺
            wallPositions.Add(new Vector2Int(x, room.height - 1)); // 下辺
        }

        // 左辺と右辺
        for (int y = 1; y < room.height - 1; y++) // 中央は重複防止
        {
            wallPositions.Add(new Vector2Int(0, y)); // 左辺
            wallPositions.Add(new Vector2Int(room.width - 1, y)); // 右辺
        }

        List<Vector2Int> selectedPositions = new List<Vector2Int>();

        switch (spawnPosition)
        {
            case SpawnPosition.Center:
                // 壁の中央に近い位置を選択
                float centerX = room.width / 2f;
                float centerY = room.height / 2f;
                Vector2Int centerPosition = new Vector2Int(Mathf.RoundToInt(centerX), Mathf.RoundToInt(centerY));
                selectedPositions.Add(centerPosition);
                break;

            case SpawnPosition.Edge:
                // 壁の端に近い位置をランダムに選択
                Vector2Int edgePosition = wallPositions[Random.Range(0, wallPositions.Count)];
                selectedPositions.Add(edgePosition);
                break;

            case SpawnPosition.Random:
                // ランダムに選択
                Vector2Int randomPosition = wallPositions[Random.Range(0, wallPositions.Count)];
                selectedPositions.Add(randomPosition);
                break;

            case SpawnPosition.Cross:
                // 各側面の中央に出入り口を設置
                Vector2Int topCenter = new Vector2Int(room.width / 2, 0);
                Vector2Int bottomCenter = new Vector2Int(room.width / 2, room.height - 1);
                Vector2Int leftCenter = new Vector2Int(0, room.height / 2);
                Vector2Int rightCenter = new Vector2Int(room.width - 1, room.height / 2);

                selectedPositions.Add(topCenter);
                selectedPositions.Add(bottomCenter);
                selectedPositions.Add(leftCenter);
                selectedPositions.Add(rightCenter);
                break;

            default:
                // デフォルトはランダムに選択
                Vector2Int defaultPosition = wallPositions[Random.Range(0, wallPositions.Count)];
                selectedPositions.Add(defaultPosition);
                break;
        }

        return selectedPositions;
    }



    /// <summary>
    /// 配置位置が部屋の壁上にあるかどうかをチェック
    /// </summary>
    private bool IsValidWallPosition(Room room, Vector2Int position)
    {
        return position.x == 0 || position.x == room.width - 1 || position.y == 0 || position.y == room.height - 1;
    }

    /// <summary>
    /// 配置位置が既に入口または出口として使用されているかどうかをチェック
    /// </summary>
    private bool IsOccupied(Room room, Vector2Int position)
    {
        foreach (var entrance in room.entrances)
        {
            if (entrance.x == position.x && entrance.y == position.y)
                return true;
        }

        foreach (var exit in room.exits)
        {
            if (exit.x == position.x && exit.y == position.y)
                return true;
        }

        return false;
    }
}
