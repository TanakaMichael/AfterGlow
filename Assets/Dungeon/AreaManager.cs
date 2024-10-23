using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [Header("Dungeon Area Settings")]
    public int width = 100;
    public int height = 100;
    [Header("Partition Algorithm Settings")]
    public ScriptableObject selectedPartitioningAlgorithm;

    private IPartitioningAlgorithm PartitionAlgorithm;

    public List<Area> areas;
    private Node node;
    // 宣言の初期設定
    void Awake()
    {
        if (selectedPartitioningAlgorithm is IPartitioningAlgorithm)
        {
            PartitionAlgorithm = (IPartitioningAlgorithm)selectedPartitioningAlgorithm;
        }
        else
        {
            Debug.LogError("選択されたアルゴリズムが無効です。");
        }
        node = new Node(0, 0, width, height);
        areas = new List<Area>();
    }
    public void AreaPartition(){
        PartitionAlgorithm.Partition(node, areas);
    }
}
