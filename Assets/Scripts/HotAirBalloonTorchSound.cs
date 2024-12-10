using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class HotAirBalloonTorchSound : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip torchSound; // The sound clip for the hot air torch
    public float fadeDuration = 0.2f; // Duration for fade-in and fade-out
    public float maxDistance = 50f; // Maximum effective distance for sound attenuation
    public float falloffFactor = 1f; // Factor to adjust falloff intensity

    private AudioSource audioSource; // The audio source for playing the sound
    private bool isFadingIn = false; // Track if the sound is currently fading in
    private bool isFadingOut = false; // Track if the sound is currently fading out

    [Header("Input Actions")]
    public InputActionReference heatBalloonAction; // Button press for heating the balloon

    private Transform balloonTransform; // Reference to the balloon's transform
    private Transform cameraTransform; // Reference to the currently active camera's transform

    private void OnEnable()
    {
        heatBalloonAction.action.Enable();

        // Subscribe to button press/release events
        heatBalloonAction.action.performed += OnHeatPressed;
        heatBalloonAction.action.canceled += OnHeatReleased;

        // Set up the audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = torchSound;
        audioSource.loop = true; // Enable looping
        audioSource.spatialBlend = 1.0f; // Ensure 3D sound
        audioSource.playOnAwake = false; // Do not play on start
        audioSource.volume = 0f; // Start muted
        balloonTransform = transform;

        // Initialize the camera reference
        UpdateCameraReference();
    }

    private void OnDisable()
    {
        heatBalloonAction.action.Disable();

        // Unsubscribe from button press/release events
        heatBalloonAction.action.performed -= OnHeatPressed;
        heatBalloonAction.action.canceled -= OnHeatReleased;
    }

    private void Update()
    {
        if (audioSource.isPlaying && cameraTransform != null)
        {
            // Calculate the distance between the balloon and the camera
            float distance = Vector3.Distance(balloonTransform.position, cameraTransform.position);

            // Custom volume attenuation using a logarithmic model
            float volume = Mathf.Clamp01(1f / (1f + falloffFactor * Mathf.Pow(distance / maxDistance, 2)));

            // Adjust the volume of the audio source
            audioSource.volume = Mathf.Lerp(audioSource.volume, volume, Time.deltaTime * 10f); // Smooth transition for volume changes
        }
    }

    private void OnHeatPressed(InputAction.CallbackContext context)
    {
        // Start the torch sound with a fade-in effect
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        if (!isFadingIn)
        {
            StartCoroutine(FadeInAudio());
        }
    }

    private void OnHeatReleased(InputAction.CallbackContext context)
    {
        // Stop the torch sound with a fade-out effect
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAudio());
        }
    }

    private IEnumerator FadeInAudio()
    {
        isFadingIn = true;
        isFadingOut = false;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1f;
        isFadingIn = false;
    }

    private IEnumerator FadeOutAudio()
    {
        isFadingOut = true;
        isFadingIn = false;

        float elapsedTime = 0f;
        float startVolume = audioSource.volume;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        isFadingOut = false;
    }

    private void UpdateCameraReference()
    {
        Camera activeCamera = Camera.main; // Use the MainCamera tag
        if (activeCamera != null)
        {
            cameraTransform = activeCamera.transform;
        }
        else
        {
            Debug.LogWarning("No camera tagged as 'MainCamera' is active!");
            cameraTransform = null;
        }
    }

    // Call this method whenever the active camera changes
    public void OnActiveCameraChanged(Camera newCamera)
    {
        if (newCamera != null)
        {
            cameraTransform = newCamera.transform;
        }
        else
        {
            Debug.LogWarning("The new active camera is null!");
            cameraTransform = null;
        }
    }
}
