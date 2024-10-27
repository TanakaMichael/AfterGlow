using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoomManager
{
    private List<RandomRoomGenerator> randomRoomGenerators;

    public RandomRoomManager(List<RandomRoomGenerator> randomRoomGenerators)
    {
        this.randomRoomGenerators = randomRoomGenerators;
    }
    public bool Generate(Room room){
        RandomRoomGenerator generator = GetGeneratorForRoomType(room.GetRoomType());
        if (generator!= null){
            Room GeneratedRoom = generator.GenerateRandomRoom(room);
            if(GeneratedRoom != null){
                room.ApplyRandomRoomData(GeneratedRoom);
                return true;
            }
            else{
                Debug.LogError("Failed to generate Random Room");
                return false;
            }
        }
        else{
            Debug.LogError($"No RandomRoomGenerator found for RoomType {room.roomType}");
            return false;
        }
    }
    /// <summary>
    /// 渡されたRoomのRoomTypeに一致するランダム部屋生成器を取得する
    /// </summary>
    public RandomRoomGenerator GetGeneratorForRoomType(RoomType roomType)
    {
        List<RandomRoomGenerator> matchingGenerators = randomRoomGenerators.FindAll(rg => rg.roomType == roomType || rg.roomType ==RoomType.Free || rg.roomType ==RoomType.Empty);

        if (matchingGenerators.Count == 0)
        {
            Debug.LogWarning($"No RandomRoomGenerator found for RoomType {roomType}.");
            return null;
        }

        // ランダムにジェネレーターを選択
        int index = Random.Range(0, matchingGenerators.Count);
        return matchingGenerators[index];
    }
}
