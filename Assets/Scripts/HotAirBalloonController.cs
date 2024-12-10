using UnityEngine;
using UnityEngine.InputSystem;

public class RealisticHotAirBalloon : MonoBehaviour
{
    [Header("Movement Settings")]
    public float gravity = -1f; // Gravity pulling the balloon down
    public float buoyancyForce = 2f; // Maximum buoyancy force
    public float coolingRate = 0.1f; // Rate at which the balloon cools down
    public float heatingRate = 0.5f; // Rate at which the balloon heats up
    public float moveSpeed = 5f; // Speed for horizontal movement
    public float maxAltitudeAboveCamera = 12f; // Max height above camera
    public float maxAltitudeBelowCamera = 150f; // Max depth below camera
    public float maxDistanceFromCamera = 150f; // Max horizontal distance from the camera

    [Header("Collision Settings")]
    public Collider cameraBoundingBox; // The collider representing the box around the camera

    [Header("Input Actions")]
    public InputActionReference moveAction; // Left stick for orbit/distance control
    public InputActionReference heatBalloonAction; // Button press for adding buoyancy

    public Transform cameraTransform; // Reference to the camera's transform

    private Vector2 moveInput; // Left stick input for orbit/distance control
    private bool isHeating = false; // Tracks if the heat button is being pressed
    private float verticalVelocity = 0f; // Current vertical speed
    private float buoyancyLevel = 0f; // Current buoyancy level (heating effect)

    private void OnEnable()
    {
        moveAction.action.Enable();
        heatBalloonAction.action.Enable();

        // Subscribe to button press/release events
        heatBalloonAction.action.performed += OnHeatBalloonPressed;
        heatBalloonAction.action.canceled += OnHeatBalloonReleased;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        heatBalloonAction.action.Disable();

        // Unsubscribe from button press/release events
        heatBalloonAction.action.performed -= OnHeatBalloonPressed;
        heatBalloonAction.action.canceled -= OnHeatBalloonReleased;
    }

    private void Update()
    {
        // Read input values
        moveInput = moveAction.action.ReadValue<Vector2>();

        // Update buoyancy
        UpdateBuoyancy();

        // Apply movement
        UpdatePosition();
    }

    private void OnHeatBalloonPressed(InputAction.CallbackContext context)
    {
        isHeating = true;
    }

    private void OnHeatBalloonReleased(InputAction.CallbackContext context)
    {
        isHeating = false;
    }

    private void UpdateBuoyancy()
    {
        // Add buoyancy when the heat button is pressed
        if (isHeating)
        {
            buoyancyLevel += heatingRate * Time.deltaTime;
        }
        else
        {
            // Cool down when the heat button is not pressed
            buoyancyLevel -= coolingRate * Time.deltaTime;
        }

        // Clamp buoyancy level to reasonable values
        buoyancyLevel = Mathf.Clamp(buoyancyLevel, 0f, buoyancyForce);
    }

    private void UpdatePosition()
    {
        // Apply vertical velocity based on gravity and buoyancy
        verticalVelocity += gravity * Time.deltaTime + buoyancyLevel * Time.deltaTime;

        // Get current height relative to the camera
        float heightRelativeToCamera = transform.position.y - cameraTransform.position.y;

        // Constrain altitude above the camera
        if (heightRelativeToCamera > maxAltitudeAboveCamera && verticalVelocity > 0)
        {
            verticalVelocity = Mathf.Lerp(verticalVelocity, 0, 0.1f); // Slow ascent
        }

        // Constrain altitude below the camera
        if (heightRelativeToCamera < -maxAltitudeBelowCamera && verticalVelocity < 0)
        {
            verticalVelocity = Mathf.Lerp(verticalVelocity, 0, 0.1f); // Slow descent
        }

        // Get the balloon's current position on the circular plane
        Vector3 toBalloon = transform.position - cameraTransform.position;
        toBalloon.y = 0; // Flatten to the horizontal plane

        // Adjust distance based on forward/back input (moveInput.y)
        float distance = toBalloon.magnitude;
        distance += moveInput.y * moveSpeed * Time.deltaTime;

        // Clamp the distance to the allowed range
        distance = Mathf.Clamp(distance, 0, maxDistanceFromCamera);

        // Adjust angle based on left/right input (moveInput.x)
        float angle = Mathf.Atan2(toBalloon.z, toBalloon.x); // Current angle in radians
        angle -= moveInput.x * moveSpeed * Time.deltaTime / distance; // Adjust angle

        // Calculate the new position on the circular plane
        Vector3 newPosition = new Vector3(
            Mathf.Cos(angle) * distance,
            transform.position.y,
            Mathf.Sin(angle) * distance
        ) + cameraTransform.position;

        // Add vertical movement
        newPosition.y += verticalVelocity * Time.deltaTime;

        // Enforce bounding box constraints
        if (cameraBoundingBox != null && cameraBoundingBox.bounds.Contains(newPosition) && newPosition.y >= cameraBoundingBox.bounds.min.y)
        {
            newPosition = cameraBoundingBox.ClosestPoint(newPosition);
        }

        // Apply the new position
        transform.position = newPosition;
    }
}
