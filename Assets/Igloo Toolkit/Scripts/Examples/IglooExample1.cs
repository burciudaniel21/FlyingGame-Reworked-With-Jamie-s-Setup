using UnityEngine;
using Igloo.Common;

/// <summary>
/// An Example of Creating and Destroying the Igloo Camera System
/// </summary>
public class IglooExample1 : MonoBehaviour
{
    /// <summary>
    /// Mono Start Function, Executed during the Global Start Event
    /// </summary>
    public void Start()
    {
        if (IglooManager.instance == null) Debug.LogError("<b>[Igloo]</b> Igloo Manager must be added to the Scene");
    }

    /// <summary>
    /// Create an Igloo by calling CreateIgloo() within the Igloo Manager Class
    /// </summary>
    public void CreateIgloo()
    {
        IglooManager.instance.CreateIgloo();
    }

    /// <summary>
    /// Destroy an Igloo Camera System by calling RemoveIgloo() within the Igloo Manager Class
    /// </summary>
    public void RemoveIgloo()
    {
        IglooManager.instance.RemoveIgloo();
    }

}
