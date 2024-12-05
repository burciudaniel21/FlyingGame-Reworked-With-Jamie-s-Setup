using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Igloo.Common;
using System.Threading.Tasks;

namespace Igloo.UI {
#pragma warning disable IDE0051 // Remove unused private members
    public class SettingsManager : MonoBehaviour {

        [Header("PANEL LIST")]
        public List<GameObject> panels = new List<GameObject>();

        [Header("BUTTON LIST")]
        public List<GameObject> buttons = new List<GameObject>();

        [Header("MODAL WINDOWS")]
        public ModalWindowManager restartRequiredPopup;

        private string panelFadeIn = "Demo Panel In";
        private string panelFadeOut = "Demo Panel Out";

        private string buttonFadeIn = "Normal to Pressed";
        private string buttonFadeOut = "Pressed to Normal";

        private GameObject currentPanel;
        private GameObject nextPanel;

        private GameObject currentButton;
        private GameObject nextButton;

        private Animator currentPanelAnimator;
        private Animator nextPanelAnimator;

        private Animator currentButtonAnimator;
        private Animator nextButtonAnimator;

        [HideInInspector] public int currentPanelIndex = 0;
        [HideInInspector] public int currentButtonlIndex = 0;
        [HideInInspector] public float animValue = 1;
        [HideInInspector] public bool enableScrolling = false;
       
        bool applyToAllDisplays = false;
        bool hardResetRequired = false;
        bool softResetRequired = false;

        int activeDisplay;

        #region Settings Assets
        [Header("Network Assets")]
        [SerializeField] private TMP_InputField inputPortField;
        [SerializeField] private TMP_InputField outputPortField;
        [SerializeField] private TMP_InputField IPAddressField;

        [Header("Display Assets")]
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private HorizontalSelector TextureShareMode;
        // Spout / NDI
        [SerializeField] private Toggle CompositeTextureField;
        [SerializeField] private Toggle CubemapToEquirectangularField;
        [SerializeField] private TMP_InputField OutputResolutionFieldX, OutputResolutionFieldY;
        // DX Window
        [SerializeField] private Toggle UseWarpBlendField;
        [SerializeField] private TMP_InputField WarperPathField;
        [Header("3D System")]
        [SerializeField] private Toggle ThreeDModeField;
        // Eye offsets
        [SerializeField] private TMP_InputField leftEyeXField, leftEyeYField, leftEyeZField;
        [SerializeField] private TMP_InputField rightEyeXField, rightEyeYField, rightEyeZField;

        [Header("Player Assets")]
        [SerializeField] private Toggle usePlayerToggle;
        [SerializeField] private HorizontalSelector rotationInputField, movementInputField, movementModeField, rotationModeField, crosshairShowField;
        [SerializeField] private TMP_InputField walkSpeedField, runSpeedField;

        [Header("UI Settings Assets")]
        [SerializeField] private Toggle useUIToggle;
        [SerializeField] private HorizontalSelector uiScreenTypeSelector;
        [SerializeField] private TMP_InputField screenPositon_XField, screenPositon_YField, screenPositon_ZField;
        [SerializeField] private TMP_InputField screenRotation_XField, screenRotation_YField, screenRotation_ZField;
        [SerializeField] private TMP_InputField screenScale_XField, screenScale_YField, screenScale_ZField;
        [SerializeField] private Toggle followCrosshairToggle;
        [SerializeField] private TMP_InputField followCrosshairSpeedField;
        [SerializeField] private Toggle useTouchScreenSystemToggle;

        [Header("System Settings Assets")]
        [SerializeField] private HorizontalSelector vSyncModeSelector;
        [SerializeField] private TMP_InputField FPSInputField;
        [SerializeField] private Toggle useGlobalDisplaySettingsToggle;
        #endregion


        void Start() {
            this.GetComponent<Canvas>().enabled = false;
            SetCurrentSettings();
        }

        void Update() {
            // If Escape, and canvas is Enabled. Turn it off. 
            // IF Disabled, turn it on, and setup the animators.
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCanvas();
            }
        }

