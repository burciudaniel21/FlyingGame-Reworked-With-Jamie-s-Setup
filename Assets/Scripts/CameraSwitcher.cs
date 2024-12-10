using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    private Camera[] cameras; // Array to store all cameras
    private int currentCameraIndex = 0; // Index to track the current active camera
    [SerializeField]
    private int initialCamera = 0;

    void Start()
    {
        // Find all active cameras in the scene
        UpdateCameraList();
        // Set cuurent Camera to initialCamera if it exists.
        if (initialCamera > cameras.Length || initialCamera < 0) initialCamera = 0;
        ActivateCamera(initialCamera);
    }

    void Update()
    {
        // Switch cameras when the player presses the Tab key
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("Start Button"))
        {
            SwitchCamera();
        }
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

    // Switches to the next camera 
    public void SwitchCamera()
    {
        if (cameras == null || cameras.Length == 0) return;

        //Switch between cameras 0 and 1
        currentCameraIndex = (currentCameraIndex == 0) ? 1 : 0;


        // Activate the next camera
        ActivateCamera(currentCameraIndex);
    }
}
