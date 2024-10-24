using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RoomTypeSetting", menuName = "Dungeon/Room/GenerateRoomTypeSetting")]
public class RoomTypeSetting : ScriptableObject
{
    public RoomType roomType;        // 部屋のタイプ
    public int minCount = -1;        // 最小数（-1は任意設定）
    public int maxCount = -1;        // 最大数（-1は任意設定）
    public float weight = 1.0f;      // 部屋選択の重み（確率を調整するため）
    public bool isMandatory = false; // 必須かどうか
    public float isFixedProbability = 0.0f; // 部屋が固定かどうかの確率
}