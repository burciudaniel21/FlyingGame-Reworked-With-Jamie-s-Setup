using System.Collections.Generic;
using UnityEngine;
#if IGLOO_URP
using UnityEngine.Rendering.Universal;
#endif

namespace Igloo.Common
{
#pragma warning disable IDE0090 // Use New()...

    /// <summary>
    /// The Igloo Display Class
    /// </summary>
    /// <remarks>
    /// Creates and manages the cameras that capture the world and format it for the Igloo
    /// </remarks>
    [ExecuteInEditMode]
    public class Display : MonoBehaviour
    {
        /// <summary>
        /// Camera System Name
        /// </summary>
        public string Name;

        /// <summary>
        /// If true, Setup the camera system on start.
        /// </summary>
        public bool autoSetupOnStart = false;

        /// <summary>
        /// The Igloo Head Manager
        /// </summary>
        public HeadManager headManager;

        /// <summary>
        /// The Igloo Display Manager
        /// </summary>
        public DisplayManager displayManager;

        /// <summary>
        /// A camera prefab to be used as a base for the generated cameras
        /// </summary>
        public GameObject cameraPrefab;

        /// <summary>
        /// Enum Identification for each cube map face
        /// </summary>
        public enum IglooCubemapFace { Left, Front, Right, Back, Down, Up }

        /// <summary>
        /// The current Igloo Cube Map Face
        /// </summary>
        public IglooCubemapFace iglooCubemapFace;

        /// <summary>
        /// The active Igloo Cameras
        /// </summary>
        [SerializeField]
        protected Dictionary<Igloo.EYE, Camera> activeCameras;

        /// <summary>
        /// Get method for the Igloo Cameras
        /// </summary>
        /// <returns>Dictionary of active cameras</returns>
        public Dictionary<Igloo.EYE, Camera> GetActiveCameras() { return activeCameras; }

        /// <summary>
        /// Returns a list of the Camera Components. 
        /// </summary>
        /// <remarks>
        /// If Display is 3D there will be 2 cameras; left, right
        /// If Display is not 3D there will be 1 camera; center
        /// </remarks>
        /// <returns>
        /// List of camera components
        /// </returns>
        public List<Camera> GetCameras()
        {
            List<Camera> cams = new List<Camera>();
            foreach (var cam in activeCameras)
            {
                cams.Add(cam.Value);
            }
            return cams;
        }

        /// <summary>
        /// Left eye render texture
        /// </summary>
        public RenderTexture leftTexture = null;

        /// <summary>
        /// Center camera render texture
        /// </summary>
        public RenderTexture centerTexture = null;

        /// <summary>
        /// Right eye render texture
        /// </summary>
        public RenderTexture rightTexture = null;

        /// <summary>
        /// Field of View amount
        /// </summary>
        protected float fov;

        /// <summary>
        /// Gets / Sets the Field of View of all the active cameras.
        /// </summary>
        public virtual float FOV
        {
            get { return fov; }
            set
            {
                foreach (var cam in activeCameras)
                {
                    cam.Value.fieldOfView = value;
                }
                fov = value;
            }
        }

        /// <summary>
        /// The near clip plane of the cameras
        /// </summary>
        protected float nearClipPlane = 0.01f;

        /// <summary>
        /// Gets / Sets the near clip plane of the active cameras.
        /// </summary>
        public float NearClipPlane
        {
            get { return nearClipPlane; }
            set
            {
                foreach (var cam in activeCameras)
                {
                    cam.Value.nearClipPlane = value;
                }
                nearClipPlane = value;
            }
        }


        /// <summary>
        /// The far clip plane of the cameras
        /// </summary>
        protected float farClipPlane = 1000.0f;
        /// <summary>
        /// Gets / Sets the far clip plane of the active cameras.
        /// </summary>
        public float FarClipPlane
        {
            get { return farClipPlane; }
            set
            {
                foreach (var cam in activeCameras)
                {
                    cam.Value.farClipPlane = value;
                }
                farClipPlane = value;
            }
        }

        /// <summary>
        /// Current camera rotation
        /// </summary>
        protected Vector3 camRotation;

        /// <summary>
        /// Is the camera rendering.
        /// </summary>
        /// <remarks>
        /// Read from settings file, and should not be modified during runtime.
        /// </remarks>
        private bool isRendering;

        /// <summary>
        /// Enables / disables the active cameras when set. 
        /// Get returns the current state. 
        /// </summary>
        public bool SetRendering
        {
            get { return isRendering; }
            set
            {
                if (isRendering)
                {
                    foreach (var cam in activeCameras)
                    {
                        cam.Value.enabled = value;
                    }
                }
            }
        }

