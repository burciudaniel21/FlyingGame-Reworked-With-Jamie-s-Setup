using UnityEngine;

namespace Igloo.Common
{
    /// <summary>
    /// Igloo Virtual Display class. Overrides the Igloo Display Class
    /// </summary>
    public class VirtualDisplay : Display
    {
        /// <summary>
        /// The current Texture Share Mode
        /// </summary>
        public TextureShareUtility.TextureShareMode textureShareMode;

        /// <summary>
        /// The target physical display
        /// </summary>
        public int targetDisplay = -1;

        /// <summary>
        /// Writes the IglooSettings.xml display settings changed by this script.
        /// </summary>
        /// <param name="settings"></param>
        public override void SetSettings(DisplayItem settings)
        {
            base.SetSettings(settings);
            textureShareMode = (TextureShareUtility.TextureShareMode)settings.textureShareMode;
            targetDisplay = settings.targetDisplay;
        }

        /// <summary>
        /// Reads the current Display settings required for this script
        /// </summary>
        /// <returns>A populated DisplayItem settings class</returns>
        public override DisplayItem GetSettings()
        {
            DisplayItem settings = base.GetSettings();
            settings.textureShareMode = (int)textureShareMode;
            settings.targetDisplay = targetDisplay;
            return settings;
        }

        /// <summary>
        /// Creates the displays, and assigns them their texture share mode.
        /// </summary>
        public override void SetupDisplay()
        {
            base.SetupDisplay();
            foreach (var cam in activeCameras)
            {
                if (cam.Key == EYE.LEFT)
                {
                    // Sequencial naming
                    if (cam.Value.targetTexture != null) TextureShareUtility.AddTextureSender
                            (textureShareMode, this.gameObject, Name, ref leftTexture);
                }
                if (cam.Key == EYE.CENTER)
                {
                    if (cam.Value.targetTexture != null) TextureShareUtility.AddTextureSender
                            (textureShareMode, this.gameObject, Name, ref centerTexture);
                }
                if (cam.Key == EYE.RIGHT)
                {
                    // Sequencial naming
                    string nameTrimmed = Name.TrimEnd(Name[Name.Length - 1]);
                    char lastChar = Name[Name.Length - 1];
                    int camIndex = (int)char.GetNumericValue(lastChar);

                    camIndex += IglooManager.instance.settings.DisplaySettings.Displays.Length;

                    string newName = nameTrimmed + camIndex.ToString();
                    if (cam.Value.targetTexture != null) TextureShareUtility.AddTextureSender
                            (textureShareMode, this.gameObject, newName, ref rightTexture);
                }
            }
        }


        /// <summary>
        /// Creates the cameras and activates the displays
        /// </summary>
        public override void InitialiseCameras()
        {
            if (is3D)
            {
                Camera leftCam = headManager.CreateEye(EYE.LEFT, Name, isOffAxis ? this.transform.localEulerAngles : camRotation, cameraPrefab);
                Camera rightCam = headManager.CreateEye(EYE.RIGHT, Name, isOffAxis ? this.transform.localEulerAngles : camRotation, cameraPrefab, leftCam.transform.parent);
                if (targetDisplay >= 0)
                {
                    leftCam.targetDisplay = targetDisplay;
                    rightCam.targetDisplay = targetDisplay;
                    if (UnityEngine.Display.displays.Length > targetDisplay) UnityEngine.Display.displays[targetDisplay].Activate();
                }
                activeCameras.Add(EYE.LEFT, leftCam);
                activeCameras.Add(EYE.RIGHT, rightCam);
            }
            else
            {
                Camera centerCam = headManager.CreateEye(EYE.CENTER, "Camera - " + name, isOffAxis ? Vector3.zero : camRotation, cameraPrefab);
                activeCameras.Add(EYE.CENTER, centerCam);
                if (targetDisplay >= 0)
                {
                    centerCam.targetDisplay = targetDisplay;
                    if (UnityEngine.Display.displays.Length > targetDisplay) UnityEngine.Display.displays[targetDisplay].Activate();
                }
            }
        }
    }

}

