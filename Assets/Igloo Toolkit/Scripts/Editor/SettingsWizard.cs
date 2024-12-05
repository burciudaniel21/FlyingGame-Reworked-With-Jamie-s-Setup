using UnityEngine;
using UnityEditor;
using Igloo.Controllers;

namespace Igloo.Common
{
    public class SettingsWizard : EditorWindow
    {
        public enum SettingsType { Project, File }
        public SettingsType settingsType;

        public enum ConfigType { EquirectangularStrip, EquirectangularFull, CUBE, CAVE }
        public ConfigType configType = ConfigType.EquirectangularStrip;

        Texture logo;

        bool showNetwork = false;
        bool showDisplay = false;
        bool showPlayer = false;
        bool showUI = false;
        bool showHeadTracking = false;
        bool showSystem = false;

        int compositSelected = 0;
        static string[] compositeOptions = new string[] { "None", "Cubemap To Equi", "Combine Displays" };

        Settings settings;
        int activeDisplay;
        Vector2 scrollPos;

        bool applyToAllDisplays = false;

        [MenuItem("Igloo/Settings Wizard")]
        static void Init()
        {
            SettingsWizard window = (SettingsWizard)EditorWindow.GetWindow(typeof(SettingsWizard));
            window.Show();
        }

        public Settings LoadSettings(SettingsType type, string path = null)
        {
            Settings settings = null;
            Serializer serializer = new Serializer();
            switch (type)
            {
                case SettingsType.Project:
                    settings = serializer.Load(System.IO.Path.Combine(Utils.GetDataPath() + "/", "IglooSettings"));
                    break;
                case SettingsType.File:
                    settings = serializer.Load(path);
                    break;
            }

            return settings;
        }

        public Settings LoadDefaultSettings(ConfigType type = ConfigType.EquirectangularStrip)
        {
            Settings settings = null;
            switch (type)
            {
                case ConfigType.EquirectangularStrip:
                    settings = DefaultConfigurations.EquirectangularCylinder();
                    break;
                case ConfigType.EquirectangularFull:
                    settings = DefaultConfigurations.EquirectangularFull();
                    break;
                case ConfigType.CUBE:
                    settings = DefaultConfigurations.Cube();
                    break;
                case ConfigType.CAVE:
                    settings = DefaultConfigurations.Cave();
                    break;
                default:
                    break;
            }
            return settings;
        }

        public bool SaveSettings(Settings _settings, bool saveAs = false)
        {
            settings.version = Utils.GetVersion();
            Igloo.Serializer serializer = new Serializer();
            string path = System.IO.Path.Combine(Utils.GetDataPath() + "/", "IglooSettings");
            if (saveAs) path = EditorUtility.SaveFilePanel("Set Save Location", "", "IglooSettings", "xml");
            serializer.Save(path, settings);
            return true;
        }

        private void OnGUI()
        {

            EditorGUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));

            if (logo)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box(logo, GUILayout.Width(200), GUILayout.Height(108), GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                bool hasPro = UnityEditorInternal.InternalEditorUtility.HasPro();
                if (hasPro) logo = Resources.Load<Texture>("IglooLogoWhite");
                else logo = Resources.Load<Texture>("IglooLogoBlack");
            }

            if (settings != null)
            {
                // Network
                GUILayout.Label("Network Settings", EditorStyles.boldLabel);
                if (showNetwork)
                {
                    if (GUILayout.Button("Network Settings -")) { showNetwork = false; }

                    settings.NetworkSettings.inPort = EditorGUILayout.IntField("In Port:", settings.NetworkSettings.inPort);
                    settings.NetworkSettings.outPort = EditorGUILayout.IntField("Out Port:", settings.NetworkSettings.outPort);

                    char separator = '.';
                    string[] ip = settings.NetworkSettings.outIP.Split(separator);

                    if (ip.Length == 4)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("IP:");
                        ip[0] = EditorGUILayout.IntField("", int.Parse(ip[0]), GUILayout.MaxWidth(100.0f)).ToString();
                        ip[1] = EditorGUILayout.IntField("", int.Parse(ip[1]), GUILayout.MaxWidth(100.0f)).ToString();
                        ip[2] = EditorGUILayout.IntField("", int.Parse(ip[2]), GUILayout.MaxWidth(100.0f)).ToString();
                        ip[3] = EditorGUILayout.IntField("", int.Parse(ip[3]), GUILayout.MaxWidth(100.0f)).ToString();
                        GUILayout.EndHorizontal();
                        settings.NetworkSettings.outIP = ip[0] + "." + ip[1] + "." + ip[2] + "." + ip[3];
                    }
                }
                else
                {
                    if (GUILayout.Button("Network Settings +")) { showNetwork = true; }
                }

