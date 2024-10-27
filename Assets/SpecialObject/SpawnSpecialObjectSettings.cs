using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SpawnSpecialObjectSettings
{
    // SpawnSpecialObjectSettingsはManagerに送る設定のみ行う
    // その他のロジックはManagerに任せる
    public SpecialObjectSpawnPattern specialObjectSpawnPattern = new SpecialObjectSpawnPattern(); ///
    public SpecialObject specialObject;
    public bool isEnable = false;
}
