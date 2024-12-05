using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IglooExample1))]
public class IglooExample1Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        IglooExample1 tester = (IglooExample1)target;
        GUILayout.Space(10f);

        //EditorGUILayout.HelpBox("Keys \n c- create igloo \n r- remove igloo \n z- disable displays " +
        //    "\n x- enable displays \n  f- follow object \n  p- use player",
        //    MessageType.Info, false);

        if (GUILayout.Button("Create Igloo"))
        {
            tester.CreateIgloo();
        }
        if (GUILayout.Button("Remove Igloo"))
        {
            tester.RemoveIgloo();
        }

    }
}
