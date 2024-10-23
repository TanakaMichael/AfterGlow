using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/AreaPartition/Grid Generation")]
public class GridGeneration : ScriptableObject, IPartitioningAlgorithm
{
    public int num_grid_width = 10; // num_grid_width分横に分割する
    public int num_grid_height = 10;
    public void Partition(Node node, List<Room> rooms){

        // BSP木構造を生成する

    }
}
