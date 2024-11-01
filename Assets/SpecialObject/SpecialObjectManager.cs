using System.Collections.Generic;
using UnityEngine;

public class SpecialObjectManager : MonoBehaviour
{
    [Header("References")]
    public DungeonMapManager dungeonMapManager; // DungeonMapManager への参照

    [Header("Special Object Sets")]
    public List<SpecialObjectSetEntry> specialObjectSets; // SpecialObjectSpawnPattern ごとの SpecialObjectSet のリスト

    private Dictionary<SpecialObjectSpawnPattern, SpecialObjectSet> patternToObjectSet;

    public void SpawnSpecialObjects()
    {
        if (dungeonMapManager == null)
        {
            Debug.LogError("DungeonMapManager is not assigned.");
            return;
        }

        // SpecialObjectSpawnPattern と SpecialObjectSet のマッピングを作成
        patternToObjectSet = new Dictionary<SpecialObjectSpawnPattern, SpecialObjectSet>();

        foreach (var entry in specialObjectSets)
        {
            if (!patternToObjectSet.ContainsKey(entry.spawnPattern))
            {
                patternToObjectSet.Add(entry.spawnPattern, entry.specialObjectSet);
            }
            else
            {
                Debug.LogWarning($"Duplicate SpecialObjectSpawnPattern {entry.spawnPattern} in SpecialObjectManager.");
            }
        }

        // タイルを走査して SpecialObject を配置
        foreach (var column in dungeonMapManager.dungeonTiles)
        {
            foreach (var tile in column)
            {
                PlaceSpecialObject(tile);
            }
        }
    }

    private void PlaceSpecialObject(DungeonTile tile)
    {
        if (tile == null || tile.spawnSpecialObjectSettings == null)
            return;

        SpawnSpecialObjectSettings settings = tile.spawnSpecialObjectSettings;

        if (!settings.isEnable)
            return;

        // 特定の SpecialObject が設定されている場合はそれを使用
        if (settings.specialObject != null && settings.specialObject.gameObject != null)
        {
            InstantiateSpecialObject(settings.specialObject.gameObject, tile);
        }
        else
        {
            // SpecialObjectSpawnPattern に対応する SpecialObjectSet からランダムに選択
            if (patternToObjectSet.TryGetValue(settings.specialObjectSpawnPattern, out SpecialObjectSet objectSet))
            {
                if (objectSet.specialObjectPrefab != null && objectSet.specialObjectPrefab.Count > 0)
                {
                    int index = Random.Range(0, objectSet.specialObjectPrefab.Count);
                    SpecialObject specialObject = objectSet.specialObjectPrefab[index];
                    if (specialObject != null && specialObject.gameObject != null)
                    {
                        InstantiateSpecialObject(specialObject.gameObject, tile);
                    }
                }
                else
                {
                    Debug.LogWarning($"SpecialObjectSet for pattern {settings.specialObjectSpawnPattern} is empty.");
                }
            }
            else
            {
                Debug.LogWarning($"No SpecialObjectSet found for pattern {settings.specialObjectSpawnPattern}.");
            }
        }
    }

    private void InstantiateSpecialObject(GameObject prefab, DungeonTile tile)
    {
        Vector3 position = new Vector3(tile.x, tile.y, 0);
        Instantiate(prefab, position, Quaternion.identity, transform);
    }
}

[System.Serializable]
public class SpecialObjectSetEntry
{
    public SpecialObjectSpawnPattern spawnPattern;
    public SpecialObjectSet specialObjectSet;
}
