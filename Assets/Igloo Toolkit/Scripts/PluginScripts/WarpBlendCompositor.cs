using System.Collections.Generic;

using UnityEngine;
#if IGLOO_URP
using UnityEngine.Rendering;
using System.Reflection;
using UnityEngine.Rendering.Universal;
#endif
namespace Igloo.Common
{
#pragma warning disable IDE0051 // Remove unused private members

    /// <summary>
    /// Composits all the warp and blend outputs that have the same target display.
    /// One intance of WarpBlendCompositor correlates to one output display. 
    /// There should never be more than one per display.
    /// </summary>
    [System.Serializable]
    public class WarpBlendCompositor : MonoBehaviour
    {
        public Camera displayCamera;
        public List<WarpBlend> warpBlends;
        public RenderTexture canvasCentre;
        public RenderTexture canvasLeft;
        public RenderTexture canvasRight;

        public RenderTexture outputTexture = null;
        public GameObject renderQuad = null;
#if IGLOO_URP
        public Material outputMaterial;
#endif

        public enum RenderingMode { TEXTURE, CAMERA }
        public RenderingMode renderingMode = RenderingMode.TEXTURE;
        public void AddWarpBlend(WarpBlend warpBlend)
        {
            warpBlends.Add(warpBlend);
            warpBlend.gameObject.name = "[" + warpBlends.Count.ToString() + "] " + warpBlend.gameObject.name;
            warpBlend.gameObject.transform.parent = this.transform;
        }
        public void ClearWarpBlends() { warpBlends.Clear(); }
        public void SetTargetDisplay(int index) { if (displayCamera != null) { displayCamera.targetDisplay = index; } targetDisplay = index; }
        public int GetTargetDisplay()
        {
            if (displayCamera != null) { return displayCamera.targetDisplay; }
            else
            {
                Debug.LogWarning("<b>[Igloo]</b> WarpBlendCompositor: Camera missing on game object. Can't get Target Display");
                return 0;
            }
        }
        private int targetDisplay = 0;



