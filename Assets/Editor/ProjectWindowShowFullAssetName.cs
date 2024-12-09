using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.MessageBox;



// This static class customizes how asset names are displayed in Unity's Project Window.
// It allows for multiline display of asset names and optional expansion of camelCase, PascalCase kebab-case and snake_case with spacing.
// Place this script in the Assets/Editor folder (create one if it doesn't exist).
[InitializeOnLoad]
public static class ProjectWindowShowFullAssetName
{
    // Editor preference keys for toggling features
    internal static readonly string PrefKey_EnableMultilineDisplay = "ProjectWindowShowFullAssetName_EnableMultilineDisplay";
    internal static readonly string PrefKey_EnableCamelCaseSpacing = "ProjectWindowShowFullAssetName_EnableCamelCaseSpacing";

    // Properties to fetch the user-defined settings
    private static bool EnableMultilineDisplay => EditorPrefs.GetBool(PrefKey_EnableMultilineDisplay, true);
    private static bool EnableCamelCaseSpacing => EditorPrefs.GetBool(PrefKey_EnableCamelCaseSpacing, true);



    static Dictionary<int, float> clickTimes = new Dictionary<int, float>(); // Track click times for each instanceID
    const float RenameDelay = 0.5f; // Time (in seconds) to wait for initiating renaming after a single click

    // Styles and visual customization variables
    static GUIStyle style;

    static GUIContent assetGUIContent;
    static Color32 backgroundColor;
    static Color32 selectedFrameColor;

    // Static constructor to hook into Unity's Project Window rendering system
    static ProjectWindowShowFullAssetName()
    {
        // Attach the Draw method to the Project Window rendering callback
        EditorApplication.projectWindowItemInstanceOnGUI -= Draw;
        EditorApplication.projectWindowItemInstanceOnGUI += Draw;

        // Initialize the style used for displaying the filenames
        style = new GUIStyle
        {
            fontSize = 10, // Font size for filenames
            wordWrap = true, // Enable word wrap for multiline display
            alignment = TextAnchor.UpperCenter, // Center the text below the icon
            margin = new RectOffset(0, 0, 0, 0), // Remove extra margins
        };

        assetGUIContent = new GUIContent();

        // Set colors based on whether the Editor is in dark or light mode
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

    // Called for each item in the Project Window to customize its appearance


    static void Draw(int instanceID, Rect selectionRect)
    {
        // Skip drawing if multiline display is disabled
        if (!EnableMultilineDisplay)
            return;

        // Check if the asset is currently being renamed
        if (IsRenaming(selectionRect, instanceID))
            return;

        // Skip drawing for sub-assets or very small display areas
        if (selectionRect.height <= 20 || AssetDatabase.IsSubAsset(instanceID))
            return;

        // Get the asset's name
        var path = AssetDatabase.GetAssetPath(instanceID);
        if (string.IsNullOrWhiteSpace(path))
            return;

        var assetName = Path.GetFileNameWithoutExtension(path);

        // Add spaces to camelCase/PascalCase names if the feature is enabled
        if (EnableCamelCaseSpacing)
        {
            assetName = AddSpacesToCamelCase(assetName);
        }

        // Set the asset name for rendering
        assetGUIContent.text = assetName;
        float textHeight = style.CalcHeight(assetGUIContent, selectionRect.width);

        // Define the area where the filename will be rendered
        var nameRect = new Rect(selectionRect.x, selectionRect.yMax - 12, selectionRect.width, textHeight + 4);

        // Handle mouse click events for selecting and renaming the asset
        if (Event.current.type == EventType.MouseDown && nameRect.Contains(Event.current.mousePosition))
        {
            if (!clickTimes.ContainsKey(instanceID))
                clickTimes[instanceID] = 0;

            float currentTime = (float)EditorApplication.timeSinceStartup;
            float lastClickTime = clickTimes[instanceID];
            clickTimes[instanceID] = currentTime;

            if (Selection.activeInstanceID == instanceID)
            {
                if (Event.current.clickCount == 2) // Double-click
                {
                    // Open the asset
                    AssetDatabase.OpenAsset(instanceID);
                    Event.current.Use();
                }
                else if (currentTime - lastClickTime > RenameDelay) // Single-click after delay
                {
                    // Start renaming the asset
                    EditorApplication.delayCall += () =>
                    {
                        if (Selection.activeInstanceID == instanceID) // Ensure still selected
                            EditorApplication.ExecuteMenuItem("Assets/Rename");
                    };
                    Event.current.Use();
                }
            }
            else
            {
                Selection.activeInstanceID = instanceID; // Select the asset
                Event.current.Use();
            }
        }

        // Check if the item is selected
        bool isSelected = Selection.instanceIDs.Contains(instanceID);
        style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : (isSelected ? Color.white : Color.black);

        // Draw background and filename
        var backgroundRect = new Rect(nameRect.x - 6, nameRect.y - 1, nameRect.width + 12, nameRect.height + 3);
        EditorGUI.DrawRect(backgroundRect, isSelected ? selectedFrameColor : backgroundColor);
        GUI.Label(nameRect, assetName, style);
    }




    // Helper method to check if the asset is currently being renamed
    static bool IsRenaming(Rect selectionRect, int instanceID)
    {
        return Selection.activeInstanceID == instanceID && EditorGUIUtility.editingTextField;
    }

    // Adds spaces to camelCase or PascalCase strings for better readability
    static string AddSpacesToCamelCase(string text)
    {
        return Regex.Replace(text, "([a-z])([A-Z])", "$1 $2")
                    .Replace("_", " ") // Replace underscores with spaces
                    .Replace("-", " "); // Replace dashes with spaces
    }
}

// Preferences window for enabling/disabling features
public class ProjectWindowPreferences : SettingsProvider
{
    public ProjectWindowPreferences() : base("Preferences/Project Window", SettingsScope.User)
    { }

    public override void OnGUI(string searchContext)
    {
        const float labelWidth = 200f; // Width for aligning checkboxes

        // Toggle for enabling/disabling multiline display
        GUILayout.BeginHorizontal();
        GUILayout.Label("Enable Multiline Display", GUILayout.Width(labelWidth));
        bool currentMultilineSetting = EditorPrefs.GetBool(ProjectWindowShowFullAssetName.PrefKey_EnableMultilineDisplay, true);
        bool newMultilineSetting = EditorGUILayout.Toggle(currentMultilineSetting);
        GUILayout.EndHorizontal();

        if (newMultilineSetting != currentMultilineSetting)
        {
            EditorPrefs.SetBool(ProjectWindowShowFullAssetName.PrefKey_EnableMultilineDisplay, newMultilineSetting);
            EditorApplication.RepaintProjectWindow(); // Refresh the Project window
        }

        // Toggle for enabling/disabling camelCase spacing (only enabled if multiline display is on)
        EditorGUI.BeginDisabledGroup(!newMultilineSetting);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Enable Expanded Filenames", GUILayout.Width(labelWidth));
        bool currentCamelCaseSetting = EditorPrefs.GetBool(ProjectWindowShowFullAssetName.PrefKey_EnableCamelCaseSpacing, true);
        bool newCamelCaseSetting = EditorGUILayout.Toggle(currentCamelCaseSetting);
        GUILayout.EndHorizontal();

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