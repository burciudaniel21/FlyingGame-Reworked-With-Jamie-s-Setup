using UnityEngine;
#if ESRI
using Esri.HPFramework;
using Unity.Mathematics;
#endif


namespace Igloo.Common
{

    /// <summary>
    /// The Igloo Follow Object Transform class
    /// </summary>
    public class FollowObjectTransform : MonoBehaviour
    {
        /// <summary>
        /// Follow Object Mode
        /// 0 - Object
        /// 1 - Main Camera in scene
        /// </summary>
        public enum Follow { OBJECT, MAIN_CAMERA }

        /// <summary>
        /// Current Follow Object Mode
        /// </summary>
        public Follow followType = Follow.OBJECT;

        /// <summary>
        /// Object in the scene to follow
        /// </summary>
        public GameObject followObject;

        /// <summary>
        /// Transform in the scene to follow
        /// </summary>
#if !ESRI
        private Transform followTransform;
#else
        private HPTransform followTransform;

        /// <summary>
        /// Transform of this object
        /// </summary>
        public HPTransform thisTransform;
#endif
        /// <summary>
        /// If True, Follow the position
        /// </summary>
        public bool followPosition;

        /// <summary>
        /// If True, Follow the rotation
        /// </summary>
        public bool followRotation;

        /// <summary>
        /// If True, Follow the scale
        /// </summary>
        public bool followScale;

        /// <summary>
        /// The position vector - Choose which axis of the position should be followed
        /// </summary>
        public enum PositionVector { XYZ, X, Y, Z, XY, XZ, YZ };

        /// <summary>
        /// Current Position vector
        /// </summary>
        public PositionVector positionVector = PositionVector.XYZ;

        /// <summary>
        /// The rotation vector. - Choose which axis of the rotaion should be followed
        /// </summary>
        public enum RotationVector { XYZ, X, Y, Z, XY, XZ, YZ };

        /// <summary>
        /// Current rotation vector
        /// </summary>
        public RotationVector rotationVector = RotationVector.XYZ;

        /// <summary>
        /// The scale vector. - Choose which axis of scale should be followed
        /// </summary>
        public enum ScaleVector { XYZ, X, Y, Z, XY, XZ, YZ };

        /// <summary>
        /// Current scale vector
        /// </summary>
        public ScaleVector scaleVector = ScaleVector.XYZ;

        /// <summary>
        /// Adds an offset to the position follow system
        /// </summary>
#if ESRI
        public double3 positionOffset;
#else
        public Vector3 positionOffset;
#endif

        /// <summary>
        /// Mono Late Update Function
        /// Sets the follow type
        /// Activates all follow functions based on bool switches.
        /// </summary>
        void LateUpdate()
        {
            switch (followType)
            {
#if ESRI
                case Follow.MAIN_CAMERA:
                    followTransform = Camera.main.GetComponent<HPTransform>();
                    break;
                case Follow.OBJECT:
                    followTransform = followObject.GetComponent<HPTransform>();
                    break;
#else
                case Follow.MAIN_CAMERA:
                    followTransform = Camera.main.transform;
                    break;
                case Follow.OBJECT:
                    followTransform = followObject.transform;
                    break;
#endif
                default:
                    break;
            }
#if ESRI
            if (!followTransform) Debug.Log("No Follow Object Found");
#else
            if (!followTransform) followTransform = Camera.main.transform;
#endif
            if (followPosition) SetPositionTransform();
            if (followRotation) SetRotationTransform();
            if (followScale) SetScaleTrasform();
        }

