using UnityEngine;

public class BoundaryEnforcer : MonoBehaviour
{
    [Header("Settings")]
    public Transform[] objectsToConstrain; // Array of objects to constrain
    public float maxDistance = 400f;       // Maximum allowed distance from the origin
    public float returnSpeed = 10f;        // Speed at which objects return to the origin
    public float rotationSpeed = 180f;     // Rotation speed in degrees per second

    void Update()
    {
        foreach (Transform obj in objectsToConstrain)
        {
            if (obj == null) continue;

            // Calculate the distance from the origin
            float distanceFromOrigin = Vector3.Distance(obj.position, Vector3.zero);

            // If the object exceeds the maximum distance
            if (distanceFromOrigin > maxDistance)
            {
                // Determine the direction back to the origin
                Vector3 directionToOrigin = (Vector3.zero - obj.position).normalized;

                // Calculate the target rotation to face the origin
                Quaternion targetRotation = Quaternion.LookRotation(directionToOrigin);

                // Smoothly rotate towards the target rotation
                obj.rotation = Quaternion.RotateTowards(obj.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Move the object towards the origin
                obj.position = Vector3.MoveTowards(obj.position, Vector3.zero, returnSpeed * Time.deltaTime);
            }
        }
    }
}
