using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Igloo.Common
{
#pragma warning disable IDE0090 // Use New()...
#pragma warning disable IDE0044 // Add readonly modifier
   
    /// <summary>
    /// Warp and Blend variable structure.
    /// Contains all variables to adjust warp and blend system.
    /// </summary>
    [System.Serializable]
    public struct WarpDisplay
    {
        public GameObject go;
        public int dispayIndex; // as per IglooWarper settings
        public int windowWidth;
        public int windowHeight;
        public int targetDisplay; // as per Windows display settings
        public int targetDisplay2; // for dual window 3D mode
        public float canvasPosU;
        public float canvasWidth;
        public float overlapLeft;
        public float overlapRight;
        public int stereoMode;
        public string warpImagesPath;
        public float positionX;
        public float positionY;
        public float scaleX;
        public float scaleY;
    }

    /// <summary>
    /// The Igloo Warp and Blend Manager Class
    /// </summary>
    public class WarpBlendManager : MonoBehaviour
    {
        /// <summary>
        /// Center RenderTexture. For 2D display
        /// </summary>
        public RenderTexture canvasCentre;

        /// <summary>
        /// Left RenderTexture, for left eye of 3D display
        /// </summary>
        public RenderTexture canvasLeft;

        /// <summary>
        /// Right RenderTexture, for right eye of 3D display
        /// </summary>
        public RenderTexture canvasRight;

        /// <summary>
        /// A list of the active warper displays
        /// </summary>
        public List<WarpDisplay> displays;

        /// <summary>
        /// List of active warp/blend compositiors
        /// </summary>
        public List<WarpBlendCompositor> WarpCompositors;

        /// <summary>
        /// Data path to the Igloo Warper data folder.
        /// </summary>
        public string warperDataPath;

        /// <summary>
        /// Warping and Blending settings from the Igloo Settings XML 
        /// </summary>
        WarpBlendSettings settings = null;

        /// <summary>
        /// If True, Warp settings are loaded from the Igloo Settings XML
        /// </summary>
        bool isWarpSettingsLoaded = false;

        /// <summary>
        /// If True, Debug textures will be used.
        /// </summary>
        bool debug = false;

        /// <summary>
        /// Debug Texture
        /// </summary>
        public Texture2D debugTex;

        /// <summary>
        /// Unlit Material for Warp and Blend Blit functions
        /// </summary>
        public Material unlitMat;
        public void SetSettings(WarpBlendSettings s)
        {
            if (s != null)
            {
                warperDataPath = s.warperDataPath;
                settings = s;
            }
            else
            {
                settings = new WarpBlendSettings
                {
                    Windows = new WarpWindowItem[0]
                };
            }
        }

        /// <summary>
        /// Returns the settings from Igloo Settings Data
        /// </summary>
        /// <returns>A populated WarpBlendSettings class</returns>
        public WarpBlendSettings GetSettings()
        {
            if (isWarpSettingsLoaded)
            {
                settings = new WarpBlendSettings
                {
                    warperDataPath = warperDataPath
                };
                WarpWindowItem[] windows = new WarpWindowItem[displays.Count];
                for (int i = 0; i < displays.Count; i++)
                {
                    WarpWindowItem disp = new WarpWindowItem
                    {
                        targetDisplayPrimary = displays[i].targetDisplay,
                        targetDisplaySecondary = displays[i].targetDisplay2,
                        positionX = displays[i].positionX,
                        positionY = displays[i].positionY,
                        scaleX = displays[i].scaleX,
                        scaleY = displays[i].scaleY,
                        stereoMode = displays[i].stereoMode
                    };
                    windows[i] = disp;
                }
                settings.Windows = windows;
            }
            return settings;
        }

        /// <summary>
        /// Returns the warp and blend compositor on the target display
        /// </summary>
        /// <param name="targetDisplay">Int, the target display to return the warp and blend compositor on.</param>
        /// <returns>The warp and blend compositor attatched to this display</returns>
        public WarpBlendCompositor GetDisplayCompositor(int targetDisplay)
        {
            //Returns null if compositor does not exists
            for (int i = 0; i < WarpCompositors.Count; i++)
            {
                if (WarpCompositors[i].GetTargetDisplay() == targetDisplay)
                {
                    return WarpCompositors[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a warp compositor on the given display.
        /// </summary>
        /// <param name="targetDisplay">Int, the display on which to create the warps compositor on</param>
        /// <returns>The created warp and blend compositor</returns>
        public WarpBlendCompositor CreateWarpCompositor(int targetDisplay)
        {
            WarpBlendCompositor newCompositor = GetDisplayCompositor(targetDisplay);
            if (newCompositor != null)
            {
                //Compositor already exists for this display
                Debug.LogWarning("<b>[Igloo]</b> Display Compositor already exists for display :" + targetDisplay);
            }
            else
            {
                //Create new GameObject with a compositor and add to list
                GameObject go = new GameObject();
                go.transform.parent = this.transform;
                go.name = "Warp&Blend Display Output: " + targetDisplay.ToString();
                go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
#if IGLOO_URP || IGLOO_HDRP
                // Adjusting Z pos as a DIY occulusion method when using multiple orthographic compositing cameras 
                go.transform.localPosition = new Vector3(0.0f, 3000.0f, targetDisplay * 50.0f);
#endif
                Camera newCam = go.AddComponent<Camera>();
                newCompositor = go.AddComponent<WarpBlendCompositor>();
                newCompositor.displayCamera = newCam;
                newCompositor.SetTargetDisplay(targetDisplay);
                newCompositor.canvasCentre = canvasCentre;
                newCompositor.canvasLeft = canvasLeft;
                newCompositor.canvasRight = canvasRight;
                WarpCompositors.Add(newCompositor);
            }
            return newCompositor;
        }

        /// <summary>
        /// Adds the warp and blend to the compositor system
        /// Creates a compositor if one is not returned.
        /// </summary>
        /// <param name="warpBlend">A populated warpblend structure</param>
        public void AddWarpBlendToCompositor(WarpBlend warpBlend)
        {
            int targetDisplay = warpBlend.targetDisplay;

            WarpBlendCompositor displayCompositor = GetDisplayCompositor(targetDisplay);
            if (displayCompositor == null)
            {
                //Create new Compositor if one doesn't exists for the targetDisplay
                displayCompositor = CreateWarpCompositor(targetDisplay);
            }

            displayCompositor.AddWarpBlend(warpBlend);

        }

        /// <summary>
        /// Gets prepared to load the warper data settings
        /// Starts the setup displays function once the warper settings are loaded.
        /// </summary>
        public void Setup()
        {
            if (string.IsNullOrEmpty(warperDataPath))
            {
                warperDataPath = Utils.GetWarperDataPath();
            }
            if (!Directory.Exists(warperDataPath))
            {
                Debug.LogError("<b>[Igloo]</b> Warper settings path does not exist: " + warperDataPath);
                return;
            }

            isWarpSettingsLoaded = LoadWarperSettings();
            if (isWarpSettingsLoaded) SetupDisplays();
        }

        /// <summary>
        /// Returns the warper settings from the warper data path
        /// </summary>
        /// <returns>True if warper settings were loaded correctly</returns>
        private bool LoadWarperSettings()
        {
            Debug.Log("<b>[Igloo]</b> Loading warper settings files from: " + warperDataPath);
            // Load Canvas Settings
            string mainPath = Path.Combine(warperDataPath, "AppSettings.xml");
            if (!File.Exists(mainPath)) mainPath = Path.Combine(warperDataPath, "IglooWarperMainSettings.xml");

            _ = new XmlDocument();

            // Load Blend Settings
            string blendPath = Path.Combine(warperDataPath, "BlenderSettings.xml");
            XmlDocument blendDoc = new XmlDocument();

            if (File.Exists(mainPath) && File.Exists(blendPath))
            {
                displays = new List<WarpDisplay>();

                // The Warper main settings does not have a single root node so manually add one
                string xmlString = System.IO.File.ReadAllText(mainPath);
                XDocument xdoc = XDocument.Parse("<root>" + xmlString + "</root>");
                _ = new XmlDocument();
                XmlDocument mainDoc = ToXmlDocument(xdoc);

                blendDoc.Load(blendPath);
                var displayElements = mainDoc.GetElementsByTagName("display");
                int numDisplays = displayElements.Count;

                // number of displays changed
                if (settings.Windows.Length != numDisplays) settings.Windows = new WarpWindowItem[0];

                for (int i = 0; i < numDisplays; i++)
                {
                    WarpDisplay disp = new WarpDisplay();

                    // Set setting from IglooSetting  , 
                    if (i < settings.Windows.Length)
                    {
                        disp.targetDisplay = settings.Windows[i].targetDisplayPrimary;
                        disp.targetDisplay2 = settings.Windows[i].targetDisplaySecondary;
                        disp.stereoMode = settings.Windows[i].stereoMode;
                        disp.positionX = settings.Windows[i].positionX;
                        disp.positionY = settings.Windows[i].positionY;
                        disp.scaleX = settings.Windows[i].scaleX;
                        disp.scaleY = settings.Windows[i].scaleY;
                    }
                    // If no settings are found apply some defaults
                    else
                    {
                        disp.targetDisplay = i;
                        disp.targetDisplay2 = i + numDisplays;
                        disp.scaleX = 1.0f;
                        disp.scaleY = 1.0f;
                    }
                    disp.dispayIndex = i;
                    disp.warpImagesPath = Path.Combine(warperDataPath, "warps\\high performance");

                    // Get Canvas Settings
                    var canvasUAttrib = displayElements[i].Attributes["canvasPosU"];
                    if (canvasUAttrib != null)
                    {
                        if (float.TryParse(canvasUAttrib.Value, out float canvasU)) disp.canvasPosU = canvasU;
                    }
                    var canvasWidthAttrib = displayElements[i].Attributes["canvasSubsectionWidth"];
                    if (canvasWidthAttrib != null)
                    {
                        if (float.TryParse(canvasWidthAttrib.Value, out float canvasWidth)) disp.canvasWidth = canvasWidth;
                    }

                    // Get Blend Settings
                    string leftTag = "leftBlendingOverlap-" + i.ToString();
                    string rightTag = "rightBlendingOverlap-" + i.ToString();
                    var left = blendDoc.GetElementsByTagName(leftTag);
                    if (left.Count > 0)
                    {
                        if (float.TryParse(left[0].InnerText, out float leftBlend)) disp.overlapLeft = leftBlend / 100.0f;
                        var right = blendDoc.GetElementsByTagName(rightTag);
                        if (float.TryParse(right[0].InnerText, out float rightBlend)) disp.overlapRight = rightBlend / 100.0f;
                        displays.Add(disp);
                        Debug.Log("<b>[Igloo]</b> Warp display settings loaded, index: " + i + " , Canvas U: " + disp.canvasPosU + " , canvas Width: " + disp.canvasWidth + " , left overlap:" + disp.overlapLeft + " , right overlap:" + disp.overlapRight);
                    }


                }
                return true;
            }
            else
            {
                Debug.LogError("<b>[Igloo]</b> Warper settings could not be found");
                return false;
            }
        }

        /// <summary>
        /// Creates a display for each camera input. 
        /// Attaches the warp and blend component to each one
        /// Sets up the warp and blend composition. 
        /// </summary>
        private void SetupDisplays()
        {
            for (int i = 0; i < displays.Count; i++)
            {
                // Stereo 3D - Dual window mode i.e a window per eye 
                if (displays[i].stereoMode == 3)
                {
                    // Left Eye
                    GameObject go = new GameObject
                    {
                        name = "Warp&Blend Left Eye"// + displays[i].dispayIndex.ToString();
                    };
                    WarpBlend warp = go.AddComponent<WarpBlend>();
                    warp.centreTex = canvasLeft;
                    if (debug) warp.debugTex = debugTex;
                    warp.Setup(displays[i]);
                    warp.targetDisplay = displays[i].targetDisplay;
                    AddWarpBlendToCompositor(warp);
                    // Right Eye
                    GameObject go2 = new GameObject
                    {
                        name = "Warp&Blend Right Eye"// + displays[i].dispayIndex.ToString();
                    };
                    WarpBlend warp2 = go2.AddComponent<WarpBlend>();
                    warp2.centreTex = canvasRight;
                    if (debug) warp2.debugTex = debugTex;
                    warp2.Setup(displays[i]);
                    warp2.targetDisplay = displays[i].targetDisplay;
                    AddWarpBlendToCompositor(warp2);

                }
                // Single window - either not stereo (0) , side-by-side (1) stereo or top-bottom stereo (2)
                else
                {
                    GameObject go = new GameObject();
                    go.transform.parent = this.transform;
                    go.name = "Warp&Blend";// + displays[i].dispayIndex.ToString();
                    WarpBlend warp = go.AddComponent<WarpBlend>();
                    warp.centreTex = canvasCentre;
                    warp.leftEyeTex = canvasLeft;
                    warp.rightEyeTex = canvasRight;
                    if (debug) warp.debugTex = debugTex;
                    warp.Setup(displays[i]);
                    warp.targetDisplay = displays[i].targetDisplay;
                    AddWarpBlendToCompositor(warp);
                }
            }
        }

        /// <summary>
        /// Quick XML Serializer for XML Document
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns>Populated, readable, XML Document</returns>
        public XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }
    }
}


