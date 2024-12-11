using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectToggleController : MonoBehaviour
{
    public GameObject object1; // Assign the first GameObject in the Inspector
    public GameObject object2; // Assign the second GameObject in the Inspector

    [Header("Input Action")]
    public InputActionReference switchVehicleAction; // Reference to the SwitchVehicle action

    private void OnEnable()
    {
        // Enable the input action
        switchVehicleAction.action.Enable();
        switchVehicleAction.action.performed += OnSwitchVehiclePerformed;
    }

    private void OnDisable()
    {
        // Disable the input action
        switchVehicleAction.action.Disable();
        switchVehicleAction.action.performed -= OnSwitchVehiclePerformed;
    }

    private void OnSwitchVehiclePerformed(InputAction.CallbackContext context)
    {
        ToggleObjects();
    }

    private void ToggleObjects()
    {
        bool isObject1Active = object1.activeSelf;

        object1.SetActive(!isObject1Active); // Toggle object1 (e.g., the plane)
        object2.SetActive(isObject1Active);  // Toggle object2 (e.g., the balloon)

        // Refresh the camera list to ensure active cameras are included
        CameraSwitcher cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        if (cameraSwitcher != null)
        {
            cameraSwitcher.UpdateCameraList();
            cameraSwitcher.ActivateIglooCamera(); // Ensure the Igloo camera is active by default
        }
    }

    // Utility method to disable a child camera
    private void DisableChildCamera(GameObject parentObject)
    {
        if (parentObject != null)
        {
            Camera childCamera = parentObject.GetComponentInChildren<Camera>(true);
            if (childCamera != null)
            {
                childCamera.enabled = false;
                childCamera.gameObject.SetActive(false); // Prevent Unity from activating it
            }
        }
    }
}
