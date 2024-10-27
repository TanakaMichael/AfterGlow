using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/AreaPartition/BSP Generation")]
public class BSPGeneration : ScriptableObject, IPartitioningAlgorithm
{
    public int minSplitSize = 20; // 分割エリアの最小サイズ
    public int maxSplitSize = 10; // 分割エリアの最大サイズ
    public int maxSplitDepth = 5; // 最大分割深度

    private int nodeIDCount = 0; // ノードIDカウント

    public void Partition(Node node, List<Area> areas){
        // BSP木構造を生成する
        areaPartition(node, areas);
        // 生成したエリアをテスト表示
        string str = "";
        foreach (Area area in areas)
        {
            str += $"エリア X: {area.x}, Y: {area.y}, 幅: {area.width}, 高さ: {area.height}\n";
        }
        GameManager.Log(str);
    }
    public void areaPartition(Node node, List<Area> area, int depth = 0){
        // 最大深度に達した場合又は、最小分割サイズ以下であったなら停止
        if(depth >= maxSplitDepth || node.width <= minSplitSize || node.height <= minSplitSize) {
            area.Add(new Area(node.id, node.x, node.y, node.width, node.height));
            return;
        }

        // 分割方向を選択
        bool vertically = chooseSplitDirection(node.width, node.height);
        if(vertically){
            // 縦方向
            int maxSplit = node.width - minSplitSize;
            int minSplit = minSplitSize;

            if(maxSplit <= minSplit){ 
                area.Add(new Area(node.id, node.x, node.y, node.width, node.height));
                return;
            }
            int split = Random.Range(minSplit, maxSplit);
            node.left_child = new Node(node.x, node.y, split, node.height, nodeIDCount++);
            node.right_child = new Node(node.x + split, node.y, node.width - split, node.height, nodeIDCount++);
            node.IsVertical_split = true;
            node.split_line_position = node.x + split;
        }
        else{
            // 横方向
            int maxSplit = node.height - minSplitSize;
            int minSplit = minSplitSize;

            if(maxSplit <= minSplitSize){
                area.Add(new Area(node.id, node.x, node.y, node.width, node.height));
                return;
            }
            int split = Random.Range(minSplit, maxSplit);
            node.left_child = new Node(node.x, node.y, node.width, split, nodeIDCount++);
            node.right_child = new Node(node.x, node.y + split, node.width, node.height - split, nodeIDCount++);
            node.IsVertical_split = false;
            node.split_line_position = node.y + split;
        }

        areaPartition(node.left_child,  area, depth + 1);
        areaPartition(node.right_child, area, depth + 1);   
    }

    public bool chooseSplitDirection(int width, int height)
    {
        if ((double)width / height >= 1.25) return true; // 縦割り true
        else if ((double)height / width >= 1.25) return false; // 横割り
        else return UnityEngine.Random.value > 0.5f ? true : false;
    }
}
