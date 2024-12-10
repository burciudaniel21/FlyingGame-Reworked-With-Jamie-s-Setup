using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Material[] skyboxes; // Array to hold the skybox materials
    private int currentSkyboxIndex = 0; // Index to track the current skybox

    void Update()
    {
        // Check if the "X" button on the controller is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            // Increment the skybox index and wrap around if it exceeds the array length
            currentSkyboxIndex = (currentSkyboxIndex + 1) % skyboxes.Length;

            // Set the new skybox
            RenderSettings.skybox = skyboxes[currentSkyboxIndex];

            // Update ambient lighting based on the new skybox
            DynamicGI.UpdateEnvironment();
        }
    }
}
