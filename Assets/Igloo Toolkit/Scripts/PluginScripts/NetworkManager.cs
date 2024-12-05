using System.Collections;
using UnityEngine;

namespace Igloo.Common
{
#pragma warning disable IDE0090

    /// <summary>
    /// The Igloo Network Message Class
    /// </summary>
    public class NetworkMessage
    {

        /// <summary>
        /// OSC message address
        /// </summary>
        public string address;

        /// <summary>
        /// OSC message argument array
        /// </summary>
        public ArrayList arguments = new ArrayList();

        /// <summary>
        /// Adds an Int Argument to the OSC Message
        /// </summary>
        /// <param name="value">The int value to add</param>
        public void AddIntArgument(int value) { arguments.Add(value); }

        /// <summary>
        /// Adds an float Argument to the OSC Message
        /// </summary>
        /// <param name="value">The float value to add</param>
        public void AddFloatArgument(float value) { arguments.Add(value); }

        /// <summary>
        /// Adds an string Argument to the OSC Message
        /// </summary>
        /// <param name="value">The string value to add</param>
        public void AddStringArgument(string value) { arguments.Add(value); }

        /// <summary>
        /// Adds an bool Argument to the OSC Message
        /// </summary>
        /// <param name="value">The bool value to add</param>
        public void AddBoolArgument(bool value) { arguments.Add(value); }
    }

    /// <summary>
    /// The Igloo Network Manager class
    /// </summary>
    public class NetworkManager : Singleton<NetworkManager>
    {
        /// <summary>
        /// Port used for OSC input
        /// </summary>
        [HideInInspector] public int inPort = 9007;

        /// <summary>
        /// Port used for OSC output
        /// </summary>
        [HideInInspector] public int outPort = 9001;

        /// <summary>
        /// IP address for OSC output
        /// </summary>
        [HideInInspector] public string outIp = "127.0.0.1";

