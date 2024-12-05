using UnityEngine;

namespace Igloo
{

    /// <summary>
    /// Default Igloo Settings configurations
    /// </summary>
    public static class DefaultConfigurations
    {
        /// <summary>
        /// Five Camera system settings
        /// </summary>
        /// <returns>Currently returns Equirectangular Cylinder settings</returns>
        public static Settings CylinderFiveCamera()
        {
            // Todo 
            return EquirectangularCylinder();
        }

        /// <summary>
        /// Equirectangular full dome system
        /// </summary>
        /// <returns>Currently returns Equirectangular Cylinder settings</returns>
        public static Settings Equirectangular360()
        {
            // Todo 
            return EquirectangularCylinder();
        }

        /// <summary>
        /// Cave based Igloo System
        /// </summary>
        /// <returns>Populated Settings File for an Igloo Cave System</returns>
        public static Settings Cave()
        {
            Settings settings = new Settings();

            SystemSettings systemSettings = new SystemSettings
            {
                targetFPS = 0,
                vSyncMode = 0,
                useDisplaySettingsOverride = true,
                displaySettingsOverridePath = "%ProgramData%/Igloo Vision/IglooCoreEngine/settings/Unity/displaySettingsOverride"
            };

            settings.SystemSettings = systemSettings;

            NetworkSettings networkSettings = new NetworkSettings
            {
                inPort = 9007,
                outPort = 9001,
                outIP = "127.0.0.1"
            };

            settings.NetworkSettings = networkSettings;

            DisplaySettings displaySettings = new DisplaySettings
            {
                Name = "IglooUnity",
                useCompositeTexture = false,
                useCubemapToEquirectangular = false,
                useFramepackTopBottom3D = false,
                textureShareMode = 0,
                horizontalFOV = 360f,
                verticalFOV = 70f,
                equirectangularTextureResolution = new Vector2IntItem(8000, 1000)
            };

            HeadSettings headSettings = new HeadSettings
            {
                headRotationOffset = new Vector3Item(Vector3.zero),
                headPositionOffset = new Vector3Item(0, 1, 0),
                leftEyeOffset = new Vector3Item(-0.05f, 0, 0),
                rightEyeOffset = new Vector3Item(0.05f, 0, 0),
                headtracking = false,
                optitrackServerIP = "127.0.0.1",
                optitrackHeadRigidBodyID = 1
            };

            displaySettings.HeadSettings = headSettings;


            int numCams = 4;
            DisplayItem[] displayItems = new DisplayItem[numCams];

            for (int i = 0; i < numCams; i++)
            {
                DisplayItem displayItem = new DisplayItem
                {
                    Name = displaySettings.Name + (i + 1).ToString(),
                    fov = 90f,
                    cubemapFace = i,
                    is3D = false,
                    isFisheye = false,
                    isRendering = true,
                    isRenderTextures = false,
                    textureShareMode = 0,
                    nearClipPlane = 0.01f,
                    farClipPlane = 1000f
                };
                if (i < 4) displayItem.cameraRotation = new Vector3Item(0, (i * 90f), 0f);
                displayItem.renderTextureSize = new Vector2IntItem(2000, 2000);
                displayItem.fisheyeStrength = new Vector2Item(0, 0);
                displayItem.isOffAxis = true;
                if (i == 0)
                {
                    displayItem.viewportRotation = new Vector3Item(0, -90, 0);
                    displayItem.viewportPosition = new Vector3Item(-1, 1, 0);
                }
                else if (i == 1)
                {
                    displayItem.viewportRotation = new Vector3Item(0, 0, 0);
                    displayItem.viewportPosition = new Vector3Item(0, 1, 1);
                }
                else if (i == 2)
                {
                    displayItem.viewportRotation = new Vector3Item(0, 90, 0);
                    displayItem.viewportPosition = new Vector3Item(1, 1, 0);
                }
                else if (i == 3)
                {
                    displayItem.viewportRotation = new Vector3Item(90, 0, 0);
                    displayItem.viewportPosition = new Vector3Item(0, 0, 0);
                }
                displayItem.viewportSize = new Vector2Item(2.0f, 2.0f);
                displayItem.targetDisplay = i + 1;
                displayItems[i] = displayItem;
            }

            displaySettings.Displays = displayItems;

            settings.DisplaySettings = displaySettings;

            PlayerSettings playerSettings = new PlayerSettings
            {
                Name = "player",
                usePlayer = true,
                rotationInput = 0,
                rotationMode = 0,
                movementInput = 0,
                movementMode = 2,
                runSpeed = 10,
                walkSpeed = 5,
                smoothTime = 10,
                isCrosshair3D = false,
                crosshairHideMode = 0,
                optitrackControllerRigidBodyID = 2
            };

            settings.PlayerSettings = playerSettings;


            UISettings uiSettings = new UISettings
            {
                useUI = true,
                screenName = "Cylinder",
                screenPos = new Vector3Item(Vector3.zero),
                screenRot = new Vector3Item(Vector3.zero),
                screenScale = new Vector3Item(6.0f, 2.1f, 6.0f),
                followCrosshair = true,
                followSpeed = 10,
                debugUIMode = false
            };

            TouchScreenSettings touchScreenSettings = new TouchScreenSettings {
                UseTouchScreen = false,
                XPositionStart = 1.0f,
                XPositionEnd = 0.0f,
                YPositionStart = 1.0f,
                YPositionEnd = 0.0f
            };

            settings.TouchScreenSettings = touchScreenSettings;

            settings.UISettings = uiSettings;

            return settings;

        }