        public void ToggleCanvas()
        {
            if (this.GetComponent<Canvas>().enabled) {
                this.GetComponent<Canvas>().enabled = false;
                IglooManager.instance.UIManager.SetUIVisible(false);
            }
            else {
                this.GetComponent<Canvas>().enabled = true;
                IglooManager.instance.UIManager.SetUIVisible(true);
                currentButton = buttons[currentPanelIndex];
                currentButtonAnimator = currentButton.GetComponent<Animator>();
                currentButtonAnimator.Play(buttonFadeIn);

                currentPanel = panels[currentPanelIndex];
                currentPanelAnimator = currentPanel.GetComponent<Animator>();
                currentPanelAnimator.Play(panelFadeIn);
            }
        }

        public void PanelAnim(int newPanel) {
            AsyncPanelAnim(newPanel);
        }

        private async void AsyncPanelAnim(int newPanel) {
            if (newPanel != currentPanelIndex) {
                currentPanel = panels[currentPanelIndex];

                currentPanelIndex = newPanel;
                nextPanel = panels[currentPanelIndex];

                currentPanelAnimator = currentPanel.GetComponent<Animator>();
                nextPanelAnimator = nextPanel.GetComponent<Animator>();

                currentPanelAnimator.Play(panelFadeOut);
                await Task.Delay(400);
                currentPanelAnimator.gameObject.SetActive(false);
                nextPanelAnimator.gameObject.SetActive(true);
                nextPanelAnimator.Play(panelFadeIn);

                currentButton = buttons[currentButtonlIndex];

                currentButtonlIndex = newPanel;
                nextButton = buttons[currentButtonlIndex];

                currentButtonAnimator = currentButton.GetComponent<Animator>();
                nextButtonAnimator = nextButton.GetComponent<Animator>();

                currentButtonAnimator.Play(buttonFadeOut);

                if (!nextButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed"))
                    nextButtonAnimator.Play(buttonFadeIn);

            }
        }

        #region Network Settings
        public void SetInPort(string Port) { 
            IglooManager.instance.settings.NetworkSettings.inPort = int.Parse(Port);
            #if !UNITY_WEBGL && EXT_OSC
            IglooManager.instance.NetworkManager.oscIn.LocalPort = int.Parse(Port);
#endif
        }

        public void SetOutPort(string Port) { 
            IglooManager.instance.settings.NetworkSettings.outPort = int.Parse(Port);
#if !UNITY_WEBGL && EXT_OSC
            IglooManager.instance.NetworkManager.oscOut.RemotePort = int.Parse(Port);
#endif
        }

        public void SetIP(string IP) { 
            IglooManager.instance.settings.NetworkSettings.outIP = IP;
#if !UNITY_WEBGL && EXT_OSC
            IglooManager.instance.NetworkManager.oscOut.RemoteHost = IP;
#endif
        }
        #endregion

        #region Display Settings 

        public void SetOutputName(string name) { 
            IglooManager.instance.settings.DisplaySettings.Name = name;
            // Does not require soft reset. 

            IglooManager.instance.DisplayManager.sharingName = name;
#if KLAK_SPOUT
            if (IglooManager.instance.DisplayManager.textureShareMode == TextureShareUtility.TextureShareMode.SPOUT)
            {
                foreach(Klak.Spout.SpoutSender sender in FindObjectsOfType<Klak.Spout.SpoutSender>())
                {
                    sender.spoutName = name; 
                }
            }
#endif
#if KLAK_NDI
            if (IglooManager.instance.DisplayManager.textureShareMode == TextureShareUtility.TextureShareMode.NDI)
            {
                foreach (Klak.Ndi.NdiSender sender in FindObjectsOfType<Klak.Ndi.NdiSender>())
                {
                    sender.ndiName = name;
                }
            }
#endif
        }

        public void SetCubemapToEquirect(bool isActive) { 
            IglooManager.instance.settings.DisplaySettings.useCubemapToEquirectangular = isActive;
            softResetRequired = true;
        }

        public void SetUseCompTexture(bool isActive) { 
            IglooManager.instance.settings.DisplaySettings.useCompositeTexture = isActive;
            softResetRequired = true;
        }

        public void SetUseWarpBlend(bool isActive) {
            IglooManager.instance.settings.DisplaySettings.useWarpBlend = isActive;
            softResetRequired = true;
        }

        public void SetWarperPath(string path) {
            IglooManager.instance.settings.DisplaySettings.WarpBlendSettings.warperDataPath = path;
            softResetRequired = true;
        }

        public void SetTextureShareMode(int mode) { IglooManager.instance.settings.DisplaySettings.textureShareMode = mode;
            softResetRequired = true;
        }

        public void SetUseFramepackTopBottom3D(bool isActive) { 
            IglooManager.instance.settings.DisplaySettings.useFramepackTopBottom3D = isActive;
            softResetRequired = true;
        }

        public void SetEquirectTextureResX(string xRes)
        {
            IglooManager.instance.settings.DisplaySettings.equirectangularTextureResolution = new Vector2IntItem(
               int.Parse(xRes), IglooManager.instance.settings.DisplaySettings.equirectangularTextureResolution.Vector2Int.y);
            softResetRequired = true;
        }

        public void SetEquirectTextureResy(string yRes)
        {
            IglooManager.instance.settings.DisplaySettings.equirectangularTextureResolution = new Vector2IntItem(
                IglooManager.instance.settings.DisplaySettings.equirectangularTextureResolution.Vector2Int.x, int.Parse(yRes));
            softResetRequired = true;
        }

        public void Set3D(bool is3D)
        {
           foreach (var item in IglooManager.instance.settings.DisplaySettings.Displays) item.is3D = IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].is3D;
            softResetRequired = true;
        }

