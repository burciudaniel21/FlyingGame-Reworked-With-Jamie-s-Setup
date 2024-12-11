using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CameraSwitcher : MonoBehaviour
{
    public List<Camera> toggleCameras = new List<Camera>(); // List to store cameras to toggle
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

        // Explicitly deactivate all cameras except the Igloo camera
        foreach (var camera in toggleCameras)
        {
            if (camera != toggleCameras[0]) // Igloo camera is index 0
            {
                camera.enabled = false;
                camera.gameObject.SetActive(false);
            }
        }

        // Activate the Igloo camera (always index 0)
        ActivateIglooCamera();
    }


    // Updates the list of cameras dynamically
    public void UpdateCameraList()
    {
        toggleCameras.Clear();

        // Add the dynamically generated Igloo camera
        Camera iglooCamera = GameObject.Find(iglooCameraPath)?.GetComponent<Camera>();
        if (iglooCamera != null)
        {
            toggleCameras.Add(iglooCamera);
            Debug.Log($"Added Igloo camera: {iglooCamera.name}");
        }
        else
        {
            Debug.LogWarning("Igloo camera not found!");
        }

        // Add the manually placed FollowCamera
        if (manuallyPlacedCamera != null)
        {
            toggleCameras.Add(manuallyPlacedCamera);
            Debug.Log($"Added manually placed camera: {manuallyPlacedCamera.name}");
        }
        else
        {
            Debug.LogWarning("Manually placed camera not assigned.");
        }

        // Add the plane camera
        GameObject plane = GameObject.Find("Plane");
        if (plane != null)
        {
            Camera planeCamera = plane.GetComponentInChildren<Camera>(true); // Include inactive objects
            if (planeCamera != null)
            {
                toggleCameras.Add(planeCamera);
                Debug.Log($"Added plane camera: {planeCamera.name}");
            }
            else
            {
                Debug.LogWarning("Plane camera not found!");
            }
        }

        // Add the balloon camera
        GameObject balloon = GameObject.Find("Balloon");
        if (balloon != null)
        {
            Camera balloonCamera = balloon.GetComponentInChildren<Camera>(true); // Include inactive objects
            if (balloonCamera != null)
            {
                toggleCameras.Add(balloonCamera);
                Debug.Log($"Added balloon camera: {balloonCamera.name}");
            }
            else
            {
                Debug.LogWarning("Balloon camera not found!");
            }
        }

        Debug.Log($"Total cameras in toggleCameras: {toggleCameras.Count}");
    }






    // Activates the camera at the given index
    private void ActivateCamera(int index)
    {
        for (int i = 0; i < toggleCameras.Count; i++)
        {
            bool shouldEnable = i == index;

            // Activate/deactivate GameObject and enable/disable Camera component
            toggleCameras[i].gameObject.SetActive(shouldEnable);
            toggleCameras[i].enabled = shouldEnable;

            // Update the MainCamera tag
            if (shouldEnable)
            {
                toggleCameras[i].tag = "MainCamera";
                Debug.Log($"Tagged {toggleCameras[i].name} as MainCamera.");
            }
            else if (toggleCameras[i].tag == "MainCamera")
            {
                toggleCameras[i].tag = "Untagged";
                Debug.Log($"Untagged {toggleCameras[i].name}.");
            }

            if (shouldEnable)
            {
                Debug.Log($"Activated camera: {toggleCameras[i].name}");
            }
            else
            {
                Debug.Log($"Deactivated camera: {toggleCameras[i].name}");
            }
        }

        currentCameraIndex = index; // Update the current camera index
    }





    // Activates the Igloo camera (always index 0)
    public void ActivateIglooCamera()
    {
        if (toggleCameras.Count > 0)
        {
            ActivateCamera(0); // Force Igloo camera activation
        }
        else
        {
            Debug.LogWarning("No cameras available to activate.");
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
