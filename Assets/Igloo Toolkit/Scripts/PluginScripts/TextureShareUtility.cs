using UnityEngine;
#if KLAK_NDI
using Klak.Ndi;
#endif
#if UNITY_STANDALONE_WIN && KLAK_SPOUT
using Klak.Spout;
#endif

namespace Igloo.Common
{
    /// <summary>
    /// Igloo Texture Share System
    /// Adds and removes Texture sharing methods from the Igloo Camera System
    /// </summary>
    public static class TextureShareUtility
    {
        /// <summary>
        /// Texture Share Mode Enum structure
        /// 0 - None
        /// 1 - Spout
        /// 2 - NDI
        /// 3 - Display
        /// </summary>
        public enum TextureShareMode { NONE, SPOUT, NDI, DISPLAY }

        /// <summary>
        /// Adds a texture sender mode to a gameobject, with a sender name and a reference rendertexture
        /// </summary>
        /// <param name="shareMode">Texture Share mode</param>
        /// <param name="go">GameObject to share</param>
        /// <param name="senderName">Name of the sender (if Spout or NDI)</param>
        /// <param name="texture">Reference Render Texture to send</param>
        public static void AddTextureSender(TextureShareMode shareMode, GameObject go, string senderName, ref RenderTexture texture)
        {
            switch (shareMode)
            {
                case TextureShareMode.NONE:
                    break;
                case TextureShareMode.DISPLAY:
                    if (go.GetComponent<DisplaySender>())
                    {
                        DisplaySender[] senders = go.GetComponents<DisplaySender>();
                        foreach (DisplaySender sender in senders)
                        {
                            if (sender.senderName == senderName) UnityEngine.Object.DestroyImmediate(sender);
                        }
                    }
                    DisplaySender displaySender = go.AddComponent<DisplaySender>();
                    displaySender.senderName = senderName;
                    displaySender.sourceTexture = texture;
                    break;
                case TextureShareMode.SPOUT:
#if UNITY_STANDALONE_WIN && KLAK_SPOUT
                    if (go.GetComponent<SpoutSender>())
                    {
                        SpoutSender[] senders = go.GetComponents<SpoutSender>();
                        foreach (SpoutSender sender in senders)
                        {
                            if (sender.spoutName == senderName) UnityEngine.Object.DestroyImmediate(sender);
                        }
                    }
                    SpoutSender spoutSender = go.AddComponent<SpoutSender>();
                    spoutSender.SetResources(IglooManager.instance.spoutResources);
                    spoutSender.captureMethod = Klak.Spout.CaptureMethod.Texture;
                    spoutSender.spoutName = senderName;
                    spoutSender.sourceTexture = texture;
#endif
                    break;
                case TextureShareMode.NDI:
#if KLAK_NDI
                    if (go.GetComponent<NdiSender>()) {
                        NdiSender[] senders = go.GetComponents<NdiSender>();
                        foreach (NdiSender sender in senders) {
                            if (sender.ndiName == senderName) UnityEngine.Object.DestroyImmediate(sender);
                        }
                    }
                    NdiSender ndiSender = go.AddComponent<NdiSender>();
                    ndiSender.SetResources(IglooManager.instance.ndiResources);
                    ndiSender.captureMethod = Klak.Ndi.CaptureMethod.Texture;
                    ndiSender.ndiName = senderName;
                    ndiSender.sourceTexture = texture;
                    break;
                default:
#endif
                    break;
            }
        }

        /// <summary>
        /// Removes all possible texture sender components from an object
        /// </summary>
        /// <param name="go">The object to remove components from</param>
        public static void RemoveAllSendersFromObject(GameObject go)
        {
#if UNITY_STANDALONE_WIN && KLAK_SPOUT
            if (go.GetComponent<SpoutSender>())
            {
                SpoutSender[] senders = go.GetComponents<SpoutSender>();
                foreach (SpoutSender sender in senders)
                {
                    UnityEngine.Object.DestroyImmediate(sender);
                }
            }
#endif
#if KLAK_NDI
            if (go.GetComponent<NdiSender>()) {
                NdiSender[] senders = go.GetComponents<NdiSender>();
                foreach (NdiSender sender in senders) {
                    UnityEngine.Object.DestroyImmediate(sender);
                }
            }
#endif
        }
    }

}
