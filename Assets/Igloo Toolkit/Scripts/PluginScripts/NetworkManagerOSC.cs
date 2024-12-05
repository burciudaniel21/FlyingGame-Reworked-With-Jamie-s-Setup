using UnityEngine;
#if !UNITY_WEBGL && EXT_OSC
using extOSC;
#endif

namespace Igloo.Common
{
    /// <summary>
    /// The Igloo Network Manager OSC Class
    /// </summary>
    public class NetworkManagerOSC : NetworkManager
    {
#if !UNITY_WEBGL  && EXT_OSC
        /// <summary>
        /// The extOSC Receiver Component
        /// </summary>
        [HideInInspector] public OSCReceiver oscIn;

        /// <summary>
        /// The extOSC Transmitter Component
        /// </summary>
        [HideInInspector] public OSCTransmitter oscOut;

        /// <summary>
        /// Awake function once Initialized. 
        /// Stops the object being destroyed on scene changes.
        /// </summary>
        protected override void AwakeInternal()
        {
            if (Application.isPlaying && transform.parent == null && IglooManager.instance) DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Setup Override Function. 
        /// Sets up the extOSC transmitter and reciever, and binds the required events to incoming OSC messages.
        /// </summary>
        public override void Setup()
        {
            base.Setup();

            if (!oscIn) oscIn = gameObject.AddComponent<OSCReceiver>();
            oscIn.LocalPort = inPort;
            oscIn.Connect();

            if (!oscOut) oscOut = gameObject.AddComponent<OSCTransmitter>();
            // oscOut.LocalHost = outIp; - Not required
            oscOut.RemoteHost = outIp;
            oscOut.RemotePort = outPort;

            // Igloo Player messages
            oscIn.Bind("/gameEngine/*/localPosition", PlayerPositionMessage);
            oscIn.Bind("/gameEngine/*/localEulerRotation", PlayerRotationMessage);
            oscIn.Bind("/gameEngine/*/walkSpeed", PlayerWalkSpeedMessage);
            oscIn.Bind("/gameEngine/*/runSpeed", PlayerRunSpeedMessage);
            oscIn.Bind("/gameEngine/*/rotationInput", PlayerRotationInputMessage);
            oscIn.Bind("/gameEngine/*/rotationMode", PlayerRotationModeMessage);
            oscIn.Bind("/gameEngine/*/movementInput", PlayerMovementInputMessage);
            oscIn.Bind("/gameEngine/*/movementMode", PlayerMovementModeMessage);
            oscIn.Bind("/gameEngine/*/smoothTime", PlayerSmoothTimeMessage);
            oscIn.Bind("/gameEngine/*/dPadValue", PlayerDPadMovementMessage);
            oscIn.Bind("/gameEngine/*/xboxActionValue", PlayerActionEventMessage);

            // Legacy messages for rotation data
            oscIn.Bind("/ximu/euler", WarperRotationMessage);

            // GyrOSC app messages 
            oscIn.Bind("/gyrosc/player/gyro", GYROSCRotationMessage);
            oscIn.Bind("/gyrosc/player/button", GYROSCButtonMessage);

            // Igloo Manager messages
            oscIn.Bind("/gameEngine/camera/verticalFOV", VerticalFOVMessage);
            oscIn.Bind("/gameEngine/camera/horizontalFOV", HorizontalFOVMessage);
            oscIn.Bind("/gameEngine/camera/setDisplaysEnabled", SetDisplaysEnabledMessage);
            oscIn.Bind("/gameEngine/camera/farClipDistance", FarClipMessage);
            oscIn.Bind("/gameEngine/camera/nearClipDistance", NearClipMessage);
            oscIn.Bind("/gameEngine/camera/eyeSeparation", EyeSeparationMessage);

            // Tracker Messages
            oscIn.Bind("/gameEngine/*/headPosition", HeadPositionMessage);
            oscIn.Bind("/gameEngine/*/headRotation", HeadRotationMessage);

            // Controller messages
            oscIn.Bind("/openVRController/*/button/*", OpenVrButtonMessage);
            oscIn.Bind("/openVRController/*/pad/*", OpenVrPadPositionMessage);
            oscIn.Bind("/openVRController/*/trigger/*", OpenVrTriggerMessage);
            oscIn.Bind("/openVRController/*/gyroEuler/*", OpenVrControllerGyroMessage);
            oscIn.Bind("/openVRController/*/position/*", OpenVrControllerPositionMessage);
            oscIn.Bind("/openVRTracker/*/gyroEuler/*", OpenVrTrackerGyroMessage);
            oscIn.Bind("/openVRTracker/*/postition/*", OpenVrTrackerPositionMessage);

            // System
            oscIn.Bind("/gameEngine/quit", QuitMessage);
            oscIn.Bind("/gameEngine/getInfo", GetInfoMessage);

            // Touch Input control
            oscIn.Bind("/gameEngine/*/canvasPosition", OnTouchInputPositionMessage);
            oscIn.Bind("/gameEngine/*/click", OnTouchInputClickMessage);
        }

        /// <summary>
        /// Override for Send Message Function. 
        /// Sends an OSC message via the extOSC transmitter.
        /// </summary>
        /// <param name="msg"></param>
        public override void SendMessage(Igloo.Common.NetworkMessage msg)
        {
            var msgOSC = new OSCMessage(msg.address);
            foreach (object obj in msg.arguments)
            {
                if (obj is int intVal)
                {
                    msgOSC.AddValue(OSCValue.Int(intVal));
                }
                else if (obj is float floatVal)
                {
                    msgOSC.AddValue(OSCValue.Float(floatVal));
                }
                else if (obj is bool boolVal)
                {
                    msgOSC.AddValue(OSCValue.Bool(boolVal));
                }
                else if (obj is string stringVal)
                {
                    msgOSC.AddValue(OSCValue.String(stringVal));
                }
                else Debug.Log($"<b>[Igloo]</b> Unknown argument type in OSC message: {msg.address}");
            }
            oscOut.Send(msgOSC);
        }

        #region Player events
        private void PlayerWalkSpeedMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerWalkSpeed?.Invoke(playerName, msg.Values[0].FloatValue);
        }

