using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameReset : MonoBehaviour
{
    public InputActionReference resetActionReference; // Reference to the Reset input action

    private void OnEnable()
    {
        if (resetActionReference != null)
        {
            resetActionReference.action.Enable();
            resetActionReference.action.performed += OnReset;
        }
    }

    private void OnDisable()
    {
        if (resetActionReference != null)
        {
            resetActionReference.action.performed -= OnReset;
            resetActionReference.action.Disable();
        }
    }

    private void OnReset(InputAction.CallbackContext context)
    {
        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
