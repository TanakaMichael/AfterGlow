using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new SpecialObjectSet", menuName = "SpecialObject/Set")]
public class SpecialObjectSet : ScriptableObject
{
    public List<SpecialObject> specialObjectPrefab;
}
