using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
[CreateAssetMenu(fileName = "new SpawnSpecialObject", menuName = "Dungeon/Room/Spawn/NPC")]
public class SpawnNPCSettings : ScriptableObject
{
    public NPCSpawnPattern SpawnType { get; set; }

    [Tooltip("Specificを選択時のみ有効")]
    public List<NPC> NPCList { get; set; } // SpawnTypeでSpecificを選択時のみ
    public bool isEnable = false;
    public SpawnNPCSettings(bool isEnable = false){
        this.isEnable = isEnable;
    }
}
