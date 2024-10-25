using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// おそらく変わる
[CreateAssetMenu(fileName ="newEnemy", menuName = "Dungeon/Enemies/NewEnemies")]
public class Enemy : ScriptableObject
{
    public GameObject me;
    public float priority;
}
