using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider))]
public class Collectable : MonoBehaviour
{
    public AudioClip collectSound; // Reference to the sound effect
    private AudioSource audioSource;

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

            // Disable the object after the sound finishes
            StartCoroutine(DeactivateAfterSound());
        }
    }

    IEnumerator DeactivateAfterSound()
    {
        // Wait for the sound to finish playing
        yield return new WaitForSeconds(collectSound.length);

        // Deactivate the collectable object
        gameObject.SetActive(false);
    }
}