        private void PlayerRunSpeedMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerRunSpeed?.Invoke(playerName, msg.Values[0].FloatValue);
        }

        private void PlayerRotationMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerRotation?.Invoke(playerName, new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        }

        private void WarperRotationMessage(OSCMessage msg)
        {
            OnPlayerRotationWarper?.Invoke("player", new Vector3(msg.Values[0].FloatValue, -msg.Values[2].FloatValue, msg.Values[1].FloatValue));
        }

        private void PlayerPositionMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerPosition?.Invoke(playerName, new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        }

        private void PlayerRotationInputMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerRotationInput?.Invoke(playerName, msg.Values[0].IntValue);
        }

        private void PlayerRotationModeMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerRotationMode?.Invoke(playerName, msg.Values[0].IntValue);
        }

        private void PlayerMovementInputMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerMovementInput?.Invoke(playerName, msg.Values[0].IntValue);
        }

        private void PlayerMovementModeMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerMovementMode?.Invoke(playerName, msg.Values[0].IntValue);
        }

        private void PlayerSmoothTimeMessage(OSCMessage msg)
        {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerSmoothTime?.Invoke(playerName, msg.Values[0].FloatValue);
        }

        private void PlayerDPadMovementMessage(OSCMessage msg) {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerDPadMovement?.Invoke(playerName, new Vector2(msg.Values[0].FloatValue, msg.Values[1].FloatValue));
        }

        private void PlayerActionEventMessage(OSCMessage msg) {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            OnPlayerActionEvent?.Invoke(playerName, msg.Values[0].BoolValue, msg.Values[1].BoolValue, msg.Values[2].BoolValue, msg.Values[3].BoolValue);
        }
        #endregion

        #region Manager events
        private void HorizontalFOVMessage(OSCMessage msg)
        {
            base.SetHorizontalFOV(msg.Values[0].FloatValue);
        }

        private void VerticalFOVMessage(OSCMessage msg)
        {
            base.SetVerticalFOV(msg.Values[0].FloatValue);
        }

        private void SetDisplaysEnabledMessage(OSCMessage msg)
        {
            if (GetBoolValue(msg.Values[0], out bool state)) base.SetDisplaysEnabled(state);
        }

        private void NearClipMessage(OSCMessage msg)
        {
            base.SetNearClip(msg.Values[0].FloatValue);
        }

        private void FarClipMessage(OSCMessage msg)
        {
            base.SetFarClip(msg.Values[0].FloatValue);
        }

        private void EyeSeparationMessage(OSCMessage msg)
        {
            base.SetEyeSeparation(msg.Values[0].FloatValue);
        }

        private void QuitMessage(OSCMessage msg)
        {
            base.Quit();
        }

        private void GetInfoMessage(OSCMessage msg)
        {
            OSCMessage newMessage = new OSCMessage("/gameEngine/info");
            newMessage.AddValue(OSCValue.String(Igloo.Utils.GetVersion()));
            if (oscOut) oscOut.Send(newMessage);
        }

        private void SetUIVisibleMessage(OSCMessage msg)
        {
            OnSetUIVisible?.Invoke(msg.Values[0].BoolValue);
        }

        #endregion

        #region GyrOSC events
        void GYROSCButtonMessage(OSCMessage message)
        {
            Vector3 movementAxis = new Vector3();
            if ((int)message.Values[1].IntValue == 1 && (int)message.Values[0].IntValue == 1) movementAxis.x = 1;
            if ((int)message.Values[1].IntValue == 0 && (int)message.Values[0].IntValue == 1) movementAxis.x = 0;
            if ((int)message.Values[1].IntValue == 1 && (int)message.Values[0].IntValue == 2) movementAxis.x = -1;
            if ((int)message.Values[1].IntValue == 0 && (int)message.Values[0].IntValue == 2) movementAxis.x = 0;

            if ((int)message.Values[1].IntValue == 1 && (int)message.Values[0].IntValue == 3) movementAxis.z = -1;
            if ((int)message.Values[1].IntValue == 0 && (int)message.Values[0].IntValue == 3) movementAxis.z = 0;
            if ((int)message.Values[1].IntValue == 1 && (int)message.Values[0].IntValue == 9) movementAxis.z = 1;
            if ((int)message.Values[1].IntValue == 0 && (int)message.Values[0].IntValue == 9) movementAxis.z = 0;

            if ((int)message.Values[1].IntValue == 1 && (int)message.Values[0].IntValue == 8) movementAxis.y = -1;
            if ((int)message.Values[1].IntValue == 0 && (int)message.Values[0].IntValue == 8) movementAxis.y = 0;
            if ((int)message.Values[1].IntValue == 1 && (int)message.Values[0].IntValue == 7) movementAxis.y = 1;
            if ((int)message.Values[1].IntValue == 0 && (int)message.Values[0].IntValue == 7) movementAxis.y = 0;

            OnButtonGYROSC?.Invoke("player", movementAxis);
        }

        void GYROSCRotationMessage(OSCMessage message)
        {
            Vector3 rotation = new Vector3();
            float x = message.Values[0].FloatValue;
            float z = message.Values[1].FloatValue;
            float y = message.Values[2].FloatValue;

            rotation.x = -((x / (Mathf.PI / 2)) * 65.0f);
            rotation.y = -(y / Mathf.PI) * 180.0f;
            rotation.z = -((z / (Mathf.PI / 2)) * 65.0f);

            OnRotationGYROSC?.Invoke("player", rotation);
        }
        #endregion

        #region Head Tracking events
        private void HeadPositionMessage(OSCMessage msg)
        {
            OnHeadPosition?.Invoke(new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        }
        private void HeadRotationMessage(OSCMessage msg)
        {
            OnHeadRotation?.Invoke(new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        }
        #endregion

        #region OpenVR events
        private void OpenVrButtonMessage(OSCMessage msg)
        {
            int deviceID = -1;
            int buttonID = -1;
            int.TryParse(Utils.StringSplitter(msg.Address, new char[] { '/' }, 2), out deviceID);
            int.TryParse(Utils.StringSplitter(msg.Address, new char[] { '/' }, 4), out buttonID);
            if (!msg.Values[0].BoolValue)
            {
                msg.Values[0].BoolValue = msg.Values[0].FloatValue == 1.0f;
            }
            OnVrButtonEvent?.Invoke(deviceID, buttonID, msg.Values[0].BoolValue);
        }
        private void OpenVrPadPositionMessage(OSCMessage msg)
        {
            int deviceID = -1;
            int.TryParse(Utils.StringSplitter(msg.Address, new char[] { '/' }, 2), out deviceID);
            OnVrPadPositionEvent?.Invoke(deviceID, new Vector2(msg.Values[0].FloatValue, msg.Values[1].FloatValue));
        }
        private void OpenVrTriggerMessage(OSCMessage msg)
        {
            int deviceID = -1;
            int.TryParse(Utils.StringSplitter(msg.Address, new char[] { '/' }, 2), out deviceID);
            OnVrTriggerEvent?.Invoke(deviceID, msg.Values[0].FloatValue);
        }
        private void OpenVrControllerGyroMessage(OSCMessage msg)
        {
            int deviceID = -1;
            int.TryParse(Utils.StringSplitter(msg.Address, new char[] { '/' }, 2), out deviceID);
            OnVrControllerGyroEvent?.Invoke(deviceID, new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        }
        private void OpenVrControllerPositionMessage(OSCMessage msg)
        {
            int deviceID = -1;
            int.TryParse(Utils.StringSplitter(msg.Address, new char[] { '/' }, 2), out deviceID);
            OnVrControllerPositionEvent?.Invoke(deviceID, new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        }
        private void OpenVrTrackerPositionMessage(OSCMessage msg)
        {
            int deviceID = -1;
            int.TryParse(Utils.StringSplitter(msg.Address, new char[] { '/' }, 2), out deviceID);
            OnVrTrackerPositionEvent?.Invoke(deviceID, new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        }
        private void OpenVrTrackerGyroMessage(OSCMessage msg)
        {
            int deviceID = -1;
            int.TryParse(Utils.StringSplitter(msg.Address, new char[] { '/' }, 2), out deviceID);
            OnVrTrackerGyroEvent?.Invoke(deviceID, new Vector3(msg.Values[0].FloatValue, msg.Values[1].FloatValue, msg.Values[2].FloatValue));
        }
        #endregion

        #region OSC Utility functions
        /// <summary>
        /// Retuns an Int Value if the OSCValue contains an int
        /// </summary>
        /// <param name="value">OSC Input Value</param>
        /// <param name="i">int. Output int value</param>
        /// <returns>bool. True = OSCValue was an Integer</returns>
        private bool GetIntValue(OSCValue value, out int i)
        {
            bool ok = false;
            switch (value.Type)
            {
                case OSCValueType.Int:
                    i = value.IntValue;
                    ok = true;
                    break;
                case OSCValueType.True:
                    i = 1;
                    ok = true;
                    break;
                case OSCValueType.False:
                    i = 0;
                    ok = true;
                    break;
                case OSCValueType.Float:
                    i = (int)value.FloatValue;
                    ok = true;
                    break;
            }
            i = -1;
            Debug.LogWarning("<b>[Igloo]</b> Can't convert OSC parameter, likely incorrect type or value sent");
            return ok;
        }

        /// <summary>
        /// Retuns a float Value if the OSCValue contains a float
        /// </summary>
        /// <param name="value">OSC Input Value</param>
        /// <param name="f">float. Output float value</param>
        /// <returns>bool. True = OSCValue was a float</returns>
        private bool GetFloatValue(OSCValue value, out float f)
        {
            bool ok = false;
            switch (value.Type)
            {
                case OSCValueType.Float:
                    f = value.FloatValue;
                    ok = true;
                    break;
                case OSCValueType.True:
                    f = 1.0f;
                    ok = true;
                    break;
                case OSCValueType.False:
                    f = 0.0f;
                    ok = true;
                    break;
                case OSCValueType.Int:
                    f = (float)value.IntValue;
                    break;
            }
            f = -1.0f;
            Debug.LogWarning("<b>[Igloo]</b> Can't convert OSC parameter, likely incorrect type or value sent");
            return ok;
        }

        /// <summary>
        /// Retuns a bool Value if the OSCValue contains a bool
        /// </summary>
        /// <param name="value">OSC Input Value</param>
        /// <param name="b">bool. Output bool value</param>
        /// <returns>bool. True = OSCValue was a bool</returns>
        private bool GetBoolValue(OSCValue value, out bool b)
        {
            bool ok = false;
            switch (value.Type)
            {
                case OSCValueType.Int:
                    if (value.IntValue == 0)
                    {
                        b = false;
                        ok = true;
                    }
                    else if (value.IntValue == 1)
                    {
                        b = true;
                        ok = true;
                    }
                    break;
                case OSCValueType.True:
                    b = true;
                    ok = true;
                    break;
                case OSCValueType.False:
                    b = false;
                    ok = true;
                    break;
                case OSCValueType.Float:
                    if (value.FloatValue == 0.0f)
                    {
                        b = false;
                        ok = true;
                    }
                    else if (value.FloatValue == 1.0f)
                    {
                        b = true;
                        ok = true;
                    }
                    break;
            }
            b = false;
            Debug.LogWarning("<b>[Igloo]</b> Can't convert OSC parameter, likely incorrect type or value sent");
            return ok;
        }
        #endregion

        #region Touch Input System

        private void OnTouchInputPositionMessage(OSCMessage msg) {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            if (OnTouchInputPosition != null) OnTouchInputPosition(playerName, new Vector2(msg.Values[0].FloatValue, msg.Values[1].FloatValue));
        }

        private void OnTouchInputClickMessage(OSCMessage msg) {
            string playerName = Utils.StringSplitter(msg.Address, new char[] { '/' }, 2);
            /*if (OnTouchInputClick != null && msg.Values[0].FloatValue == 1.0f)*/ OnTouchInputClick(playerName);
            //else if (OnTouchInputClick != null && msg.Values[0].FloatValue == 0.0f) OnTouchInputRelease(playerName);
        }

        #endregion

        /// <summary>
        /// On Destroy Function.
        /// Unbinds all extOSC bound events.
        /// </summary>
        public override void OnDestroy()
        {
            if (oscIn) oscIn.ClearBinds();
        }
#endif       
    }

}
