using System.Collections.Generic;
using UnityEngine;

public enum AreaSelectionMethod
{
    CenterFocused,     // 中央に寄せる
    Random,            // 完全ランダム
    EdgeFocused,       // 端より
    CircularPattern,   // 円形に配置
    LinearPattern      // 線形に配置
}
[System.Serializable]
public class RoomSelector
{
    [Header("Area Selection Settings")]
    public AreaSelectionMethod selectionMethod = AreaSelectionMethod.CenterFocused;
    public int numberOfRoomsToSelect = 5;
    public List<Area> SelectAreasForRooms(List<Area> areas)
    {
        List<Area> selectedAreas = new List<Area>();
        if(numberOfRoomsToSelect == -1) numberOfRoomsToSelect = areas.Count;
        Debug.Log(numberOfRoomsToSelect);
        switch (selectionMethod)
        {
            case AreaSelectionMethod.CenterFocused:
                selectedAreas = SelectWeightedRandomFromSortedAreas(SelectCenterFocusedAreas(areas), numberOfRoomsToSelect);
                break;

            case AreaSelectionMethod.Random:
                selectedAreas = SelectRandomAreas(areas, numberOfRoomsToSelect);
                break;

            case AreaSelectionMethod.EdgeFocused:
                selectedAreas = SelectWeightedRandomFromSortedAreas(SelectEdgeFocusedAreas(areas), numberOfRoomsToSelect);
                break;

            case AreaSelectionMethod.CircularPattern:
                selectedAreas = SelectWeightedRandomFromSortedAreas(SelectCircularPatternAreas(areas), numberOfRoomsToSelect);
                break;

            case AreaSelectionMethod.LinearPattern:
                selectedAreas = SelectWeightedRandomFromSortedAreas(SelectLinearPatternAreas(areas), numberOfRoomsToSelect);
                break;
        }

        return selectedAreas;
    }

    // 中央に寄せてエリアを選択する（ソート）
    private List<Area> SelectCenterFocusedAreas(List<Area> areas)
    {
        Vector2 center = CalculateCenter(areas);
        areas.Sort((a, b) =>
        {
            float distanceA = Vector2.Distance(center, new Vector2(a.x + a.width / 2f, a.y + a.height / 2f));
            float distanceB = Vector2.Distance(center, new Vector2(b.x + b.width / 2f, b.y + b.height / 2f));
            return distanceA.CompareTo(distanceB);
        });
        return areas;
    }

    // 端に寄せてエリアを選択する（ソート）
    private List<Area> SelectEdgeFocusedAreas(List<Area> areas)
    {
        areas.Sort((a, b) =>
        {
            float distanceA = Mathf.Min(a.x, a.y, areas[0].width - a.x, areas[0].height - a.y);
            float distanceB = Mathf.Min(b.x, b.y, areas[0].width - b.x, areas[0].height - b.y);
            return distanceA.CompareTo(distanceB);
        });
        return areas;
    }

    // 円形にエリアを選択する（ソート）
    private List<Area> SelectCircularPatternAreas(List<Area> areas)
    {
        Vector2 center = CalculateCenter(areas);
        areas.Sort((a, b) =>
        {
            float distanceA = Vector2.Distance(center, new Vector2(a.x + a.width / 2f, a.y + a.height / 2f));
            float distanceB = Vector2.Distance(center, new Vector2(b.x + b.width / 2f, b.y + b.height / 2f));
            return distanceA.CompareTo(distanceB);
        });
        return areas;
    }

    // 線形にエリアを選択する（ソート）
    private List<Area> SelectLinearPatternAreas(List<Area> areas)
    {
        // とりあえずx座標が偶数のものを優先してソート
        areas.Sort((a, b) => (a.x % 2).CompareTo(b.x % 2));
        return areas;
    }

    // ランダムにエリアを選択する
    private List<Area> SelectRandomAreas(List<Area> areas, int numberToSelect)
    {
        List<Area> shuffledAreas = new List<Area>(areas);
        ShuffleList(shuffledAreas);

        List<Area> selectedAreas = new List<Area>();
        for (int i = 0; i < Mathf.Min(numberToSelect, shuffledAreas.Count); i++)
        {
            selectedAreas.Add(shuffledAreas[i]);
        }

        return selectedAreas;
    }

    // ソートしたリストから重みに基づいてランダムにエリアを選択する
    private List<Area> SelectWeightedRandomFromSortedAreas(List<Area> sortedAreas, int numberToSelect)
    {
        List<Area> selectedAreas = new List<Area>();
        GameManager.Log($"SelectWeightedRandomFromSortedAreas: {sortedAreas.Count}, {numberToSelect}");

        for (int i = 0; i < numberToSelect; i++)
        {
            if (sortedAreas.Count == 0)
            {
                break; // 選択可能なエリアがなくなった場合
            }

            // インデックスに基づいて重みを計算（先頭が選ばれやすくなるように設定）
            float totalWeight = 0f;
            List<float> cumulativeWeights = new List<float>();

            for (int j = 0; j < sortedAreas.Count; j++)
            {
                float weight = 1f / (j + 1); // インデックスが低いほど高い重みを持つ
                totalWeight += weight;
                cumulativeWeights.Add(totalWeight);
            }

            // ランダムな値を選択
            float randomValue = Random.Range(0, totalWeight);

            // 重みに基づいてエリアを選択
            for (int j = 0; j < cumulativeWeights.Count; j++)
            {
                if (randomValue <= cumulativeWeights[j])
                {
                    selectedAreas.Add(sortedAreas[j]);
                    sortedAreas.RemoveAt(j);
                    break;
                }
            }
        }
        GameManager.Log($"SelectWeightedRandomFromSortedAreas: {sortedAreas.Count}");
        return selectedAreas;
    }

    // リストをシャッフルするヘルパーメソッド
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    // 中心点を求めるヘルパーメソッド
    private Vector2 CalculateCenter(List<Area> areas)
    {
        float totalX = 0f;
        float totalY = 0f;

        foreach (var area in areas)
        {
            totalX += area.x + area.width / 2f;
            totalY += area.y + area.height / 2f;
        }

        return new Vector2(totalX / areas.Count, totalY / areas.Count);
    }
    private int roomIdCounter = 0;

    // 部屋を生成する前段階：エリアIDと部屋IDを割り当てる
    public List<Room> GenerateRoomsFromAreas(List<Area> selectedAreas)
    {
        List<Room> rooms = new List<Room>();

        foreach (Area area in selectedAreas)
        {
            // 部屋IDを割り当て
            int roomId = roomIdCounter++;

            // エリアから部屋を作成（部屋の位置とサイズはエリアと同じに設定）
            Room room = new Room(roomId, area.x, area.y, area.width, area.height, area.id);

            // Roomリストに追加
            rooms.Add(room);
        }

        return rooms;
    }
}
