using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("References")]
    public DungeonMapManager dungeonMapManager; // DungeonManagerへの参照
    public LightingManager lightingManager; // LightingManager への参照
    [Header("NPC Object Sets")]
    public List<NPCSetEntry> npcObjectSets; // SpecialObjectSpawnPattern ごとの SpecialObjectSet のリスト
    // NPC の生成されたオブジェクトのリスト
    public List<NPCInstance> npcInstances = new List<NPCInstance>();
    private Dictionary<NPCSpawnPattern, NPCSet> patternToObjectSet;

    public void SpawnNPC(){
        if (dungeonMapManager == null)
        {
            Debug.LogError("DungeonMapManager is not assigned.");
            return;
        }

        // NPCSpawnPattern ごとの NPCSet のマッピングを作成
        patternToObjectSet = new Dictionary<NPCSpawnPattern, NPCSet>();

        foreach (var entry in npcObjectSets)
        {
            if (!patternToObjectSet.ContainsKey(entry.spawnPattern))
            {
                patternToObjectSet.Add(entry.spawnPattern, entry.npcSet);
            }
            else
            {
                Debug.LogError($"NPCSet with {entry.spawnPattern} already exists.");
            }
        }
        
        // ランダムな場所から NPC を生成
        foreach (var column in dungeonMapManager.dungeonTiles){
            foreach(var tile in column){
                PlaceNPC(tile);
            }
        }
    }

    private void PlaceNPC(DungeonTile tile){
        if (tile == null || tile.spawnNPCSettings == null) return;
        SpawnNPCSettings settings = tile.spawnNPCSettings;

        if(!settings.isEnable) return;

        // 特定の NPC が設定されている場合はそれを使用
        if(settings.NPCList != null && settings.NPCList != null)
        {
            InstantiateNPC(settings.NPCList.GetRandomElement().gameObject, tile);
        }
        else
        {
            // NPCSpawnPattern に対応する NPCSet からランダムに選択
            if(patternToObjectSet.TryGetValue(settings.SpawnType, out NPCSet objectSet))
            {
                InstantiateNPC(objectSet.npcPrefab.GetRandomElement().gameObject, tile);
            }
        }
    }

    private void InstantiateNPC(GameObject npcPrefab, DungeonTile tile)
    {
        if(npcPrefab == null) return;

        Vector3 position = new Vector3(tile.x, tile.y, 0);
        GameObject npcObj = Instantiate(npcPrefab, position, Quaternion.identity, transform);
        LightSource lightSource = npcObj.GetComponent<LightSource>();
        if(lightSource != null)
        {
            NPCInstance instance = new NPCInstance(npcObj, lightSource);
            npcInstances.Add(instance);
            // ライティングマネージャーに登録
            lightingManager.AddLightSource(lightSource);
        }
        else{
            Debug.LogWarning($"NPC prefab {npcPrefab.name} does not have LightSource component.");
        }
        
    }
}
[System.Serializable]
public class NPCSetEntry
{
    public NPCSpawnPattern spawnPattern;
    public NPCSet npcSet;
}

