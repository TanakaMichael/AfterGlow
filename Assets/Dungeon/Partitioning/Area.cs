using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    public int id;
    public int x;
    public int y;
    public int width;
    public int height;

    public Area(int id, int x, int y, int width, int height)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    // 中心座標を取得するメソッド
    public Vector2 GetCenter()
    {
        return new Vector2(x + width / 2f, y + height / 2f);
    }
}
