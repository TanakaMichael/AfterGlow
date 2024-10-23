using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Area Selection Settings")]
    public AreaSelectionType selectionMethod = AreaSelectionType.CenterFocused;
    public int numberOfRoomsToSelect = 5;
    [Header("Room Type Settings")]
    public List<RoomTypeSetting> roomTypeSettings;
    private AssignRoomTypes assignRoomTypes = new AssignRoomTypes();
    private RoomSelector roomSelector = new RoomSelector();

    private List<Room> rooms;

    public void AssignRoomType(){
        assignRoomTypes.Assign(rooms, roomTypeSettings);
    }
    public void AssignRoomPositions(List<Area> areas){
        roomSelector.SelectAreasForRooms()
    }
}