        /// <summary>
        /// Standard Equirectangular Cylinder System settings
        /// </summary>
        /// <returns>A populated settings file for an Equirectangular Cylinder based igloo</returns>
        public static Settings EquirectangularCylinder()
        {

            Settings settings = new Settings();

            SystemSettings systemSettings = new SystemSettings
            {
                targetFPS = 0,
                vSyncMode = 0,
                useDisplaySettingsOverride = true,
                displaySettingsOverridePath = "%ProgramData%/Igloo Vision/IglooCoreEngine/settings/Unity/displaySettingsOverride"
            };

            settings.SystemSettings = systemSettings;

            NetworkSettings networkSettings = new NetworkSettings
            {
                inPort = 9007,
                outPort = 9001,
                outIP = "127.0.0.1"
            };

            settings.NetworkSettings = networkSettings;

            DisplaySettings displaySettings = new DisplaySettings
            {
                Name = "IglooUnity",
                useCompositeTexture = false,
                useCubemapToEquirectangular = true,
                useFramepackTopBottom3D = false,
                textureShareMode = 1,
                horizontalFOV = 360f,
                verticalFOV = 70f,
                equirectangularTextureResolution = new Vector2IntItem(8000, 1000)
            };

            HeadSettings headSettings = new HeadSettings
            {
                headRotationOffset = new Vector3Item(Vector3.zero),
                headPositionOffset = new Vector3Item(0, 1.8f, 0),
                leftEyeOffset = new Vector3Item(-0.05f, 0, 0),
                rightEyeOffset = new Vector3Item(0.05f, 0, 0),
                headtracking = false,
                optitrackServerIP = "127.0.0.1",
                optitrackHeadRigidBodyID = 1
            };

            displaySettings.HeadSettings = headSettings;


            int numCams = 4;
            DisplayItem[] displayItems = new DisplayItem[numCams];

            for (int i = 0; i < numCams; i++)
            {
                DisplayItem displayItem = new DisplayItem
                {
                    Name = displaySettings.Name + (i + 1).ToString(),
                    fov = 90f,
                    cubemapFace = i,
                    is3D = false,
                    isFisheye = false,
                    isRendering = true,
                    isRenderTextures = true,
                    textureShareMode = 0,
                    nearClipPlane = 0.01f,
                    farClipPlane = 1000f
                };
                if (i < 4) displayItem.cameraRotation = new Vector3Item(0, (i * 90f), 0f);
                displayItem.renderTextureSize = new Vector2IntItem(2000, 2000);
                displayItem.fisheyeStrength = new Vector2Item(0, 0);
                displayItem.isOffAxis = false;
                displayItem.viewportRotation = new Vector3Item(Vector3.zero);
                displayItem.viewportPosition = new Vector3Item(Vector3.zero);
                displayItem.viewportSize = new Vector2Item(Vector2.zero);
                displayItem.targetDisplay = -1;

                displayItems[i] = displayItem;
            }

            displaySettings.Displays = displayItems;

            settings.DisplaySettings = displaySettings;

            PlayerSettings playerSettings = new PlayerSettings
            {
                Name = "player",
                usePlayer = true,
                rotationInput = 0,
                rotationMode = 0,
                movementInput = 0,
                movementMode = 2,
                runSpeed = 10,
                walkSpeed = 5,
                smoothTime = 10,
                isCrosshair3D = false,
                crosshairHideMode = 0,
                optitrackControllerRigidBodyID = 2
            };

            settings.PlayerSettings = playerSettings;


            UISettings uiSettings = new UISettings
            {
                useUI = true,
                screenName = "Cylinder",
                screenPos = new Vector3Item(Vector3.zero),
                screenRot = new Vector3Item(Vector3.zero),
                screenScale = new Vector3Item(6.0f, 2.1f, 6.0f),
                followCrosshair = true,
                followSpeed = 10,
                debugUIMode = false
            };

            settings.UISettings = uiSettings;

            TouchScreenSettings touchScreenSettings = new TouchScreenSettings {
                UseTouchScreen = false,
                XPositionStart = 1.0f,
                XPositionEnd = 0.0f,
                YPositionStart = 1.0f,
                YPositionEnd = 0.0f
            };

            settings.TouchScreenSettings = touchScreenSettings;

            return settings;
        }