        /// <summary>
        /// Based on the Position vector, mimics the follow object's position to this object.
        /// </summary>
        void SetPositionTransform()
        {
            // Position 
            switch (positionVector)
            {
#if ESRI
                case PositionVector.XYZ:
                    thisTransform.LocalPosition = followTransform.LocalPosition + positionOffset;
                    break;
                case PositionVector.X:
                    thisTransform.LocalPosition = new double3(followTransform.LocalPosition.x, thisTransform.LocalPosition.y, thisTransform.LocalPosition.z) + positionOffset;
                    break;
                case PositionVector.Y:
                    thisTransform.LocalPosition = new double3(thisTransform.LocalPosition.x, followTransform.LocalPosition.y, thisTransform.LocalPosition.z) + positionOffset;
                    break;
                case PositionVector.Z:
                    thisTransform.LocalPosition = new double3(thisTransform.LocalPosition.x, thisTransform.LocalPosition.y, followTransform.LocalPosition.z) + positionOffset;
                    break;
                case PositionVector.XY:
                    thisTransform.LocalPosition = new double3(followTransform.LocalPosition.x, followTransform.LocalPosition.y, thisTransform.LocalPosition.z) + positionOffset;
                    break;
                case PositionVector.XZ:
                    thisTransform.LocalPosition = new double3(followTransform.LocalPosition.x, thisTransform.LocalPosition.y, followTransform.LocalPosition.z) + positionOffset;
                    break;
                case PositionVector.YZ:
                    thisTransform.LocalPosition = new double3(thisTransform.LocalPosition.x, followTransform.LocalPosition.y, followTransform.LocalPosition.z) + positionOffset;
                    break;
#else
                case PositionVector.XYZ:
                    this.transform.position = new Vector3(followTransform.position.x, followTransform.position.y, followTransform.position.z) + positionOffset;
                    break;
                case PositionVector.X:
                    this.transform.position = new Vector3(followTransform.position.x, transform.position.y, transform.position.z) + positionOffset;
                    break;
                case PositionVector.Y:
                    this.transform.position = new Vector3(transform.position.x, followTransform.position.y, transform.position.z) + positionOffset;
                    break;
                case PositionVector.Z:
                    this.transform.position = new Vector3(transform.position.x, transform.position.y, followTransform.position.z) + positionOffset;
                    break;
                case PositionVector.XY:
                    this.transform.position = new Vector3(followTransform.position.x, followTransform.position.y, transform.position.z) + positionOffset;
                    break;
                case PositionVector.XZ:
                    this.transform.position = new Vector3(followTransform.position.x, transform.position.y, followTransform.position.z) + positionOffset;
                    break;
                case PositionVector.YZ:
                    this.transform.position = new Vector3(transform.position.x, followTransform.position.y, followTransform.position.z) + positionOffset;
                    break;
#endif
                default:
                    Debug.Log("<b>[Igloo]</b> Incorrect Position Vector");
                    break;
            }
        }

        /// <summary>
        /// Based on the Rotation Vector, mimics the follow object's rotation to this object.
        /// </summary>
        void SetRotationTransform()
        {

            switch (rotationVector)
            {
#if ESRI

#else
                case RotationVector.XYZ:
                    this.transform.eulerAngles = new Vector3(followTransform.eulerAngles.x, followTransform.eulerAngles.y, followTransform.eulerAngles.z);
                    break;
                case RotationVector.X:
                    this.transform.eulerAngles = new Vector3(followTransform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                    break;
                case RotationVector.Y:
                    this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, followTransform.eulerAngles.y, transform.eulerAngles.z);
                    break;
                case RotationVector.Z:
                    this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, followTransform.eulerAngles.z);
                    break;
                case RotationVector.XY:
                    this.transform.eulerAngles = new Vector3(followTransform.eulerAngles.x, followTransform.eulerAngles.y, transform.eulerAngles.z);
                    break;
                case RotationVector.XZ:
                    this.transform.eulerAngles = new Vector3(followTransform.eulerAngles.x, transform.eulerAngles.y, followTransform.eulerAngles.z);
                    break;
                case RotationVector.YZ:
                    this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, followTransform.eulerAngles.y, followTransform.eulerAngles.z);
                    break;
#endif
                default:
                    Debug.Log("<b>[Igloo]</b> Incorrect Rotation Vector");
                    break;
            }
        }

        /// <summary>
        /// Based on the scale vector, mimics the follow object's scale to this object
        /// </summary>
        void SetScaleTrasform()
        {
            switch (scaleVector)
            {
#if ESRI

#else
                case ScaleVector.XYZ:
                    this.transform.localScale = new Vector3(followTransform.localScale.x, followTransform.localScale.y, followTransform.localScale.z);
                    break;
                case ScaleVector.X:
                    this.transform.localScale = new Vector3(followTransform.localScale.x, transform.localScale.y, transform.localScale.z);
                    break;
                case ScaleVector.Y:
                    this.transform.localScale = new Vector3(transform.localScale.x, followTransform.localScale.y, transform.localScale.z);
                    break;
                case ScaleVector.Z:
                    this.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, followTransform.localScale.z);
                    break;
                case ScaleVector.XY:
                    this.transform.localScale = new Vector3(followTransform.localScale.x, followTransform.localScale.y, transform.localScale.z);
                    break;
                case ScaleVector.XZ:
                    this.transform.localScale = new Vector3(followTransform.localScale.x, transform.localScale.y, followTransform.localScale.z);
                    break;
                case ScaleVector.YZ:
                    this.transform.localScale = new Vector3(transform.localScale.x, followTransform.localScale.y, followTransform.localScale.z);
                    break;
#endif
                default:
                    Debug.Log("<b>[Igloo]</b> Incorrect Scale Vector");
                    break;
            }
        }
    }
}

