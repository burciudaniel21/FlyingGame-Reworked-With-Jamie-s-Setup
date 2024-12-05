using UnityEngine;

/// <summary>
/// Simple Rotate object class.
/// Will rotate any object it's a component of, on it's Up Axis.
/// </summary>
public class Rotate : MonoBehaviour
{
#pragma warning disable IDE0051 // Remove unused private members

    /// <summary>
    /// Rotation speed of this object
    /// </summary>
    float speed = 0;

    /// <summary>
    /// Mono Update Function: 
    /// Sets speed if it's 0. 
    /// Rotates this gameobject based on Speed * delta time
    /// </summary>
	void Update()
    {
        if (speed == 0) speed = Random.Range(-100, 100);
        this.transform.Rotate(transform.TransformDirection(Vector3.up), speed * Time.deltaTime);
    }
}