                // Display
                GUILayout.Label("Display Settings", EditorStyles.boldLabel);
                if (showDisplay)
                {
                    if (GUILayout.Button("Display Settings -")) { showDisplay = false; }
                    settings.DisplaySettings.Name = EditorGUILayout.TextField("Name:", settings.DisplaySettings.Name);

                    GUILayout.Space(10f);
                    GUILayout.Label("Custom Output Settings:", EditorStyles.boldLabel);

                    // TODO - Make this less hacky
                    if (settings.DisplaySettings.useCubemapToEquirectangular) compositSelected = 1;
                    else if (settings.DisplaySettings.useCompositeTexture) compositSelected = 2;
                    else compositSelected = 0;
                    compositSelected = GUILayout.SelectionGrid(compositSelected, compositeOptions, compositeOptions.Length, EditorStyles.radioButton);

                    if (compositSelected == 0)
                    {
                        settings.DisplaySettings.useCubemapToEquirectangular = false;
                        settings.DisplaySettings.useCompositeTexture = false;
                    }
                    else if (compositSelected == 1)
                    {
                        settings.DisplaySettings.useCubemapToEquirectangular = true;
                        settings.DisplaySettings.useCompositeTexture = false;
                    }
                    else if (compositSelected == 2)
                    {
                        settings.DisplaySettings.useCubemapToEquirectangular = false;
                        settings.DisplaySettings.useCompositeTexture = true;
                    }

                    GUILayout.Space(10f);
                    if (settings.DisplaySettings.useCubemapToEquirectangular)
                    {
                        settings.DisplaySettings.textureShareMode = (int)(TextureShareUtility.TextureShareMode)EditorGUILayout.EnumPopup("Sharing Mode:", (TextureShareUtility.TextureShareMode)settings.DisplaySettings.textureShareMode);
                        GUILayout.Space(5f);
                        settings.DisplaySettings.useFramepackTopBottom3D = EditorGUILayout.Toggle("Composite 3D Top/Bottom :", settings.DisplaySettings.useFramepackTopBottom3D);
                        GUILayout.Space(5f);
                        settings.DisplaySettings.equirectangularTextureResolution = new Vector2IntItem(EditorGUILayout.Vector2IntField("Equirectangular Output Resolution:", settings.DisplaySettings.equirectangularTextureResolution.Vector2Int));
                        settings.DisplaySettings.horizontalFOV = EditorGUILayout.FloatField("Horizontal FOV:", settings.DisplaySettings.horizontalFOV);
                        settings.DisplaySettings.verticalFOV = EditorGUILayout.FloatField("Vertical FOV:", settings.DisplaySettings.verticalFOV);
                    }
                    else if (settings.DisplaySettings.useCompositeTexture)
                    {
                        settings.DisplaySettings.textureShareMode = (int)(TextureShareUtility.TextureShareMode)EditorGUILayout.EnumPopup("Sharing Mode:", (TextureShareUtility.TextureShareMode)settings.DisplaySettings.textureShareMode);
                        GUILayout.Space(5f);
                        settings.DisplaySettings.useFramepackTopBottom3D = EditorGUILayout.Toggle("Composite 3D Top/Bottom :", settings.DisplaySettings.useFramepackTopBottom3D);
                    }

                    GUILayout.Space(10f);
                    GUILayout.Label("Head Position Settings:", EditorStyles.boldLabel);
                    settings.DisplaySettings.HeadSettings.headPositionOffset = new Vector3Item(EditorGUILayout.Vector3Field("Head Position offset:", settings.DisplaySettings.HeadSettings.headPositionOffset.Vector3));
                    settings.DisplaySettings.HeadSettings.headRotationOffset = new Vector3Item(EditorGUILayout.Vector3Field("Head Rotation offset:", settings.DisplaySettings.HeadSettings.headRotationOffset.Vector3));

                    GUILayout.Space(10f);
                    GUILayout.Label("Display Selected: " + (activeDisplay + 1).ToString(), EditorStyles.boldLabel);

                    applyToAllDisplays = EditorGUILayout.Toggle("Apply to all displays:", applyToAllDisplays);
                    if (applyToAllDisplays) EditorGUILayout.HelpBox("Changes to properties hightlighted in red will be applied to all displays", MessageType.Info);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("<"))
                    {
                        activeDisplay--;
                        if (activeDisplay < 0) activeDisplay = settings.DisplaySettings.Displays.Length - 1;
                    }
                    if (GUILayout.Button(">"))
                    {
                        activeDisplay++;
                        activeDisplay = activeDisplay % settings.DisplaySettings.Displays.Length;
                    }
                    GUILayout.EndHorizontal();