        /// <summary>
        /// True, Active Cameras are 3D
        /// </summary>
        protected bool is3D;

        /// <summary>
        /// Get: Returns active camera 3D status
        /// </summary>
        public bool Is3D { get { return is3D; } }

        /// <summary>
        /// True, active cameras are being merged into a single render texture.
        /// False, camera feeds are being sent seperatly via spout to the warper.
        /// </summary>
        protected bool isRenderTextures;
        /// <summary>
        /// Returns current isRenderTexture status
        /// </summary>
        public bool IsRenderTextures { get { return IsRenderTextures; } }

        /// <summary>
        /// Size of RenderTexture
        /// </summary>
        protected Vector2Int renderTextureSize;

        /// <summary>
        /// Returns the size of RenderTexture
        /// </summary>
        public Vector2Int RenderTextureSize { get { return renderTextureSize; } }

        /// <summary>
        /// Camera view port rect
        /// </summary>
        protected Rect viewPortRect;
        /// <summary>
        /// Returns the current camera view port rect size
        /// Set: Set's the active cameras to the new view port rect size.
        /// </summary>
        public Rect ViewportRect
        {
            get { return viewPortRect; }
            set
            {
                foreach (var cam in activeCameras)
                {
                    cam.Value.rect = value;
                }
                viewPortRect = value;
            }
        }

        /// <summary>
        /// True, Active cameras are using Fisheye effect
        /// </summary>
        protected bool isFisheye;

        /// <summary>
        /// Returns the current Fisheye use status
        /// Set: Adds the fishEye component to the cameras or destroys it.
        /// </summary>
        public bool IsFisheye
        {
            get { return isFisheye; }
            set
            {
                foreach (var cam in activeCameras)
                {
                    if (value)
                    {
                        if (cam.Value.gameObject.GetComponent<Fisheye>() == null) cam.Value.gameObject.AddComponent<Fisheye>();
                    }
                    else if (!value)
                    {
                        if (cam.Value.gameObject.GetComponent<Fisheye>()) Destroy(gameObject.GetComponent<Fisheye>());
                    }
                }
                isFisheye = value;
            }
        }

        /// <summary>
        /// The active fish eye strength
        /// </summary>
        protected Vector2 fisheyeStrength;
        /// <summary>
        /// Returns the current fish eye strength
        /// Set: Get's fisheye component on active cameras, and adjusts strength
        /// </summary>
        public Vector2 FisheyeStrength
        {
            get { return fisheyeStrength; }
            set
            {
                foreach (var cam in activeCameras)
                {
                    if (cam.Value.gameObject.GetComponent<Fisheye>() != null)
                    {
                        cam.Value.gameObject.GetComponent<Fisheye>().strengthX = value.x;
                        cam.Value.gameObject.GetComponent<Fisheye>().strengthX = value.x;
                    }
                }
                fisheyeStrength = value;
            }
        }

        /// <summary>
        /// true, off axis projection is being used.
        /// </summary>
        public bool isOffAxis;

        /// <summary>
        /// Current viewport width
        /// </summary>
        public float viewportWidth;

        /// <summary>
        /// Current viewport height
        /// </summary>
        public float viewportHeight;

        /// <summary>
        /// Calculates half the viewport width
        /// </summary>
        /// <returns>float, half active viewport width</returns>
        protected float halfWidth() { return viewportWidth * 0.5f; }

        /// <summary>
        /// Calculates half the viewport height
        /// </summary>
        /// <returns>float, half active viewport height</returns>
        protected float halfHeight() { return viewportHeight * 0.5f; }

        /// <summary>
        /// Find the world point for the upper right corner of the active camera viewport
        /// </summary>
        /// <returns>Vector3, World Space point</returns>
        public Vector3 UpperRight { get { return transform.localToWorldMatrix * new Vector4(halfWidth(), halfHeight(), 0.0f, 1.0f); } }

        /// <summary>
        /// Find the world point for the upper left corner of the active camera viewport
        /// </summary>
        /// <returns>Vector3, World Space point</returns>
        public Vector3 UpperLeft { get { return transform.localToWorldMatrix * new Vector4(-halfWidth(), halfHeight(), 0.0f, 1.0f); } }

        /// <summary>
        /// Find the world point for the lower left corner of the active camera viewport
        /// </summary>
        /// <returns>Vector3, World Space point</returns>
        public Vector3 LowerLeft { get { return transform.localToWorldMatrix * new Vector4(-halfWidth(), -halfHeight(), 0.0f, 1.0f); } }

