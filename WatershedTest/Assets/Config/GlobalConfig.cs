using UnityEngine;
using System.Collections;


/// <summary>
/// This class will hold the different configurations between different projects.
/// </summary>
public static class GlobalConfig 
{
    public static bool loading;
    public static string Location = "Dry Creek";
    public static string State = "Idaho";

    // Terrain Position
    public static Vector3 TerrainStartPosition = Vector3.zero;

    // Terrain Height Max
    public static float HeightMax;
    public static float HeightMin;

    // Character Controller
    // Starting Location
    public static Vector3 Position;

    public static Rect TerrainBoundingBox = new Rect();
    
    /// <summary>
    /// Some hardcodeness on what the projection of the current terrain is.
    /// </summary>
    public static int GlobalProjection = 26911;
    public static Rect BoundingBox = new Rect(565889.360f, 4844940.560f, 569986.360f - 565889.360f, 4844940.560f - 4840843.560f);
}
