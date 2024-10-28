using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntranceExitOptions
{
    public SpawnPosition spawnPosition;
    public List<WeightedPosition> weightedPositions;
    public DesignCategory designCategory;

    public EntranceExitOptions(SpawnPosition pos)
    {
        spawnPosition = pos;
        weightedPositions = new List<WeightedPosition>();
        designCategory = DesignCategory.Gate;
    }
}
