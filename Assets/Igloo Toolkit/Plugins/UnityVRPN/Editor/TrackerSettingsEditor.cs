using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(UnityVRPN.TrackerSettings))]
public class TrackerSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UnityVRPN.TrackerSettings settings = target as UnityVRPN.TrackerSettings;

        if (settings != null)
        {
            settings.HostSettings = (UnityVRPN.TrackerHostSettings)EditorGUILayout.ObjectField("Host Settings", settings.HostSettings, typeof(UnityVRPN.TrackerHostSettings), true);
            settings.ObjectName = EditorGUILayout.TextField("Object Name", settings.ObjectName);
            settings.Channel = EditorGUILayout.IntField("Channel", settings.Channel);
            settings.TrackPosition = EditorGUILayout.Toggle("Track Position", settings.TrackPosition);
            settings.TrackRotation = EditorGUILayout.Toggle("Track Rotation", settings.TrackRotation);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
