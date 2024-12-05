using UnityEngine;

/// <summary>
/// Utility Class containing functions that place instances of a gameobject into a range of layout formations.
/// Useful for creating quick test scenes from a single prefab
/// </summary>
public static class Tools_Layout
{
#pragma warning disable IDE0090 // Use New()...

    /// <summary>
    /// Creates a Grid layout by instantiating a prefab into an organised grid formation 
    /// using the defined amount of columns and rows, and padding.
    /// </summary>
    /// <param name="rows">How many rows to use</param>
    /// <param name="columns">How many columns to use</param>
    /// <param name="depth">Depth of grid</param>
    /// <param name="spacingScale">Scale between items</param>
    /// <param name="item">The prefab to create a grid of</param>
    public static void InstantiateItemsInGrid(this int rows, int columns, int depth, float spacingScale, GameObject item)
    {
        GameObject gridParent = new GameObject
        {
            name = "Grid Of " + item.name + "s"
        };

        for (int y = 0; y < depth; y++)
        {
            for (int z = 0; z < rows; z++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Vector3 position = new Vector3(x * spacingScale, y * spacingScale, z * spacingScale);
                    GameObject newItem = GameObject.Instantiate(item, position, Quaternion.identity) as GameObject;
                    newItem.transform.parent = gridParent.transform;
                }
            }
        }
    }


    /// <summary>
    /// A simplified override of Instantiate Items in Grid. 
    /// Spacing scale pre-defined as 1.0f
    /// </summary>
    /// <param name="rows">How many rows to use</param>
    /// <param name="columns">How many columns to use</param>
    /// <param name="depth">Depth of grid</param>
    /// <param name="item">The prefab to create a grid of</param>
    public static void InstantiateItemsInGrid(this int rows, int columns, int depth, GameObject item)
    {
        InstantiateItemsInGrid(rows, columns, depth, 1.0f, item);
    }

    /// <summary>
    /// Creates a circle arrangement of instances of a prefab based on the
    /// center position of the circle, and the number of items to create in a circle.
    /// </summary>
    /// <param name="numItems">How many items to create in a circle</param>
    /// <param name="centerPos">Center position of the circle</param>
    /// <param name="item">The prefab to create a circle of</param>
    public static void InstantiateItemsInCircle(this int numItems, Vector3 centerPos, GameObject item)
    {
        GameObject circleParent = new GameObject
        {
            name = "Circle Of " + item.name + "s"
        };

        for (int itemNumber = 0; itemNumber < numItems; itemNumber++)
        {
            float i = (itemNumber * 1.0f) / numItems;
            float theta = i * Mathf.PI * 2;

            float x = Mathf.Sin(theta);
            float z = Mathf.Cos(theta);

            Vector3 position = new Vector3(x, 0, z) + centerPos;

            GameObject newItem = GameObject.Instantiate(item, position, Quaternion.identity) as GameObject;
            newItem.transform.parent = circleParent.transform;

        }
    }

}