        public WarpBlendCompositor(int display)
        {
            targetDisplay = display;
            warpBlends = new List<WarpBlend>();
        }
        public WarpBlendCompositor()
        {
            warpBlends = new List<WarpBlend>();
        }
        public void OnEnable()
        {
            if (displayCamera == null) CreateCamera(targetDisplay);

#if IGLOO_URP
            RenderPipelineManager.beginCameraRendering += BeginCameraRender;
            var _pipelineAssetCurrent = Camera.main.GetUniversalAdditionalCameraData().scriptableRenderer;
            var features = _pipelineAssetCurrent.GetType().GetProperty("rendererFeatures", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(_pipelineAssetCurrent, null) as List<ScriptableRendererFeature>;
            BlitMaterialFeature BlitMatFeature = features.Find(x => x.name == "NewBlitMaterialFeature") as BlitMaterialFeature;

            if (BlitMatFeature == null) Debug.LogError("No Blit Material Feature in URP Forward Renderer Render Features");
            else
            {
                outputMaterial = BlitMatFeature.Material;
                if (outputMaterial == null) Debug.LogError("Output Material is not set in URP Forward Renderer Feature: Blit Material Feature ");
            }
#endif
        }

        private void CreateCamera(int targetDisplay = 0)
        {

            displayCamera = gameObject.GetComponent<Camera>();
            if (displayCamera == null)
                displayCamera = gameObject.AddComponent<Camera>();
            displayCamera.depth = 20;
            displayCamera.targetDisplay = targetDisplay;

#if IGLOO_URP
            // TODO: Use custom post processor effect once available
            // - https://portal.productboard.com/8ufdwj59ehtmsvxenjumxo82/c/37-post-processing-custom-effects
            // For URP - postprocessing is off by default
            switch (renderingMode)
            {
                case RenderingMode.CAMERA:
                    displayCamera.tag = "WarpBlendOutputCamera";
                    break;

                case RenderingMode.TEXTURE:
                    var cameraData = displayCamera.GetUniversalAdditionalCameraData();

                    // Should be set to a default render with no postprocessing etc  
                    cameraData.SetRenderer(0);

                    displayCamera.orthographic = true;
                    displayCamera.cullingMask = LayerMask.GetMask("IglooCompositeQuad");
                    displayCamera.farClipPlane = 5.0f;
                    renderQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    renderQuad.layer = LayerMask.NameToLayer("IglooCompositeQuad");

                    renderQuad.gameObject.transform.parent = this.transform;
                    renderQuad.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                    UpdateRenderQuadTransform();
                    break;
            }
#endif

#if IGLOO_HDRP
            displayCamera.orthographic = true;
            displayCamera.cullingMask = LayerMask.GetMask("IglooCompositeQuad");
            displayCamera.farClipPlane = 5.0f;
            renderQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            renderQuad.layer = LayerMask.NameToLayer("IglooCompositeQuad");

            renderQuad.gameObject.transform.parent = this.transform;
            renderQuad.GetComponent<MeshRenderer>().material = new Material(Shader.Find("HDRP/Unlit"));
            UpdateRenderQuadTransform();
#endif
        }

#if IGLOO_URP
        void BeginCameraRender(ScriptableRenderContext context, Camera camera) {

            if (camera.name == displayCamera.name) {
                switch (renderingMode) {
                    case RenderingMode.CAMERA:
                        if (outputTexture == null)
                        {
                            Debug.Log("<b>[Igloo]</b> Output texture is null, creating one now");
                            outputTexture = new RenderTexture(displayCamera.pixelWidth, displayCamera.pixelHeight, 8);
                            Debug.Assert(outputTexture.Create(), "Failed to create camera warp and blend texture");
                            outputMaterial.SetTexture("_BaseMap", outputTexture);
                        }
                        if (outputTexture.width != displayCamera.pixelWidth || outputTexture.height != displayCamera.pixelHeight)
                        {
                            Debug.Log("<b>[Igloo]</b> Output texture resolution " + outputTexture.width + " x " + outputTexture.height + " , doesn't match screen res " + displayCamera.pixelWidth + " x " + displayCamera.pixelHeight);
                            outputTexture.Release();
                            outputTexture = new RenderTexture(displayCamera.pixelWidth, displayCamera.pixelHeight, 8);
                            Debug.Assert(outputTexture.Create(), "Failed to create camera warp and blend texture");
                            outputMaterial.SetTexture("_BaseMap", outputTexture);
                        }

                        if (outputTexture != null)
                        {
                            for (int i = 0; i < warpBlends.Count; i++)
                            {
                                if (warpBlends[i].debugTex != null) Graphics.Blit(warpBlends[i].debugTex, outputTexture, warpBlends[i].warpMat);
                                Graphics.Blit(canvasCentre, outputTexture, warpBlends[i].warpMat);
                            }
                        }

                        break;
                    case RenderingMode.TEXTURE:
                        if (renderQuad != null)
                        {
                            if (outputTexture == null)
                            {
                                Debug.Log("<b>[Igloo]</b> Output texture is null, creating one now");
                                outputTexture = new RenderTexture(displayCamera.pixelWidth, displayCamera.pixelHeight, 0);
                                UpdateRenderQuadTransform();
                                renderQuad.GetComponent<Renderer>().material.SetTexture("_BaseMap", outputTexture);
                            }
                            if (outputTexture.width != displayCamera.pixelWidth || outputTexture.height != displayCamera.pixelHeight)
                            {
                                Debug.Log("<b>[Igloo]</b> Output texture resolution " + outputTexture.width + " x " + outputTexture.height + " , doesn't match screen res " + displayCamera.pixelWidth + " x " + displayCamera.pixelHeight);
                                outputTexture.Release();
                                outputTexture = new RenderTexture(displayCamera.pixelWidth, displayCamera.pixelHeight, 0);
                                renderQuad.GetComponent<Renderer>().material.SetTexture("_BaseMap", outputTexture);

                                UpdateRenderQuadTransform();
                            }

                            if (outputTexture != null)
                            {
                                for (int i = 0; i < warpBlends.Count; i++)
                                {
                                    if (warpBlends[i].debugTex != null) Graphics.Blit(warpBlends[i].debugTex, outputTexture, warpBlends[i].warpMat);
                                    Graphics.Blit(canvasCentre, outputTexture, warpBlends[i].warpMat);
                                }
                            }
                        }
                        break;
                    default:
                        break;

                }
            }
        }
#endif



        void Update()
        {
            RenderHRDP();
        }


        private void RenderHRDP()
        {
#if IGLOO_HDRP
            switch (renderingMode) {
                case RenderingMode.CAMERA:
                    throw new System.NotImplementedException("<b>[Igloo]</b> Camera mode is currently not supported with HDRP, Please change to Texture rendering mode");
            case RenderingMode.TEXTURE:
                    if (outputTexture == null) {
                        Debug.Log("<b>[Igloo]</b> Output texture is null, creating one now");
                        outputTexture = new RenderTexture(displayCamera.pixelWidth, displayCamera.pixelHeight, 0);
                        UpdateRenderQuadTransform();
                        renderQuad.GetComponent<Renderer>().material.SetTexture("_UnlitColorMap", outputTexture);
                    }
                    if (outputTexture.width != displayCamera.pixelWidth || outputTexture.height != displayCamera.pixelHeight) {
                        Debug.Log("<b>[Igloo]</b> Output texture resolution " + outputTexture.width + " x " + outputTexture.height + " , doesn't match screen res " + displayCamera.pixelWidth + " x " + displayCamera.pixelHeight);
                        outputTexture.Release();
                        outputTexture = new RenderTexture(displayCamera.pixelWidth, displayCamera.pixelHeight, 0);
                        renderQuad.GetComponent<Renderer>().material.SetTexture("_UnlitColorMap", outputTexture);

                        UpdateRenderQuadTransform();
                    }
                    if (outputTexture != null) {
                        for (int i = 0; i < warpBlends.Count; i++) {
                            if (warpBlends[i].debugTex != null) Graphics.Blit(warpBlends[i].debugTex, outputTexture, warpBlends[i].warpMat);
                            Graphics.Blit(canvasCentre, outputTexture, warpBlends[i].warpMat);
                        }
                    }
                    break;
                default:
                    break;

            }
#endif
        }

        void UpdateRenderQuadTransform()
        {
            if (renderQuad != null)
            {
                renderQuad.transform.localPosition = new Vector3(0, 0, displayCamera.nearClipPlane + (displayCamera.farClipPlane - displayCamera.nearClipPlane) / 2);
                renderQuad.transform.localEulerAngles = new Vector3(0, 0, 0);
                renderQuad.transform.localScale = new Vector3(displayCamera.orthographicSize * 2 * ((float)displayCamera.pixelWidth / displayCamera.pixelHeight), displayCamera.orthographicSize * 2, 1);
            }

        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            switch (renderingMode)
            {
                case RenderingMode.CAMERA:
                    for (int i = 0; i < warpBlends.Count; i++)
                    {
                        Graphics.Blit(source, destination, warpBlends[i].warpMat);
                    }
                    break;
                case RenderingMode.TEXTURE:
                    for (int i = 0; i < warpBlends.Count; i++)
                    {
                        if (warpBlends[i].debugTex != null) Graphics.Blit(warpBlends[i].debugTex, destination, warpBlends[i].warpMat);
                        else Graphics.Blit(canvasCentre, destination, warpBlends[i].warpMat);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnDisable()
        {
#if IGLOO_URP
            RenderPipelineManager.beginCameraRendering -= BeginCameraRender;
#endif
        }
    }
}
