using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new NPCSet", menuName = "NPC/Set")]
public class NPCSet : MonoBehaviour
{
    public List<NPC> npcPrefab = new List<NPC>();
}
