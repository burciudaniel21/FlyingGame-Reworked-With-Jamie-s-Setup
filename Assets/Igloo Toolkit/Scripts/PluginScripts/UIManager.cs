using UnityEngine;
using Igloo.Common;
using UnityEngine.UI;

namespace Igloo.UI
{
#pragma warning disable IDE0090 // Use New()...

    /// <summary>
    /// Igloo UI Manager Class
    /// </summary>
    [System.Serializable]
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// The Igloo DrawUI component.
        /// </summary>
        public DrawUI drawUI;

        /// <summary>
        /// An Array of Screens
        /// </summary>
        public GameObject[] screens;

        /// <summary>
        /// The current active screen
        /// </summary>
        public GameObject activeScreen;

        /// <summary>
        /// Bool. True when the UI has completed it's setup function
        /// </summary>
        public bool uiSetupDone = false;

        /// <summary>
        /// Bool. Is the UI being used.
        /// </summary>
        public bool useUI = false;

        /// <summary>
        /// The current screen name
        /// </summary>
        private string screenName;

        /// <summary>
        /// The cached screen position
        /// </summary>
        private Vector3 startScreenPos;

        /// <summary>
        /// The cached screen rotation
        /// </summary>
        private Vector3 startScreenRot;

        /// <summary>
        /// The cached screen scale
        /// </summary>
        private Vector3 startScreenScale;

        /// <summary>
        /// The World Space Canvas UI
        /// </summary>
        public Canvas canvas;

        /// <summary>
        /// The cursor object placed at the root of the Canvas UI
        /// </summary>
        public RectTransform cursor;

        /// <summary>
        /// The RenderTexture captured from the UI Camera
        /// </summary>
        public RenderTexture texture;

        private bool debugMode = false;

        /// <summary>
        /// Setup Function, to initilize the Igloo UI System.
        /// </summary>
        public void Setup()
        {
            if (canvas && cursor && texture && useUI)
            {
                SetScreen(screenName);
                drawUI.canvasUI = canvas;
                drawUI.cursorUI = cursor;
                cursor.GetComponent<Image>().enabled = debugMode;
                activeScreen.transform.localPosition = startScreenPos;
                activeScreen.transform.localEulerAngles = startScreenRot;
                activeScreen.transform.localScale = startScreenScale;
                activeScreen.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
                drawUI.Initialise();
                uiSetupDone = true;
            }
            else
            {

                uiSetupDone = false;
            }
        }

        /// <summary>
        /// Writes the UI Specific settings to the IglooSettings.xml
        /// </summary>
        /// <param name="us">Populated UISettings class</param>
        public void SetSettings(UISettings us)
        {
            if (us == null)
            {
                useUI = false;
                startScreenPos = activeScreen.transform.localPosition;
                startScreenRot = activeScreen.transform.localEulerAngles;
                startScreenScale = activeScreen.transform.localScale;
                return;
            }
            useUI = us.useUI;
            screenName = us.screenName;
            drawUI.followCrosshair = us.followCrosshair;
            drawUI.movementSpeed = us.followSpeed;
            startScreenPos = us.screenPos.Vector3;
            startScreenRot = us.screenRot.Vector3;
            startScreenScale = us.screenScale.Vector3;
            debugMode = us.debugUIMode;
        }

        /// <summary>
        /// Reads the UI Specific settings from the IglooSettings.xml
        /// </summary>
        /// <returns>Populated UI Settings Class</returns>
        public UISettings GetSettings()
        {
            UISettings us = new UISettings();
            if (activeScreen != null)
            {
                us.useUI = useUI;
                us.screenName = activeScreen.name;
                us.followCrosshair = drawUI.followCrosshair;
                us.followSpeed = drawUI.movementSpeed;
                us.screenPos = new Vector3Item(startScreenPos);
                us.screenRot = new Vector3Item(startScreenRot);
                us.screenScale = new Vector3Item(startScreenScale);
                us.debugUIMode = debugMode;
            }
            return us;
        }

        /// <summary>
        /// TODO - Find out what this does. Looks useful. 
        /// </summary>
        /// <param name="name"></param>
        public void SetScreen(string name)
        {
            Debug.Log($"<b>[Igloo]</b> Setting screen to: {name}");
            bool found = false;
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].name == name)
                {
                    activeScreen = screens[i];
                    if (activeScreen.GetComponent<DrawUI>() != null)
                    {
                        drawUI = activeScreen.GetComponent<DrawUI>();
                        found = true;
                    }
                }
            }
            if (!found) Debug.LogError("<b>[Igloo]</b> Screen name " + name + " not found, using default screen instead");

        }

        /// <summary>
        /// Set the UI to be visible, or hidden.
        /// </summary>
        /// <remarks>
        /// Binding this to a controller button, like Xbox-Y is a really good idea for a Pause Menu.
        /// </remarks>
        /// <param name="state">bool. True = Visible</param>
        public void SetUIVisible(bool state)
        {
            Debug.Log($"<b>[Igloo]</b> Setting UI Visible: {state}");
            if (activeScreen && uiSetupDone)
            {
                canvas.enabled = state;
                activeScreen.SetActive(state);
                PlayerPointer.instance.SetUIActive(state);
                //if (IglooManager.instance.PlayerManager)
                //{
                //    if (state)
                //    {
                //        rotationMode = IglooManager.instance.PlayerManager.rotationMode;
                //        IglooManager.instance.PlayerManager.rotationMode = PlayerManager.ROTATION_MODE.IGLOO_360;
                //    }
                //    else if (!state)
                //    {
                //        IglooManager.instance.PlayerManager.rotationMode = (PlayerManager.ROTATION_MODE)rotationMode;
                //    }
                //}


            }
        }

        /// <summary>
        /// Sets the follow speed of the crosshair to the UI.
        /// </summary>
        /// <param name="speed"></param>
        public void SetFollowSpeed(float speed) {
            drawUI.movementSpeed = speed;
        }

        /// <summary>
        /// Set the UI to follow the cursor when it leaves the UI canvas area
        /// </summary>
        /// <param name="state">bool. True = Follow the Cursor.</param>
        public void SetFollowCursor(bool state)
        {
            drawUI.followCrosshair = state;
        }

    }
}

