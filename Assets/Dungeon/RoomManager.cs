using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Area Selection Settings")]
    public AreaSelectionType selectionMethod = AreaSelectionType.CenterFocused;
    public int numberOfRoomsToSelect = 5;
    [Header("Room Type Settings")]
    public RoomType defaultRoomType = RoomType.Standard;
    public List<RoomTypeSetting> roomTypeSettings;
    private AssignRoomTypes assignRoomTypes = new AssignRoomTypes();
    private RoomSelector roomSelector = new RoomSelector();

    private List<Room> rooms;

    public void AssignRoomType(){
        assignRoomTypes.Assign(rooms, roomTypeSettings, defaultRoomType);
    }
    public void AssignRoomPositions(List<Area> areas){
        List<Area> selectArea = roomSelector.SelectAreasForRooms(areas);
        rooms = roomSelector.GenerateRoomsFromAreas(selectArea); 
    }

    public void GenerateRoom(){
        
    }
}
