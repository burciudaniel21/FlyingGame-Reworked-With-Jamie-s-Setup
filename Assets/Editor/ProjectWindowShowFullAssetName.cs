using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ProjectWindowShowFullAssetName
{
    internal static readonly string PrefKey_EnableMultilineDisplay = "ProjectWindowShowFullAssetName_EnableMultilineDisplay";
    internal static readonly string PrefKey_EnableCamelCaseSpacing = "ProjectWindowShowFullAssetName_EnableCamelCaseSpacing";

    private static bool EnableMultilineDisplay => EditorPrefs.GetBool(PrefKey_EnableMultilineDisplay, true);
    private static bool EnableCamelCaseSpacing => EditorPrefs.GetBool(PrefKey_EnableCamelCaseSpacing, true);

    static GUIStyle style;
    static GUIContent assetGUIContent;
    static Color32 backgroundColor;
    static Color32 selectedFrameColor;

    static ProjectWindowShowFullAssetName()
    {
        EditorApplication.projectWindowItemInstanceOnGUI -= Draw;
        EditorApplication.projectWindowItemInstanceOnGUI += Draw;

        style = new GUIStyle
        {
            fontSize = 10,
            wordWrap = true,
            alignment = TextAnchor.UpperCenter,
            margin = new RectOffset(0, 0, 0, 0),
        };

        assetGUIContent = new GUIContent();

        if (EditorGUIUtility.isProSkin)
        {
            backgroundColor = new Color32(51, 51, 51, 255);
            selectedFrameColor = new Color32(44, 93, 135, 255);
        }
        else
        {
            backgroundColor = new Color32(190, 190, 190, 255);
            selectedFrameColor = new Color32(58, 114, 176, 255);
        }
    }

    static void Draw(int instanceID, Rect selectionRect)
    {
        // Check if the multiline display is disabled
        if (!EnableMultilineDisplay)
            return;

        if (IsRenaming(selectionRect, instanceID))
            return;

        if (selectionRect.height <= 20 || AssetDatabase.IsSubAsset(instanceID))
            return;

        var path = AssetDatabase.GetAssetPath(instanceID);
        if (string.IsNullOrWhiteSpace(path))
            return;

        var assetName = Path.GetFileNameWithoutExtension(path);

        if (EnableCamelCaseSpacing)
        {
            assetName = AddSpacesToCamelCase(assetName);
        }

        assetGUIContent.text = assetName;
        float textHeight = style.CalcHeight(assetGUIContent, selectionRect.width);

        var nameRect = new Rect(selectionRect.x, selectionRect.yMax - 12, selectionRect.width, textHeight + 4);

        if (Event.current.type == EventType.MouseDown && nameRect.Contains(Event.current.mousePosition))
        {
            Selection.activeInstanceID = instanceID;
            Event.current.Use();
        }

        bool isSelected = Selection.instanceIDs.Contains(instanceID);
        style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : (isSelected ? Color.white : Color.black);

        var backgroundRect = new Rect(nameRect.x - 6, nameRect.y - 1, nameRect.width + 12, nameRect.height + 3);
        EditorGUI.DrawRect(backgroundRect, isSelected ? selectedFrameColor : backgroundColor);
        GUI.Label(nameRect, assetName, style);
    }

    static bool IsRenaming(Rect selectionRect, int instanceID)
    {
        if (Selection.activeInstanceID == instanceID && EditorGUIUtility.editingTextField)
            return true;

        return false;
    }

    static string AddSpacesToCamelCase(string text)
    {
        return Regex.Replace(text, "([a-z])([A-Z])", "$1 $2")
                    .Replace("_", " ")
                    .Replace("-", " ");
    }
}

public class ProjectWindowPreferences : SettingsProvider
{
    public ProjectWindowPreferences() : base("Preferences/Project Window", SettingsScope.User)
    { }

    public override void OnGUI(string searchContext)
    {
        // Multiline Display Toggle
        bool currentMultilineSetting = EditorPrefs.GetBool(ProjectWindowShowFullAssetName.PrefKey_EnableMultilineDisplay, true);
        bool newMultilineSetting = EditorGUILayout.Toggle("Enable Multiline Display", currentMultilineSetting);
        if (newMultilineSetting != currentMultilineSetting)
        {
            EditorPrefs.SetBool(ProjectWindowShowFullAssetName.PrefKey_EnableMultilineDisplay, newMultilineSetting);
            EditorApplication.RepaintProjectWindow(); // Refresh the Project window
        }

        // CamelCase Spacing Toggle (disabled if multiline display is off)
        EditorGUI.BeginDisabledGroup(!newMultilineSetting);
        bool currentCamelCaseSetting = EditorPrefs.GetBool(ProjectWindowShowFullAssetName.PrefKey_EnableCamelCaseSpacing, true);
        bool newCamelCaseSetting = EditorGUILayout.Toggle("Enable Expanded Filenames", currentCamelCaseSetting);
        if (newCamelCaseSetting != currentCamelCaseSetting)
        {
            EditorPrefs.SetBool(ProjectWindowShowFullAssetName.PrefKey_EnableCamelCaseSpacing, newCamelCaseSetting);
            EditorApplication.RepaintProjectWindow(); // Refresh the Project window
        }
        EditorGUI.EndDisabledGroup();
    }

    [SettingsProvider]
    public static SettingsProvider CreatePreferencesProvider()
    {
        return new ProjectWindowPreferences();
    }
}
