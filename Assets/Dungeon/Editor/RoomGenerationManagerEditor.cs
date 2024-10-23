using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomManager))]
public class RoomGenerationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomManager manager = (RoomManager)target;

        // デフォルトのプロパティ描画
        DrawDefaultInspector();

        if (manager.selectedRoomGenerationAlgorithm != null)
        {
            SerializedObject algorithmObject = new SerializedObject(manager.selectedRoomGenerationAlgorithm);
            SerializedProperty property = algorithmObject.GetIterator();

            // 内部のプロパティを描画
            property.NextVisible(true);
            while (property.NextVisible(false))
            {
                EditorGUILayout.PropertyField(property, true);
            }

            algorithmObject.ApplyModifiedProperties();
        }
    }
}
