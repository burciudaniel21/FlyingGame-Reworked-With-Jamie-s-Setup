using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The player or object the camera will orbit around
    public float rotationSpeed = 100f; // Speed of rotation

    private float horizontalInput;
    private float verticalInput;

    void Update()
    {
        // Rotate around the target based on input
        transform.RotateAround(target.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
        transform.RotateAround(target.position, transform.right, -verticalInput * rotationSpeed * Time.deltaTime);

        // Maintain camera's offset from the target
        transform.LookAt(target);
    }
}