        /// <summary>
        /// Find the world point for the lower right corner of the active camera viewport
        /// </summary>
        /// <returns>Vector3, World Space point</returns>
        public Vector3 LowerRight { get { return transform.localToWorldMatrix * new Vector4(halfWidth(), -halfHeight(), 0.0f, 1.0f); } }

        /// <summary>
        /// Mono Function: Awake. 
        /// </summary>
        public virtual void Awake()
        {
            if (Application.isPlaying && transform.parent == null && IglooManager.instance.dontDestroyOnLoad) DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Mono function: Late Update. 
        /// Calculates off axis camera positioning.
        /// </summary>
        public virtual void LateUpdate()
        {
            // update camera projection matrices for offaxis projection
            if (isOffAxis)
            {
                // Old Method
                foreach (var cam in activeCameras)
                {
                    cam.Value.projectionMatrix = Utils.GetAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, cam.Value.gameObject.transform.position, headManager.NearClipPlane, headManager.FarClipPlane);
                    cam.Value.gameObject.transform.rotation = transform.rotation;
                }

                // Stereo Matrix Test
                //bool isSetStereoMat = false;
                //foreach (var cam in activeCameras)
                //{
                //    if (cam.Key == EYE.CENTER) {
                //        cam.Value.projectionMatrix = Utils.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, cam.Value.gameObject.transform.position, headManager.NearClipPlane, headManager.FarClipPlane);
                //    }
                //    else if (cam.Key == EYE.LEFT)  {
                //        if (isSetStereoMat) {
                //            Matrix4x4 leftMatrix = Utils.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, cam.Value.gameObject.transform.position, headManager.NearClipPlane, headManager.FarClipPlane);
                //            cam.Value.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, leftMatrix);
                //        }
                //        else cam.Value.projectionMatrix = Utils.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, cam.Value.gameObject.transform.position, headManager.NearClipPlane, headManager.FarClipPlane);
                //    }
                //    else if (cam.Key == EYE.RIGHT)
                //    {
                //        if (isSetStereoMat) {
                //            Matrix4x4 rightMatrix = Utils.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, cam.Value.gameObject.transform.position, headManager.NearClipPlane, headManager.FarClipPlane);
                //            cam.Value.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, rightMatrix);
                //        }
                //        else cam.Value.projectionMatrix = Utils.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, cam.Value.gameObject.transform.position, headManager.NearClipPlane, headManager.FarClipPlane);
                //    }
                //    cam.Value.gameObject.transform.rotation = transform.rotation;
                //}
            }
        }

        /// <summary>
        /// Sets the current Igloo Display settings ready to write into the 
        /// Igloo Settings xml file. 
        /// </summary>
        /// <param name="settings">A populated DisplayItem settings class</param>
        public virtual void SetSettings(DisplayItem settings)
        {
            Name = settings.Name;
            isRendering = settings.isRendering;
            is3D = settings.is3D;
            isOffAxis = settings.isOffAxis;
            fov = settings.fov;
            isFisheye = settings.isFisheye;
            isRenderTextures = settings.isRenderTextures;
            iglooCubemapFace = (IglooCubemapFace)settings.cubemapFace;

            if (settings.nearClipPlane != 0) nearClipPlane = settings.nearClipPlane;
            if (settings.farClipPlane != 0) farClipPlane = settings.farClipPlane;
            if (settings.cameraRotation != null) camRotation = settings.cameraRotation.Vector3;
            if (settings.viewportRotation != null) this.transform.localEulerAngles = settings.viewportRotation.Vector3;
            if (settings.renderTextureSize != null) renderTextureSize = settings.renderTextureSize.Vector2Int;
            if (settings.viewportPosition != null) this.transform.localPosition = settings.viewportPosition.Vector3;
            if (settings.viewportSize != null)
            {
                viewportWidth = settings.viewportSize.x;
                viewportHeight = settings.viewportSize.y;
            }
            if (settings.fisheyeStrength != null)
            {
                fisheyeStrength.x = settings.fisheyeStrength.x;
                fisheyeStrength.y = settings.fisheyeStrength.y;
            }

        }

        /// <summary>
        /// Reads the Display settings from the current IglooSettings.xml file.
        /// </summary>
        /// <returns>A populated DisplayItem settings class</returns>
        public virtual DisplayItem GetSettings()
        {
            DisplayItem settings = new DisplayItem
            {
                Name = Name,
                isRendering = isRendering,
                cameraRotation = new Vector3Item(camRotation),
                is3D = is3D,
                isOffAxis = isOffAxis,
                viewportPosition = new Vector3Item(this.transform.localPosition),
                viewportSize = new Vector2Item(viewportWidth, viewportHeight),
                fov = fov,
                isFisheye = isFisheye,
                isRenderTextures = isRenderTextures,
                cubemapFace = (int)iglooCubemapFace,
                viewportRotation = new Vector3Item(this.transform.localEulerAngles),
                nearClipPlane = nearClipPlane,
                farClipPlane = farClipPlane,
                fisheyeStrength = new Vector2Item(fisheyeStrength)
            };
            if (renderTextureSize != null) settings.renderTextureSize = new Vector2IntItem(renderTextureSize);

            return settings;
        }

