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
    public bool NorthHemisphere;
    public UTMCoordinateSystem(int zone=11)
        : base()
    {
        LocalZone = zone;
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
        int WorldCoordZone = CoordinateUtils.GetZone(WorldCoord.y,WorldCoord.x);
        Vector2 TransformedCoord = CoordinateUtils.transformToUTM(WorldCoord.x, WorldCoord.y);
        // This criterion is choosen based on http://geokov.com/education/utm.aspx --- In polar regions there is a large amount of distortion
        // This makes sense with UTM.
        if(WorldCoord.y > 84 || WorldCoord.y < -80 || WorldCoordZone != LocalZone)
        {
            // Time to do some haversine magic -- to get the distance between the two points
            float distance = (float)CoordinateUtils.GetDistanceKM(WorldOrigin.x, WorldOrigin.y, WorldCoord.x, WorldCoord.y)*1000.0f; // convert to meters

            // Now lets compute the Unity Point -- by getting a direction vector and multiplying the distance to it.
            return (WorldCoord - WorldOrigin).normalized * distance;
        }
        // If these two points are not in the same hemisphere
        else if (NorthHemisphere != (WorldCoord.y > 0) )
        {
            TransformedCoord.y = TransformedCoord.y - 10000000;// false northing for south hemisphere -- we are assuming the equator to be 0,0.
        }
        

        return new Vector2(-(InternalOrigin.x - TransformedCoord.x), -(InternalOrigin.y - TransformedCoord.y));
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

    public override void UpdateInternalOrigin()
    {
        // Detecting if the point is in the north hemisphere.
        NorthHemisphere = WorldOrigin.y > 0;
        InternalOrigin = CoordinateUtils.transformToUTM(WorldOrigin.x, WorldOrigin.y);
    }
}
