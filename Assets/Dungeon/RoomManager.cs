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
    [Header("Fixed Room")]
    public List<FixedRoom> fixedRooms = new List<FixedRoom>();
    [Header("Room Generators")]
    public List<RoomGeneratorBase> roomGenerators; // インスペクターから設定可能

    public SpawnManager spawnManager;
    private AssignRoomTypes assignRoomTypes = new AssignRoomTypes();
    private FixedRoomManager fixedRoomManager;
    private EntranceExitManager entranceExitManager = new EntranceExitManager();

    public List<Room> rooms;
    public void init(){
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

    public void GenerateRooms(){
        init();
        if (rooms == null || rooms.Count == 0){
            Debug.LogError("No Room");
            return;
        }
        foreach (Room room in rooms)
        {
            if(room.isFixed){
                GenerateFixedRoom(room);
            }
            else{
                GenerateRandomRoom(room);
            }
        }
    }

    public void AssignSpawnData(){
        foreach (Room room in rooms){
            RoomTypeSetting rts = roomTypeSettings.FirstOrDefault(setting => setting.roomType == room.roomType);
            if(rts != null){
            // NPC
            if(rts.npcSettings != null) spawnManager.AssignNPCData(room, rts.npcSettings);
            // enemy
            if(rts.enemySettings != null) spawnManager.AssignEnemyData(room, rts.enemySettings);
            // special
            if(rts.specialObjectSettings != null)spawnManager.AssignSpecialObjectData(room, rts.specialObjectSettings);
            }
        }
    }

    private void GenerateRandomRoom(Room room){
        RoomGeneratorBase generator = roomGenerators[Random.Range(0, roomGenerators.Count)];
        generator.GenerateRoom(room);
        entranceExitManager.AssignEntrancesAndExits(room, generator.entranceOptions, generator.exitOptions, generator.entranceCount, generator.exitCount);
    }
    private void GenerateFixedRoom(Room room){
        if(fixedRoomManager.Generate(room)){
            return;
        }
        else{
            GenerateRandomRoom(room);
        }
    }

}