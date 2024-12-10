using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    public float maxDistance = 350f; // Maximum allowed distance from the origin
    public float returnSpeed = 10f;  // Speed at which the player returns to the origin

    void Update()
    {
        // Calculate the distance from the origin
        float distanceFromOrigin = Vector3.Distance(transform.position, Vector3.zero);

        // If the player exceeds the maximum distance
        if (distanceFromOrigin > maxDistance)
        {
            // Determine the direction back to the origin
            Vector3 directionToOrigin = (Vector3.zero - transform.position).normalized;

            // Smoothly rotate towards the origin
            Quaternion targetRotation = Quaternion.LookRotation(directionToOrigin);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * returnSpeed);

            // Move the player towards the origin
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, returnSpeed * Time.deltaTime);
        }
    }
}