        public void SetRenderTextureSize(Vector2 texSize)
        {
            IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].renderTextureSize = new Vector2IntItem((int)texSize.x, (int)texSize.y);
            if (applyToAllDisplays)
            {
                foreach (var item in IglooManager.instance.settings.DisplaySettings.Displays) item.renderTextureSize = IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].renderTextureSize;
            }
        }

#region OffAxis Projection
        public void SetIsOffAxisProjection(bool isActive) { IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].isOffAxis = isActive; }
        public void SetOffAxisViewportPosition(Vector3 pos) { IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].viewportPosition = new Vector3Item(pos); }
        public void SetOffAxisViewportRotation(Vector3 rot) { IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].viewportRotation = new Vector3Item(rot); }
        public void SetOffAxisViewportSize(Vector2 viewSize) { IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].viewportSize = new Vector2Item(viewSize); }
        public void SetDisplayCameraRotation(Vector3 rot) { IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].cameraRotation = new Vector3Item(rot); }
        public void SetDisplayTargetDisplay(int displayID) { IglooManager.instance.settings.DisplaySettings.Displays[activeDisplay].targetDisplay = displayID; }
        public void SetNextActiveDisplay() { activeDisplay++; activeDisplay = activeDisplay % IglooManager.instance.settings.DisplaySettings.Displays.Length; }
        public void SetPreviousActiveDisplay() { activeDisplay--; if (activeDisplay < 0) activeDisplay = IglooManager.instance.settings.DisplaySettings.Displays.Length - 1; }
#endregion

#region 3D Settings
        public void SetRightEyeOffsetX(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.DisplaySettings.HeadSettings.rightEyeOffset.x = f;
            Vector3 offset = IglooManager.instance.DisplayManager.headManager.RightEyeOffset;
            offset.x = f;
            IglooManager.instance.DisplayManager.headManager.RightEyeOffset = offset;
        }
        public void SetRightEyeOffsetY(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.DisplaySettings.HeadSettings.rightEyeOffset.y = f;
            Vector3 offset = IglooManager.instance.DisplayManager.headManager.RightEyeOffset;
            offset.y = f;
            IglooManager.instance.DisplayManager.headManager.RightEyeOffset = offset;
        }
        public void SetRightEyeOffsetZ(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.DisplaySettings.HeadSettings.rightEyeOffset.z = f;
            Vector3 offset = IglooManager.instance.DisplayManager.headManager.RightEyeOffset;
            offset.z = f;
            IglooManager.instance.DisplayManager.headManager.RightEyeOffset = offset;
        }

        public void SetLeftEyeOffsetX(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.DisplaySettings.HeadSettings.leftEyeOffset.x = f;
            Vector3 offset = IglooManager.instance.DisplayManager.headManager.LeftEyeOffset;
            offset.x = f;
            IglooManager.instance.DisplayManager.headManager.LeftEyeOffset = offset;
        }
        public void SetLeftEyeOffsetY(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.DisplaySettings.HeadSettings.leftEyeOffset.y = f;
            Vector3 offset = IglooManager.instance.DisplayManager.headManager.LeftEyeOffset;
            offset.y = f;
            IglooManager.instance.DisplayManager.headManager.LeftEyeOffset = offset;
        }
        public void SetLeftEyeOffsetZ(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.DisplaySettings.HeadSettings.leftEyeOffset.z = f;
            Vector3 offset = IglooManager.instance.DisplayManager.headManager.LeftEyeOffset;
            offset.z = f;
            IglooManager.instance.DisplayManager.headManager.LeftEyeOffset = offset;
        }
