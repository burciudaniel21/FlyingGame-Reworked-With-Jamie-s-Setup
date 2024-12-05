using Igloo.Common;
using Igloo.Controllers;
using System;
using UnityEngine;

namespace Igloo.UI
{
    /// <summary>
    /// Igloo Draw UI class
    /// </summary>
    public class DrawUI : MonoBehaviour
    {
        /// <summary>
        /// The speed multiplier for when moving the UI larger distances
        /// </summary>
        [Tooltip("The speed multiplier for when moving the UI larger distances")]
        public float distSpeedMultiplier = 5.0f;

        /// <summary>
        /// The minimum distance of UI travel where the speed multiplier kicks in
        /// </summary>
        [Tooltip("The minimum distance of UI travel where the speed multiplier kicks in")]
        public float distThreshold = 0.7f;

        /// <summary>
        /// The size of the UI mesh
        /// </summary>
        public float size = 1.0f;

        /// <summary>
        /// The last position of X before this Update.
        /// </summary>
        public float lastX = 0.0f;

        /// <summary>
        /// UI's movement speed when catching up with the crosshair
        /// </summary>
        public float movementSpeed = 10f;

        /// <summary>
        /// If True, UI will follow the crosshairs position
        /// </summary>
        public bool followCrosshair = true;

        /// <summary>
        /// If True, the crosshair will be hidden when it's on the UI screen. 
        /// </summary>
        /// <remarks>
        /// Usefull if using the CursorRect as the UI cursor
        /// </remarks>
        public bool hideCrosshairOnScreen = false;

        /// <summary>
        /// If True, the UI is active
        /// </summary>
        public bool on = true;

        /// <summary>
        /// The UI Canvas to display
        /// </summary>
        public Canvas canvasUI;

        /// <summary>
        /// The Rect Transform of the Cursor on the Canvas
        /// </summary>
        public RectTransform cursorUI;

        /// <summary>
        /// The X coordinates of the crosshair
        /// </summary>
        private float x = 0.0f;

        /// <summary>
        /// The Y coordinates of the crosshair
        /// </summary>
        private float y = 0.0f;

        /// <summary>
        /// Returns the Aspect Ratio of the UI Mesh
        /// </summary>
        private float AspectRatioMesh { get => GetMeshApectRatio(); }

        /// <summary>
        /// Returns the Aspect Ratio of the Canvas
        /// </summary>
        private float AspectRatioUI { get => canvasUI.pixelRect.width / canvasUI.pixelRect.height; }

        /// <summary>
        /// If True, the UI is initialized
        /// </summary>
        bool isInit = false;

        /// <summary>
        /// If True, the UI is animated
        /// </summary>
        bool animateUi = false;

        /// <summary>
        /// Amount of missed late updates
        /// </summary>
        int missedUpdateCount;

        /// <summary>
        /// Material used to display the UI on the UI Mesh
        /// </summary>
        Material mat;

        /// <summary>
        /// The Igloo Crosshair currently in use
        /// </summary>
        Crosshair Crosshair { get => IglooManager.instance.PlayerManager.crosshair; }

        private Vector4 _touchScreenAdjustment = new Vector4();
        private bool _useTouchScreenSystem;

        /// <summary>
        /// Setup the UI system
        /// Binds the Player Pointer events to UI events
        /// </summary>
        public void Initialise()
        {
            if (PlayerPointer.instance)
            {
                PlayerPointer.instance.OnScreenHitPosition += SetCursorPos;
                PlayerPointer.instance.OnScreenMiss += SetCursorMiss;
            }
            if (VRController.instance)
            {
                VRController.instance.OnScreenHitPosition += SetCursorPos;
                VRController.instance.OnScreenMiss += SetCursorMiss;

            }
            mat = GetComponent<Material>();
            Debug.Log("<b>[Igloo]</b> Draw UI is initialised");

            Igloo.Common.NetworkManager.instance.OnTouchInputPosition += PositionInputFromTouch;
            isInit = true;
        }

        public void SetTouchScreenSettings(TouchScreenSettings TSS)
        {
            if (TSS == null) return;
            Debug.Log("<b>[Igloo]</b> Setting Touch screen settings");
            _useTouchScreenSystem = TSS.UseTouchScreen;
            _touchScreenAdjustment = new Vector4(TSS.XPositionStart, TSS.XPositionEnd, TSS.YPositionStart, TSS.YPositionEnd);

        }

        public TouchScreenSettings GetTouchScreenSettings() {
            TouchScreenSettings TSS = new TouchScreenSettings {
                XPositionStart = _touchScreenAdjustment.x,
                XPositionEnd = _touchScreenAdjustment.y,
                YPositionStart = _touchScreenAdjustment.z,
                YPositionEnd = _touchScreenAdjustment.w,
                UseTouchScreen = _useTouchScreenSystem
            };
            return TSS;
        }

