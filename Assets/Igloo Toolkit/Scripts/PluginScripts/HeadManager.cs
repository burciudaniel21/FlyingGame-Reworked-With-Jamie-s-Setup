using UnityEngine;
using Igloo.Controllers;

using System.Net.Sockets;
using System.Net;

namespace Igloo.Common
{
#pragma warning disable IDE0090 // Use New()...
    /// <summary>
    /// The Igloo Head Manager class
    /// </summary>
    public class HeadManager : MonoBehaviour
    {
        /// <summary>
        /// Head Tracking System Type
        /// 0 - Default
        /// 1 - Optitrack 
        /// 2 - VRPN
        /// </summary>
        public enum HeadTrackingInput { Default, Optitrack, VRPN };

        /// <summary>
        /// Current Head Tracking Input System
        /// </summary>
        public HeadTrackingInput headTrackingInput = HeadTrackingInput.Default;

        /// <summary>
        /// If True, Head Tracking is enabled
        /// </summary>
        private bool isHeadTracking = false;

        /// <summary>
        /// Position offset for left eye
        /// </summary>
        private Vector3 leftEyeOffset = Vector3.zero;

        /// <summary>
        /// Position offset for Right Eye
        /// </summary>
        private Vector3 rightEyeOffset = Vector3.zero;

        /// <summary>
        /// Position offset for Center eye
        /// </summary>
        private Vector3 CenterEyeOffset { get => (leftEyeOffset + rightEyeOffset) * 0.5f; }

        /// <summary>
        /// Positon Cache for last update
        /// </summary>
        private Vector3 positionCached = Vector3.zero;

        /// <summary>
        /// Rotation Cache for last update
        /// </summary>
        private Vector3 rotationCached = Vector3.zero;

        /// <summary>
        /// Called when any settings have been changed for the HeadManager
        /// </summary>
        public HeadSettingsChange OnHeadSettingsChange;

        /// <summary>
        /// The server address of the Optitrack machine, default 127.0.0.1
        /// </summary>
        public string optitrackServerIPAddress = "127.0.0.1";

        /// <summary>
        /// The rigidbody ID that the optitrack system is assigned to follow
        /// </summary>
        public int optitrackHeadRigidBodyID = 1;

        /// <summary>
        /// Delegate for HeadSettingsChange
        /// </summary>
        public delegate void HeadSettingsChange();

        /// <summary>
        /// Returns left eye offset whilst calling OnHeadSettingsChange
        /// </summary>
        public Vector3 LeftEyeOffset
        {
            get { return leftEyeOffset; }
            set
            {
                leftEyeOffset = value;
                OnHeadSettingsChange();
            }
        }

        /// <summary>
        /// Returns right eye offset whilst calling OnHeadSettingsChange
        /// </summary>
        public Vector3 RightEyeOffset
        {
            get { return rightEyeOffset; }
            set
            {
                rightEyeOffset = value;
                OnHeadSettingsChange();
            }
        }

        /// <summary>
        /// Returns HeadTracking Enabled whilst calling OnHeadSettingsChange
        /// </summary>
        public bool HeadTracking
        {
            get { return isHeadTracking; }
            set
            {
                isHeadTracking = value;
                if (!isHeadTracking)
                {
                    transform.localPosition = positionCached;
                    transform.localEulerAngles = rotationCached;
                }
            }
        }

        /// <summary>
        /// Near Clip Plane for created cameras
        /// </summary>
        public float NearClipPlane { get { return nearClipPlane; } set { nearClipPlane = value; } }
        private float nearClipPlane = 0.03f;

        /// <summary>
        /// Far clip plane for created cameras
        /// </summary>
        public float FarClipPlane { get { return farClipPlane; } set { farClipPlane = value; } }
        private float farClipPlane = 1000f;

        /// <summary>
        /// Writes the current Head Settings to the Igloo Settings xml
        /// </summary>
        /// <param name="hs">A populated Head Settings class</param>
        public void SetSettings(HeadSettings hs)
        {
            transform.localPosition = hs.headPositionOffset.Vector3;
            positionCached = hs.headPositionOffset.Vector3;
            transform.localEulerAngles = hs.headRotationOffset.Vector3;
            rotationCached = hs.headRotationOffset.Vector3;
            isHeadTracking = hs.headtracking;
            if(hs.optitrackServerIP != "") optitrackServerIPAddress = hs.optitrackServerIP;
            if (hs.optitrackHeadRigidBodyID != 0) optitrackHeadRigidBodyID = hs.optitrackHeadRigidBodyID;
            headTrackingInput = (HeadTrackingInput)hs.headTrackingInput;
            if (hs.leftEyeOffset != null) leftEyeOffset = hs.leftEyeOffset.Vector3;
            if (hs.rightEyeOffset != null) rightEyeOffset = hs.rightEyeOffset.Vector3;

            IglooManager.instance.NetworkManager.OnHeadPosition += HandlePositonMessage;
            IglooManager.instance.NetworkManager.OnHeadRotation += HandleRotationMessage;

            if (headTrackingInput == HeadTrackingInput.Optitrack && isHeadTracking)
            {
                OptitrackRigidBodyIgloo optitrack = this.gameObject.AddComponent<OptitrackRigidBodyIgloo>();
                optitrack.Setup(optitrackHeadRigidBodyID, true, false, optitrackServerIPAddress, GetLocalIPAddress());
            }
            else if (headTrackingInput == HeadTrackingInput.VRPN && isHeadTracking)
            {
                UnityVRPN.TrackerSettings vrpn = this.gameObject.AddComponent<UnityVRPN.TrackerSettings>();
                vrpn.Setup("Head", 0, true, false, optitrackServerIPAddress);
            }
        }

