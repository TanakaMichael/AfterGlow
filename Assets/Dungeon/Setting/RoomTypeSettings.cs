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
    [Header("enemy setting")]
    public SpawnEnemySettings enemySettings; // random部屋を生成するとき
    public int enemyCount = 0;
    [Header("NPC setting")]
    public SpawnNPCSettings npcSettings;
    public int npcCount = 0;
    [Header("SpecialObject setting")]
    public SpawnSpecialObjectSettings specialObjectSettings;
    public int specialObjectCount = 0;
    [Header("Probability setting")]

    [Range(0f, 1f)]
    public float isFixedProbability = 0.0f; // 部屋が固定かどうかの確率

    public EntranceExitSettings entranceSettings;
    public EntranceExitSettings exitSettings;
}