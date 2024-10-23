using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int id;
    public int x, y;
    public int width, height;
    public RoomType roomType;
    public Room(int id, int x, int y, int width, int height){
        this.id = id;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.roomType = RoomType.Empty;
    }

}
