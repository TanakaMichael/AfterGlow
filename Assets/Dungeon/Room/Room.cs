using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int id;
    public int areaId;
    public int x, y;
    public int width, height;
    public RoomType roomType;
    public bool isFixed = false;
    public Room(int id, int x, int y, int width, int height, int areaId = 0){
        this.id = id;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.roomType = RoomType.Empty;
        this.areaId = areaId;
        this.isFixed = false;
    }
    public Vector2Int GetCenterInt()
    {
        return new Vector2Int(x + width / 2, y + height / 2);
    }
    public Vector2 GetCenterFloat(){
        return new Vector2(x + width / 2f, y + height / 2f);
    }

}
