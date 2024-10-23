using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AreaManager))]
public class PartitioningEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AreaManager manager = (AreaManager)target;

        // デフォルトのプロパティ描画
        DrawDefaultInspector();

        if (manager.selectedPartitioningAlgorithm != null)
        {
            SerializedObject algorithmObject = new SerializedObject(manager.selectedPartitioningAlgorithm);
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