        /// <summary>
        /// Full dome equirectangular igloo system settings
        /// </summary>
        /// <returns>A populated settings file for a full dome Igloo system</returns>
        public static Settings EquirectangularFull()
        {

            Settings settings = new Settings();

            SystemSettings systemSettings = new SystemSettings
            {
                targetFPS = 0,
                vSyncMode = 0,
                useDisplaySettingsOverride = true,
                displaySettingsOverridePath = "%ProgramData%/Igloo Vision/IglooCoreEngine/settings/Unity/displaySettingsOverride"
            };

            settings.SystemSettings = systemSettings;

            NetworkSettings networkSettings = new NetworkSettings
            {
                inPort = 9007,
                outPort = 9001,
                outIP = "127.0.0.1"
            };

            settings.NetworkSettings = networkSettings;

            DisplaySettings displaySettings = new DisplaySettings
            {
                Name = "IglooUnity",
                useCompositeTexture = false,
                useCubemapToEquirectangular = true,
                useFramepackTopBottom3D = false,
                textureShareMode = 1,
                horizontalFOV = 360f,
                verticalFOV = 180f,
                equirectangularTextureResolution = new Vector2IntItem(8000, 4000)
            };

            HeadSettings headSettings = new HeadSettings
            {
                headRotationOffset = new Vector3Item(Vector3.zero),
                headPositionOffset = new Vector3Item(0, 1.8f, 0),
                leftEyeOffset = new Vector3Item(-0.05f, 0, 0),
                rightEyeOffset = new Vector3Item(0.05f, 0, 0),
                headtracking = false,
                optitrackServerIP = "127.0.0.1",
                optitrackHeadRigidBodyID = 1
            };


            displaySettings.HeadSettings = headSettings;


            int numCams = 6;
            DisplayItem[] displayItems = new DisplayItem[numCams];

            for (int i = 0; i < numCams; i++)
            {
                DisplayItem displayItem = new DisplayItem
                {
                    Name = displaySettings.Name + (i + 1).ToString(),
                    fov = 90f,
                    cubemapFace = i,
                    is3D = false,
                    isFisheye = false,
                    isRendering = true,
                    isRenderTextures = true,
                    textureShareMode = 0,
                    nearClipPlane = 0.01f,
                    farClipPlane = 1000f
                };
                if (i < 4) displayItem.cameraRotation = new Vector3Item(0, (i * 90f), 0f);
                else if (i == 4) displayItem.cameraRotation = new Vector3Item(-90f, 0f, 0f);
                else if (i == 5) displayItem.cameraRotation = new Vector3Item(90f, 0f, 0f);
                displayItem.renderTextureSize = new Vector2IntItem(2000, 2000);
                displayItem.fisheyeStrength = new Vector2Item(0, 0);
                displayItem.isOffAxis = false;
                displayItem.viewportRotation = new Vector3Item(Vector3.zero);
                displayItem.viewportPosition = new Vector3Item(Vector3.zero);
                displayItem.viewportSize = new Vector2Item(Vector2.zero);
                displayItem.targetDisplay = -1;

                displayItems[i] = displayItem;
            }

            displaySettings.Displays = displayItems;

            settings.DisplaySettings = displaySettings;

            PlayerSettings playerSettings = new PlayerSettings
            {
                Name = "player",
                usePlayer = true,
                rotationInput = 0,
                rotationMode = 0,
                movementInput = 0,
                movementMode = 2,
                runSpeed = 10,
                walkSpeed = 5,
                smoothTime = 10,
                isCrosshair3D = false,
                crosshairHideMode = 0,
                optitrackControllerRigidBodyID = 2
            };

            settings.PlayerSettings = playerSettings;


            UISettings uiSettings = new UISettings
            {
                useUI = true,
                screenName = "Cylinder",
                screenPos = new Vector3Item(Vector3.zero),
                screenRot = new Vector3Item(Vector3.zero),
                screenScale = new Vector3Item(6.0f, 2.1f, 6.0f),
                followCrosshair = true,
                followSpeed = 10,
                debugUIMode = false
            };

            settings.UISettings = uiSettings;

            TouchScreenSettings touchScreenSettings = new TouchScreenSettings {
                UseTouchScreen = false,
                XPositionStart = 1.0f,
                XPositionEnd = 0.0f,
                YPositionStart = 1.0f,
                YPositionEnd = 0.0f
            };

            settings.TouchScreenSettings = touchScreenSettings;

            return settings;
        }

