using System.Collections;
using System.IO;
using UnityEngine;
using Igloo.UI;
using UnityEngine.SceneManagement;
#if UNITY_STANDALONE_WIN && KLAK_SPOUT
using Klak.Spout;
#endif
#if KLAK_NDI
using Klak.Ndi;
#endif

namespace Igloo.Common
{
#pragma warning disable IDE0090 // Use New()...
#pragma warning disable IDE0051 // Remove unused private members

    /// <summary>
    /// The Igloo Manager Class
    /// </summary>
    /// <remarks>
    /// This class controls, sets up, and handles, most of the Igloo camera systems large functions.
    /// It also serves as a reference for the sub classes within the Igloo camera system hierarchy. 
    /// </remarks>
    [RequireComponent(typeof(NetworkManager))]
    [System.Serializable]
    public class IglooManager : Singleton<IglooManager>
    {
        /// <summary>
        /// The XML Serializer
        /// </summary>
        /// <remarks>
        /// Turns the XML script into a machine readable format.
        /// </remarks>
        private Serializer serializer;

        /// <summary>
        /// The igloo settings class reference.
        /// </summary>
        [HideInInspector] public Settings settings = null;

        /// <summary>
        /// Creates an Igloo Camera rig, when the IglooManager is established as a Singleton
        /// </summary>
        public bool createOnAwake = true;

        /// <summary>
        /// When the Quit function is called, should the IglooManager attempt to save it's settings
        /// </summary>
        /// <remarks>
        /// If the application is closed by force, i.e. TaskManager quit. This may cause the settings file to
        /// become corrupt.
        /// </remarks>
        public bool saveOnQuit = true;

        /// <summary>
        /// If true, the IglooManager will not be deleted when the scene is changed.
        /// </summary>
        public bool dontDestroyOnLoad = false;

        /// <summary>
        /// If true, sends an OSC message to the Igloo warper, to inform it that this application has started.
        /// </summary>
        private bool sendStartupMessage = false;

        /// <summary>
        /// The Igloo Game Object. This is the parent object of the Igloo Camera System
        /// </summary>
        /// <remarks>
        /// If an igloo camera system object is placed here, one will not be created by the IglooManager.
        /// </remarks>
        public GameObject igloo;

        /// <summary>
        /// The Camera Prefab GameObject.
        /// </summary>
        /// <remarks>
        /// If a camera prefab is placed here, the IglooManager will instruct the IglooDisplayManager to use 
        /// that camera as the base camera for it's camera system construction. This is useful for adding 
        /// post processing effects to the igloo camera system.
        /// </remarks>
        public GameObject cameraPrefab;

        /// <summary>
        /// The Object for the Igloo Camera System to follow.
        /// </summary>
        /// <remarks>
        /// If an object is placed here, the Igloo Player System will be disabled.
        /// Instead the Igloo Follow Object system will be enabled, and will follow the placed object.
        /// </remarks>
        public GameObject followObject;

        /// <summary>
        /// The parent object of the Igloo Manager.
        /// </summary>
        [HideInInspector] public GameObject parentObject;

        /// <summary>
        /// The Igloo Network Manager class reference
        /// </summary>
        public NetworkManagerOSC NetworkManager { get; set; }

        /// <summary>
        /// The Igloo Player Manager class reference 
        /// </summary>
        public PlayerManager PlayerManager { get; set; }

        /// <summary>
        /// The Igloo Display Manager class reference
        /// </summary>
        public DisplayManager DisplayManager { get; set; }

        /// <summary>
        /// The Igloo UI Manager class reference
        /// </summary>
        public UIManager UIManager { get; set; }

        /// <summary>
        /// The Igloo UI Window class reference
        /// </summary>
        public WindowManager WindowManager { get; set; }

        /// <summary>
        /// External window settings class reference
        /// </summary>
        public UWPExternalWindowSettings UwpSettings { get; set; }
        /// <summary>
        /// The UI Canvas to be used by the Igloo UI System
        /// </summary>
        [Tooltip("The Unity Canvas that should be displayed on the Igloo UI system")]
        public Canvas canvasUI;

        /// <summary>
        /// The UI cursor to be used by the Igloo UI System
        /// </summary>
        private RectTransform cursorUI;

        /// <summary>
        /// The render texture that is used to pass the UI Canvas to the Igloo UI system.
        /// </summary>
        [Tooltip("The Render Texture used for the UI Canvas System")]
        public RenderTexture textureUI;

        /// <summary>
        /// Set within the Settings XML. Set's the VSync mode.
        /// </summary>
        /// <example> 
        /// 0 - No Vsync. Outputs frames as fast as possible.
        /// 1 - Will output one frame update for every screen update.
        /// 2 - Will output one frame update for every other screen update.
        /// </example>
        private int vSyncMode;

