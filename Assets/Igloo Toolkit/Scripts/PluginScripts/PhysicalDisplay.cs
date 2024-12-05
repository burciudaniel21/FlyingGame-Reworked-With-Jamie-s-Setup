using UnityEngine;

namespace Igloo.Common
{
#pragma warning disable IDE0051 // Remove unused private members

    /// <summary>
    /// Igloo Phyiscal Display Class, Overrides the Igloo Display class
    /// </summary>
    public class PhysicalDisplay : Display
    {
        /// <summary>
        /// The target physical display to render to
        /// </summary>
        public int targetDisplay;

        /// <summary>
        /// The bounds of the window to be created
        /// </summary>
        public RectInt windowBounds;

        /// <summary>
        /// 3D Variable, the left window viewport size
        /// </summary>
        public RectInt leftWindowViewport;

        /// <summary>
        /// 3D Variable, the right window viewport size
        /// </summary>
        public RectInt rightWindowViewport;

        /// <summary>
        /// True, the application is demanding exclusive full scre
        /// </summary>
        public bool exclusiveFullscreen;

        /// <summary>
        /// Writes overridden settings to Display section of IglooSettings.xml
        /// </summary>
        /// <param name="settings"></param>
        public override void SetSettings(DisplayItem settings)
        {
            base.SetSettings(settings);
            //viewPortRect = new Rect(settings.viewportRect.x, settings.viewportRect.y, settings.viewportRect.w, settings.viewportRect.h);
        }

        /// <summary>
        /// Reads the Display section of IglooSettings.xml
        /// </summary>
        /// <returns>Populated DisplayItem settings class</returns>
        public override DisplayItem GetSettings()
        {
            DisplayItem settings = base.GetSettings();
            //RectItem viewportRectItem = new RectItem();
            //viewportRectItem.x = viewPortRect.x;
            //viewportRectItem.y = viewPortRect.y;
            //viewportRectItem.w = viewPortRect.width;
            //viewportRectItem.h = viewPortRect.height;
            //settings.viewportRect = viewportRectItem;

            return settings;
        }

        /// <summary>
        /// Sets the left camera up differently for phyiscal camera.
        /// </summary>
        public override void InitialiseCameras()
        {
            base.InitialiseCameras();
            //Camera leftCam = headManager.CreateLeftEye(name, isOffAxis ? Vector3.zero : camRotation);
            //if (is3D) leftCam.stereo;
        }
    }
}

