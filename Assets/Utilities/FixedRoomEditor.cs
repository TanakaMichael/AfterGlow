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