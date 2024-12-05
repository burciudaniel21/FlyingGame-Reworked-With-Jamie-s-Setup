using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

namespace UnityVRPN
{
    public class TrackerSettings : MonoBehaviour
    {
        [SerializeField]
        private TrackerHostSettings hostSettings;
        [SerializeField]
        private string objectName = "";
        [SerializeField]
        private int channel = 0;
        [SerializeField]
        private bool trackPosition = true;
        [SerializeField]
        private bool trackRotation = true;

        public TrackerHostSettings HostSettings
        {
            get { return hostSettings; }
            set
            {
                hostSettings = value;
            }
        }

        public string ObjectName
        {
            get { return objectName; }
            set
            {
                objectName = value;
            }
        }

        public int Channel
        {
            get { return channel; }
            set
            {
                channel = value;
            }
        }

        public bool TrackPosition
        {
            get { return trackPosition; }
            set
            {
                trackPosition = value;
                StopCoroutine("Position");
                if (trackPosition && Application.isPlaying)
                {
                    StartCoroutine("Position");
                }
            }
        }

        public bool TrackRotation
        {
            get { return trackRotation; }
            set
            {
                trackRotation = value;
                StopCoroutine("Rotation");
                if (trackRotation && Application.isPlaying)
                {
                    StartCoroutine("Rotation");
                }
            }
        }

        public void Setup(string _objectName, int _channel = 0, bool _trackPostion = true, bool _trackRotation = true, string _host = "127.0.0.1")
        {
            objectName = _objectName;
            channel = _channel;
            trackPosition = _trackPostion;
            trackRotation = _trackRotation;

            if (hostSettings == null)
            {
                TrackerHostSettings[] allClients = FindObjectsOfType<TrackerHostSettings>();

                if (allClients.Length == 0)
                {
                    Debug.Log("Unable to locate any " + typeof(TrackerHostSettings).FullName + " components. Adding one now");
                    hostSettings = gameObject.AddComponent<TrackerHostSettings>();
                }
                else if (allClients.Length == 1)
                {
                    Debug.LogWarning("Exisiting " + typeof(TrackerHostSettings).FullName + " component found in scene; default to use this.");
                    hostSettings = allClients[0];
                }
                else if (allClients.Length > 1)
                {
                    Debug.LogWarning("Multiple " + typeof(TrackerHostSettings).FullName + " components found in scene; defaulting to first available.");
                    hostSettings = allClients[0];
                }
            }
            hostSettings.Hostname = _host;

            if (trackPosition)
            {
                StartCoroutine("Position");
            }

            if (trackRotation)
            {
                StartCoroutine("Rotation");
            }
        }

        private IEnumerator Position()
        {
            while (true)
            {
                Vector3 pos = hostSettings.GetPosition(objectName, channel);
                //Debug.Log("Got position" + pos);
                transform.localPosition = pos;
                yield return null;
            }
        }

        private IEnumerator Rotation()
        {
            while (true)
            {
                Quaternion q = hostSettings.GetRotation(objectName, channel);
                transform.localEulerAngles = new Vector3(q.eulerAngles.x, -q.eulerAngles.y, q.eulerAngles.z);
                yield return null;
            }
        }
    }
}
