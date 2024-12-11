using UnityEngine;

public class PlaneAudio : MonoBehaviour
{
    public AudioSource planeAudio; // Reference to the AudioSource
    public Transform originPoint; // Reference to the origin (e.g., (0, 0, 0))

    [Header("Audio Settings")]
    [Range(0, 5)] public float dopplerLevel = 1.0f; // Doppler effect intensity
    [Range(0, 1)] public float spatialBlend = 1.0f; // 3D audio blend
    public float minDistance = 1.0f; // Distance for full volume
    public float maxDistance = 150.0f; // Distance at which the sound becomes minimal
    public float falloffFactor = 0.5f; // Controls the steepness of the falloff

    void Start()
    {
        if (planeAudio == null)
        {
            // Automatically fetch AudioSource if not set
            planeAudio = GetComponent<AudioSource>();
            if (planeAudio == null)
            {
                Debug.LogError("PlaneAudio script requires an AudioSource component!");
                return;
            }
        }

        // Configure the AudioSource for 3D sound
        planeAudio.spatialBlend = spatialBlend; // Fully 3D sound
        planeAudio.dopplerLevel = dopplerLevel; // Enable Doppler effect
        planeAudio.loop = true;

        if (!planeAudio.isPlaying)
        {
            planeAudio.Play(); // Start the engine sound
        }
    }

    void Update()
    {
        if (planeAudio == null || originPoint == null) return;

        // Calculate the distance from the origin
        float distance = Vector3.Distance(transform.position, originPoint.position);

        // Custom volume attenuation using a logarithmic model
        float volume = Mathf.Clamp01(1f / (1f + falloffFactor * Mathf.Pow(distance / maxDistance, 2)));

        // Set the volume
        planeAudio.volume = volume;

        // Debugging
        //Debug.Log($"Distance: {distance}, Volume: {volume}");
    }
}
