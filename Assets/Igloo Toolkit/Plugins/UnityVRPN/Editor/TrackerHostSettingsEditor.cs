using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityVRPN.TrackerHostSettings))]
public class TrackerHostSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UnityVRPN.TrackerHostSettings settings = target as UnityVRPN.TrackerHostSettings;

        if (settings != null)
        {
            settings.Hostname = EditorGUILayout.TextField("Hostname", settings.Hostname);
            settings.Preset = (UnityVRPN.TrackerPreset)EditorGUILayout.EnumPopup("Type", settings.Preset);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
