using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBoundary : MonoBehaviour
{
    public float maxDistance = 400f; // Maximum allowed distance from the origin

    void Update()
    {
        // Calculate the vector from the origin to the player's current position
        Vector3 offset = transform.position - Vector3.zero;

        // Check if the player is beyond the maximum allowed distance
        if (offset.magnitude > maxDistance)
        {
            // Clamp the player's position to the maximum distance from the origin
            transform.position = Vector3.zero + Vector3.ClampMagnitude(offset, maxDistance);
        }
    }
}
