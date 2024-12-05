using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // Get input from keyboard or controller
        float horizontal = Input.GetAxis("Horizontal"); // Left stick X-axis
        float vertical = Input.GetAxis("Vertical");     // Left stick Y-axis
        float upDown = Input.GetAxis("ZAxis");          // Map this axis for controller triggers or right stick

        // Calculate movement direction
        Vector3 movement = new Vector3(horizontal, upDown, vertical) * moveSpeed * Time.deltaTime;

        // Apply movement to the character
        transform.Translate(movement);
    }
}
