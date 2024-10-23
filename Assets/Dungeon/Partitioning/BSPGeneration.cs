using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/AreaPartition/BSP Generation")]
public class BSPGeneration : ScriptableObject, IPartitioningAlgorithm
{
    public int min_split_size = 20; // 分割エリアの最小サイズ
    public int max_split_size = 10; // 分割エリアの最大サイズ
    public int max_split_depth = 5; // 最大分割深度

    public void Partition(Node node){
        // BSP木構造を生成する
        
    }
}
