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
        // Toggle the active states of the two objects
        bool isObject1Active = object1.activeSelf;

        object1.SetActive(!isObject1Active); // Set object1 to the opposite state
        object2.SetActive(isObject1Active);  // Set object2 to the opposite state
    }
}
