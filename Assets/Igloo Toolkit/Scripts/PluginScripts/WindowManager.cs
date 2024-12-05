using System;
using System.Collections;
using UnityEngine;

namespace Igloo.Common
{
#pragma warning disable IDE0090 // Use New()...
    /// <summary>
    /// Igloo Window Manager Class
    /// </summary>
    public class WindowManager : MonoBehaviour
    {

        /// <summary>
        /// The window settings from the Igloo Settings XML 
        /// </summary>
        WindowSettings windowSettings = null;

        /// <summary>
        /// Starts the igloo window creation coroutine
        /// </summary>
        public void SetupWindows()
        {
            if (windowSettings == null) return;
            if (windowSettings.enabled == true)
            {
                Debug.Log("<b>[Igloo]</b> multi window is enabled, attempting to create windows");
                if (windowSettings.Windows != null)
                {
                    StartCoroutine(ApplyWindowSettings());
                }
            }
        }

        /// <summary>
        /// Applies the window settings to the window system
        /// Creates window items, based on those settings.
        /// Links the window items to a display.
        /// </summary>
        /// <returns>null</returns>
        IEnumerator ApplyWindowSettings()
        {
            for (int i = 0; i < UnityEngine.Display.displays.Length; i++)
            {
                UnityEngine.Display.displays[i].Activate();
                if (i >= windowSettings.Windows.Length)
                {
                    Debug.LogWarning("<b>[Igloo]</b> No settings found for display " + i + "creating default now");
                    WindowItem window = new WindowItem() { width = 0, height = 0, positionOffsetX = 0, positionOffsetY = 0 };
                    Array.Resize<WindowItem>(ref windowSettings.Windows, windowSettings.Windows.Length + 1);
                    windowSettings.Windows[windowSettings.Windows.Length - 1] = window;
                }
            }

            yield return new WaitForSeconds(1);

            for (int i = 0; i < UnityEngine.Display.displays.Length; i++)
            {
                if (i < windowSettings.Windows.Length)
                {
                    WindowItem window = windowSettings.Windows[i];
                    if (window.width > 0 && window.height > 0)
                        UnityEngine.Display.displays[i].SetParams(window.width, window.height, window.positionOffsetX, window.positionOffsetY);
                }
                //yield return new WaitForSeconds(0.2f);
            }

        }

        /// <summary>
        /// Writes the Window Settings to the Igloo Settings XML
        /// </summary>
        /// <param name="s">populated Window Settings class</param>
        public void SetSettings(WindowSettings s)
        {
            windowSettings = s;
        }

        /// <summary>
        /// Read the Window Settings from Igloo Settings XML
        /// </summary>
        /// <returns>A populated window settings class</returns>
        public WindowSettings GetSettings()
        {
            if (windowSettings == null)
            {
                // Create default window setting
                windowSettings = new WindowSettings
                {
                    enabled = false
                };
                WindowItem[] windows = new WindowItem[1];
                WindowItem window = new WindowItem() { width = 0, height = 0, positionOffsetX = 0, positionOffsetY = 0 };
                windows[0] = window;
                windowSettings.Windows = windows;
            }
            return windowSettings;
        }
    }
}

