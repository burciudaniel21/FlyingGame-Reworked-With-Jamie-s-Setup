using UnityEngine;
using Igloo.Common;

namespace Igloo.Controllers
{
#pragma warning disable IDE0090 // Use New()...
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members

    /// <summary>
    /// Igloo Tracker Controller class
    /// </summary>
    public class Tracker : MonoBehaviour
    {
        /// <summary>
        /// If True, Position is being tracked
        /// </summary>
        public bool TrackPosition;

        /// <summary>
        /// If True, Rotation is being tracked
        /// </summary>
        public bool TrackRotation;

        /// <summary>
        /// A position offset for the tracker.
        /// </summary>
        public Vector3 positionOffset;

        /// <summary>
        /// A rotation offset for the tracker
        /// </summary>
        public Vector3 rotationOffset;

        /// <summary>
        /// Mono Start Function
        /// Binds position and rotation OSC messages to internal events
        /// </summary>
        private void Start()
        {
            IglooManager.instance.NetworkManager.OnHeadPosition += HandlePositonMessage;
            IglooManager.instance.NetworkManager.OnHeadRotation += HandleRotationMessage;
        }

        /// <summary>
        /// Change the position of the tracker object based on incoming OSC message data
        /// </summary>
        /// <param name="pos">Vector3, new position</param>
        public virtual void HandlePositonMessage(Vector3 pos)
        {
            this.transform.localPosition = pos + positionOffset;
        }

        /// <summary>
        /// Change the rotation of the tracker object based on incoming OSC message data
        /// </summary>
        /// <param name="rotation">Vector3, Euler new rotation</param>
        public virtual void HandleRotationMessage(Vector3 rotation)
        {
            this.transform.localEulerAngles = rotation + rotationOffset;
        }

    }


}

