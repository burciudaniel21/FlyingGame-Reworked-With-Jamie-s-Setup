using UnityEngine;

/// <summary>
/// Simple Class that moves the object it is a component of, towards an array of positions 
/// one at a time. Creating a 'patrol' system.
/// </summary>
public class MoveToPostion : MonoBehaviour
{
#pragma warning disable IDE0051 // Remove unused private members
    /// <summary>
    /// List of positions to move to
    /// </summary>
    public Vector3[] positions;

    /// <summary>
    /// Speed to move between positions
    /// </summary>
    public float speed = 1;

    /// <summary>
    /// Current position within positions array.
    /// </summary>
    private int currentPos = 0;

    /// <summary>
    /// Mono Update Function
    /// Moves this object towards the next position in the positions array.
    /// Changes positions it's moving towards when it reaches the current position.
    /// loops through array once completed.
    /// </summary>
    void Update()
    {
        if (positions.Length > 0)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, positions[currentPos], step);
            if (transform.position == positions[currentPos]) currentPos += 1;
            currentPos %= positions.Length;
        }
    }
}
