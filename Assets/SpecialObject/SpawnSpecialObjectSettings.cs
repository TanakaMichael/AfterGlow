using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new SpawnSpecialObject", menuName = "Dungeon/Room/Spawn/SpecialObject")]
public class SpawnSpecialObjectSettings : ScriptableObject
{
    // SpawnSpecialObjectSettingsはManagerに送る設定のみ行う
    // その他のロジックはManagerに任せる
    public SpecialObjectSpawnPattern specialObjectSpawnPattern = new SpecialObjectSpawnPattern(); ///
    public SpecialObject specialObject;
    public bool isEnable = false;
}
