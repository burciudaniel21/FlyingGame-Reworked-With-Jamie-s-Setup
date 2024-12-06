using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 50f; // Base speed of the plane
    public float pitchSpeed = 50f; // Pitch control speed
    public float rollSpeed = 50f; // Roll control speed
    public float yawSpeed = 20f; // Yaw control speed
    public float throttleSpeed = 10f; // Throttle adjustment speed
    public float maxSpeed = 100f; // Maximum speed
    public float minSpeed = 10f; // Minimum speed

    private float throttleInput = 0f;

    [Header("Input Settings")]
    public string pitchAxis = "Vertical"; // Default for keyboard is "Vertical"
    public string rollAxis = "Horizontal"; // Default for keyboard is "Horizontal"
    public string yawAxis = "Yaw"; // Custom input axis for yaw
    public string throttleAxis = "Throttle"; // Custom input axis for throttle (optional for controller)

    private void Update()
    {
        HandleThrottle();
        HandleFlightControls();
    }

    private void HandleThrottle()
    {
        // Throttle adjustment with keyboard keys or controller axis
        if (Input.GetAxis(throttleAxis) != 0)
        {
            throttleInput -= Input.GetAxis(throttleAxis) * throttleSpeed * Time.deltaTime; // Invert controller throttle
        }
        else
        {
            throttleInput -= Input.GetKey(KeyCode.W) ? throttleSpeed * Time.deltaTime : 0f; // W decreases throttle
            throttleInput += Input.GetKey(KeyCode.S) ? throttleSpeed * Time.deltaTime : 0f; // S increases throttle
        }

        throttleInput = Mathf.Clamp(throttleInput, minSpeed, maxSpeed);
    }

    private void HandleFlightControls()
    {
        float pitch = Input.GetAxis(pitchAxis) * -pitchSpeed * Time.deltaTime; // Inverted pitch
        float roll = Input.GetAxis(rollAxis) * rollSpeed * Time.deltaTime;
        float yaw = Input.GetAxis(yawAxis) * yawSpeed * Time.deltaTime;

        // Apply rotation to the plane
        transform.Rotate(Vector3.right, -pitch);
        transform.Rotate(Vector3.up, yaw);
        transform.Rotate(Vector3.forward, -roll);

        // Move forward based on throttle
        transform.position += transform.forward * throttleInput * Time.deltaTime;
    }
}