#endregion

#endregion

#region Player Settings

        public void SetUsePlayer(bool playerActive) { 
            IglooManager.instance.settings.PlayerSettings.usePlayer = playerActive;
            softResetRequired = true;
        }
        public void SetRotationInput(int rot) { 
            IglooManager.instance.settings.PlayerSettings.rotationInput = rot;
            IglooManager.instance.PlayerManager.rotationInput = (PlayerManager.ROTATION_INPUT)rot;
        }
        public void SetRotationMode(int mode) {
            IglooManager.instance.settings.PlayerSettings.rotationMode = mode;
            IglooManager.instance.PlayerManager.rotationMode = (PlayerManager.ROTATION_MODE)mode;
        }
        public void SetMovementMode(int mode) { 
            IglooManager.instance.settings.PlayerSettings.movementMode = mode;
            IglooManager.instance.PlayerManager.movementMode = (PlayerManager.MOVEMENT_MODE)mode;
        }
        public void SetMovementInput(int input) { 
            IglooManager.instance.settings.PlayerSettings.movementInput = input;
            IglooManager.instance.PlayerManager.movementInput = (PlayerManager.MOVEMENT_INPUT)input;
        }
        public void SetWalkSpeed(string speed) {
            int i = int.Parse(speed);
            IglooManager.instance.settings.PlayerSettings.walkSpeed = i;
            IglooManager.instance.PlayerManager.m_WalkSpeed = i;

        }
        public void SetRunSpeed(string speed) {
            int i = int.Parse(speed);
            IglooManager.instance.settings.PlayerSettings.runSpeed = i;
            IglooManager.instance.PlayerManager.m_RunSpeed = i;

        }
        public void SetSmoothtime(string smooth) {
            int i = int.Parse(smooth);
            IglooManager.instance.settings.PlayerSettings.smoothTime = i;
            IglooManager.instance.PlayerManager.SmoothTime = i;

        }
        public void SetCrosshairMode(int mode) {
            IglooManager.instance.settings.PlayerSettings.crosshairHideMode = mode;
            IglooManager.instance.PlayerManager.SetCrosshairMode((Controllers.Crosshair.CROSSHAIR_MODE)mode);

        }

#endregion

