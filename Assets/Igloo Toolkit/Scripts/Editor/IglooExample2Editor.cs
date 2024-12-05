using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IglooExample2))]

public class IglooExample2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        IglooExample2 tester = (IglooExample2)target;
        GUILayout.Space(10f);

        if (GUILayout.Button("Follow Player"))
        {
            tester.FollowPlayer();
        }

        if (tester.followObject != null)
        {
            if (GUILayout.Button("Follow Object"))
            {
                tester.FollowObject();
            }
        }
        else EditorGUILayout.HelpBox("Follow Object must be assigned", MessageType.Error, false);

    }
}