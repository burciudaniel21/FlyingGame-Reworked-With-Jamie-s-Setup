using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Igloo.Common
{
#pragma warning disable IDE0090 // Use New()...
#pragma warning disable IDE0044 // Add readonly modifier

    public class DisplaySender : MonoBehaviour
    {
        /// <summary>
        /// Private Source RenderTexture
        /// </summary>
        [SerializeField] RenderTexture _sourceTexture;

        /// <summary>
        /// Public method to return source rendertexture
        /// </summary>
        public RenderTexture sourceTexture
        {
            get { return _sourceTexture; }
            set { _sourceTexture = value; }
        }

        /// <summary>
        /// Private name for output method sender
        /// </summary>
        [SerializeField] string _senderName;

        /// <summary>
        /// Public method to get/set name for output method sender
        /// </summary>
        public string senderName
        {
            get { return _senderName; }
            set { _senderName = value; }
        }

        /// <summary>
        /// Material used to copy the rendertexture. 
        /// </summary>
        public Material copyMaterial;

        /// <summary>
        /// If True, the swap chain system is ready.
        /// </summary>
        private bool swapChainInitialized = false;

        /// <summary>
        /// Temporary render texture for blit methods
        /// </summary>
        private RenderTexture tempRT;

        /// <summary>
        /// Queue of prepared rendertextures to be sent via output method
        /// </summary>
        private Queue<RenderTexture> renderQueue = new Queue<RenderTexture>();

        /// <summary>
        /// Creates the swapchain system
        /// </summary>
        /// <param name="w">Width of Rendertexture</param>
        /// <param name="h">Height of Rendertexture</param>
        private void InitializeSwapChain(int w, int h)
        {
            SwapChainWrapper.CreateWin(w, h);
            StartCoroutine("ApplyWindowSettings");
        }

        /// <summary>
        /// Sets up the external windows based on the Igloo Settings xml 
        /// </summary>
        /// <returns>Null</returns>
        IEnumerator ApplyWindowSettings()
        {
            yield return new WaitForSeconds(2.0f);

            int x = 10, y = 10, w = 1920, h = 1080;
            if (IglooManager.instance.settings.DisplaySettings.UWPExternalWindowSettings != null)
            {
                Igloo.UWPExternalWindowSettings settings = IglooManager.instance.settings.DisplaySettings.UWPExternalWindowSettings;
                x = settings.posX;
                y = settings.posY;
                w = settings.sizeX;
                h = settings.sizeY;
            }

            Igloo.Utils.SetWindowPos(Igloo.Utils.FindWindow(null, "Display Window"), 0, x, y, w, h, 0);
        }

        /// <summary>
        /// Wait's one millisecond, and then sends the texture via the swap chain wrapper to the 
        /// external window. Then removes that texture from the queue another millisecond later.
        /// </summary>
        /// <param name="rQueue"></param>
        /// <returns></returns>
        IEnumerator WriteTexture(Queue<RenderTexture> rQueue)
        {
            yield return new WaitForSeconds(0.001f);
            Debug.Log(SwapChainWrapper.CopyTexture(rQueue.Peek().GetNativeTexturePtr()));
            yield return new WaitForSeconds(0.001f);
            rQueue.Dequeue();
        }

        /// <summary>
        /// Pushes the render texture to the swapchain system
        /// Flips the texture on it's y-axis, and add's it to the output queue
        /// </summary>
        /// <param name="rt"></param>
        void SendRenderTexture(RenderTexture rt)
        {
            if (!swapChainInitialized)
            {
                InitializeSwapChain(rt.width, rt.height);
                swapChainInitialized = true;
                Debug.Log("SWAP CHAIN INITIALIZED...");
                return;
            }

            if (copyMaterial == null)
            {
                copyMaterial = new Material(Shader.Find("Hidden/CopyTexture"))
                {
                    hideFlags = HideFlags.DontSave
                };
                Debug.Log("Copy material initialized...");
                Debug.Log(copyMaterial.ToString());
            }

            if (!tempRT)
            {
                tempRT = RenderTexture.GetTemporary
                        (rt.width, rt.height);
            }
            //we need to flip the texture in Y axis hence copying it using a shader
            Graphics.Blit(rt, tempRT, copyMaterial);
            AddToQueue(tempRT);
        }

        /// <summary>
        /// Add the current rendertexture to the output queue
        /// Being the write texture co-routine.
        /// </summary>
        /// <param name="rTex">Render Texture to add to queue</param>
        private void AddToQueue(RenderTexture rTex)
        {
            if (renderQueue.Count > 0)
            {
                return;
            }
            renderQueue.Enqueue(rTex);
            StartCoroutine(WriteTexture(renderQueue));
        }

        /// <summary>
        /// Mono: Late update function
        /// Push the current rendertexture to the output method.
        /// </summary>
        void LateUpdate()
        {
            // Render texture mode update
            if (GetComponent<Camera>() == null && _sourceTexture != null)
                SendRenderTexture(_sourceTexture);
        }
    }
}
