using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    private Camera[] cameras; // Array to store all cameras
    private int currentCameraIndex = 0; // Index to track the current active camera
    [SerializeField]
    private int initialCamera = 0;

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
        // Find all active cameras in the scene
        UpdateCameraList();

        // Set current camera to initialCamera if it exists
        if (initialCamera >= cameras.Length || initialCamera < 0)
            initialCamera = 0;
        ActivateCamera(initialCamera);
    }

    // Updates the list of cameras (in case cameras are added dynamically)
    public void UpdateCameraList()
    {
        cameras = Camera.allCameras;
    }

    // Activates the camera at the given index
    private void ActivateCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == index);
        }
    }

    // Handles the input action to switch the camera
    private void OnSwitchCameraPerformed(InputAction.CallbackContext context)
    {
        SwitchCamera();
    }

    // Switches to the next camera
    public void SwitchCamera()
    {
        if (cameras == null || cameras.Length == 0) return;

        // Switch between cameras
        currentCameraIndex = (currentCameraIndex == 0) ? 1 : 0;

        // Activate the next camera
        ActivateCamera(currentCameraIndex);
    }
}

