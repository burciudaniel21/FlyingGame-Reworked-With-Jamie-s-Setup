using UnityEngine;
using Igloo.Common;

/// <summary>
/// An Example on how to use the Follow Object System within the Igloo Manager Class.
/// </summary>
public class IglooExample2 : MonoBehaviour
{
    /// <summary>
    /// The GameObject for the Igloo Camera System to follow. If FollowObject is called.
    /// </summary>
    public GameObject followObject;

    /// <summary>
    /// Mono Start Function. Executed at Global Start
    /// </summary>
    private void Start()
    {
        if (IglooManager.instance == null) Debug.LogError("<b>[Igloo]</b> Igloo Manager must be added to the Scene");
    }

    /// <summary>
    /// Follow Player Function. Turns the Igloo Player Manager on, and disables the Igloo Follow Object
    /// </summary>
    public void FollowPlayer()
    {
        IglooManager.instance.igloo.GetComponent<FollowObjectTransform>().enabled = false;
        IglooManager.instance.igloo.GetComponent<PlayerManager>().UsePlayer = true;
    }

    /// <summary>
    /// Follow Object Function. Turns the Igloo Player Manager off, and enables the Igloo Follow Object script. 
    /// </summary>
    public void FollowObject()
    {
        IglooManager.instance.igloo.GetComponent<FollowObjectTransform>().enabled = true;
        IglooManager.instance.igloo.GetComponent<FollowObjectTransform>().followObject = followObject;
        IglooManager.instance.igloo.GetComponent<PlayerManager>().UsePlayer = false;
    }
}
