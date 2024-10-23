using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/AreaPartition/Grid Generation")]
public class GridGeneration : ScriptableObject, IPartitioningAlgorithm
{
    public int numGridWidth = 10; // num_grid_width分横に分割する
    public int numGridHeight = 10;
    private int IDCounter = 0;
    public void Partition(Node node, List<Area> areas){
        // グリッドの幅と高さを算出
        int gridWidth = node.width / numGridWidth; 
        int gridHeight = node.height / numGridHeight;
        // 余った部分を最後のエリアに追加するために記録
        int remainderWidth = node.width % numGridWidth;
        int remainderHeight = node.height % numGridHeight;

        // グリッドを作成
        for(int r = 0; r < numGridHeight; r++){
            for(int c = 0; c < numGridWidth; c++){
                int areaX = node.x + r * gridWidth;
                int areaY = node.y + c * gridHeight;

                int currentWidth = gridWidth;
                int currentHeight = gridHeight;

                // 最後の列には余った幅を追加
                if (c == numGridWidth - 1) currentWidth += remainderWidth;

                // 最後の行には余った高さを追加
                if (r == numGridHeight - 1) currentHeight += remainderHeight;

                // 新しいエリアをリストに追加
                Area newArea = new Area(IDCounter++, areaX, areaY, currentWidth, currentHeight);
                areas.Add(newArea);
            }
        }
         // 生成したエリアをテスト表示
        foreach (Area area in areas)
        {
            GameManager.Log($"エリア X: {area.x}, Y: {area.y}, 幅: {area.width}, 高さ: {area.height}");
        }
    }

    
}