#region UI Settings

        public void SetUseUI(bool UIActive) {
            IglooManager.instance.settings.UISettings.useUI = UIActive;
            softResetRequired = true;
        }
        public void SetScreenName(string newName) { 
            IglooManager.instance.settings.UISettings.screenName = newName;
            IglooManager.instance.UIManager.SetScreen(newName);
        }
        public void SetScreenPositionX(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenPos.x = f;
            Vector3 localPos = IglooManager.instance.UIManager.activeScreen.transform.localPosition;
            localPos.x = f;
            IglooManager.instance.UIManager.activeScreen.transform.localPosition = localPos;
        }
        public void SetScreenPositionY(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenPos.y = f;
            Vector3 localPos = IglooManager.instance.UIManager.activeScreen.transform.localPosition;
            localPos.y = f;
            IglooManager.instance.UIManager.activeScreen.transform.localPosition = localPos;
        }
        public void SetScreenPositionZ(string val) { 
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenPos.z = f;
            Vector3 localPos = IglooManager.instance.UIManager.activeScreen.transform.localPosition;
            localPos.z = f;
            IglooManager.instance.UIManager.activeScreen.transform.localPosition = localPos;
        }
        public void SetScreenRotationX(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenRot.x = f;
            Vector3 localRot = IglooManager.instance.UIManager.activeScreen.transform.localEulerAngles;
            localRot.x = f;
            IglooManager.instance.UIManager.activeScreen.transform.localEulerAngles = localRot;

        }
        public void SetScreenRotationY(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenRot.y = f;
            Vector3 localRot = IglooManager.instance.UIManager.activeScreen.transform.localEulerAngles;
            localRot.z = f;
            IglooManager.instance.UIManager.activeScreen.transform.localEulerAngles = localRot;

        }
        public void SetScreenRotationZ(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenRot.z = f;
            Vector3 localRot = IglooManager.instance.UIManager.activeScreen.transform.localEulerAngles;
            localRot.z = f;
            IglooManager.instance.UIManager.activeScreen.transform.localEulerAngles = localRot;

        }
        public void SetScreenScaleX(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenScale.x = f;
            Vector3 localScale = IglooManager.instance.UIManager.activeScreen.transform.localScale;
            localScale.x = f;
            IglooManager.instance.UIManager.activeScreen.transform.localScale = localScale;

        }
        public void SetScreenScaleY(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenScale.y = f;
            Vector3 localScale = IglooManager.instance.UIManager.activeScreen.transform.localScale;
            localScale.y = f;
            IglooManager.instance.UIManager.activeScreen.transform.localScale = localScale;
        }
        public void SetScreenScaleZ(string val) {
            float f = float.Parse(val);
            IglooManager.instance.settings.UISettings.screenScale.z = f;
            Vector3 localScale = IglooManager.instance.UIManager.activeScreen.transform.localScale;
            localScale.z = f;
            IglooManager.instance.UIManager.activeScreen.transform.localScale = localScale;
        }
        public void SetFollowCrosshair(bool isActive) { 
            IglooManager.instance.settings.UISettings.followCrosshair = isActive;
            IglooManager.instance.UIManager.SetFollowCursor(isActive);
        }
        public void SetFollowSpeed(string speed) {
            int i = int.Parse(speed);
            IglooManager.instance.settings.UISettings.followSpeed = i;
            IglooManager.instance.UIManager.SetFollowSpeed(i);
        }
        public void SetUseTouchScreen(bool isActive) { /* TODO IglooManager.instance.settings.UISettings = UIActive; */}


#endregion

#region SystemSettings

        public void SetVSyncMode(int mode) { IglooManager.instance.settings.SystemSettings.vSyncMode = mode; }
        public void SetTargetFPS(string fps) { IglooManager.instance.settings.SystemSettings.targetFPS = int.Parse(fps); }
        public void SetUseGlobalDisplaySettings(bool isActive) { IglooManager.instance.settings.SystemSettings.useDisplaySettingsOverride = isActive; }
#endregion

#region Save Load

        /// <summary>
        /// Saves the current game settings to XML via the Save button.
        /// Also tells the Igloo manager if it needs a hard reset or a soft reset.
        /// </summary>
        public void SaveCurrentSettings()
        {
            // Save the settings
            IglooManager.instance.GetAndSaveSettings();
            // If a game restart flag is called, restart the scene.
            if (hardResetRequired)
            {
                restartRequiredPopup.OpenWindow();
                hardResetRequired = false;
            }
            // if a soft restart is required, restart only the igloo camera system.
            else if (softResetRequired)
            {
                IglooManager.instance.SoftReset();
                softResetRequired = false;
            }
        }

        /// <summary>
        /// Called from the 'accept' button on the DisplayRestartRequiredPopup
        /// Calls function on Igloo Manager to Hard Reset the game.
        /// </summary>
        public void HardResetConfirmed()
        {
            IglooManager.instance.HardReset();
            hardResetRequired = false;
        }
        
