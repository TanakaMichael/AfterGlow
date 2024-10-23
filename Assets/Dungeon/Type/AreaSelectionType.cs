using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaSelectionType
{
    CenterFocused,     // 中央に寄せる
    Random,            // 完全ランダム
    EdgeFocused,       // 端より
    CircularPattern,   // 円形に配置
    LinearPattern      // 線形に配置
}
