using UnityEngine;

namespace Igloo.Controllers
{
#pragma warning disable IDE0090 // Use New()...
#pragma warning disable IDE0044 // Add readonly modifier
    /// <summary>
    /// The Igloo Crosshair Class
    /// </summary>
    public class Crosshair : MonoBehaviour
    {
        /// <summary>
        /// Enum defining how the crosshair is displayed to the user.
        /// </summary>
        public enum CROSSHAIR_MODE { SHOW, SHOW_ON_MOVE, HIDE };

        /// <summary>
        /// Current crosshair display mode
        /// </summary>
        public CROSSHAIR_MODE crosshairMode = CROSSHAIR_MODE.SHOW;

        /// <summary>
        /// Previous position of the crosshair
        /// </summary>
        Vector3 previousPos = new Vector3();

        /// <summary>
        /// Renderer component of the crosshair object
        /// Get returns the component attached to this object
        /// </summary>
        private Renderer crosshairRenderer { get => this.GetComponent<Renderer>(); }

        /// <summary>
        /// if True, crosshair will be hidden regardless of crosshair mode.
        /// </summary>
        /// <remarks>
        /// Usefull for cinematics
        /// </remarks>
        bool forceHide = false;

        /// <summary>
        /// Mono late update function
        /// Apply the current crosshair mode method
        /// </summary>
        void LateUpdate()
        {
            if (forceHide)
            {
                crosshairRenderer.enabled = false;
                return;
            }

            switch (crosshairMode)
            {
                case CROSSHAIR_MODE.SHOW:
                    crosshairRenderer.enabled = true;
                    break;
                case CROSSHAIR_MODE.HIDE:
                    crosshairRenderer.enabled = false;
                    break;
                case CROSSHAIR_MODE.SHOW_ON_MOVE:
                    if (crosshairRenderer.enabled == false && transform.position != previousPos)
                    {
                        crosshairRenderer.enabled = true;
                    }
                    else if (crosshairRenderer.enabled == true && transform.position == previousPos)
                    {
                        crosshairRenderer.enabled = false;
                    }
                    previousPos = transform.position; break;
            }
        }

        /// <summary>
        /// Set a new crosshair display mode
        /// </summary>
        /// <param name="mode">New crosshair display mode</param>
        public void SetMode(CROSSHAIR_MODE mode)
        {
            crosshairMode = mode;
        }

        /// <summary>
        /// Set the Force Hide bool
        /// </summary>
        /// <param name="state">new force hide state</param>
        public void ForceHide(bool state)
        {
            forceHide = state;
        }
    }
}