                    Color defaultColor = GUI.color;
                    settings.DisplaySettings.Displays[activeDisplay].Name = EditorGUILayout.TextField("Display Name:", settings.DisplaySettings.Displays[activeDisplay].Name);

                    if (applyToAllDisplays) GUI.color = Color.red;
                    settings.DisplaySettings.Displays[activeDisplay].fov = EditorGUILayout.FloatField("Camera FOV:", settings.DisplaySettings.Displays[activeDisplay].fov);
                    if (applyToAllDisplays)
                    {
                        foreach (var item in settings.DisplaySettings.Displays) item.fov = settings.DisplaySettings.Displays[activeDisplay].fov;
                        GUI.color = defaultColor;
                    }

                    if (applyToAllDisplays) GUI.color = Color.red;
                    settings.DisplaySettings.Displays[activeDisplay].nearClipPlane = EditorGUILayout.FloatField("Near Clip:", settings.DisplaySettings.Displays[activeDisplay].nearClipPlane);
                    if (applyToAllDisplays)
                    {
                        foreach (var item in settings.DisplaySettings.Displays) item.nearClipPlane = settings.DisplaySettings.Displays[activeDisplay].nearClipPlane;
                        GUI.color = defaultColor;
                    }

                    if (applyToAllDisplays) GUI.color = Color.red;
                    settings.DisplaySettings.Displays[activeDisplay].farClipPlane = EditorGUILayout.FloatField("Far Clip:", settings.DisplaySettings.Displays[activeDisplay].farClipPlane);
                    if (applyToAllDisplays)
                    {
                        foreach (var item in settings.DisplaySettings.Displays) item.farClipPlane = settings.DisplaySettings.Displays[activeDisplay].farClipPlane;
                        GUI.color = defaultColor;
                    }

                    if (applyToAllDisplays) GUI.color = Color.red;
                    settings.DisplaySettings.Displays[activeDisplay].is3D = EditorGUILayout.Toggle("3D:", settings.DisplaySettings.Displays[activeDisplay].is3D);
                    if (applyToAllDisplays)
                    {
                        foreach (var item in settings.DisplaySettings.Displays) item.is3D = settings.DisplaySettings.Displays[activeDisplay].is3D;
                        GUI.color = defaultColor;
                    }


                    if (settings.DisplaySettings.useCubemapToEquirectangular)
                        settings.DisplaySettings.Displays[activeDisplay].cubemapFace = (int)(Display.IglooCubemapFace)EditorGUILayout.EnumPopup("Cubemap Face:", (Display.IglooCubemapFace)settings.DisplaySettings.Displays[activeDisplay].cubemapFace);

                    if (applyToAllDisplays) GUI.color = Color.red;
                    settings.DisplaySettings.Displays[activeDisplay].isFisheye = EditorGUILayout.Toggle("Apply Fisheye:", settings.DisplaySettings.Displays[activeDisplay].isFisheye);
                    if (applyToAllDisplays)
                    {
                        foreach (var item in settings.DisplaySettings.Displays) item.isFisheye = settings.DisplaySettings.Displays[activeDisplay].isFisheye;
                        GUI.color = defaultColor;
                    }

