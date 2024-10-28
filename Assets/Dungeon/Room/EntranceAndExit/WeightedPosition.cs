using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedPosition
{
    public Vector2Int position;
    public float weight;

    public WeightedPosition(Vector2Int pos, float wt)
    {
        position = pos;
        weight = wt;
    }
}
