using UnityEngine;

public class BoundaryEnforcer : MonoBehaviour
{
    [Header("Settings")]
    public Transform[] objects; // Array of objects to monitor
    public Vector3 origin = Vector3.zero; // Origin point
    public float maxDistance = 350f; // Maximum allowed distance from the origin
    public float returnSpeed = 5f; // Speed at which objects return to the origin

    void Update()
    {
        foreach (Transform obj in objects)
        {
            if (obj == null) continue;

            // Calculate the distance from the origin
            float distance = Vector3.Distance(obj.position, origin);

            // If the object exceeds the maximum distance
            if (distance > maxDistance)
            {
                // Determine the direction back to the origin
                Vector3 directionToOrigin = (origin - obj.position).normalized;

                // Rotate the object to face the origin
                obj.rotation = Quaternion.LookRotation(directionToOrigin);

                // Move the object towards the origin
                obj.position = Vector3.MoveTowards(obj.position, origin, returnSpeed * Time.deltaTime);
            }
        }
    }
}
