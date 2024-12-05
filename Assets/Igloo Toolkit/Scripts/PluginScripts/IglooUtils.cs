using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;


namespace Igloo
{
    /// <summary>
    /// Enum structure of Eye Modes
    /// 0 - Left
    /// 1 - Center
    /// 2 - Right
    /// </summary>
    public enum EYE { LEFT, CENTER, RIGHT }

    /// <summary>
    /// Enum structure of Vsync modes
    /// 0 - Dont
    /// 1 - Every Frame
    /// 2 - Every Second Frame
    /// </summary>
    public enum VSyncMode { DONT, EVERY, EVERY_SECOND }

    /// <summary>
    /// Igloo Utilities Class
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Returns the current Igloo Toolkit version number
        /// </summary>
        /// <returns>string, The igloo Toolkit version number</returns>
        public static string GetVersion()
        {
            return "1.2.2";
        }

        /// <summary>
        /// Returns advanced mode bool
        /// </summary>
        /// <returns>Bool, If True: Advanced mode Active</returns>
        public static bool IsAdvancedMode()
        {
            return false;
        }

        /// <summary>
        /// Returns a 4x4 projection matrix from the 4 wall vector3 positions. 
        /// TODO - More detail required. 
        /// </summary>
        /// <param name="pa"></param>
        /// <param name="pb"></param>
        /// <param name="pc"></param>
        /// <param name="pe"></param>
        /// <param name="ncp"></param>
        /// <param name="fcp"></param>
        /// <returns></returns>
        public static Matrix4x4 GetAsymProjMatrix(Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pe, float ncp, float fcp)
        {
            //compute orthonormal basis for the screen - could pre-compute this...
            Vector3 vr = (pb - pa).normalized;
            Vector3 vu = (pc - pa).normalized;
            Vector3 vn = Vector3.Cross(vr, vu).normalized;

            //compute screen corner vectors
            Vector3 va = pa - pe;
            Vector3 vb = pb - pe;
            Vector3 vc = pc - pe;

            //find the distance from the eye to screen plane
            float n = ncp;
            float f = fcp;
            float d = Vector3.Dot(va, vn); // distance from eye to screen
            float nod = n / d;
            float l = Vector3.Dot(vr, va) * nod;
            float r = Vector3.Dot(vr, vb) * nod;
            float b = Vector3.Dot(vu, va) * nod;
            float t = Vector3.Dot(vu, vc) * nod;

            //put together the matrix - bout time amirite?
            Matrix4x4 m = Matrix4x4.zero;

            //from http://forum.unity3d.com/threads/using-projection-matrix-to-create-holographic-effect.291123/
            m[0, 0] = 2.0f * n / (r - l);
            m[0, 2] = (r + l) / (r - l);
            m[1, 1] = 2.0f * n / (t - b);
            m[1, 2] = (t + b) / (t - b);
            m[2, 2] = -(f + n) / (f - n);
            m[2, 3] = (-2.0f * f * n) / (f - n);
            m[3, 2] = -1.0f;

            return m;
        }

        /// <summary>
        /// Splits up a string into smaller strings based on an array of characters
        /// </summary>
        /// <param name="s">String Input to be split</param>
        /// <param name="delim">Characters to split at</param>
        /// <param name="at">position to split at</param>
        /// <returns></returns>
        public static string StringSplitter(string s, char[] delim, int at)
        {
            string _s = null;
            string[] split = s.Split(delim);
            _s = split[at];
            return _s;
        }

        /// <summary>
        /// Copies components from one object to another.
        /// Components copied using this method: 
        /// Transform, MeshFilter, Mesh Renderer, Camera, AudioListener
        /// </summary>
        /// <param name="sourceGO">Source object containing special components</param>
        /// <param name="targetGO">Target object to receieve special components</param>
        public static void CopySpecialComponents(GameObject sourceGO, GameObject targetGO)
        {
#if UNITY_EDITOR
            foreach (var component in sourceGO.GetComponents<Component>())
            {
                var componentType = component.GetType();
                if (componentType != typeof(Transform) &&
                    componentType != typeof(MeshFilter) &&
                    componentType != typeof(MeshRenderer) &&
                    componentType != typeof(Camera) &&
                    componentType != typeof(AudioListener)
                    )
                {
                    Debug.Log("<b>[Igloo]</b> Found a component of type " + component.GetType());
                    UnityEditorInternal.ComponentUtility.CopyComponent(component);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGO);
                    Debug.Log("<b>[Igloo]</b> Copied " + component.GetType() + " from " + sourceGO.name + " to " + targetGO.name);
                }
            }
#endif
        }

        /// <summary>
        /// Sets the external window position based on X/Y coordinates
        /// </summary>
        /// <param name="x">New X position of the window</param>
        /// <param name="y">New Y position of the window</param>
        public static void SetWindowPostion(int x, int y)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            SetWindowPos(FindWindow(null, Application.productName), 0, x, y, Screen.width, Screen.height, Screen.width * Screen.height == 0 ? 1 : 0);
#endif
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        /// <summary>
        /// Sets the external window position using User32 dll commands
        /// TODO - More info required.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="Y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="wFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        /// <summary>
        /// returns a pointer to the display window
        /// </summary>
        /// <param name="className"></param>
        /// <param name="windowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(System.String className, System.String windowName);
#endif

        /// <summary>
        /// Returns the path to the warper based on the local application data path of the user
        /// </summary>
        /// <returns>string, system path to the warper data folder.</returns>
        public static string GetWarperDataPath()
        {
            //string appdata = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            //return Path.Combine(appdata, "Igloo Vision\\IglooWarper");          
            return Path.Combine("C:\\ProgramData\\Igloo Vision\\IglooWarper");
        }

        /// <summary>
        /// Returns the data path based on if using persistant data path, or streaming assets.
        /// </summary>
        /// <returns>string, an application path</returns>
        public static string GetDataPath()
        {
            string path = Application.streamingAssetsPath;
#if usePersistentDatapath
            path = Application.persistentDataPath;
#endif
            return path;
        }

        public static Texture2D LoadFloatTexture(string path, int w, int h, int ignoreBytes = 0)
        {
            UnityEngine.Debug.Log("<b>[Igloo]</b> About to attempt loading float texture, path: " + path + " ,w: " + w + " h: " + h);
            int texWidth = w;
            int texHeight = h;
            Texture2D tex = null;

            if (!System.IO.File.Exists(path))
            {
                UnityEngine.Debug.LogWarning("<b>[Igloo]</b> Cannot find Igloo Warp Image at: " + path);
                return tex;
            }

            byte[] sArray = File.ReadAllBytes(path);

            float[] dArray = new float[(sArray.Length - ignoreBytes) / 4];

            UnityEngine.Debug.Log("sArray size: " + sArray.Length + "  dArray size: " + dArray.Length);

            Buffer.BlockCopy(sArray, ignoreBytes, dArray, 0, sArray.Length - ignoreBytes);

            int bArrayPos = 0;
            int pixelPos = 0;

            Color[] pixels = new Color[texWidth * texHeight];

            for (int y = 0; y < texHeight; y++)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    float r = dArray[bArrayPos];
                    float g = 1 - dArray[bArrayPos + 1];
                    float b = dArray[bArrayPos + 2];
                    float a = dArray[bArrayPos + 3];

                    Color pixel;
                    pixel = new Color(r, g, b, a);

                    pixels[pixelPos] = pixel;
                    bArrayPos += 4;
                    pixelPos++;
                }
            }

            tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBAFloat, false);
            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }

    }
}