        /// <summary>
        /// Activates the Igloo Cameras
        /// </summary>
        public virtual void InitialiseCameras() { }

        /// <summary>
        /// Creates a new Display from the active cameras
        /// </summary>
        public virtual void SetupDisplay()
        {
            activeCameras = new Dictionary<Igloo.EYE, Camera>();

            // Creates Camera objects and adds them to the activeCameras dictionary
            InitialiseCameras();

            IsFisheye = isFisheye;
            FisheyeStrength = fisheyeStrength;

            foreach (var cam in activeCameras)
            {
                if (!isOffAxis) cam.Value.fieldOfView = fov;
                cam.Value.enabled = isRendering;
                cam.Value.nearClipPlane = nearClipPlane;
                cam.Value.farClipPlane = farClipPlane;

#if IGLOO_URP
                // For URP - postprocessing is off by default
                var cameraData = cam.Value.GetUniversalAdditionalCameraData();
                cameraData.renderPostProcessing = true;
                cameraData.SetRenderer(0);
#endif
                if (cam.Key == EYE.LEFT)
                {
                    if (isRenderTextures)
                    {
                        leftTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0)
                        {
                            name = gameObject.name + "_" + cam.Key
                        };
                        cam.Value.targetTexture = leftTexture;
                    }
                }
                else if (cam.Key == EYE.CENTER)
                {
                    if (isRenderTextures)
                    {
                        centerTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0)
                        {
                            name = gameObject.name + "_" + cam.Key
                        };
                        cam.Value.targetTexture = centerTexture;
                    }
                }
                else if (cam.Key == EYE.RIGHT)
                {
                    if (isRenderTextures)
                    {
                        rightTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0)
                        {
                            name = gameObject.name + "_" + cam.Key
                        };
                        cam.Value.targetTexture = rightTexture;
                    }
                }
            }

            headManager.OnHeadSettingsChange += HeadSettingsChanges;
        }

        /// <summary>
        /// Updates the camera settings via the Head Manager when called.
        /// </summary>
        void HeadSettingsChanges()
        {
            foreach (var cam in activeCameras)
            {
                headManager.ApplyCameraSettings(cam.Value, cam.Key);
            }
        }

        /// <summary>
        /// Allows viewing of the Igloo Camera System within the Unity Development space
        /// </summary>
        void EditorDraw()
        {
            _ = transform.localToWorldMatrix;
            Gizmos.color = Color.white;
            Gizmos.DrawLine(UpperRight, UpperLeft);
            Gizmos.DrawLine(UpperLeft, LowerLeft);
            Gizmos.DrawLine(LowerLeft, LowerRight);
            Gizmos.DrawLine(LowerRight, UpperRight);
        }

        /// <summary>
        /// Mono: On draw Gizmo pass
        /// </summary>
        void OnDrawGizmos()
        {
            if (isOffAxis) EditorDraw();
        }

        /// <summary>
        /// Mono: On Draw Gizmo for Selected Item pass
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (isOffAxis)
            {
                var mat = transform.localToWorldMatrix;
                Vector3 right = mat * new Vector4(halfWidth() * 0.75f, 0.0f, 0.0f, 1.0f);
                Vector3 up = mat * new Vector4(0.0f, halfHeight() * 0.75f, 0.0f, 1.0f);
                Gizmos.color = new Color(0.75f, 0.25f, 0.25f);
                Gizmos.DrawLine((transform.position * 2.0f + right) / 3.0f, right);
                Gizmos.color = new Color(0.25f, 0.75f, 0.25f);
                Gizmos.DrawLine((transform.position * 2.0f + up) / 3.0f, up);
            }
        }

        /// <summary>
        /// Mono: On Component Destroyed Function
        /// </summary>
        /// <remarks>
        /// Destroys the active cameras, if there are any
        /// </remarks>
        public virtual void OnDestroy()
        {
            if (activeCameras != null)
            {
                foreach (var cam in activeCameras)
                {
                    if (cam.Value != null) DestroyImmediate(cam.Value.gameObject);
                }
            }

        }
    }
}

