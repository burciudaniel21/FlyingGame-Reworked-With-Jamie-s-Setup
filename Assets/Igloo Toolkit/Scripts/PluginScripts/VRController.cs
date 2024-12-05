using UnityEngine;
using Igloo.Common;

namespace Igloo.Controllers
{
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members.

    /// <summary>
    /// The Igloo VR Controller System Class
    /// </summary>
    public class VRController : Singleton<VRController>
    {
        /// <summary>
        /// The Crosshair object
        /// </summary>
        public GameObject crosshair;

        /// <summary>
        /// The renderer component of the crosshair object
        /// </summary>
        Renderer crosshairRenderer;


        /// <summary>
        /// A lineRenderer component
        /// </summary>
        /// <remarks>Used for debugging direction</remarks>
        LineRenderer lineRenderer;

        /// <summary>
        /// True, Uses the lineRenderer component to draw a line
        /// </summary>
        bool drawLine = false;

        /// <summary>
        /// The last Raycast Hit of the crosshair
        /// </summary>
        public RaycastHit hit;

        /// <summary>
        /// Line width for LineRenderer component
        /// </summary>
        float lineWidth = 0.01f;

        /// <summary>
        /// Line length for LineRenderer component
        /// </summary>
        float maxLineLength = 20.0f;

        /// <summary>
        /// True, raycast has hit an object
        /// </summary>
        bool hasHit = false;

        /// <summary>
        /// Minimum size of the crosshair object
        /// </summary>
        public float crosshairSize = 0.015f;

        /// <summary>
        /// Mono: Start Function. 
        /// Sets up the line renderer component, and delegates position and rotation events 
        /// to incoming gyro OSC message events.
        /// </summary>
        protected override void AwakeInternal()
        {
            lineRenderer = GetComponent<LineRenderer>();
            Vector3[] initLinePositions = new Vector3[2] { Vector3.zero, Vector3.zero };
            lineRenderer.SetPositions(initLinePositions);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.enabled = true;


            if (IglooManager.instance != null)
            {
                IglooManager.instance.NetworkManager.OnVrControllerGyroEvent += SetRotation;
                IglooManager.instance.NetworkManager.OnVrControllerPositionEvent += SetPosition;
            }
            crosshairRenderer = crosshair.GetComponent<Renderer>();
        }

        /// <summary>
        /// Sets the position of the object based on incoming OSC message event data
        /// </summary>
        /// <param name="deviceID">VR controller device ID</param>
        /// <param name="position">Vector3, world space position of the controller</param>
        void SetPosition(int deviceID, Vector3 position)
        {
            if (deviceID == 1)
            {
                this.transform.localPosition = position;
            }
        }

        /// <summary>
        /// Sets the rotation of this gameObject based on incoming OSC message event data
        /// </summary>
        /// <param name="deviceID">VR Controller device ID</param>
        /// <param name="rot">Vector3, Euler rotation of the controller</param>
        void SetRotation(int deviceID, Vector3 rot)
        {
            if (deviceID == 1)
            {
                this.transform.localEulerAngles = rot;
            }
        }

        /// <summary>
        /// Mono: Update Function
        /// Raycasts from this object, into scene. Returns raycast hit, and miss events. 
        /// Sets line renderer based on raycast
        /// Places the crosshair where a raycast hit happens.
        /// </summary>
        public virtual void Update()
        {
            _ = Vector3.zero;

            Vector3 endPos;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                // Detect screen collision
                if (hit.transform.gameObject.CompareTag("IglooScreen"))
                {
                    ScreenHit(hit.textureCoord);
                }
                else
                    ScreenMiss();

                crosshair.transform.position = hit.point;
                endPos = hit.point;
                // Crosshair positioning system
                crosshair.transform.rotation = Quaternion.FromToRotation(crosshair.transform.up, hit.normal) * crosshair.transform.rotation;
                if (!hasHit)
                {
                    crosshairRenderer.enabled = true;
                    crosshair.transform.localScale = new Vector3(crosshairSize, crosshairSize, crosshairSize);
                    hasHit = true;
                }
            }
            else
            {
                hasHit = false;
                ScreenMiss();
                crosshairRenderer.enabled = false;
                endPos = this.transform.position + (maxLineLength * this.transform.TransformDirection(Vector3.forward));
                crosshair.transform.rotation = Quaternion.FromToRotation(crosshair.transform.up, this.transform.forward) * crosshair.transform.rotation;

            }
            if (drawLine)
            {
                lineRenderer.SetPosition(0, this.transform.position);
                lineRenderer.SetPosition(1, endPos);
            }
        }

        public ScreenHitPosition OnScreenHitPosition;
        public delegate void ScreenHitPosition(Vector2 pos);

        public ScreenMissCallback OnScreenMiss;
        public delegate void ScreenMissCallback();
        private void ScreenHit(Vector2 pos)
        {
            OnScreenHitPosition?.Invoke(pos);
        }
        private void ScreenMiss()
        {
            if (OnScreenHitPosition != null) OnScreenMiss();
        }
    }


}