        /// <summary>
        /// Settings for a cube based Igloo System
        /// </summary>
        /// <returns>A populated settings file for a cube based Igloo system</returns>
        public static Settings Cube()
        {

            Settings settings = new Settings();

            SystemSettings systemSettings = new SystemSettings
            {
                targetFPS = 0,
                vSyncMode = 0,
                useDisplaySettingsOverride = true,
                displaySettingsOverridePath = "%ProgramData%/Igloo Vision/IglooCoreEngine/settings/Unity/displaySettingsOverride"
            };

            settings.SystemSettings = systemSettings;

            NetworkSettings networkSettings = new NetworkSettings
            {
                inPort = 9007,
                outPort = 9001,
                outIP = "127.0.0.1"
            };

            settings.NetworkSettings = networkSettings;

            DisplaySettings displaySettings = new DisplaySettings
            {
                Name = "IglooUnity",
                useCompositeTexture = true,
                useCubemapToEquirectangular = false,
                useFramepackTopBottom3D = false,
                textureShareMode = 1,
                horizontalFOV = 360f,
                verticalFOV = 180f,
                equirectangularTextureResolution = new Vector2IntItem(8000, 4000)
            };

            HeadSettings headSettings = new HeadSettings
            {
                headRotationOffset = new Vector3Item(Vector3.zero),
                headPositionOffset = new Vector3Item(0, 1.8f, 0),
                leftEyeOffset = new Vector3Item(-0.05f, 0, 0),
                rightEyeOffset = new Vector3Item(0.05f, 0, 0),
                headtracking = false,
                optitrackServerIP = "127.0.0.1",
                optitrackHeadRigidBodyID = 1
            };

            displaySettings.HeadSettings = headSettings;


            int numCams = 4;
            DisplayItem[] displayItems = new DisplayItem[numCams];

            for (int i = 0; i < numCams; i++)
            {
                DisplayItem displayItem = new DisplayItem
                {
                    Name = displaySettings.Name + (i + 1).ToString(),
                    fov = 90f,
                    cubemapFace = i,
                    is3D = false,
                    isFisheye = false,
                    isRendering = true,
                    isRenderTextures = true,
                    textureShareMode = 0,
                    nearClipPlane = 0.01f,
                    farClipPlane = 1000f
                };
                if (i < 4) displayItem.cameraRotation = new Vector3Item(0, (i * 90f), 0f);
                displayItem.renderTextureSize = new Vector2IntItem(2000, 2000);
                displayItem.fisheyeStrength = new Vector2Item(0, 0);
                displayItem.isOffAxis = false;
                displayItem.viewportRotation = new Vector3Item(Vector3.zero);
                displayItem.viewportPosition = new Vector3Item(Vector3.zero);
                displayItem.viewportSize = new Vector2Item(Vector2.zero);
                displayItem.targetDisplay = -1;

                displayItems[i] = displayItem;
            }

            displaySettings.Displays = displayItems;

            settings.DisplaySettings = displaySettings;

            PlayerSettings playerSettings = new PlayerSettings
            {
                Name = "player",
                usePlayer = true,
                rotationInput = 0,
                rotationMode = 0,
                movementInput = 0,
                movementMode = 2,
                runSpeed = 10,
                walkSpeed = 5,
                smoothTime = 10,
                isCrosshair3D = false,
                crosshairHideMode = 0,
                optitrackControllerRigidBodyID = 2
            };

            settings.PlayerSettings = playerSettings;


            UISettings uiSettings = new UISettings {
                useUI = true,
                screenName = "Cylinder",
                screenPos = new Vector3Item(Vector3.zero),
                screenRot = new Vector3Item(Vector3.zero),
                screenScale = new Vector3Item(6.0f, 2.1f, 6.0f),
                followCrosshair = true,
                followSpeed = 10,
                debugUIMode = false
            };

            settings.UISettings = uiSettings;

            TouchScreenSettings touchScreenSettings = new TouchScreenSettings {
                UseTouchScreen = false,
                XPositionStart = 1.0f,
                XPositionEnd = 0.0f,
                YPositionStart = 1.0f,
                YPositionEnd = 0.0f
            };

            settings.TouchScreenSettings = touchScreenSettings;

            return settings;
        }
    }

}
