using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPartitioningAlgorithm
{
    void Partition(Node node, List<Area> area);
}