        /// <summary>
        /// Handles an OSC message with a new position vector
        /// </summary>
        /// <param name="pos">Vector3, New Position</param>
        public virtual void HandlePositonMessage(Vector3 pos)
        {
            if (isHeadTracking && headTrackingInput != HeadTrackingInput.Optitrack) transform.localPosition = pos;
        }

        public string GetLocalIPAddress() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Handles an OSC message with a new rotation vector
        /// </summary>
        /// <param name="rotation">Vector3, New Euler Rotation</param>
        public virtual void HandleRotationMessage(Vector3 rotation)
        {
            if (isHeadTracking && headTrackingInput != HeadTrackingInput.Optitrack) transform.localEulerAngles = rotation;
        }

        /// <summary>
        /// Reads the Head Settings of the Igloo Settings XML
        /// </summary>
        /// <returns>A populated Head Settings class</returns>
        public HeadSettings GetSettings()
        {
            HeadSettings hs = new HeadSettings {
                // Use starting head position as head tracking is not friendly here
                headPositionOffset = new Vector3Item(positionCached),
                headRotationOffset = new Vector3Item(rotationCached),
                leftEyeOffset = new Vector3Item(leftEyeOffset),
                rightEyeOffset = new Vector3Item(rightEyeOffset),
                optitrackServerIP = optitrackServerIPAddress,
                optitrackHeadRigidBodyID = optitrackHeadRigidBodyID,
                headtracking = isHeadTracking,
                headTrackingInput = (int)headTrackingInput
            };
            return hs;
        }

        /// <summary>
        /// Creates a camera system (or Eye) based on Head Settings
        /// </summary>
        /// <param name="eye">Left, Right, or Center eye.</param>
        /// <param name="_name">Name of camera</param>
        /// <param name="rotation">Euler Rotation Vector for Camera</param>
        /// <param name="prefab">Camera Prefab to use as Instantiation</param>
        /// <param name="_parentTransform">Parent to attach the created camera to</param>
        /// <returns>A created 'Eye' Camera</returns>
        public Camera CreateEye(EYE eye, string _name, Vector3 rotation, GameObject prefab = null, Transform _parentTransform = null)
        {
            GameObject cameraGO;
            if (prefab != null) cameraGO = Instantiate(prefab) as GameObject;
            else cameraGO = new GameObject();
            cameraGO.name = _name + "_" + eye.ToString();
            if (cameraGO.GetComponent<AudioListener>()) Destroy(cameraGO.GetComponent<AudioListener>());
            if (!cameraGO.GetComponent<Camera>()) cameraGO.AddComponent<Camera>();
            Camera res = cameraGO.GetComponent<Camera>();

            switch (eye)
            {
                case EYE.LEFT:
                    GameObject camParent = new GameObject("Stereo Camera Pair for - " + _name);
                    camParent.transform.parent = transform;
                    camParent.transform.localPosition = Vector3.zero;
                    camParent.transform.localEulerAngles = rotation;

                    res.transform.parent = camParent.transform;
                    res.transform.localEulerAngles = Vector3.zero;
                    break;
                case EYE.CENTER:
                    res.stereoTargetEye = StereoTargetEyeMask.None;
                    res.transform.parent = transform;
                    res.transform.localEulerAngles = rotation;
                    break;
                case EYE.RIGHT:
                    if (_parentTransform)
                    {
                        res.transform.parent = _parentTransform.transform;
                        res.transform.localEulerAngles = Vector3.zero;
                    }
                    else Debug.LogError("Parent object for Right eye does not exist");
                    break;
                default:
                    break;
            }

            ApplyCameraSettings(res, eye);
            return res;
        }

        /// <summary>
        /// Sets the eye seperation and updates the head settings
        /// </summary>
        /// <param name="value">new Eye offset value</param>
        public void SetEyeSeparation(float value)
        {
            leftEyeOffset = new Vector3(-value / 2, 0, 0);
            rightEyeOffset = new Vector3(value / 2, 0, 0);
            OnHeadSettingsChange();
        }

        /// <summary>
        /// Applies any updated settings to the created camera
        /// Useful for changing the eye of a camera without re-instantiating it.
        /// </summary>
        /// <param name="cam">The camera to apply the settings to</param>
        /// <param name="eye">Which Eye this camera now represents </param>
        public void ApplyCameraSettings(Camera cam, EYE eye)
        {
            cam.nearClipPlane = NearClipPlane;
            cam.farClipPlane = FarClipPlane;
            switch (eye)
            {
                case EYE.LEFT:
                    cam.transform.localPosition = leftEyeOffset;
                    break;
                case EYE.CENTER:
                    cam.transform.localPosition = CenterEyeOffset;
                    break;
                case EYE.RIGHT:
                    cam.transform.localPosition = rightEyeOffset;
                    break;
            }
        }
    }
}