        /// <summary>
        /// Sets the cursor position based on Touch input from Igloo Touch Screen
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position">Vector2 position data from touch screem</param>
        private void PositionInputFromTouch(string name, Vector2 position) {
            
            Vector2 newPos = new Vector2(Normalize(position.x,  _touchScreenAdjustment.x, _touchScreenAdjustment.y, 0, 1), Normalize(position.y, _touchScreenAdjustment.z, _touchScreenAdjustment.w, 0, 1));
            // Debug.Log($"Incoming Position Value {position} | New Position Value {newPos} | Touch Screen Adjustment H {_touchScreenAdjustment.x} x {_touchScreenAdjustment.y} | Touch Screen Adjustment V {_touchScreenAdjustment.z} x {_touchScreenAdjustment.w}");
            SetCursorPos(newPos);
        }

        /// <summary>
        /// Mono Start function
        /// Calls Initialise.
        /// </summary>
        void Start()
        {
            Initialise();
        }

        /// <summary>
        /// Sets the X coordinates
        /// </summary>
        /// <param name="newX">New X coordinate</param>
        public void SetX(float newX) { x = newX; }

        /// <summary>
        /// Sets the Y coordinates
        /// </summary>
        /// <param name="newY">New Y coordinate</param>
        public void SetY(float newY) { y = newY; }

        /// <summary>
        /// Finds the Aspect Ratio of the current mesh in use
        /// </summary>
        /// <returns>Float, the Aspect Ratio</returns>
        float GetMeshApectRatio()
        {
            float w = gameObject.transform.localScale.x;
            float h = gameObject.transform.localScale.y;
            if (gameObject.name == "Cylinder")
            {
                return (2 * Mathf.PI * w / 2) / h;
            }
            else if (gameObject.name == "Plane")
            {
                return w / h;
            }
            else return 1;
        }

       

        /// <summary>
        /// Mono Late Update Function
        /// Moves the UI if following the crosshair
        /// Sets the texture scale if the mesh has changed.
        /// </summary>
        void LateUpdate()
        {
            if (!isInit || canvasUI == null || cursorUI == null)
            {
                missedUpdateCount++;
                if(missedUpdateCount == 180)
                {
                    this.gameObject.SetActive(false);
                }
                return;
            }

            if (!mat) mat = GetComponent<Renderer>().material;

            float xScaleFactor = AspectRatioMesh / AspectRatioUI;

            float xScale = (1 + (1 - size)) * xScaleFactor;
            float yScale = 1 + (1 - size);
            mat.mainTextureScale = new Vector2(xScale, yScale);

            float xPos = -x * xScale;
            float yPos = -y;

            if (followCrosshair)
            {
                if (animateUi)
                {
                    if (lastX < 0.65f && lastX > 0.45f)
                        animateUi = false;
                    else
                    {
                        if (lastX > 0.5f && lastX < (xScale * 0.7f) - xPos)
                            gameObject.transform.Rotate(new Vector3(0.0f, movementSpeed, 0.0f));
                        else
                            gameObject.transform.Rotate(new Vector3(0.0f, -movementSpeed, 0.0f));
                    }
                }
            }
            mat.mainTextureOffset = new Vector2(xPos, yPos);
        }

        /// <summary>
        /// If the cursor has missed, stop animating the UI
        /// </summary>
        public void SetCursorMiss()
        {
            return;
        }

        /// <summary>
        /// Sets the cursor position on the UI, based on the Crosshair position
        /// </summary>
        /// <param name="pos">The Raycast hit position on the UI mesh</param>
        public void SetCursorPos(Vector2 pos)
        {
            if (!mat) return;
            Vector2 posMapped = new Vector2
            {
                x = (pos.x * mat.mainTextureScale.x) + mat.mainTextureOffset.x,
                y = (pos.y * mat.mainTextureScale.y) + mat.mainTextureOffset.y
            };

            lastX = posMapped.x;
            if (lastX > 1.0f || lastX < 0)
                animateUi = true;

            if ((Mathf.Clamp(posMapped.x, 0, 1) == posMapped.x) && (Mathf.Clamp(posMapped.y, 0, 1) == posMapped.y))
            {
                if (hideCrosshairOnScreen && Crosshair) Crosshair.ForceHide(true);
                cursorUI.anchoredPosition = new Vector2(canvasUI.GetComponent<RectTransform>().rect.width * posMapped.x, canvasUI.GetComponent<RectTransform>().rect.height * posMapped.y);
            }

        }

        float Normalize(float val, float valmin, float valmax, float min, float max) {
            return (((val - valmin) / (valmax - valmin)) * (max - min)) + min;
        }
    }
}
