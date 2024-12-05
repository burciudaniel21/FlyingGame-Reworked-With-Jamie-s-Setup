using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Igloo.Common
{
#pragma warning disable IDE0090 // Use New()...
#pragma warning disable IDE0044 // Add readonly modifier

    /// <summary>
    /// Creates and manages Display objects also allowing the following optional functionality 
    /// Compositing - multiple camera render textures can be combined into a single texture
    /// Cubemap conversion - resulting textures can be processed to perform cubemap-equirectangular conversion
    /// Sharing - resulting textures can be shared via the TextureShareUtility
    /// </summary>
    [ExecuteInEditMode]
    public class DisplayManager : MonoBehaviour
    {
        /// <summary>
        /// The Igloo Head manager class
        /// </summary>
        public HeadManager headManager;

        /// <summary>
        /// The camera prefab used to as the base for the Igloo cameras.
        /// </summary>
        public GameObject cameraPrefab;

        /// <summary>
        /// List of the active Igloo Displays
        /// </summary>
        [SerializeField] public List<Display> displays;

        /// <summary>
        /// The name for sharing via Spout and NDI
        /// </summary>
        public string sharingName = "IglooUnity";

        /// <summary>
        /// Render texture for left eye output
        /// </summary>
        private RenderTexture outputTextureLeft = null;

        /// <summary>
        /// Render texture for standard igloo output
        /// </summary>
        public RenderTexture outputTextureCenter = null;

        /// <summary>
        /// Render texture for right eye output
        /// </summary>
        private RenderTexture outputTextureRight = null;

        /// <summary>
        /// Render texture for top/bottom 3D
        /// </summary>
        public RenderTexture topBottom3DTexture = null;

        /// <summary>
        /// If True, Output via a single texture via Spout or NDI
        /// </summary>
        public bool useCompositeTexture = false;

        /// <summary>
        /// If True, Top / Bottom 3D will be packed into a single rendertexture
        /// </summary>
        public bool useFramepackTopBottom3D = false;

        /// <summary>
        /// Material used to blit framepack 3D together.
        /// </summary>
        private Material topBottomMaterial = null;

        /// <summary>
        /// Igloo Texture Share mode. 
        /// 0 - None
        /// 1 - Spout
        /// 2 - NDI
        /// </summary>
        public TextureShareUtility.TextureShareMode textureShareMode = TextureShareUtility.TextureShareMode.NONE;

        /// <summary>
        /// If True, use Igloo Warp & Blend
        /// </summary>
        private bool useWarpBlend = false;

        /// <summary>
        /// The system path for the warper data
        /// </summary>
        /// <remarks>
        /// Looks in the Igloo Warper data folder for the Igloo Warps.
        /// </remarks>
        private string warperDataPath;

        /// <summary>
        /// The Igloo Warp Blend Manager class
        /// </summary>
        public WarpBlendManager warpBlendManager;

        #region Cubemap To Equirectangular
        /// <summary>
        /// If True, Create a cubemap, and blit to an equirectangular before output
        /// to Igloo system.
        /// </summary>
        /// <remarks>
        /// This can now be achieved by the warper, with little overhead. It may be prudent
        /// in the future to send a cubemap and allow the warper to convert it. 
        /// </remarks>
        public bool useCubemapToEqui = false;

        public RenderTexture cubemapToEquiOutputCenter = null;
        public RenderTexture cubemapToEquiOutputLeft = null;
        public RenderTexture cubemapToEquiOutputRight = null;

        public bool useTruePerspective = false;

        public Texture2D warpTex;
        public Material truePerspectiveMatCenter;
        public Material truePerspectiveMatLeft;
        public Material truePerspectiveMatRight;

        /// <summary>
        /// The resolution of the equirectangular texture
        /// </summary>
        private Vector2Int equirectangularTexuteRes;

        /// <summary>
        /// Material used to blit the left eye of the cubemap
        /// </summary>
        private Material cubeToEquiMatLeft = null;

        /// <summary>
        /// Material used to blit the main cubemap
        /// </summary>
        private Material cubeToEquiMatCenter = null;

        /// <summary>
        /// Material used to blit the right eye of the cubemap
        /// </summary>
        private Material cubeToEquiMatRight = null;

        /// <summary>
        /// Horizontal Field of View to render
        /// </summary>
        private float horizontalFOV = 360f;

        /// <summary>
        /// Vertical field of view to render
        /// </summary>
        private float verticalFOV = 70f;
        #endregion

        /// <summary>
        /// Setup Display manager and create Display object using settings 
        /// </summary>
        /// <param name="ds"></param>
        public void Setup(DisplaySettings ds)
        {
            if (ds.Name != null) sharingName = ds.Name;
            useCompositeTexture = ds.useCompositeTexture;
            useCubemapToEqui = ds.useCubemapToEquirectangular;
            horizontalFOV = ds.horizontalFOV;
            verticalFOV = ds.verticalFOV;
            useFramepackTopBottom3D = ds.useFramepackTopBottom3D;
            useWarpBlend = ds.useWarpBlend;
            useTruePerspective = ds.useTruePerspective;
            textureShareMode = (TextureShareUtility.TextureShareMode)ds.textureShareMode;
            if (ds.equirectangularTextureResolution != null) equirectangularTexuteRes = ds.equirectangularTextureResolution.Vector2Int;
            if (ds.equirectangularTexuteRes != null) equirectangularTexuteRes = ds.equirectangularTexuteRes.Vector2Int;
            if (headManager == null)
            {
                GameObject go = new GameObject("Head");
                if (this.gameObject.transform.parent != null) go.transform.parent = this.gameObject.transform;
                headManager = go.AddComponent<HeadManager>();
            }

            if (ds.HeadSettings != null) headManager.SetSettings(ds.HeadSettings);

            if (ds.useWarpBlend)
            {
                if (!GetComponent<WarpBlendManager>()) gameObject.AddComponent<WarpBlendManager>();
                warpBlendManager = GetComponent<WarpBlendManager>();
                warpBlendManager.SetSettings(ds.WarpBlendSettings);
            }


            CreateDisplays(ds.Displays);
            if (useCubemapToEqui) SetupCubemapToEqui();
            else if (useCompositeTexture) SetupComposition();
            if (useTruePerspective) SetupTruePerspective();

            SetupTextureSharing();
        }

        /// <summary>
        /// Returns DisplaySettings, used by serialiser for saving
        /// </summary>
        /// <returns></returns>
        public DisplaySettings GetSettings()
        {
            DisplaySettings settings = new DisplaySettings
            {
                Name = sharingName,
                useCompositeTexture = useCompositeTexture,
                useCubemapToEquirectangular = useCubemapToEqui,
                horizontalFOV = horizontalFOV,
                verticalFOV = verticalFOV,
                useFramepackTopBottom3D = useFramepackTopBottom3D,
                textureShareMode = (int)textureShareMode,
                equirectangularTextureResolution = new Vector2IntItem(equirectangularTexuteRes),
                equirectangularTexuteRes = new Vector2IntItem(equirectangularTexuteRes),
                useWarpBlend = useWarpBlend
            };

            if (useWarpBlend && warpBlendManager != null)
            {
                WarpBlendSettings warpBlendSettings = new WarpBlendSettings();
                warpBlendSettings = warpBlendManager.GetSettings();
                settings.WarpBlendSettings = warpBlendSettings;
            }
            if (headManager != null) settings.HeadSettings = headManager.GetSettings();

            DisplayItem[] displayItems = new DisplayItem[displays.Count];
            for (int i = 0; i < displays.Count; i++)
            {
                displayItems[i] = displays[i].GetSettings();
            }
            settings.Displays = displayItems;
            return settings;
        }

        /// <summary>
        /// Removes current Dispays and creates new Displays based on 
        /// DisplayItem array
        /// </summary>
        /// <param name="displayItems"></param>
        public void CreateDisplays(DisplayItem[] displayItems)
        {
            RemoveDisplays();

            //displays = new List<Display>();

            for (int i = 0; i < displayItems.Length; i++)
            {
                //GameObject displayObj = new GameObject(sharingName+ (i+1).ToString());
                GameObject displayObj = new GameObject("Display: " + sharingName + (i + 1).ToString());
                displayObj.transform.parent = this.transform;
                VirtualDisplay vd = displayObj.AddComponent<VirtualDisplay>();
                vd.displayManager = this;
                vd.headManager = headManager;
                if (cameraPrefab) vd.cameraPrefab = cameraPrefab;
                if (useCompositeTexture) displayItems[i].isRenderTextures = false;
                if (useCubemapToEqui) displayItems[i].isFisheye = false;

                // If Display Name is not set then use DisplaySettings Name + index
                if (string.IsNullOrEmpty(displayItems[i].Name)) displayItems[i].Name = sharingName + (i + 1).ToString();
                vd.SetSettings(displayItems[i]);
                vd.SetupDisplay();
                displays.Add(vd);
            }
        }


        /// <summary>
        /// Destroys all Display objects
        /// </summary>
        public void RemoveDisplays()
        {
            if (displays != null)
            {
                foreach (Display display in displays)
                {
                    if (display != null) DestroyImmediate(display.gameObject);
                }
                displays.Clear();
            }
        }

        /// <summary>
        /// Sets the enabled state for all display cameras
        /// </summary>
        /// <param name="state">If True, displays are active</param>
        public void SetDisplaysEnabled(bool state)
        {
            displays.ForEach((t) =>
            {
                t.SetRendering = state;
            });
        }


        /// <summary>
        /// Sets near clipping plane distance for all cameras
        /// and will also save the settings
        /// </summary>
        /// <param name="distance">near clip distance</param>
        public void SetNearClip(float distance)
        {
            headManager.NearClipPlane = distance;
        }

        /// <summary>
        /// Sets far clipping plane distance for all cameras
        /// and will also save the settings
        /// </summary>
        /// <param name="distance">Far clip distance</param>
        public void SetFarClip(float distance)
        {
            headManager.FarClipPlane = distance;
        }

        /// <summary>
        /// Sets the eye seperation for all active cameras
        /// </summary>
        /// <param name="distance">Distance between eyes</param>
        public void SetEyeSeparation(float distance)
        {
            headManager.SetEyeSeparation(distance);
        }

        /// <summary>
        /// Sets the Horizontal Field of View for all display types
        /// Gets the current Horizontal Field of View
        /// </summary>
        public float HorizontalFOV
        {
            get { return horizontalFOV; }
            set
            {
                if (value < 1 || value > 360) return;
                horizontalFOV = value;
                if (cubeToEquiMatLeft) cubeToEquiMatLeft.SetFloat("_HorizontalFov", horizontalFOV * Mathf.Deg2Rad);
                if (cubeToEquiMatCenter) cubeToEquiMatCenter.SetFloat("_HorizontalFov", horizontalFOV * Mathf.Deg2Rad);
                if (cubeToEquiMatRight) cubeToEquiMatRight.SetFloat("_HorizontalFov", horizontalFOV * Mathf.Deg2Rad);
            }
        }

        /// <summary>
        /// Sets the Vertical Field of View for all display types
        /// Gets the current vertical field of view
        /// </summary>
        public float VerticalFOV
        {
            get { return verticalFOV; }
            set
            {
                if (value < 1 || value > 180) return;
                verticalFOV = value;
                if (cubeToEquiMatLeft) cubeToEquiMatLeft.SetFloat("_VerticalFov", verticalFOV * Mathf.Deg2Rad);
                if (cubeToEquiMatCenter) cubeToEquiMatCenter.SetFloat("_VerticalFov", verticalFOV * Mathf.Deg2Rad);
                if (cubeToEquiMatRight) cubeToEquiMatRight.SetFloat("_VerticalFov", verticalFOV * Mathf.Deg2Rad);
            }
        }

        /// <summary>
        /// Gets all cameras from all displays. Useful for modifying camera properties
        /// </summary>
        /// <returns>List of cameras</returns>
        public List<Camera> GetCameras()
        {
            List<Camera> cameras = new List<Camera>();
            displays.ForEach((t) =>
            {
                if (t != null)
                {
                    cameras.AddRange(t.GetCameras());
                }
            });
            return cameras;
        }

        /// <summary>
        /// Mono. On Awake function
        /// </summary>
        private void Awake()
        {
            if (Application.isPlaying && transform.parent == null && IglooManager.instance) DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Setup materials for shader pass
        /// </summary>
        private void SetupCubemapToEqui()
        {
            if (cubeToEquiMatLeft) DestroyImmediate(cubeToEquiMatLeft);
            if (cubeToEquiMatCenter) DestroyImmediate(cubeToEquiMatCenter);
            if (cubeToEquiMatRight) DestroyImmediate(cubeToEquiMatRight);

            for (int i = 0; i < displays.Count; i++)
            {
                Dictionary<Igloo.EYE, Camera> cams = displays[i].GetActiveCameras();
                foreach (var cam in cams)
                {
                    if (cam.Key == Igloo.EYE.LEFT)
                    {
                        if (cubeToEquiMatLeft == null)
                        {
                            if (useTruePerspective)
                            {
                                cubemapToEquiOutputLeft = new RenderTexture(equirectangularTexuteRes.x, equirectangularTexuteRes.y, 0)
                                {
                                    name = "truePerspectvieLeftIn",
                                    wrapMode = TextureWrapMode.Repeat
                                };

                            }
                            else
                            {
                                outputTextureLeft = new RenderTexture(equirectangularTexuteRes.x, equirectangularTexuteRes.y, 0)
                                {
                                    name = "outputTextureLeft",
                                    wrapMode = TextureWrapMode.Repeat
                                };
                            }
                            cubeToEquiMatLeft = new Material(Shader.Find("Hidden/Igloo/Equirectangular"));
                            cubeToEquiMatLeft.SetFloat("_HorizontalFov", horizontalFOV * Mathf.Deg2Rad);
                            cubeToEquiMatLeft.SetFloat("_VerticalFov", verticalFOV * Mathf.Deg2Rad);
                        }
                        cubeToEquiMatLeft.SetTexture(GetFaceName((int)displays[i].iglooCubemapFace), displays[i].leftTexture);
                    }
                    else if (cam.Key == Igloo.EYE.CENTER)
                    {
                        if (cubeToEquiMatCenter == null)
                        {
                            if (useTruePerspective)
                            {
                                cubemapToEquiOutputCenter = new RenderTexture(equirectangularTexuteRes.x, equirectangularTexuteRes.y, 0)
                                {
                                    name = "truePerspectvieCenterIn",
                                    wrapMode = TextureWrapMode.Repeat
                                };

                            }
                            else {
                                outputTextureCenter = new RenderTexture(equirectangularTexuteRes.x, equirectangularTexuteRes.y, 0)
                                {
                                    name = "outputTextureCenter",
                                    wrapMode = TextureWrapMode.Repeat
                                };
                            }

                            cubeToEquiMatCenter = new Material(Shader.Find("Hidden/Igloo/Equirectangular"));
                            cubeToEquiMatCenter.SetFloat("_HorizontalFov", horizontalFOV * Mathf.Deg2Rad);
                            cubeToEquiMatCenter.SetFloat("_VerticalFov", verticalFOV * Mathf.Deg2Rad);
                        }
                        cubeToEquiMatCenter.SetTexture(GetFaceName((int)displays[i].iglooCubemapFace), displays[i].centerTexture);
                    }
                    else if (cam.Key == Igloo.EYE.RIGHT)
                    {
                        if (cubeToEquiMatRight == null)
                        {
                            if (useTruePerspective)
                            {
                                cubemapToEquiOutputRight = new RenderTexture(equirectangularTexuteRes.x, equirectangularTexuteRes.y, 0)
                                {
                                    name = "truePerspectvieRightIn",
                                    wrapMode = TextureWrapMode.Repeat
                                };
                            }
                            else
                            {
                                outputTextureRight = new RenderTexture(equirectangularTexuteRes.x, equirectangularTexuteRes.y, 0)
                                {
                                    name = "outputTextureRight",
                                    wrapMode = TextureWrapMode.Repeat
                                };
                            }
                            cubeToEquiMatRight = new Material(Shader.Find("Hidden/Igloo/Equirectangular"));
                            cubeToEquiMatRight.SetFloat("_HorizontalFov", horizontalFOV * Mathf.Deg2Rad);
                            cubeToEquiMatRight.SetFloat("_VerticalFov", verticalFOV * Mathf.Deg2Rad);
                        }
                        cubeToEquiMatRight.SetTexture(GetFaceName((int)displays[i].iglooCubemapFace), displays[i].rightTexture);
                    }
                }
            }
            if (useFramepackTopBottom3D && outputTextureLeft != null && outputTextureRight != null)
            {
                int totalWidth = 0;
                int totalHeight = 0;
                totalWidth = outputTextureLeft.width > outputTextureRight.width ? outputTextureLeft.width : outputTextureRight.width;
                totalHeight = outputTextureLeft.height + outputTextureRight.height;

                topBottom3DTexture = new RenderTexture(totalWidth, totalHeight, 0)
                {
                    name = "topBottomSharingTexture",
                    wrapMode = TextureWrapMode.Repeat
                };
                topBottomMaterial = new Material(Shader.Find("Hidden/Igloo/CombineTopBottom"));
                topBottomMaterial.SetTexture("Texture1", outputTextureLeft);
                topBottomMaterial.SetTexture("Texture2", outputTextureRight);
            }
        }

        private void SetupTruePerspective()
        {
            string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string warpPath = Path.Combine(programDataPath, "Igloo Vision\\IglooCoreEngine\\settings\\TruePerspectiveMap32.bmp");
            int tpWidth = 0;
            int tpHeight = 0;

            using (FileStream fs = new FileStream(warpPath, FileMode.Open, FileAccess.Read))
            {
                byte[] header = new byte[54];
                fs.Read(header, 0, 54);
                tpWidth = BitConverter.ToInt32(header, 18);
                tpHeight = BitConverter.ToInt32(header, 22);
            }

            warpTex = Utils.LoadFloatTexture(warpPath, tpWidth, tpHeight, 54);

            if (warpTex != null) {
                for (int i = 0; i < displays.Count; i++)
                {
                    Dictionary<Igloo.EYE, Camera> cams = displays[i].GetActiveCameras();
                    foreach (var cam in cams)
                    {
                        if (cam.Key == Igloo.EYE.LEFT)
                        {
                            truePerspectiveMatLeft = new Material(Shader.Find("Igloo/TruePerspective"));
                            truePerspectiveMatLeft.SetTexture("_WarpTex", warpTex);
                            outputTextureLeft = new RenderTexture(tpWidth, tpHeight, 0)
                            {
                                name = "outputTextureLeft",
                                wrapMode = TextureWrapMode.Repeat
                            };
                        }
                        else if (cam.Key == Igloo.EYE.CENTER)
                        {
                            truePerspectiveMatCenter = new Material(Shader.Find("Igloo/TruePerspective"));
                            truePerspectiveMatCenter.SetTexture("_WarpTex", warpTex);
                            outputTextureCenter = new RenderTexture(tpWidth, tpHeight, 0)
                            {
                                name = "outputTextureCenter",
                                wrapMode = TextureWrapMode.Repeat
                            };
                        }
                        else if (cam.Key == Igloo.EYE.RIGHT)
                        {
                            truePerspectiveMatRight = new Material(Shader.Find("Igloo/TruePerspective"));
                            truePerspectiveMatRight.SetTexture("_WarpTex", warpTex);
                            outputTextureRight = new RenderTexture(tpWidth, tpHeight, 0)
                            {
                                name = "outputTextureRight",
                                wrapMode = TextureWrapMode.Repeat
                            };
                        }
                    }
                }
            } 
            else
            {
                UnityEngine.Debug.LogError("<b>[Igloo]</b> Failed to load Warp texture from " + warpPath);
            }
        }

        /// <summary>
        /// Returns the name of the cubemap face ID
        /// </summary>
        /// <param name="i">Cubemap Face ID</param>
        /// <returns>String, cubemap Face name</returns>
        private string GetFaceName(int i)
        {
            string name = "null";
            if (i == 0) name = "_FaceTexPZ";
            else if (i == 1) name = "_FaceTexPX";
            else if (i == 2) name = "_FaceTexNZ";
            else if (i == 3) name = "_FaceTexNX";
            else if (i == 4) name = "_FaceTexNY";
            else if (i == 5) name = "_FaceTexPY";
            return name;
        }

        /// <summary>
        /// Sets up the displays, cameras, and texture output method. 
        /// </summary>
        private void SetupComposition()
        {
            int totalWidth = 0;
            int totalHeight = 0;
            // Calculate the resoltion required for the compsition texture
            foreach (Display display in displays)
            {
                totalWidth += display.RenderTextureSize.x;
                if (display.RenderTextureSize.y > totalHeight) totalHeight = display.RenderTextureSize.y;
            }
            Debug.Log("<b>[Igloo]</b> SetupComposite: combined texture size: " + totalWidth + " , " + totalHeight);

            if (totalWidth < 1 || totalHeight < 1) return;

            float currentX = 0;
            float currentY = 0;
            for (int i = 0; i < displays.Count; i++)
            {
                float w = displays[i].RenderTextureSize.x / (float)totalWidth;
                float h = displays[i].RenderTextureSize.y / (float)totalHeight;
                float x = currentX;
                float y = currentY;

                displays[i].ViewportRect = new Rect(x, y, w, h);

                // Create composition textures and assing them as appropriate camera render targets
                Dictionary<Igloo.EYE, Camera> cams = displays[i].GetActiveCameras();
                foreach (var cam in cams)
                {
                    if (cam.Key == Igloo.EYE.LEFT)
                    {
                        if (outputTextureLeft == null)
                        {
                            outputTextureLeft = new RenderTexture(totalWidth, totalHeight, 0)
                            {
                                name = "outputTextureLeft",
                                wrapMode = TextureWrapMode.Repeat
                            };
                        }
                        cam.Value.targetTexture = outputTextureLeft;
                    }
                    else if (cam.Key == Igloo.EYE.CENTER)
                    {
                        if (outputTextureCenter == null)
                        {
                            outputTextureCenter = new RenderTexture(totalWidth, totalHeight, 0)
                            {
                                name = "outputTextureCenter",
                                wrapMode = TextureWrapMode.Repeat
                            };
                        }
                        cam.Value.targetTexture = outputTextureCenter;
                    }
                    else if (cam.Key == Igloo.EYE.RIGHT)
                    {
                        if (outputTextureRight == null)
                        {
                            outputTextureRight = new RenderTexture(totalWidth, totalHeight, 0)
                            {
                                name = "outputTextureRight",
                                wrapMode = TextureWrapMode.Repeat
                            };
                        }
                        cam.Value.targetTexture = outputTextureRight;
                    }
                }
                currentX += w;
            }

            if (useFramepackTopBottom3D && outputTextureLeft != null && outputTextureRight != null)
            {
                topBottom3DTexture = new RenderTexture(totalWidth, totalHeight * 2, 0)
                {
                    name = "topBottomSharingTexture"
                };
                topBottomMaterial = new Material(Shader.Find("Hidden/Igloo/CombineTopBottom"));
                topBottomMaterial.SetTexture("Texture1", outputTextureLeft);
                topBottomMaterial.SetTexture("Texture2", outputTextureRight);
            }
        }

        /// <summary>
        /// Removes all texture sharing methods from the display.
        /// Adds the new texture sharing method, based on the on the type of display method 
        /// </summary>
        private void SetupTextureSharing()
        {
            TextureShareUtility.RemoveAllSendersFromObject(this.gameObject);

            if (topBottom3DTexture != null)
            {
                TextureShareUtility.AddTextureSender(textureShareMode, this.gameObject, sharingName, ref topBottom3DTexture);
            }
            else
            {
                if (useWarpBlend) warpBlendManager.enabled = true;

                if (outputTextureLeft != null)
                {
                    TextureShareUtility.AddTextureSender(textureShareMode, this.gameObject, sharingName + "_Left", ref outputTextureLeft);
                    if (useWarpBlend) warpBlendManager.canvasLeft = outputTextureLeft;
                }
                if (outputTextureCenter != null)
                {
                    TextureShareUtility.AddTextureSender(textureShareMode, this.gameObject, sharingName, ref outputTextureCenter);
                    if (useWarpBlend) warpBlendManager.canvasLeft = outputTextureCenter;
                }
                if (outputTextureRight != null)
                {
                    TextureShareUtility.AddTextureSender(textureShareMode, this.gameObject, sharingName + "_Right", ref outputTextureRight);
                    if (useWarpBlend) warpBlendManager.canvasRight = outputTextureRight;
                }
                // Enable warping and blending
                if (useWarpBlend)
                {
                    warpBlendManager.enabled = true;
                    warpBlendManager.canvasCentre = outputTextureCenter;
                    warpBlendManager.Setup();
                }
            }
        }

        /// <summary>
        /// Mono. Late update function
        /// Graphics processes for each output method
        /// </summary>
        private void LateUpdate()
        {
            if (useCubemapToEqui)
            {
                if (useTruePerspective)
                {                  
                    // Cubemap to Equirectangual
                    if (cubeToEquiMatLeft != null && cubemapToEquiOutputLeft != null) Graphics.Blit(null, outputTextureLeft, cubeToEquiMatLeft);
                    if (cubeToEquiMatCenter != null && cubemapToEquiOutputCenter != null) Graphics.Blit(null, cubemapToEquiOutputCenter, cubeToEquiMatCenter);
                    if (cubeToEquiMatRight != null && cubemapToEquiOutputRight != null) Graphics.Blit(null, outputTextureRight, cubeToEquiMatRight);
                    
                    // True Perspective
                    if (truePerspectiveMatLeft != null && cubemapToEquiOutputLeft != null) Graphics.Blit(cubemapToEquiOutputLeft, outputTextureLeft, truePerspectiveMatLeft);
                    if (truePerspectiveMatCenter != null && cubemapToEquiOutputCenter != null) Graphics.Blit(cubemapToEquiOutputCenter, outputTextureCenter, truePerspectiveMatCenter);
                    if (truePerspectiveMatRight != null && cubemapToEquiOutputRight != null) Graphics.Blit(cubemapToEquiOutputRight, outputTextureRight, truePerspectiveMatRight);
                }
                else {
                    if (cubeToEquiMatLeft != null && outputTextureLeft != null) Graphics.Blit(null, outputTextureLeft, cubeToEquiMatLeft);
                    if (cubeToEquiMatCenter != null && outputTextureCenter != null) Graphics.Blit(null, outputTextureCenter, cubeToEquiMatCenter);
                    if (cubeToEquiMatRight != null && outputTextureRight != null) Graphics.Blit(null, outputTextureRight, cubeToEquiMatRight);
                }
            }


            if (useFramepackTopBottom3D)
            {
                if (topBottom3DTexture != null && outputTextureLeft != null && outputTextureRight != null)
                {
                    Graphics.CopyTexture(outputTextureRight, 0, 0, 0, 0, outputTextureRight.width, outputTextureRight.height, topBottom3DTexture, 0, 0, 0, 0);
                    Graphics.CopyTexture(outputTextureLeft, 0, 0, 0, 0, outputTextureLeft.width, outputTextureLeft.height, topBottom3DTexture, 0, 0, 0, outputTextureRight.height);
                }
            }
        }

        /// <summary>
        /// Mono. On object destroyed
        /// Remove all displays, and destroy the head manager object.
        /// </summary>
        private void OnDestroy()
        {
            RemoveDisplays();
            if (headManager != null) DestroyImmediate(headManager.gameObject);
        }

    }
}

