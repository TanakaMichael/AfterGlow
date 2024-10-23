using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomSelector
{
    public List<Area> SelectAreasForRooms(List<Area> areas, AreaSelectionType selectType, int numberOfRoomsToSelect)
    {
        List<Area> selectedAreas = new List<Area>();


        switch (selectType)
        {
            case AreaSelectionType.CenterFocused:
                selectedAreas = SelectCenterFocusedAreas(areas, numberOfRoomsToSelect);
                break;

            case AreaSelectionType.Random:
                selectedAreas = SelectRandomAreas(areas, numberOfRoomsToSelect);
                break;

            case AreaSelectionType.EdgeFocused:
                selectedAreas = SelectEdgeFocusedAreas(areas, numberOfRoomsToSelect);
                break;

            case AreaSelectionType.CircularPattern:
                selectedAreas = SelectCircularPatternAreas(areas, numberOfRoomsToSelect);
                break;

            case AreaSelectionType.LinearPattern:
                selectedAreas = SelectLinearPatternAreas(areas, numberOfRoomsToSelect);
                break;
        }

        return selectedAreas;
    }
    // 中央に寄せてエリアを選択する
    private List<Area> SelectCenterFocusedAreas(List<Area> areas, int numberToSelect)
    {
        Vector2 center = CalculateCenter(areas);

        // エリアを中心からの距離でソート
        areas.Sort((a, b) =>
        {
            float distanceA = Vector2.Distance(center, new Vector2(a.x + a.width / 2f, a.y + a.height / 2f));
            float distanceB = Vector2.Distance(center, new Vector2(b.x + b.width / 2f, b.y + b.height / 2f));
            return distanceA.CompareTo(distanceB);
        });

        // 中央に寄せて選択
        List<Area> selectedAreas = new List<Area>();
        for (int i = 0; i < Mathf.Min(numberToSelect, areas.Count); i++)
        {
            selectedAreas.Add(areas[i]);
        }

        return selectedAreas;
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

    // 端に寄せてエリアを選択する
    private List<Area> SelectEdgeFocusedAreas(List<Area> areas, int numberToSelect)
    {
        // 端（エッジ）からの距離を求め、ソート
        areas.Sort((a, b) =>
        {
            float distanceA = Mathf.Min(a.x, a.y, areas[0].width - a.x, areas[0].height - a.y);
            float distanceB = Mathf.Min(b.x, b.y, areas[0].width - b.x, areas[0].height - b.y);
            return distanceA.CompareTo(distanceB);
        });

        List<Area> selectedAreas = new List<Area>();
        for (int i = 0; i < Mathf.Min(numberToSelect, areas.Count); i++)
        {
            selectedAreas.Add(areas[i]);
        }

        return selectedAreas;
    }

    // 円形にエリアを選択する
    private List<Area> SelectCircularPatternAreas(List<Area> areas, int numberToSelect)
    {
        // 中心点を求める
        Vector2 center = CalculateCenter(areas);

        // 円形に選択
        List<Area> selectedAreas = new List<Area>();

        for (int i = 0; i < areas.Count && selectedAreas.Count < numberToSelect; i++)
        {
            float distance = Vector2.Distance(center, new Vector2(areas[i].x + areas[i].width / 2f, areas[i].y + areas[i].height / 2f));
            if (distance <= Mathf.Max(areas[0].width, areas[0].height) / 2f)
            {
                selectedAreas.Add(areas[i]);
            }
        }

        return selectedAreas;
    }

    // 線形にエリアを選択する
    private List<Area> SelectLinearPatternAreas(List<Area> areas, int numberToSelect)
    {
        // 水平方向に選択
        List<Area> selectedAreas = new List<Area>();

        for (int i = 0; i < areas.Count && selectedAreas.Count < numberToSelect; i++)
        {
            if (areas[i].x % 2 == 0)
            {
                selectedAreas.Add(areas[i]);
            }
        }

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
    // エリアからルームに変換するメソッド (多分使わない)
    private void ConvertAreasToRooms(List<Area> areas, List<Room> rooms){
        if(rooms == null) rooms = new List<Room>();

        foreach(Area area in areas){
            Room room = new Room(area.id, area.x, area.y, area.width, area.height);
            rooms.Add(room);
        }
    }
}