using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    public float maxDistance = 350f; // Maximum allowed distance from the origin
    public float returnSpeed = 10f;  // Speed at which the player returns to the origin
    public float turnSpeed = 2f;     // Speed at which the plane turns toward the center
    public float dampingFactor = 0.9f; // Damping factor to reduce velocity for smoother turning

    private Vector3 currentVelocity; // Tracks the plane's current velocity

    void Update()
    {
        // Calculate the distance from the origin
        float distanceFromOrigin = Vector3.Distance(transform.position, Vector3.zero);

        // If the plane exceeds the maximum distance
        if (distanceFromOrigin > maxDistance)
        {
            // Determine the direction back to the origin
            Vector3 directionToOrigin = (Vector3.zero - transform.position).normalized;

            // Gradually rotate the plane to face the center
            Quaternion targetRotation = Quaternion.LookRotation(directionToOrigin);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Reduce velocity gradually to prepare for the return loop
            currentVelocity *= dampingFactor;

            // Add a force toward the center to create a graceful loop effect
            currentVelocity += directionToOrigin * returnSpeed * Time.deltaTime;
        }
        else
        {
            // Allow normal movement if within bounds
            currentVelocity = transform.forward * returnSpeed;
        }

        // Move the plane based on the calculated velocity
        transform.position += currentVelocity * Time.deltaTime;
    }
}
