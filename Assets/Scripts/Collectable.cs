using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider))]
public class Collectable : MonoBehaviour
{
    public AudioClip collectSound; // Reference to the sound effect
    private AudioSource audioSource;
    private Renderer objectRenderer; // Reference to the renderer
    private Transform objectTransform; // Reference to the object's transform
    public float shrinkDuration = 1f; // Duration of the shrinking animation
    public float spinSpeed = 360f; // Spin speed in degrees per second

    private Vector3 targetPosition; // Final position to zip toward

    void Start()
    {
        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure the AudioSource
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D sound (set to 0.0 for 2D sound)

        // Get the renderer to hide the object
        objectRenderer = GetComponent<Renderer>();

        // Get the object's transform
        objectTransform = transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected the ring!");

            // Play the sound effect
            if (collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            // Disable the collider
            GetComponent<Collider>().enabled = false;

            // Calculate the exact center of the collider mesh
            targetPosition = CalculateMeshCentre(other);

            // Start the zip, shrink, and spin effect
            StartCoroutine(ZipShrinkAndSpin());
        }
    }

    private Vector3 CalculateMeshCentre(Collider collider)
    {
        MeshFilter meshFilter = collider.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            // Transform the mesh's local bounds center into world space
            Bounds bounds = meshFilter.sharedMesh.bounds;
            return collider.transform.TransformPoint(bounds.center);
        }

        // Fallback: use collider's bounds center
        return collider.bounds.center;
    }

    IEnumerator ZipShrinkAndSpin()
    {
        float elapsedTime = 0f;

        // Store the initial scale
        Vector3 initialScale = objectTransform.localScale;

        // Perform the zip, shrinking, and spinning animation
        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.deltaTime;

            // Interpolate position to zip towards the target
            objectTransform.position = Vector3.Lerp(objectTransform.position, targetPosition, elapsedTime / shrinkDuration);

            // Shrink the object
            float scale = Mathf.Lerp(1f, 0f, elapsedTime / shrinkDuration);
            objectTransform.localScale = initialScale * scale;

            // Spin the object
            objectTransform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);

            yield return null;
        }

        // Ensure the object is fully shrunk and at the target position
        objectTransform.localScale = Vector3.zero;

        // Wait for the sound to finish playing before deactivating
        yield return new WaitForSeconds(collectSound.length - shrinkDuration);

        // Deactivate the object
        gameObject.SetActive(false);
    }
}
