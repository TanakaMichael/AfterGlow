using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoomGenerationAlgorithm
{
    Room GenerateRoom(Room baseRoom);
}