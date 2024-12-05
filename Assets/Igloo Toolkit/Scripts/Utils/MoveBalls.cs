using UnityEngine;

/// <summary>
/// A simple Class to move the rotating spheres within our camera test scene. 
/// </summary>
public class MoveBalls : MonoBehaviour
{
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0066 // Convert switch statement to expression

    /// <summary>
    /// Starting scale of the spheres we're going to move
    /// </summary>
    private const float DEFAULT_SCALE = 0.2f;

    /// <summary>
    /// Amount the sphere will rise up during it's ping pong system
    /// </summary>
    public float offsetY = 0.0f;

    /// <summary>
    /// The speed in which the sphere will rotate
    /// </summary>
    public float spinSpeed = 1.0f;

    /// <summary>
    /// Public function to set local Y Offset Value using OSC input
    /// </summary>
    /// <param name="y">Float, New Y offset value</param>
    public void SetOffsetY(float y) { offsetY = y; }

    /// <summary>
    /// Public function to set local Spin Speed Value using OSC input
    /// </summary>
    /// <param name="x">Float, New Spin Speed value</param>
    public void SetSpinSpeed(float x) { spinSpeed = x; }

    /// <summary>
    /// Public function to set local Spin Speed Value using a normalised OSC input
    /// </summary>
    /// <param name="x">Float, New Normalised Spin Speed value (0-1)</param>
    public void SetSpinSpeedNormalized(float x) { spinSpeed = 100 * Mathf.Max(0.0f, Mathf.Min(1.0f, x)); }

    /// <summary>
    /// Public function to set sphere shape based on OSC input.
    /// Adjusts scale to achieve this.
    /// </summary>
    /// <param name="type">Int, new sphere type (0, 1, 2)</param>
    public void SetSphereShapes(int type)
    {
        Vector3 newScale;
        switch (type)
        {
            case 0:
                newScale = new Vector3(DEFAULT_SCALE, DEFAULT_SCALE, DEFAULT_SCALE);
                break;
            case 1:
                newScale = new Vector3(DEFAULT_SCALE, 100.0f, DEFAULT_SCALE);
                break;
            case 2:
                newScale = new Vector3(0.001f, DEFAULT_SCALE, DEFAULT_SCALE);
                break;
            default:
                newScale = new Vector3(DEFAULT_SCALE, DEFAULT_SCALE, DEFAULT_SCALE);
                break;
        }

        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Mono Update Function
    /// Changes the position of the sphere this class is attached to.
    /// Also changes the rotation, and y position to make it spin and bounce up and down.
    /// </summary>
    void Update()
    {
        Vector3 position = this.transform.position;
        position.y = Mathf.Sin(Time.time * 3) * offsetY;
        this.transform.position = position;
        this.transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }
}
