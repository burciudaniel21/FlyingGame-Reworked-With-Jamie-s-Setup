using UnityEngine;

public class ObjectToggleController : MonoBehaviour
{
    public GameObject object1; // Assign the first GameObject in the Inspector
    public GameObject object2; // Assign the second GameObject in the Inspector

    [Header("Input Settings")]
    public KeyCode keyboardToggleKey = KeyCode.V; // Keyboard toggle key
    public string controllerToggleButton = "joystick button 3"; // Y button on most controllers

    void Update()
    {
        // Check for keyboard or controller input
        if (Input.GetKeyDown(keyboardToggleKey) || Input.GetKeyDown(controllerToggleButton))
        {
            ToggleObjects();
        }
    }

    void ToggleObjects()
    {
        // Toggle the active states of the two objects
        bool isObject1Active = object1.activeSelf;

        object1.SetActive(!isObject1Active); // Set object1 to the opposite state
        object2.SetActive(isObject1Active);  // Set object2 to the opposite state
    }
}