                    if (settings.DisplaySettings.Displays[activeDisplay].isFisheye)
                    {
                        if (applyToAllDisplays) GUI.color = Color.red;
                        settings.DisplaySettings.Displays[activeDisplay].fisheyeStrength = new Vector2Item(EditorGUILayout.Vector2Field("Fisheye:", settings.DisplaySettings.Displays[activeDisplay].fisheyeStrength.Vector2));
                        if (applyToAllDisplays)
                        {
                            foreach (var item in settings.DisplaySettings.Displays) item.fisheyeStrength = settings.DisplaySettings.Displays[activeDisplay].fisheyeStrength;
                            GUI.color = defaultColor;
                        }
                    }

                    if (applyToAllDisplays) GUI.color = Color.red;
                    settings.DisplaySettings.Displays[activeDisplay].renderTextureSize = new Vector2IntItem(EditorGUILayout.Vector2IntField("Camera Resolution:", settings.DisplaySettings.Displays[activeDisplay].renderTextureSize.Vector2Int));
                    if (applyToAllDisplays)
                    {
                        foreach (var item in settings.DisplaySettings.Displays) item.renderTextureSize = settings.DisplaySettings.Displays[activeDisplay].renderTextureSize;
                        GUI.color = defaultColor;
                    }

                    if (applyToAllDisplays) GUI.color = Color.red;
                    settings.DisplaySettings.Displays[activeDisplay].textureShareMode = (int)(TextureShareUtility.TextureShareMode)EditorGUILayout.EnumPopup("Sharing Mode:", (TextureShareUtility.TextureShareMode)settings.DisplaySettings.Displays[activeDisplay].textureShareMode);
                    if (applyToAllDisplays)
                    {
                        foreach (var item in settings.DisplaySettings.Displays) item.textureShareMode = settings.DisplaySettings.Displays[activeDisplay].textureShareMode;
                        GUI.color = defaultColor;
                    }


                    GUILayout.Space(10f);

