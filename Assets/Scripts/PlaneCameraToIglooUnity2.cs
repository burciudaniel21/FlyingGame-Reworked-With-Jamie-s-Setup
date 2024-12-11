using UnityEngine;

public class PlaneCameraToIglooUnity2 : MonoBehaviour
{
    public Camera planeCamera; // Assign your plane camera here

    private void Start()
    {
        // Ensure the plane camera is assigned
        if (planeCamera == null)
        {
            Debug.LogError("Plane camera is not assigned!");
            return;
        }

        // Assign the target display index for IglooUnity2 (Display 2 in Unity corresponds to index 1)
        planeCamera.targetDisplay = 1;

        // Activate the display if not already activated
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate(); // Activate IglooUnity2 display
        }
        else
        {
            Debug.LogWarning("Target display (IglooUnity2) is not available. Ensure multiple displays are configured.");
        }
    }
}
