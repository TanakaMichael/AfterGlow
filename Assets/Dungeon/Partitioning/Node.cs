using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int id { get; set; }
    public int x;
    public int y;
    public int width;
    public int height;
    public Node left_child;
    public Node right_child;
    public bool isSelectedForRoom = false; // 部屋として扱われるエリア
    // 分割ラインを保存するためのプロパティ
    public bool IsVertical_split { get; set; }  // 垂直分割かどうかを判定
    public int split_line_position { get; set; }  // 分割ラインの座標（XまたはY）


    public Node(int x, int y, int width, int height, int NodeID=1){
        this.id = NodeID;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.left_child = null;
        this.right_child = null;
        this.IsVertical_split = false;
        this.split_line_position = 0;
    }
    // 中心座標を取得するメソッド
    public Vector2 GetCenter()
    {
        return new Vector2(x + width / 2f, y + height / 2f);
    }
}
