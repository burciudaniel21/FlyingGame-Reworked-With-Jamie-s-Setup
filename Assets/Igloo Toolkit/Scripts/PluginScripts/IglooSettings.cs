using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace Igloo
{
#pragma warning disable IDE0090 // Use New()...

    /// <summary>
    /// Igloo Settings Root XML Class
    /// </summary>
    [XmlRoot("IglooSettings", IsNullable = false)]
    public class Settings
    {
        [XmlAttribute]
        public string version;
        public string Name;
        public SystemSettings SystemSettings;
        public PlayerSettings PlayerSettings;
        public NetworkSettings NetworkSettings;
        public UISettings UISettings;
        public DisplaySettings DisplaySettings;
        public WindowSettings WindowSettings;
        public TouchScreenSettings TouchScreenSettings;
    }

    /// <summary>
    /// Head Settings XML Class
    /// </summary>
    public class HeadSettings
    {
        public Vector3Item headPositionOffset;
        public Vector3Item headRotationOffset;
        public Vector3Item leftEyeOffset;
        public Vector3Item rightEyeOffset;
        public bool headtracking;
        public int headTrackingInput; // 0 - Default (OSC), 1 - Optitrack
        public string optitrackServerIP; // Server IP for the Optitrack system. Default 127.0.0.1
        public int optitrackHeadRigidBodyID; // 1 - Default: Optitrack Rigidbody ID that system is instructed to follow.
    }

    /// <summary>
    /// 
    /// Display Settings XML Class
    /// </summary>
    public class DisplaySettings
    {
        [XmlAttribute]
        public string Name;
        public int textureShareMode; // 0 - none, 1 - spout, 2 - ndi, 3 - DX window
        public bool useCubemapToEquirectangular;
        public Vector2IntItem equirectangularTextureResolution;
        public Vector2IntItem equirectangularTexuteRes;
        public float horizontalFOV;
        public float verticalFOV;
        public bool useCompositeTexture;
        public bool useFramepackTopBottom3D;
        public bool useWarpBlend;
        public bool useTruePerspective;
        public HeadSettings HeadSettings;
        public DisplayItem[] Displays;
        public WarpBlendSettings WarpBlendSettings;
        public UWPExternalWindowSettings UWPExternalWindowSettings;

    }

    /// <summary>
    /// UWP External Window Settings XML Class
    /// </summary>
    public class UWPExternalWindowSettings
    {
        public int posX;
        public int posY;
        public int sizeX;
        public int sizeY;
    }

    /// <summary>
    /// Warp Window Settings XML Class
    /// </summary>
    public class WarpWindowItem
    {
        public int targetDisplayPrimary;
        public int targetDisplaySecondary; // for dual window 3D mode 
        public int stereoMode; // 0 - off, 1- side by side, 2- top bottom, 3 - dual window
        public float positionX;
        public float positionY;
        public float scaleX;
        public float scaleY;
    }

    /// <summary>
    /// Warp Blend Settings XML Class
    /// </summary>
    public class WarpBlendSettings
    {
        public string warperDataPath;
        public WarpWindowItem[] Windows;
    }

    /// <summary>
    /// Window Settings XML Class
    /// </summary>
    public class WindowSettings
    {
        [XmlAttribute]
        public bool enabled;
        public WindowItem[] Windows;
    }

    /// <summary>
    /// Window Item XML Class
    /// </summary>
    public class WindowItem
    {
        public int width;
        public int height;
        public int positionOffsetX;
        public int positionOffsetY;
    }

    /// <summary>
    /// Display Item XML Class
    /// </summary>
    public class DisplayItem
    {
        [XmlAttribute]
        public string Name;
        public bool isRendering;
        public float fov;
        public bool is3D;
        public bool isRenderTextures;
        public int cubemapFace; // 0 - left, 1 - front, 2 - right, 3 - back, 4 - down, 5 - up
        public Vector2IntItem renderTextureSize;
        public int textureShareMode; // 0 - none, 1 - spout, 2 - ndi
        public Vector3Item cameraRotation;
        public float nearClipPlane;
        public float farClipPlane;
        public bool isFisheye;
        [XmlElement(IsNullable = false)]
        public Vector2Item fisheyeStrength;


        public bool isOffAxis;
        [XmlElement(IsNullable = false)]
        public Vector3Item viewportRotation;
        [XmlElement(IsNullable = false)]
        public Vector2Item viewportSize;
        [XmlElement(IsNullable = false)]
        public Vector3Item viewportPosition;

        public int targetDisplay = -1;
    }

    /// <summary>
    /// Player Settings XML Class
    /// </summary>
    public class PlayerSettings
    {
        [XmlAttribute]
        public string Name;
        public bool usePlayer;
        public int rotationInput;   // 0 - standard, 1 - warper, 2 - gyrOSC , 3 - vr controller, 4 - optitrack, 5 - VRPN
        public int rotationMode;    // 0 - igloo360, 1 - igloo non-360, 2 - game
        public int movementInput;   // 0 - standard, 1 - gyrosc 
        public int movementMode;    // 0 - walking, 1 - flying, 2 - ghost
        public float runSpeed;
        public float walkSpeed;
        public float smoothTime;
        public int optitrackControllerRigidBodyID;
        public int crosshairHideMode; 
        public bool isCrosshair3D;
    }

    /// <summary>
    /// Network Settings XML Class
    /// </summary>
    public class NetworkSettings
    {
        public int inPort;
        public int outPort;
        public string outIP;
    }

    /// <summary>
    /// System Settings XML Class
    /// </summary>
    public class SystemSettings
    {
        [XmlAttribute]
        public int vSyncMode;
        [XmlAttribute]
        public int targetFPS;
        [XmlAttribute]
        public bool sendStartupMessage;
        [XmlAttribute]
        public bool useDisplaySettingsOverride;
        [XmlAttribute]
        public string displaySettingsOverridePath;
    }

    /// <summary>
    /// Touch screen settings XML Class
    /// </summary>
    public class TouchScreenSettings {

        public float XPositionStart;
        public float XPositionEnd;
        public float YPositionStart;
        public float YPositionEnd;
        public bool UseTouchScreen;
    }

    /// <summary>
    /// UI Settings XML Class
    /// </summary>
    public class UISettings
    {
        public bool useUI;
        public string screenName;
        public Vector3Item screenPos;
        public Vector3Item screenRot;
        public Vector3Item screenScale;
        public bool followCrosshair;
        public float followSpeed;
        public bool debugUIMode;
    }

    /// <summary>
    /// Vector 2 Item structured class
    /// </summary>
    public class Vector2Item
    {
        public Vector2Item() { }
        public Vector2Item(Vector2 val) { x = val.x; y = val.y; }
        public Vector2Item(float X, float Y) { x = X; y = Y; }
        [XmlAttribute] public float x;
        [XmlAttribute] public float y;
        public Vector2 Vector2 { get { return new Vector2(x, y); } }
    }

    /// <summary>
    /// Vector2 int Item structure class
    /// </summary>
    public class Vector2IntItem
    {
        public Vector2IntItem() { }
        public Vector2IntItem(Vector2Int val) { x = val.x; y = val.y; }
        public Vector2IntItem(int X, int Y) { x = X; y = Y; }
        [XmlAttribute] public int x;
        [XmlAttribute] public int y;
        public Vector2Int Vector2Int { get { return new Vector2Int(x, y); } }
    }

    /// <summary>
    /// Vector3 Item structure class
    /// </summary>
    public class Vector3Item
    {
        public Vector3Item() { }
        public Vector3Item(Vector3 val) { x = val.x; y = val.y; z = val.z; }
        public Vector3Item(float X, float Y, float Z) { x = X; y = Y; z = Z; }
        [XmlAttribute] public float x;
        [XmlAttribute] public float y;
        [XmlAttribute] public float z;
        public Vector3 Vector3 { get { return new Vector3(x, y, z); } }
    }

    /// <summary>
    /// Rect Item structure class
    /// </summary>
    public class RectItem
    {
        public RectItem() { }
        public RectItem(Rect val) { x = val.x; y = val.y; w = val.width; h = val.height; }
        public RectItem(float X, float Y, float W, float H) { x = X; y = Y; w = W; h = H; }
        [XmlAttribute] public float x;
        [XmlAttribute] public float y;
        [XmlAttribute] public float w;
        [XmlAttribute] public float h;
        public Rect Rect { get { return new Rect(x, y, w, h); } }
    }

    /// <summary>
    /// Transform Item structure class
    /// </summary>
    public class TransformItem
    {
        [XmlAttribute] public float posX;
        [XmlAttribute] public float posY;
        [XmlAttribute] public float posZ;
        [XmlAttribute] public float rotX;
        [XmlAttribute] public float rotY;
        [XmlAttribute] public float rotZ;
        [XmlAttribute] public float scaleX;
        [XmlAttribute] public float scaleY;
        [XmlAttribute] public float scaleZ;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlCommentAttribute : Attribute {
        public XmlCommentAttribute(string value) {
            this.Value = value;
        }

        public string Value { get; set; }
    }

    /// <summary>
    /// XML Serialiser class
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Load XML from Filename
        /// </summary>
        /// <param name="path">string, The file path including the xml extension</param>
        /// <returns>Populated Settings xml class</returns>
        public Settings Load(string path)
        {
            if (!Path.HasExtension(path)) path += ".xml";
            path = Environment.ExpandEnvironmentVariables(path);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));

                serializer.UnknownNode += new
                XmlNodeEventHandler(Serializer_UnknownNode);
                serializer.UnknownAttribute += new
                XmlAttributeEventHandler(Serializer_UnknownAttribute);

                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                Settings settings = (Settings)serializer.Deserialize(fs);
                fs.Close();
                return settings;
            }
            catch (SystemException e)
            {
                Debug.LogWarning(e);
                return null;
            }
        }

        /// <summary>
        /// Saves the populated Settings xml class at filename
        /// </summary>
        /// <param name="filename">String, file name including extension</param>
        /// <param name="settings">Populated Settings XML Class</param>
        public void Save(string filename, Settings settings)
        {
            if (!Path.HasExtension(filename)) filename += ".xml";
            /// Check if the directory 'Streaming Assets' exists, and if it doesn't; Create it. 
            /// Common in new projects to not have a Streaming Assets folder.
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                Debug.Log($"<b>[Igloo]</b> {Path.GetDirectoryName(filename)} did not exist, so it has been created.");
            }
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, settings);
            writer.Close();
        }

        /// <summary>
        /// Event fallback for unknown xml node
        /// </summary>
        /// <param name="sender">XML Object Sender</param>
        /// <param name="e">Event Arguments</param>
        private void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Debug.LogWarning("<b>[Igloo]</b> Unknown Node:" + e.Name + "\t" + e.Text);
        }

        /// <summary>
        /// Event fallback for Unknown XML Attribute 
        /// </summary>
        /// <param name="sender">XML Object Sender</param>
        /// <param name="e">Event Arguments</param>
        private void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Debug.LogError("<b>[Igloo]</b> Unknown attribute " + attr.Name + "='" + attr.Value + "'");
        }
    }
}
