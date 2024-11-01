using System.Collections.Generic;
using UnityEngine;

public class FixedRoomManager
{
    private List<FixedRoom> fixedRooms;

    public FixedRoomManager(List<FixedRoom> fixedRooms)
    {
        this.fixedRooms = fixedRooms;
    }
    public bool Generate(Room room){
        // FixedRoomManagerを使用
        FixedRoom generatedFixedRoom = GenerateFixedRoom(room);
        if (generatedFixedRoom != null){
            room.ApplyFixedRoomData(generatedFixedRoom);
            return true; // 成功
        }
        else{
            Debug.Log("Failed to generate Fixed Room");
            return false; // 失敗
        }
    }
    /// <summary>
    /// 固定部屋を生成する
    /// </summary>
    public FixedRoom GenerateFixedRoom(Room room)
    {
        // 部屋のタイプとサイズがエリアに収まるFixedRoomをフィルタリング
        List<FixedRoom> matchingRooms = fixedRooms.FindAll(fr =>
            (fr.roomType == room.roomType || fr.roomType == RoomType.Free || fr.roomType == RoomType.Empty) &&
            fr.size.x <= room.width &&
            fr.size.y <= room.height
        );

        if (matchingRooms.Count == 0)
        {
            Debug.LogWarning($"No FixedRoom found for RoomType {room.roomType} that fits in the area.");
            return null;
        }

        // エリアに最もフィットするFixedRoomを選択
        FixedRoom selectedFixedRoom = null;
        int smallestSizeDifference = int.MaxValue;

        foreach (var fr in matchingRooms)
        {
            int sizeDifference = (room.width - fr.size.x) + (room.height - fr.size.y);
            if (sizeDifference < smallestSizeDifference)
            {
                smallestSizeDifference = sizeDifference;
                selectedFixedRoom = fr;
            }
        }

        // データを抽出（まだの場合）
        if (!selectedFixedRoom.isDataExtracted)
        {
            selectedFixedRoom.ExtractTileData();
        }
        selectedFixedRoom.baseSize.x = room.width;
        selectedFixedRoom.baseSize.y = room.height;

        selectedFixedRoom.basePosition.x = room.x;
        selectedFixedRoom.basePosition.y = room.y;
        return selectedFixedRoom;
    }
}

