using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PushBack : MonoBehaviour
{
    public float pushBackForce = 10f; // Initial pushback force
    public float dampingFactor = 5f; // Friction-like damping to stop the player
    public float cooldownTime = 0.5f; // Cooldown time to prevent repeated pushes

    private Rigidbody rb;
    private bool isBeingPushed = false; // To track if the player is still being pushed
    private bool canPushBack = true; // To control cooldown between pushes

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found. Attach this script to the player character.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with an obstacle and pushback is allowed
        if (canPushBack && collision.gameObject.CompareTag("Obstacle"))
        {
            // Calculate pushback direction (away from collision point)
            Vector3 pushDirection = (transform.position - collision.contacts[0].point).normalized;

            // Apply immediate force in the push direction
            rb.velocity = pushDirection * pushBackForce;

            // Start the damping process
            isBeingPushed = true;

            // Prevent further pushes during cooldown
            canPushBack = false;
            Invoke(nameof(ResetPushback), cooldownTime);

            // Debugging info
            Debug.Log($"Pushback applied! Direction: {pushDirection}, Force: {pushBackForce}");
        }
    }

    private void Update()
    {
        // If the player is being pushed, apply velocity damping
        if (isBeingPushed)
        {
            // Gradually reduce the velocity to simulate friction
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, dampingFactor * Time.deltaTime);

            // Stop movement completely if velocity is very small
            if (rb.velocity.magnitude < 0.1f)
            {
                rb.velocity = Vector3.zero;
                isBeingPushed = false; // End the pushback
            }
        }
    }

    private void ResetPushback()
    {
        canPushBack = true; // Allow pushback again
    }
}
