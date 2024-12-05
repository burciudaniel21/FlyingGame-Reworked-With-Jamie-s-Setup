using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LayoutObjects))]
[CanEditMultipleObjects]
public class LayoutObjectsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        LayoutObjects layout = (LayoutObjects)target;
        EditorGUILayout.LabelField("Item", EditorStyles.boldLabel);
        layout.publicItem = (GameObject)EditorGUILayout.ObjectField("Item", layout.publicItem, typeof(GameObject), true) as GameObject;
        // Editor options for creating items in the Grid layout
        EditorGUILayout.LabelField("Grid Layout", EditorStyles.boldLabel);
        layout.rows = EditorGUILayout.IntField("Rows", layout.rows);
        layout.columns = EditorGUILayout.IntField("Colums", layout.columns);
        layout.depth = EditorGUILayout.IntField("Depth", layout.depth);
        layout.spacingScale = EditorGUILayout.FloatField("Spacing Scale", layout.spacingScale);

        if (GUILayout.Button("Create items in Grid"))
        {
            layout.InstantiateItemsInGrid();
        }
        // Editor options for creating items in the circular layout
        EditorGUILayout.LabelField("Circular Layout", EditorStyles.boldLabel);
        layout.numItemsCircle = EditorGUILayout.IntField("Number Of Items", layout.numItemsCircle);
        layout.centerPosCircle = EditorGUILayout.Vector3Field("Center Position", layout.centerPosCircle);
        if (GUILayout.Button("Create items in Circle"))
        {
            layout.InstantiateItemsInCircle();
        }
    }
}