        /// <summary>
        /// Set from Settings XML. Sets the target FPS.
        /// </summary>
        /// <example>
        /// 30 - Unity will limit the app to 30 frames per second
        /// </example>
        private int targetFPS;

#if KLAK_SPOUT
        /// <summary>
        /// Allows the Spout Sender component to find and use the Spout blit material and shader
        /// which are not included in the build unless this field is allocated. 
        /// If you do not get a spout output after compiling a build, this will likely be the reason. 
        /// The Log will also be filled with null reference errors.
        /// </summary>
        [Tooltip("This is a required file for the Igloo Manager system to send a stream to the Igloo IMP Software. It's located in the Klak Spout package. Click the Add Spout Resources button to locate it")]
        public SpoutResources spoutResources;
#endif

#if KLAK_NDI
        /// <summary>
        /// Allows the NDI Sender component to find and use the NDI blit material and shader
        /// which are not included in the build unless this field is allocated. 
        /// If you do not get a spout output after compiling a build, this will likely be the reason. 
        /// The Log will also be filled with null reference errors.
        /// </summary>
        [Tooltip("This is a required file for the Igloo Manager system to send a stream to the Igloo IMP Software. It's located in the Klak NDI package. Click the Add NDI Resources button to locate it")]
        public NdiResources ndiResources;
#endif


        /// <summary>
        /// Called when the gameobject is initialized as a Singleton
        /// </summary>
        /// <remarks>
        /// Pulls in all the settings from the settings XML, or creates a default set of settings if one cannot 
        /// be found. Also makes the gameobject invenerable, by calling DontDestroyOnLoad.
        /// </remarks>
        protected override void AwakeInternal()
        {
            // Set Igloo Manager to be persistant during scene changes.
            if (Application.isPlaying && dontDestroyOnLoad) DontDestroyOnLoad(this.gameObject);

#if !KLAK_SPOUT
            Debug.LogError("<b>[Igloo]</b> Spout is disabled Apply Project Settings has not been selected. Press Stop, then Click Igloo->Apply Project Settings");
#endif

            LoadSettings(string.Empty);

            // Setup the Network Manager
            NetworkManager = GetComponent<NetworkManagerOSC>();
            if (settings != null) NetworkManager.SetSettings(settings.NetworkSettings);
            NetworkManager.Setup();

            // Initiate the Create Igloo Function
            if (createOnAwake) CreateIgloo();
        }

        /// <summary>
        /// Creates a new Igloo based on the Igloo Settings XML
        /// </summary>
        /// <param name="parent">optional parameter which allows to the Igloo prefab instantiated by CreateIgloo() 
        /// to be parented to the given object. If unassigned the prefab will be parented to the 
        /// IglooManager GameObject. </param>
        /// <param name="follow">optional parameter which assigns the followTransform property</param>
        public void CreateIgloo(GameObject parent = null, GameObject follow = null)
        {

            if (follow) followObject = follow;
            if (parent) parentObject = parent;

            // instantite igloo prefab if not already assigned 
            if (!igloo)
            {
                igloo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Igloo"));
                igloo.transform.parent = parent == null ? this.gameObject.transform : parent.transform;
                igloo.transform.localEulerAngles = Vector3.zero;
                igloo.transform.localPosition = Vector3.zero;
            }

            DisplayManager = igloo.GetComponent<DisplayManager>();
            PlayerManager = igloo.GetComponent<PlayerManager>();
            UIManager = igloo.GetComponentInChildren<UIManager>();

            Vector3 tempPos = igloo.transform.localPosition;
            Vector3 tempRot = igloo.transform.localEulerAngles;
            igloo.transform.localEulerAngles = tempRot;
            igloo.transform.localPosition = tempPos;
            if (PlayerManager) PlayerManager.GetComponent<CharacterController>().enabled = false;

            // display setup
            if (settings.DisplaySettings != null)
            {
                if (cameraPrefab) DisplayManager.cameraPrefab = cameraPrefab;
                else DisplayManager.cameraPrefab = null;
                DisplayManager.Setup(settings.DisplaySettings);
            }
            else Debug.LogWarning("<b>[Igloo]</b> no Igloo Display settings found");

            // player setup
            if (settings.PlayerSettings != null && PlayerManager) PlayerManager.SetSettings(settings.PlayerSettings);
            else Debug.LogWarning("<b>[Igloo]</b> no igloo Player Settings found");

            // follow object
            if (followObject)
            {
                igloo.GetComponent<FollowObjectTransform>().enabled = true;
                igloo.GetComponent<FollowObjectTransform>().followObject = followObject;
                if (PlayerManager) PlayerManager.UsePlayer = false;
                Debug.Log("Igloo - follow trasform assigned, igloo player system disabled");
            }


            SetupUISystem();

            if (!GetComponent<WindowManager>()) WindowManager = this.gameObject.AddComponent<WindowManager>();
            if (settings.WindowSettings != null)
            {
                WindowManager.SetSettings(settings.WindowSettings);
                WindowManager.SetupWindows();
            }

            // system settings
            if (settings.SystemSettings != null) SetSystemSettings(settings.SystemSettings);

            // UWP
            if (settings.DisplaySettings.UWPExternalWindowSettings != null) UwpSettings = settings.DisplaySettings.UWPExternalWindowSettings;
        }


