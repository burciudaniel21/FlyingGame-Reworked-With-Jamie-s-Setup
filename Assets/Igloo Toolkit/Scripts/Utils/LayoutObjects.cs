using UnityEngine;


/// <summary>
/// Simple Class to layout objects in a grid or a circle using the Tools_Layout class within the Igloo Utils system
/// </summary>
public class LayoutObjects : MonoBehaviour
{
    /// <summary>
    /// Prefab Item to Layout
    /// </summary>
    public GameObject publicItem;

    /// <summary>
    /// Rows in grid
    /// </summary>
    [Header("Grid Layout Options")]
    public int rows = 6;
    /// <summary>
    /// Columns in grid
    /// </summary>
    public int columns = 5;
    /// <summary>
    /// Grid depth
    /// </summary>
    public int depth = 1;

    /// <summary>
    /// Spacing of items around the circle
    /// </summary>
    [Header("Circle Layout Options")]
    public float spacingScale = 1.0f;
    /// <summary>
    /// Number of items in the circle
    /// </summary>
    public int numItemsCircle = 10;
    /// <summary>
    /// Center position of the circle
    /// </summary>
    public Vector3 centerPosCircle;

    /// <summary>
    /// Calls the Instantiate Items in Grid function from Igloo Tools_Layout class
    /// </summary>
    public void InstantiateItemsInGrid()
    {
        Tools_Layout.InstantiateItemsInGrid(rows, columns, depth, spacingScale, publicItem);
    }

    /// <summary>
    /// Calls the Instantiate Items in Circle function from Igloo Tools_Layout class
    /// </summary>
    public void InstantiateItemsInCircle()
    {
        Tools_Layout.InstantiateItemsInCircle(numItemsCircle, centerPosCircle, publicItem);
    }
}