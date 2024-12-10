using UnityEngine;
using UnityEngine.InputSystem;

public class SkyboxChanger : MonoBehaviour
{
    public Material[] skyboxes; // Array to hold the skybox materials
    private int currentSkyboxIndex = 0; // Index to track the current skybox

    [Header("Input Action")]
    public InputActionReference switchSkyboxAction; // Reference to the SwitchSkybox action

    private void OnEnable()
    {
        // Enable the input action
        switchSkyboxAction.action.Enable();
        switchSkyboxAction.action.performed += OnSwitchSkyboxPerformed;
    }

    private void OnDisable()
    {
        // Disable the input action
        switchSkyboxAction.action.Disable();
        switchSkyboxAction.action.performed -= OnSwitchSkyboxPerformed;
    }

    private void OnSwitchSkyboxPerformed(InputAction.CallbackContext context)
    {
        // Increment the skybox index and wrap around if it exceeds the array length
        currentSkyboxIndex = (currentSkyboxIndex + 1) % skyboxes.Length;

        // Set the new skybox
        RenderSettings.skybox = skyboxes[currentSkyboxIndex];

        // Update ambient lighting based on the new skybox
        DynamicGI.UpdateEnvironment();
    }
}
