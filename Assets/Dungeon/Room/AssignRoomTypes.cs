using System.Collections.Generic;
using UnityEngine;

public class AssignRoomTypes : MonoBehaviour
{
    public void Assign(List<Room> rooms, List<RoomTypeSetting> options, RoomType defaultRoomType)
    {
        // 必須の部屋タイプを先に割り当て
        foreach (var setting in options)
        {
            if (setting.isMandatory)
            {
                AssignMandatoryRoomType(rooms, setting);
            }
        }

        // EntranceとExitの割り当て
        AssignEntranceAndExit(rooms, options);

        // 必須の部屋とEntrance、Exitを割り当てた後の空き部屋を取得
        List<Room> availableRooms = new List<Room>(rooms.FindAll(r => r.roomType == RoomType.Empty));

        // 非必須の部屋タイプの割り当て数を計算
        Dictionary<RoomType, int> typeCounts = CalculateTypeCounts(availableRooms.Count, options);

        // 各部屋タイプを割り当て
        foreach (var typeCount in typeCounts)
        {
            RoomType roomType = typeCount.Key;
            int count = typeCount.Value;

            RoomTypeSetting setting = options.Find(o => o.roomType == roomType);

            for (int i = 0; i < count; i++)
            {
                if (availableRooms.Count == 0)
                {
                    Debug.LogWarning("部屋の数が不足しています。");
                    break;
                }

                Room selectedRoom = SelectRandomRoom(availableRooms);
                selectedRoom.roomType = roomType;

                // isFixedProbability に基づいて isFixed を設定
                if (Random.value < setting.isFixedProbability)
                {
                    selectedRoom.isFixed = true;
                }
                else
                {
                    selectedRoom.isFixed = false;
                }

                availableRooms.Remove(selectedRoom);
            }
        }

        // 残った空き部屋をデフォルトの部屋タイプに設定
        foreach (Room room in availableRooms)
        {
            room.roomType = defaultRoomType;

            // デフォルトの部屋タイプの設定を取得
            RoomTypeSetting defaultSetting = options.Find(o => o.roomType == defaultRoomType);

            // isFixedProbability に基づいて isFixed を設定
            if (defaultSetting != null && Random.value < defaultSetting.isFixedProbability)
            {
                room.isFixed = true;
            }
            else
            {
                room.isFixed = false;
            }
        }

        // デバッグ表示
        string str = "";
        foreach (var room in rooms)
        {
            str += $"id: {room.id}, area id: {room.areaId}, roomType: {room.roomType}, isFixed: {room.isFixed}\n";
        }
        GameManager.Log(str);
    }

    // 必須部屋の割り当て
    private void AssignMandatoryRoomType(List<Room> rooms, RoomTypeSetting setting)
    {
        List<Room> availableRooms = new List<Room>(rooms.FindAll(r => r.roomType == RoomType.Empty));
        if (availableRooms.Count == 0)
        {
            Debug.LogWarning("必須部屋を割り当てるための部屋が不足しています。");
            return;
        }

        int count = setting.minCount != -1 ? Mathf.Clamp(setting.minCount, 0, availableRooms.Count) : 1;
        for (int i = 0; i < count; i++)
        {
            Room selectedRoom = SelectRandomRoom(availableRooms);
            selectedRoom.roomType = setting.roomType;

            // isFixedProbability に基づいて isFixed を設定
            if (Random.value < setting.isFixedProbability)
            {
                selectedRoom.isFixed = true;
            }
            else
            {
                selectedRoom.isFixed = false;
            }

            availableRooms.Remove(selectedRoom);
        }
    }

    // 部屋タイプごとの割り当て数を計算
    private Dictionary<RoomType, int> CalculateTypeCounts(int availableRoomCount, List<RoomTypeSetting> options)
    {
        Dictionary<RoomType, int> typeCounts = new Dictionary<RoomType, int>();

        // 非必須かつEntrance、Exit以外の部屋タイプを取得
        List<RoomTypeSetting> nonMandatorySettings = options.FindAll(s =>
            !s.isMandatory && s.roomType != RoomType.Entrance && s.roomType != RoomType.Exit);

        // 合計の重みを計算
        float totalWeight = 0f;
        foreach (var setting in nonMandatorySettings)
        {
            totalWeight += setting.weight;
        }

        int totalAssigned = 0;

        // 各部屋タイプの割り当て数を計算
        foreach (var setting in nonMandatorySettings)
        {
            float proportion = setting.weight / totalWeight;
            int count = Mathf.RoundToInt(proportion * availableRoomCount);

            // minCount と maxCount を適用
            if (setting.minCount != -1)
            {
                count = Mathf.Max(count, setting.minCount);
            }
            if (setting.maxCount != -1)
            {
                count = Mathf.Min(count, setting.maxCount);
            }

            count = Mathf.Min(count, availableRoomCount - totalAssigned); // 利用可能な部屋数を超えないようにする

            typeCounts[setting.roomType] = count;
            totalAssigned += count;
        }

        return typeCounts;
    }

    // ランダムに部屋を選択
    private Room SelectRandomRoom(List<Room> availableRooms)
    {
        int index = Random.Range(0, availableRooms.Count);
        return availableRooms[index];
    }

    // EntranceとExitを割り当て
    private void AssignEntranceAndExit(List<Room> rooms, List<RoomTypeSetting> options)
    {
        // 空き部屋を取得
        List<Room> availableRooms = rooms.FindAll(r => r.roomType == RoomType.Empty);

        if (availableRooms.Count < 2)
        {
            Debug.LogWarning("EntranceとExitを割り当てるための部屋が不足しています。");
            return;
        }

        Room entranceRoom = null;
        Room exitRoom = null;
        float maxDistance = 0f;

        // 最も離れている2つの部屋を探す
        for (int i = 0; i < availableRooms.Count; i++)
        {
            for (int j = i + 1; j < availableRooms.Count; j++)
            {
                Room roomA = availableRooms[i];
                Room roomB = availableRooms[j];

                float distance = Vector2.Distance(roomA.GetCenterFloat(), roomB.GetCenterFloat());
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    entranceRoom = roomA;
                    exitRoom = roomB;
                }
            }
        }

        if (entranceRoom != null && exitRoom != null)
        {
            entranceRoom.roomType = RoomType.Entrance;
            exitRoom.roomType = RoomType.Exit;

            // isFixedProbability に基づいて isFixed を設定（EntranceとExitの設定を取得）
            RoomTypeSetting entranceSetting = options.Find(o => o.roomType == RoomType.Entrance);
            if (entranceSetting != null && Random.value < entranceSetting.isFixedProbability)
            {
                entranceRoom.isFixed = true;
            }
            else
            {
                entranceRoom.isFixed = false;
            }

            RoomTypeSetting exitSetting = options.Find(o => o.roomType == RoomType.Exit);
            if (exitSetting != null && Random.value < exitSetting.isFixedProbability)
            {
                exitRoom.isFixed = true;
            }
            else
            {
                exitRoom.isFixed = false;
            }
        }
    }
}
