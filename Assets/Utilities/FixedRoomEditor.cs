using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FixedRoom))]
public class FixedRoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FixedRoom fixedRoom = (FixedRoom)target;
        if (GUILayout.Button("Extract Tile Data"))
        {
            fixedRoom.ExtractTileData();
            EditorUtility.SetDirty(fixedRoom); // 変更を保存
            Debug.Log("FixedRoom: Tile data extracted successfully.");
        }
    }
}

public static class ListExtensions
{
    public static T GetRandomElement<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default; // 空リストの場合はデフォルト値を返す
        }

        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}
