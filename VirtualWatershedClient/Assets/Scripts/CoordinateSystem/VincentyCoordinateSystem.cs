using UnityEngine;
using System.Collections;

public class VincentyCoordinateSystem : WorldCoordinateSystem
{

    /// <summary>
    /// A Reminder from the base class
    /// Vector2 unityOrigin;
    /// Vector2 worldOrigin;
    /// </summary>
    public bool NorthHemisphere;
    public VincentyCoordinateSystem()
        : base()
    {
        Debug.LogError("Vincenty");
    }


    /// <summary>
    /// We are translate earth coordinates to the unity coordinate system.
    /// </summary>
    /// <param name="World">These coordinates are expected to be lat long that will be converted to utm.     
    /// WorldCoord.x: our coordinate systems x
    /// WorldCoord.y: our coordinate system y  </param>
    /// <returns></returns>
    public override Vector2 TranslateToUnity(Vector2 WorldCoord)
    {
        //return new Vector2(-(InternalOrigin.x - TransformedCoord.x), -(InternalOrigin.y - TransformedCoord.y));
        // Time to do some haversine magic -- to get the distance between the two points
        float distance = (float)CoordinateUtils.VincentyDistanceKM(WorldOrigin.x, WorldOrigin.y, WorldCoord.x, WorldCoord.y) * 1000.0f; // convert to meters

        // Now lets compute the Unity Point -- by getting a direction vector and multiplying the distance to it.
        return (WorldCoord - WorldOrigin).normalized * distance;
    }

    /// <summary>
    /// We are tranlating unity coordinates to the earth coordinate system.
    /// </summary>
    /// <param name="Unity">Unity World Coordinates. 
    /// UnityCoord.x: our coordinate systems x
    /// UnityCoord.z: our coordinate system y </param>
    /// <returns></returns>
    public override Vector2 TranslateToWorld(Vector2 UnityCoord)
    {
        float angle = CoordinateUtils.CalculateBearing(UnityOrigin.x, UnityOrigin.y, UnityCoord.x, UnityCoord.y);

        float distance = Vector2.Distance(UnityCoord, UnityOrigin);
        return CoordinateUtils.CalculateProjectedPointVincenty(WorldOrigin, angle, distance);
    }

    public override void UpdateInternalOrigin()
    {
        // Detecting if the point is in the north hemisphere.
    }
}