        /// <summary>
        /// On Initialized, stops the NetworkManager being destroyed.
        /// </summary>
        /// <remarks>
        /// Possibly superfluous as the Network Manager is a required component of the 
        /// IglooManager, which has it's own DontDestroyOnLoad function.
        /// </remarks>
        protected override void AwakeInternal()
        {
            if (Application.isPlaying && transform.parent == null) DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Setup function used to implement OSC senders and receivers.
        /// Used to bind events. 
        /// </summary>
        /// <see cref="Igloo.NetworkManagerOSC.Setup"/>
        public virtual void Setup()
        {
            Debug.Log("<b>[Igloo]</b> Setting up NetworkManager");
        }

        /// <summary>
        /// Sets the network settings part of the IglooSettings XML.
        /// </summary>
        /// <param name="ns">The Network Settings class</param>
        public virtual void SetSettings(NetworkSettings ns)
        {
            
            if (ns == null) return;
            if (ns.inPort != 0) inPort = ns.inPort;
            if (ns.outPort != 0) outPort = ns.outPort;
            if (!string.IsNullOrEmpty(ns.outIP)) outIp = ns.outIP;
        }

        /// <summary>
        /// Get function to return the network settings from the IglooSettings XML
        /// </summary>
        /// <returns>A populated network Settings class reference</returns>
        public virtual NetworkSettings GetSettings()
        {
            NetworkSettings ns = new NetworkSettings
            {
                inPort = inPort,
                outPort = outPort,
                outIP = outIp
            };
            return ns;
        }

        /// <summary>
        /// Send network message 
        /// </summary>
        /// <example>
        /// Igloo.NetworkMessage msg = new NetworkMessage();
        /// msg.address = "/test";
        /// msg.AddIntArgument(1);
        /// msg.AddFloatArgument(4.0f);
        /// msg.AddStringArgument("Test!");
        /// msg.AddBoolArgument(true);
        /// networkManager.SendMessage(msg);
        /// </example>
        /// <remarks>
        /// Can be overriden, see NetworkManagerOSC for an example.
        /// </remarks>
        /// <param name="msg"></param>
        public virtual void SendMessage(Igloo.Common.NetworkMessage msg) { }

        #region Player events
        /// <summary>
        /// Player Walk Speed Event
        /// </summary>
        public WalkSpeed OnPlayerWalkSpeed;
        /// <summary>
        /// Delegate for Player Walk Speed Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="speed">float. New walk speed</param>
        public delegate void WalkSpeed(string name, float speed);

        /// <summary>
        /// Player Run Speed Event
        /// </summary>
        public RunSpeed OnPlayerRunSpeed;
        /// <summary>
        /// Delegate for Player Run Speed Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="speed">float. New run speed</param>
        public delegate void RunSpeed(string name, float speed);

        /// <summary>
        /// Player Rotation Event
        /// </summary>
        public PlayerRotation OnPlayerRotation;
        /// <summary>
        /// Delegate for Player Rotation Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="rotation">Vector3. Euler rotation input.</param>
        public delegate void PlayerRotation(string name, Vector3 rotation);

        /// <summary>
        /// Player Rotation Warper Event
        /// </summary>
        public PlayerRotationWarper OnPlayerRotationWarper;
        /// <summary>
        /// Delegate for Player Rotation Warper Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="rotation">Vector3. Euler rotation input from the warper.</param>
        public delegate void PlayerRotationWarper(string name, Vector3 rotation);

        /// <summary>
        /// Player Position Event
        /// </summary>
        public PlayerPosition OnPlayerPosition;
        /// <summary>
        /// Delegate for Player Position Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="position">Vector3. New player position input</param>
        public delegate void PlayerPosition(string name, Vector3 position);

        /// <summary>
        /// Player Rotation Input Event
        /// </summary>
        public PlayerRotationInput OnPlayerRotationInput;
        /// <summary>
        /// Delegate for Player Rotation Input Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="value">int. New Rotation Input Type for Player</param>
        public delegate void PlayerRotationInput(string name, int value);

        /// <summary>
        /// Player Rotation Mode Event
        /// </summary>
        public PlayerRotationMode OnPlayerRotationMode;
        /// <summary>
        /// Delegate for Player Rotation Mode Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="value">int. New Rotation Mode Type for Player</param>
        public delegate void PlayerRotationMode(string name, int value);

        /// <summary>
        /// Player Movement Input Event
        /// </summary>
        public PlayerMovementInput OnPlayerMovementInput;
        /// <summary>
        /// Delegate for Player Movement Input Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="value">int. New Movement Type for Player</param>
        public delegate void PlayerMovementInput(string name, int value);

        /// <summary>
        /// Player Movement Mode Input Event
        /// </summary>
        public PlayerMovmentMode OnPlayerMovementMode;
        /// <summary>
        /// Delegate for Player Movement Mode Input Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="value">int. New Movement Mode Type for Player</param>
        public delegate void PlayerMovmentMode(string name, int value);

        /// <summary>
        /// Player Smooth Time Input Event
        /// </summary>
        public PlayerSmoothTime OnPlayerSmoothTime;
        /// <summary>
        /// Delegate for Player Smooth Time Input Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="value">float. New smooth time value for player</param>
        public delegate void PlayerSmoothTime(string name, float value);

        /// <summary>
        /// Player D Pad movement OSC event
        /// </summary>
        public PlayerDPadMovement OnPlayerDPadMovement;

        /// <summary>
        /// Delegate for Player input from OSC DPad (Cast and Control)
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="value">Vector 2. 0-1 movement of dPad</param>
        public delegate void PlayerDPadMovement(string name, Vector2 value);


        /// <summary>
        /// Player Action button OSC event
        /// </summary>
        public PlayerActionEvent OnPlayerActionEvent;

        /// <summary>
        /// Delegate void for Player Input of Action buttons (Cast and Control) 
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="Xvalue">Bool. True = X button Pressed</param>
        /// <param name="Yvalue">Bool. True = Y button Pressed</param>
        /// <param name="Avalue">Bool. True = A button Pressed</param>
        /// <param name="Bvalue">Bool. True = B button Pressed</param>
        public delegate void PlayerActionEvent(string name, bool Xvalue, bool Yvalue, bool Avalue, bool Bvalue);
        #endregion

        #region GyrOSC events
        /// <summary>
        /// GyrOSC Button Event
        /// </summary>
        public ButtonGYROSC OnButtonGYROSC;
        /// <summary>
        /// Delegate for GYROSC Event
        /// </summary>
        /// <param name="name">string. Name of GyrOSC Device</param>
        /// <param name="movement">Vector3. Position of Button press</param>
        public delegate void ButtonGYROSC(string name, Vector3 movement);

        /// <summary>
        /// GyrOSC Rotation Event
        /// </summary>
        public RotationGYROSC OnRotationGYROSC;
        /// <summary>
        /// Delegate for GyrOSC Event
        /// </summary>
        /// <param name="name">string. Name of GyrOSC Device</param>
        /// <param name="rotation">Vector3. Euler rotation of GyrOSC Device</param>
        public delegate void RotationGYROSC(string name, Vector3 rotation);
        #endregion

        #region Head Tracking events
        /// <summary>
        /// Head Position event
        /// </summary>
        public HeadPosition OnHeadPosition;
        /// <summary>
        /// Delegate for Head Position Event
        /// </summary>
        /// <param name="postion">Vector3. World space position of the Head Object</param>
        public delegate void HeadPosition(Vector3 postion);

        /// <summary>
        /// Head Rotation Event
        /// </summary>
        public HeadRotation OnHeadRotation;
        /// <summary>
        /// Delegate for Head Rotation Event
        /// </summary>
        /// <param name="rotation">Vector3. Euler rotation of the Head Object</param>
        public delegate void HeadRotation(Vector3 rotation);
        #endregion

        #region OpenVR events
        /// <summary>
        /// VR Button Event
        /// </summary>
        public VrButtonEvent OnVrButtonEvent;
        /// <summary>
        /// Delegate for VR Button Event
        /// </summary>
        /// <param name="deviceID">int. The Device ID</param>
        /// <param name="buttonID">int. The Button ID</param>
        /// <param name="state">bool. State of the Button. True = pressed</param>
        public delegate void VrButtonEvent(int deviceID, int buttonID, bool state);

        /// <summary>
        /// VR Pad Position Event
        /// </summary>
        public VrPadPositionEvent OnVrPadPositionEvent;
        /// <summary>
        /// Delegate for VR Pad Position Event
        /// </summary>
        /// <param name="deviceID">int. The Device ID</param>
        /// <param name="value">Vector2. The current Pad Position</param>
        public delegate void VrPadPositionEvent(int deviceID, Vector2 value);

        /// <summary>
        /// VR Trigger Event
        /// </summary>
        public VrTriggerEvent OnVrTriggerEvent;
        /// <summary>
        /// Delegate for VR Trigger Event
        /// </summary>
        /// <param name="deviceID">int. The Device ID</param>
        /// <param name="value">float. Trigger Pressed Percentage. 0 - 1</param>
        public delegate void VrTriggerEvent(int deviceID, float value);

        /// <summary>
        /// VR Controller Gyro Event
        /// </summary>
        public VrControllerGyroEvent OnVrControllerGyroEvent;
        /// <summary>
        /// Delegate for VR Controller Gyro Event.
        /// </summary>
        /// <param name="deviceID">int. The Device ID</param>
        /// <param name="rotationEuler">Vector3. The XYZ gyro rotation of the VR Controller.</param>
        public delegate void VrControllerGyroEvent(int deviceID, Vector3 rotationEuler);

        /// <summary>
        /// VR Controller Position Event
        /// </summary>
        public VrControllerPositionEvent OnVrControllerPositionEvent;
        /// <summary>
        /// Delegate for VR Controller Position Event
        /// </summary>
        /// <param name="deviceID">int. The Device ID</param>
        /// <param name="position">Vector3. The world space position of the VR Controller</param>
        public delegate void VrControllerPositionEvent(int deviceID, Vector3 position);

        /// <summary>
        /// VR Tracker Gyro Event
        /// </summary>
        public VrTrackerGyroEvent OnVrTrackerGyroEvent;
        /// <summary>
        /// Delegate for VR Tracker Gyro Event
        /// </summary>
        /// <param name="deviceID">int. The Device ID</param>
        /// <param name="rotationEuler">Vector3. The XYZ gyro rotation of the VR Tracker.</param>
        public delegate void VrTrackerGyroEvent(int deviceID, Vector3 rotationEuler);

        /// <summary>
        /// VR Tracker Position Event
        /// </summary>
        public VrTrackerPositionEvent OnVrTrackerPositionEvent;
        /// <summary>
        /// Delegate for VR Tracker Position Event
        /// </summary>
        /// <param name="deviceID">int. The Device ID</param>
        /// <param name="position">Vector3. The world space position of the VR Tracker.</param>
        public delegate void VrTrackerPositionEvent(int deviceID, Vector3 position);

        #endregion

        #region Manager events
        /// <summary>
        /// Sets the Horizontal Field of View
        /// </summary>
        /// <param name="val">float. Horizontal Field of View degrees</param>
        protected void SetHorizontalFOV(float val)
        {
            if (IglooManager.instance.igloo)
            {
                IglooManager.instance.igloo.GetComponent<DisplayManager>().HorizontalFOV = val;
            }
        }

        /// <summary>
        /// Sets the Vertical Field of View 
        /// </summary>
        /// <param name="val">float. The vertical Field of View degrees</param>
        protected void SetVerticalFOV(float val)
        {
            if (IglooManager.instance.igloo)
            {
                IglooManager.instance.igloo.GetComponent<DisplayManager>().VerticalFOV = val;
            }
        }

        /// <summary>
        /// Enables or Disables the IglooToolkit Output
        /// </summary>
        /// <param name="val">bool. True = Outputs active. </param>
        protected void SetDisplaysEnabled(bool val)
        {
            if (IglooManager.instance.igloo)
            {
                IglooManager.instance.igloo.GetComponent<DisplayManager>().SetDisplaysEnabled(val);
            }
        }

        /// <summary>
        /// Sets the camera's near clip value. 
        /// </summary>
        /// <param name="val">float. Near clip value</param>
        protected void SetNearClip(float val)
        {
            if (IglooManager.instance.igloo)
            {
                IglooManager.instance.igloo.GetComponent<DisplayManager>().SetNearClip(val);
            }
        }

        /// <summary>
        /// Sets the camera's far clip value
        /// </summary>
        /// <param name="val">float. Far clip value</param>
        protected void SetFarClip(float val)
        {
            if (IglooManager.instance.igloo)
            {
                IglooManager.instance.igloo.GetComponent<DisplayManager>().SetFarClip(val);
            }
        }

        /// <summary>
        /// Sets the camera eye seperation for 3D
        /// </summary>
        /// <param name="val">float. Amount of eye seperation.</param>
        protected void SetEyeSeparation(float val)
        {
            if (IglooManager.instance.igloo)
            {
                IglooManager.instance.igloo.GetComponent<DisplayManager>().SetEyeSeparation(val);
            }
        }

        /// <summary>
        /// Sets the Igloo Canvas UI visible.
        /// </summary>
        public SetUIVisible OnSetUIVisible;
        /// <summary>
        /// Delegate void for OnSetUIVisible
        /// </summary>
        /// <param name="state">bool. True = Igloo UI is Visible.</param>
        public delegate void SetUIVisible(bool state);

        /// <summary>
        /// Quits the Unity Application when called.
        /// </summary>
        /// <remarks>
        /// Necessary for Igloo Home functionality, as an OSC message can be received to terminate the program.
        /// </remarks>
        protected void Quit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Called from an OSC message input. Returns information to the sender regarding the IglooToolkit plugin version.
        /// </summary>
        /// <remarks>
        /// Could also be used to send important information to the warper: i.e. The applications FPS.  
        /// </remarks>
        /// <see cref="Igloo.NetworkManagerOSC"/>
        protected virtual void GetInfo()
        {

        }
        #endregion

        #region Touch Screen Events

        /// <summary>
        /// Touch Control Input Position Event
        /// </summary>
        public TouchInputPosition OnTouchInputPosition;
        /// <summary>
        /// Delegate for Touch Control Input Position Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        /// <param name="position">Vector2. New touch position input</param>
        public delegate void TouchInputPosition(string name, Vector2 position);

        /// <summary>
        /// Touch Click Event
        /// </summary>
        public TouchInputClick OnTouchInputClick;
        /// <summary>
        /// Delegate for Touch Click Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        public delegate void TouchInputClick(string name);

        /// <summary>
        /// Touch Click Event
        /// </summary>
        public TouchInputRelease OnTouchInputRelease;
        /// <summary>
        /// Delegate for Touch Click Event
        /// </summary>
        /// <param name="name">string. Player Name</param>
        public delegate void TouchInputRelease(string name);

        #endregion


        /// <summary>
        /// Mono on Destroy method. Used to unbind OSC events.
        /// </summary>
        /// <see cref="Igloo.NetworkManagerOSC.OnDestroy"/>
        public virtual void OnDestroy() { }

    }

}