        /// <summary>
        /// Pulled the UI setup out of the start sequence so it can be re-called from other scripts
        /// </summary>
        public void SetupUISystem() {
            // ui setup
            UIManager.SetSettings(settings.UISettings);

            if (canvasUI && cursorUI && textureUI && settings.UISettings.useUI) {
                UIManager.canvas = canvasUI;
                UIManager.cursor = cursorUI;
                UIManager.texture = textureUI;
                canvasUI.renderMode = RenderMode.ScreenSpaceCamera;
                StartCoroutine(DelayedScreenModeSwitch(true));
                UIManager.Setup();
            } else if (canvasUI && cursorUI && textureUI && !settings.UISettings.useUI) {
                // Not using UI, but UI objects are assigned, so switch UI to standard mode.
                canvasUI.renderMode = RenderMode.ScreenSpaceOverlay;
                StartCoroutine(DelayedScreenModeSwitch(false));
            }
        }

        /// <summary>
        /// Waits for one second before calling the Igloo Canvas Controller, as it's not yet created in the main loop.
        /// Stops a null reference error on startup. 
        /// </summary>
        /// <param name="IglooModeOn">If True, calls SwitchToIglooMode on the Canvas Controller</param>
        /// <returns>Null</returns>
        private IEnumerator DelayedScreenModeSwitch(bool IglooModeOn)
        {
            yield return new WaitForSeconds(1.0f);
            if (IglooCanvasController.instance != null)
            {
                if (IglooModeOn)
                {
                    IglooCanvasController.instance.SwitchToIglooMode();
                    canvasUI.worldCamera = IglooCanvasController.instance.vim.m_canvasCamera;
                }
                else IglooCanvasController.instance.SwitchToScreenMode();
            }

        }

        /// <summary>
        /// Removes the Igloo prefab from the scene if it exists
        /// </summary>
        /// <param name="save"></param>
        public void RemoveIgloo(bool save = true)
        {
            if (igloo)
            {
                if (save) GetAndSaveSettings();
                DestroyImmediate(igloo);
            }
        }

        /// <summary>
        /// Saves settings to the IglooSettings.xml file
        /// These are the current active settings in the scene, not any changed by the settings
        /// editor UI.
        /// </summary>
        public void GetAndSaveSettings()
        {
            if (settings.SystemSettings.useDisplaySettingsOverride) return;
            var newSettings = new Settings { version = Utils.GetVersion() };
            newSettings.DisplaySettings = DisplayManager.GetSettings();
            newSettings.SystemSettings = GetSystemSettings();
            if (UwpSettings != null) newSettings.DisplaySettings.UWPExternalWindowSettings = UwpSettings;
            if (PlayerManager) newSettings.PlayerSettings = PlayerManager.GetSettings();
            if (UIManager) newSettings.UISettings = UIManager.GetSettings();
            if (NetworkManager) newSettings.NetworkSettings = NetworkManager.GetSettings();
            if (WindowManager) {
                WindowSettings wSettings = WindowManager.GetSettings();
                if (wSettings != null) newSettings.WindowSettings = wSettings;
            }
            if (serializer == null) serializer = new Serializer();
            string path = System.IO.Path.Combine(Utils.GetDataPath() + "/", "IglooSettings");
            if (!Directory.Exists(Utils.GetDataPath())) Directory.CreateDirectory(Utils.GetDataPath());
            serializer.Save(path, newSettings);
        }