#endregion

        private async void SetCurrentSettings() 
        {
            await Task.Delay(300);
#region Network
            inputPortField.SetTextWithoutNotify(IglooManager.instance.settings.NetworkSettings.inPort.ToString());
            outputPortField.SetTextWithoutNotify(IglooManager.instance.settings.NetworkSettings.outPort.ToString());
            IPAddressField.SetTextWithoutNotify(IglooManager.instance.settings.NetworkSettings.outIP);
#endregion

#region Display
            // Stock Settings
            nameField.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.Name);
            TextureShareMode.SetValue(IglooManager.instance.settings.DisplaySettings.textureShareMode);

            // Spout / NDI settings
            CompositeTextureField.SetIsOnWithoutNotify(IglooManager.instance.settings.DisplaySettings.useCompositeTexture);
            CubemapToEquirectangularField.SetIsOnWithoutNotify(IglooManager.instance.settings.DisplaySettings.useCubemapToEquirectangular);
            OutputResolutionFieldX.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.equirectangularTextureResolution.x.ToString());
            OutputResolutionFieldY.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.equirectangularTextureResolution.y.ToString());

            // DX Window Settings
            UseWarpBlendField.SetIsOnWithoutNotify(IglooManager.instance.settings.DisplaySettings.useWarpBlend);
            WarperPathField.SetTextWithoutNotify(
                IglooManager.instance.settings.DisplaySettings.WarpBlendSettings != null 
                ? IglooManager.instance.settings.DisplaySettings.WarpBlendSettings.warperDataPath 
                : "C:/ProgramData/Igloo Vision/IglooWarper"
            );


            // 3D Settings
            ThreeDModeField.isOn = IglooManager.instance.settings.DisplaySettings.Displays[0].is3D;

            leftEyeXField.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.HeadSettings.leftEyeOffset.x.ToString());
            leftEyeYField.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.HeadSettings.leftEyeOffset.y.ToString());
            leftEyeZField.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.HeadSettings.leftEyeOffset.z.ToString());


            rightEyeXField.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.HeadSettings.rightEyeOffset.x.ToString());
            rightEyeYField.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.HeadSettings.rightEyeOffset.y.ToString());
            rightEyeZField.SetTextWithoutNotify(IglooManager.instance.settings.DisplaySettings.HeadSettings.rightEyeOffset.z.ToString());

#endregion

#region Player
            usePlayerToggle.isOn = IglooManager.instance.settings.PlayerSettings.usePlayer;
            walkSpeedField.SetTextWithoutNotify(IglooManager.instance.settings.PlayerSettings.walkSpeed.ToString());
            runSpeedField.SetTextWithoutNotify(IglooManager.instance.settings.PlayerSettings.runSpeed.ToString());
            crosshairShowField.defaultIndex = (int)IglooManager.instance.PlayerManager.crosshair.crosshairMode;
            rotationInputField.defaultIndex = (int)IglooManager.instance.PlayerManager.rotationInput;
            rotationModeField.defaultIndex = (int)IglooManager.instance.PlayerManager.rotationMode;
            movementInputField.defaultIndex = (int)IglooManager.instance.PlayerManager.movementInput;
            movementModeField.defaultIndex = (int)IglooManager.instance.PlayerManager.movementMode;
#endregion

#region UI Settings
            useUIToggle.isOn = IglooManager.instance.settings.UISettings.useUI;
            uiScreenTypeSelector.defaultIndex = IglooManager.instance.settings.UISettings.screenName == "Cylinder" ? 0 : 1;
            followCrosshairToggle.SetIsOnWithoutNotify(IglooManager.instance.settings.UISettings.followCrosshair);
            followCrosshairSpeedField.text = IglooManager.instance.settings.UISettings.followSpeed.ToString();
            // useTouchScreenSystemToggle.SetIsOnWithoutNotify() = // TODO
            Vector3 screenPos = IglooManager.instance.settings.UISettings.screenPos.Vector3;
            Vector3 screenRot = IglooManager.instance.settings.UISettings.screenRot.Vector3;
            Vector3 screenSca = IglooManager.instance.settings.UISettings.screenScale.Vector3;

            screenPositon_XField.text = screenPos.x.ToString();
            screenPositon_YField.text = screenPos.y.ToString();
            screenPositon_ZField.text = screenPos.z.ToString();

            screenRotation_XField.text = screenRot.x.ToString();
            screenRotation_YField.text = screenRot.y.ToString();
            screenRotation_ZField.text = screenRot.z.ToString();

            screenScale_XField.text = screenSca.x.ToString();
            screenScale_YField.text = screenSca.y.ToString();
            screenScale_ZField.text = screenSca.z.ToString();
#endregion

#region System
            FPSInputField.text = IglooManager.instance.settings.SystemSettings.targetFPS.ToString();
            vSyncModeSelector.defaultIndex = IglooManager.instance.settings.SystemSettings.vSyncMode;
            useGlobalDisplaySettingsToggle.SetIsOnWithoutNotify(IglooManager.instance.settings.SystemSettings.useDisplaySettingsOverride);
#endregion

            hardResetRequired = false;
            softResetRequired = false;

        }

    }

}

