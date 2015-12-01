using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Chase Carthen
/// UTMCoordinateSystem will use a central utm to represent it Coordinate System.
/// Out of zone conflicts will be approximated with the haversine.
/// 
/// This coordinate system is best for things that occur in the same area.
/// 
/// This coordinate system assumes that you are working in one Hemisphere!
/// </summary>
public class UTMCoordinateSystem : WorldCoordinateSystem 
{
    /// <summary>
    /// A Reminder from the base class
    /// Vector2 unityOrigin;
    /// Vector2 worldOrigin;
    /// </summary>

    public UTMCoordinateSystem()
        : base()
    {

    }

    // Supporting the local zone of Nevada!
    public int LocalZone = 11;

    /// <summary>
    /// We are translate earth coordinates to the unity coordinate system.
    /// </summary>
    /// <param name="World">These coordinates are expected to be UTM.     
    /// WorldCoord.x: our coordinate systems x
    /// WorldCoord.y: our coordinate system y  </param>
    /// <returns></returns>
    public override Vector2 TranslateToUnity(Vector2 WorldCoord)
    {
        return new Vector2(WorldOrigin.x - WorldCoord.x, WorldOrigin.y - WorldCoord.y);
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
        return new Vector3(WorldOrigin.x + UnityCoord.x, WorldOrigin.y + UnityCoord.y);
    }


    public override Vector2 Transform(Vector2 LatLong)
    {
        // This is where I will handle haversine
        throw new System.NotImplementedException();
    }

}
