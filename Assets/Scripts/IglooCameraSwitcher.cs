using UnityEngine;
using Igloo.Common; // Namespace for accessing IglooManager and its components

public class IglooCameraSwitcher : MonoBehaviour
{
    [Tooltip("Reference to the alternate camera prefab.")]
    public GameObject alternateCameraPrefab;

    private IglooManager iglooManager;

    void Start()
    {
        // Find the IglooManager in the scene
        iglooManager = FindObjectOfType<IglooManager>();

        if (iglooManager == null)
        {
            Debug.LogError("<b>[IglooCameraSwitcher]</b> IglooManager not found in the scene.");
        }
    }

    /// <summary>
    /// Switch to the alternate camera during gameplay.
    /// </summary>
    public void SwitchToAlternateCamera()
    {
        if (iglooManager == null || alternateCameraPrefab == null)
        {
            Debug.LogWarning("<b>[IglooCameraSwitcher]</b> Cannot switch camera. Missing references.");
            return;
        }

        // Destroy the current camera instance if it exists in the Igloo
        if (iglooManager.cameraPrefab != null)
        {
            var existingCamera = iglooManager.igloo?.GetComponentInChildren<Camera>();
            if (existingCamera != null)
            {
                Destroy(existingCamera.gameObject);
            }
        }

        // Instantiate the new camera prefab
        GameObject newCameraInstance = Instantiate(alternateCameraPrefab);
        newCameraInstance.name = "IglooAlternateCamera"; // Optional naming

        // Parent the new camera under the Igloo object if necessary
        if (iglooManager.igloo != null)
        {
            newCameraInstance.transform.parent = iglooManager.igloo.transform;
        }

        // Update the cameraPrefab reference in IglooManager's DisplayManager
        if (iglooManager.DisplayManager != null)
        {
            iglooManager.DisplayManager.cameraPrefab = newCameraInstance;
            iglooManager.DisplayManager.Setup(iglooManager.settings.DisplaySettings); // Reinitialize the display settings
        }
        else
        {
            Debug.LogError("<b>[IglooCameraSwitcher]</b> DisplayManager is null. Cannot switch camera.");
        }

        // Update any UI camera reference if needed
        if (iglooManager.canvasUI != null && iglooManager.canvasUI.renderMode == RenderMode.ScreenSpaceCamera)
        {
            var newCameraComponent = newCameraInstance.GetComponent<Camera>();
            if (newCameraComponent != null)
            {
                iglooManager.canvasUI.worldCamera = newCameraComponent;
            }
        }

        Debug.Log("<b>[IglooCameraSwitcher]</b> Switched to alternate camera.");
    }
}
