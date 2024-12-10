using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CameraSwitcher : MonoBehaviour
{
    private List<Camera> toggleCameras = new List<Camera>(); // List to store cameras to toggle
    private int currentCameraIndex = 0; // Index to track the current active camera

    [Header("Camera References")]
    public Camera manuallyPlacedCamera; // Manually assigned camera in the Inspector
    public string iglooCameraPath = "IglooManager/Igloo(Clone)/Head/DefaultCamera/Camera"; // Path to the dynamically generated Igloo camera
    public Transform planeCameraTransform; // Reference to the plane's camera (optional)
    public Transform balloonCameraTransform; // Reference to the hot air balloon's camera (optional)

    [Header("Input Actions")]
    public InputActionReference switchCameraAction; // Reference to the SwitchCamera action

    private void OnEnable()
    {
        // Enable the input action
        switchCameraAction.action.Enable();
        switchCameraAction.action.performed += OnSwitchCameraPerformed;
    }

    private void OnDisable()
    {
        // Disable the input action
        switchCameraAction.action.Disable();
        switchCameraAction.action.performed -= OnSwitchCameraPerformed;
    }

    private void Start()
    {
        // Populate the camera list
        UpdateCameraList();

        // Validate the camera list
        if (toggleCameras.Count == 0)
        {
            Debug.LogError("No cameras found for toggling!");
            return;
        }

        // Activate the first camera and deactivate others
        ActivateCamera(currentCameraIndex);
    }

    // Updates the list of cameras dynamically
    public void UpdateCameraList()
    {
        toggleCameras.Clear();

        // Add the manually placed camera
        if (manuallyPlacedCamera != null && manuallyPlacedCamera.isActiveAndEnabled)
        {
            toggleCameras.Add(manuallyPlacedCamera);
        }

        // Add the dynamically generated Igloo camera
        Camera iglooCamera = GameObject.Find(iglooCameraPath)?.GetComponent<Camera>();
        if (iglooCamera != null && iglooCamera.isActiveAndEnabled)
        {
            toggleCameras.Add(iglooCamera);
        }

        // Add the plane camera if available and active
        if (planeCameraTransform != null)
        {
            Camera planeCamera = planeCameraTransform.GetComponentInChildren<Camera>();
            if (planeCamera != null && planeCamera.isActiveAndEnabled)
            {
                toggleCameras.Add(planeCamera);
            }
        }

        // Add the balloon camera if available and active
        if (balloonCameraTransform != null)
        {
            Camera balloonCamera = balloonCameraTransform.GetComponentInChildren<Camera>();
            if (balloonCamera != null && balloonCamera.isActiveAndEnabled)
            {
                toggleCameras.Add(balloonCamera);
            }
        }

        // Adjust the current camera index if the active camera was removed
        if (currentCameraIndex >= toggleCameras.Count)
        {
            currentCameraIndex = 0;
        }
    }

    // Activates the camera at the given index
    private void ActivateCamera(int index)
    {
        for (int i = 0; i < toggleCameras.Count; i++)
        {
            bool shouldEnable = i == index;
            toggleCameras[i].gameObject.SetActive(shouldEnable);
            toggleCameras[i].enabled = shouldEnable;
        }
    }

    // Handles the input action to switch the camera
    private void OnSwitchCameraPerformed(InputAction.CallbackContext context)
    {
        SwitchCamera();
    }

    // Switches to the next camera in the toggle list
    public void SwitchCamera()
    {
        if (toggleCameras.Count == 0) return;

        // Increment the camera index, looping back to 0 if it exceeds the list count
        currentCameraIndex = (currentCameraIndex + 1) % toggleCameras.Count;

        // Activate the next camera
        ActivateCamera(currentCameraIndex);
    }
}