                    settings.DisplaySettings.Displays[activeDisplay].isOffAxis = EditorGUILayout.Toggle("Off Axis Projection:", settings.DisplaySettings.Displays[activeDisplay].isOffAxis);
                    if (settings.DisplaySettings.Displays[activeDisplay].isOffAxis)
                    {
                        settings.DisplaySettings.Displays[activeDisplay].viewportPosition = new Vector3Item(EditorGUILayout.Vector3Field("Viewport Position:", settings.DisplaySettings.Displays[activeDisplay].viewportPosition.Vector3));
                        settings.DisplaySettings.Displays[activeDisplay].viewportRotation = new Vector3Item(EditorGUILayout.Vector3Field("Viewport Rotation:", settings.DisplaySettings.Displays[activeDisplay].viewportRotation.Vector3));
                        settings.DisplaySettings.Displays[activeDisplay].viewportSize = new Vector2Item(EditorGUILayout.Vector2Field("Viewport Size:", settings.DisplaySettings.Displays[activeDisplay].viewportSize.Vector2));
                    }
                    else
                    {
                        settings.DisplaySettings.Displays[activeDisplay].cameraRotation = new Vector3Item(EditorGUILayout.Vector3Field("Rotation:", settings.DisplaySettings.Displays[activeDisplay].cameraRotation.Vector3));
                    }
                    GUILayout.Space(10f);
                    settings.DisplaySettings.Displays[activeDisplay].targetDisplay = EditorGUILayout.IntField("Target Display:", settings.DisplaySettings.Displays[activeDisplay].targetDisplay);

                }
                else
                {
                    if (GUILayout.Button("Display Settings +")) { showDisplay = true; }
                }

                GUILayout.Label("Headtracking Settings", EditorStyles.boldLabel);

                if (showHeadTracking) {
                    if (GUILayout.Button("Headtracking Settings -")) { showHeadTracking = false; }
                    settings.DisplaySettings.HeadSettings.headtracking = EditorGUILayout.Toggle("Use Headtracking:", settings.DisplaySettings.HeadSettings.headtracking);
                    if (settings.DisplaySettings.HeadSettings.headtracking) {
                        settings.DisplaySettings.HeadSettings.headTrackingInput = (int)(HeadManager.HeadTrackingInput)EditorGUILayout.EnumPopup("HeadTracking Input:", (HeadManager.HeadTrackingInput)settings.DisplaySettings.HeadSettings.headTrackingInput);
                        if (settings.DisplaySettings.HeadSettings.headTrackingInput == 1) {
                            settings.DisplaySettings.HeadSettings.optitrackServerIP = EditorGUILayout.TextField("Optitrack Server IP:", settings.DisplaySettings.HeadSettings.optitrackServerIP);
                            settings.DisplaySettings.HeadSettings.optitrackHeadRigidBodyID = EditorGUILayout.IntField("Head Rigidbody ID:", settings.DisplaySettings.HeadSettings.optitrackHeadRigidBodyID);

                        }
                    }


                } else {
                    if(GUILayout.Button("Headtracking Settings +")) { showHeadTracking = true; }
                }

                // Player
                GUILayout.Label("Player Settings", EditorStyles.boldLabel);
                if (showPlayer)
                {
                    if (GUILayout.Button("Player Settings -")) { showPlayer = false; }
                    settings.PlayerSettings.usePlayer = EditorGUILayout.Toggle("Use Player", settings.PlayerSettings.usePlayer);
                    if (settings.PlayerSettings.usePlayer)
                    {
                        settings.PlayerSettings.rotationInput = (int)(PlayerManager.ROTATION_INPUT)EditorGUILayout.EnumPopup("Rotation Input:", (PlayerManager.ROTATION_INPUT)settings.PlayerSettings.rotationInput);
                        if (settings.PlayerSettings.rotationInput == 4) { 
                            settings.PlayerSettings.optitrackControllerRigidBodyID = EditorGUILayout.IntField("Controller Rigidbody ID", settings.PlayerSettings.optitrackControllerRigidBodyID);
                            settings.DisplaySettings.HeadSettings.optitrackServerIP = EditorGUILayout.TextField("Optitrack Server IP:", settings.DisplaySettings.HeadSettings.optitrackServerIP);
                            GUILayout.Space(10f);
                        }
                        settings.PlayerSettings.rotationMode = (int)(PlayerManager.ROTATION_MODE)EditorGUILayout.EnumPopup("Rotation Mode:", (PlayerManager.ROTATION_MODE)settings.PlayerSettings.rotationMode);
                        settings.PlayerSettings.movementInput = (int)(PlayerManager.MOVEMENT_INPUT)EditorGUILayout.EnumPopup("Movement Input:", (PlayerManager.MOVEMENT_INPUT)settings.PlayerSettings.movementInput);
                        settings.PlayerSettings.movementMode = (int)(PlayerManager.MOVEMENT_MODE)EditorGUILayout.EnumPopup("Movement Mode:", (PlayerManager.MOVEMENT_MODE)settings.PlayerSettings.movementMode);
                        settings.PlayerSettings.walkSpeed = EditorGUILayout.FloatField("Walk Speed:", settings.PlayerSettings.walkSpeed);
                        settings.PlayerSettings.runSpeed = EditorGUILayout.FloatField("Run Speed:", settings.PlayerSettings.runSpeed);
                        settings.PlayerSettings.smoothTime = EditorGUILayout.FloatField("Rotation Smoothing:", settings.PlayerSettings.smoothTime);
                        settings.PlayerSettings.crosshairHideMode = (int)(Crosshair.CROSSHAIR_MODE)EditorGUILayout.EnumPopup("Crosshair Hide Mode:", (Crosshair.CROSSHAIR_MODE)settings.PlayerSettings.crosshairHideMode);
                    }
                }
                else
                {
                    if (GUILayout.Button("Player Settings +")) { showPlayer = true; }
                }

                // UI
                GUILayout.Label("UI Settings", EditorStyles.boldLabel);
                if (showUI)
                {
                    if (GUILayout.Button("UI Settings -")) { showUI = false; }
                    settings.UISettings.useUI = EditorGUILayout.Toggle("Use UI", settings.UISettings.useUI);
                    if (settings.UISettings.useUI)
                    {
                        EditorGUILayout.HelpBox("Screen name must be either 'Cylinder' or 'Plane' , unless using custom geometry", MessageType.Info);
                        settings.UISettings.screenName = EditorGUILayout.TextField("Screen Name:", settings.UISettings.screenName);
                        settings.UISettings.screenPos = new Vector3Item(EditorGUILayout.Vector3Field("Screen Position:", settings.UISettings.screenPos.Vector3));
                        settings.UISettings.screenRot = new Vector3Item(EditorGUILayout.Vector3Field("Screen Rotation:", settings.UISettings.screenRot.Vector3));
                        settings.UISettings.screenScale = new Vector3Item(EditorGUILayout.Vector3Field("Screen Scale:", settings.UISettings.screenScale.Vector3));
                        settings.UISettings.followCrosshair = EditorGUILayout.Toggle("Follow Crosshair:", settings.UISettings.followCrosshair);
                        settings.UISettings.followSpeed = EditorGUILayout.FloatField("Follow Speed:", settings.UISettings.followSpeed);
                        settings.UISettings.debugUIMode = EditorGUILayout.Toggle("Debug UI Cursor", settings.UISettings.debugUIMode);
                        settings.TouchScreenSettings.UseTouchScreen = EditorGUILayout.Toggle("Use Touch Screen System", settings.TouchScreenSettings.UseTouchScreen);
                        if (settings.TouchScreenSettings.UseTouchScreen) {
                            settings.TouchScreenSettings.XPositionStart = EditorGUILayout.FloatField("Window Start Horizontal Value", settings.TouchScreenSettings.XPositionStart);
                            settings.TouchScreenSettings.XPositionEnd = EditorGUILayout.FloatField("Window End Horizontal Value", settings.TouchScreenSettings.XPositionEnd);
                            settings.TouchScreenSettings.YPositionStart = EditorGUILayout.FloatField("Window Start Vertical Value", settings.TouchScreenSettings.YPositionStart);
                            settings.TouchScreenSettings.YPositionEnd = EditorGUILayout.FloatField("Window End Vertical Value", settings.TouchScreenSettings.YPositionEnd);
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("UI Settings +")) { showUI = true; }
                }

                // System
                GUILayout.Label("System Settings", EditorStyles.boldLabel);
                if (showSystem)
                {
                    if (GUILayout.Button("System Settings -")) { showSystem = false; }
                    settings.SystemSettings.vSyncMode = (int)(VSyncMode)EditorGUILayout.EnumPopup("Vsync Mode:", (VSyncMode)settings.SystemSettings.vSyncMode);
                    settings.SystemSettings.targetFPS = EditorGUILayout.IntField("FPS:", settings.SystemSettings.targetFPS);
                    settings.SystemSettings.useDisplaySettingsOverride = EditorGUILayout.Toggle("Use Global Display Settings:", settings.SystemSettings.useDisplaySettingsOverride);
                    if (settings.SystemSettings.useDisplaySettingsOverride)
                    {
                        settings.SystemSettings.displaySettingsOverridePath = EditorGUILayout.TextField("Global Settings Folder Path", settings.SystemSettings.displaySettingsOverridePath);
                    }
                }
                else
                {
                    if (GUILayout.Button("System Settings +")) { showSystem = true; }
                }

            }


            GUILayout.Space(10f);
            GUILayout.Label("Loading and Saving", EditorStyles.boldLabel);

            settingsType = (SettingsType)EditorGUILayout.EnumPopup("Load settings from:", settingsType);
            if (GUILayout.Button("Load"))
            {
                string path = null;
                if (settingsType == SettingsType.File)
                {
                    path = EditorUtility.OpenFilePanel("Select Igloo Settings file", "", "xml");
                }
                Settings settingsTemp = LoadSettings(settingsType, path);
                if (settingsTemp != null) settings = settingsTemp;
            }

            GUILayout.Space(10f);
            configType = (ConfigType)EditorGUILayout.EnumPopup("Configuration Type:", configType);
            if (GUILayout.Button("Create Default Configuration"))
            {
                settings = LoadDefaultSettings(configType);
            }
            GUILayout.Space(10f);

            if (settings != null) { if (GUILayout.Button("Save")) { SaveSettings(settings); } }
            if (settings != null) { if (GUILayout.Button("Save As..")) { SaveSettings(settings, true); } }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

        }
    }
}

