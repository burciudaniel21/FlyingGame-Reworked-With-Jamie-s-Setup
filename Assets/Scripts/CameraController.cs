using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> targets; // List of possible targets

    private Transform currentTarget; // The currently active target

    void Update()
    {
        // Find the first active target in the list
        foreach (Transform target in targets)
        {
            if (target != null && target.gameObject.activeSelf)
            {
                currentTarget = target;
                break; // Stop checking after finding the first active target
            }
        }

        // Rotate to face the active target
        if (currentTarget != null)
        {
            RotateTowardsTarget();
        }
    }

    private void RotateTowardsTarget()
    {
        // Make the camera look at the current target
        Vector3 directionToTarget = currentTarget.position - transform.position;
        transform.rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
    }
}