        /// <summary>
        /// Loads all the XML settings for the creation of the Igloo Camera System. 
        /// if no string argument for <para name="settingsPath"></para> it will load the settings from
        /// the default path of StreamingAssets.
        /// </summary>
        public void LoadSettings(string settingsPath)
        {
            // Pull the Settings from the Settings XML
            if (serializer == null) serializer = new Serializer();
            settings = null;
            string path;
            // if there is a null string in settingsPath, use the standard location of StreamingAssets
            if (settingsPath == string.Empty)
                path = System.IO.Path.Combine(Utils.GetDataPath() + "/", "IglooSettings");
            else path = settingsPath;
            // Load the settings
            settings = serializer.Load(path);

            if (settings.SystemSettings.useDisplaySettingsOverride) {
                string displaySettingsPath = Path.Combine(settings.SystemSettings.displaySettingsOverridePath + "/", "IglooSettings");
                var displaySettings = serializer.Load(displaySettingsPath);
                if(displaySettings != null) {
                    Debug.Log("<b>[Igloo]</b> Global Settings XML loaded successfully");
                    settings.DisplaySettings = displaySettings.DisplaySettings;
                    settings.WindowSettings = displaySettings.WindowSettings;
                    settings.TouchScreenSettings = displaySettings.TouchScreenSettings;
                    settings.SystemSettings.targetFPS = displaySettings.SystemSettings.targetFPS;
                    settings.SystemSettings.vSyncMode = displaySettings.SystemSettings.vSyncMode;
                }
                else Debug.LogWarning(" <b>[Igloo]</b> unable to load Global Settings XML, reverting to Local");
            }

            // if settings cannot be loaded , setup using the default cylinder configuration
            if (settings == null)
            {
                Debug.LogWarning(" <b>[Igloo]</b> unable to load Settings XML, using default instead");
                settings = DefaultConfigurations.EquirectangularCylinder();
            }
            else Debug.Log("<b>[Igloo]</b> Settings XML loaded successfully");
        }

        /// <summary>
        /// Using the Settings pulled from the IglooSettings xml, to set the system settings.
        /// </summary>
        /// <param name="settings"></param>
        private void SetSystemSettings(SystemSettings settings)
        {
            vSyncMode = settings.vSyncMode;
            targetFPS = settings.targetFPS;
            Application.targetFrameRate = targetFPS;
            if (vSyncMode >= 0 && vSyncMode <= 2) QualitySettings.vSyncCount = vSyncMode;
            sendStartupMessage = settings.sendStartupMessage;
            if (sendStartupMessage) StartCoroutine(SendStartupMessage());
            Debug.Log("<b>[Igloo]</b> System settings loaded, vsync mode: " + vSyncMode + " ,target FPS: " + targetFPS);
        }

        /// <summary>
        /// If a setting has been changed that requires a soft reset.
        /// This function will destroy the current Igloo Camera System. 
        /// Load the new settings XML, and then restart the igloo camera system with the new settings.
        /// </summary>
        public void SoftReset()
        {
            RemoveIgloo(true);
            // Awake internal is currently called, as it will load all settings, then create the Igloo.
            AwakeInternal();
        }

        /// <summary>
        /// Hard reset system, reloads the current scene.
        /// May cause issues if there is a scene system that contains multiple scenes loaded at runtime. 
        /// A better alternative might require quitting the application and reloading it. 
        /// </summary>
        public void HardReset()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// A delayed Coroutine to send a startup message to the Igloo Warper System.
        /// </summary>
        /// <returns>Null</returns>
        IEnumerator SendStartupMessage()
        {
            // Wait for camera setup to complete
            yield return new WaitForSeconds(1.5f);
            if (NetworkManager)
            {
                NetworkMessage msg2 = new NetworkMessage
                {
                    address = "/externalApplication/selected"
                };
                msg2.AddStringArgument(settings.DisplaySettings.Name);
                NetworkManager.SendMessage(msg2);

                NetworkMessage msg = new NetworkMessage
                {
                    address = "/externalApplication/selected/enabled"
                };
                msg.AddBoolArgument(true);
                NetworkManager.SendMessage(msg);
            }
        }

        /// <summary>
        /// Takes the relevant system settings from the IglooSettings.xml and sets the awaiting variables.
        /// </summary>
        /// <returns></returns>
        private SystemSettings GetSystemSettings()
        {
            SystemSettings settings = new SystemSettings
            {
                targetFPS = targetFPS,
                vSyncMode = vSyncMode,
                sendStartupMessage = sendStartupMessage
            };
            return settings;
        }

        /// <summary>
        /// On Application Quit Mono Function. Used to do a final save of the IglooSettings if required.
        /// </summary>
        private void OnApplicationQuit()
        {
            if (saveOnQuit) GetAndSaveSettings();
        }

        /// <summary>
        /// If the Igloo Manager is destoryed, it will attempt to save the IglooSettings if required.
        /// </summary>
        private void OnDestroy()
        {
            Debug.Log("<b>[Igloo]</b> Manager destructor called");
            if (saveOnQuit && igloo) GetAndSaveSettings();
            if (igloo) Destroy(igloo);
        }


    }
    public class SettingsUI
    {
        public enum SettingsType { Project, File }
        public SettingsType settingsType;

        public enum ConfigType { EquirectangularStrip, EquirectangularFull, CUBE, CAVE }
        public ConfigType configType = ConfigType.EquirectangularStrip;


        Vector2 scrollPos;

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
    }
}

