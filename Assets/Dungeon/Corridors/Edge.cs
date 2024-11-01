using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Edge クラス
public class Edge
{
    public Room roomA;
    public Room roomB;
    public float weight; // 部屋間の距離

    public Edge(Room a, Room b)
    {
        roomA = a;
        roomB = b;
        weight = Vector2.Distance(a.GetCenterFloat(), b.GetCenterFloat());
    }
}