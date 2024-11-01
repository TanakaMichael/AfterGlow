using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new DesignSet", menuName = "Design/Set")]
public class TileSet : ScriptableObject
{
    public List<TilePrefab> tilePrefabs; // DesignType と対応する GameObject のリスト
}