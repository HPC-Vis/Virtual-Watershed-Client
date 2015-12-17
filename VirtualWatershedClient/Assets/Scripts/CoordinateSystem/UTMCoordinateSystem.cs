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
    /// <param name="World">These coordinates are expected to be lat long that will be converted to utm.     
    /// WorldCoord.x: our coordinate systems x
    /// WorldCoord.y: our coordinate system y  </param>
    /// <returns></returns>
    public override Vector2 TranslateToUnity(Vector2 WorldCoord)
    {
        Debug.LogError(WorldCoord);
        Debug.LogError(WorldOrigin);
        Debug.LogError("HERE NOW YALL");
        Vector2 latlongorigin = Vector2.zero;
        if (CoordinateUtils.GetZone(WorldCoord.y, WorldCoord.x) != LocalZone)
        {
            // Now we need to calculate the haversine distance for x and y
            // calc y distance
            double ydist = 1000.0 * CoordinateUtils.GetDistanceKM(latlongorigin.x, WorldCoord.y, latlongorigin.x, latlongorigin.y);

            // calc x distance
            double xdist = 1000.0 * CoordinateUtils.GetDistanceKM(WorldCoord.x, latlongorigin.y, latlongorigin.x, latlongorigin.y);

            if(WorldCoord.x < latlongorigin.x)
            {
                xdist *= -1;
            }
            if (WorldCoord.y < latlongorigin.y)
            {
                ydist *= -1;
            }
            // Convert to utm
            return new Vector2((float)xdist,(float)ydist);//new Vector2(-(WorldOrigin.x - WorldCoord.x), -(WorldOrigin.y - WorldCoord.y));
        }
        else
        {
            WorldCoord = CoordinateUtils.transformToUTM(WorldCoord.x, WorldCoord.y);
        }
        return new Vector2(-(WorldOrigin.x - WorldCoord.x), -(WorldOrigin.y - WorldCoord.y));
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
        return new Vector3(UnityOrigin.x + UnityCoord.x, UnityOrigin.y + UnityCoord.y);
    }

}
