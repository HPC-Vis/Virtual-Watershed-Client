using UnityEngine;
using System.Collections;

public static class CoordinateUtils
{
    // http://damien.dennehy.me/blog/2011/01/15/haversine-algorithm-in-csharp/
    /// <summary>
    /// Radius of the Earth in Kilometers.
    /// </summary>
    private const double EARTH_RADIUS_KM = 6371;

    /// <summary>
    /// Converts an angle to a radian.
    /// </summary>
    /// <param name="input">The angle that is to be converted.</param>
    /// <returns>The angle in radians.</returns>
    private static double ToRad(double input)
    {
        return input * (System.Math.PI / 180);
    }

    /// <summary>
    /// Calculates the distance between two geo-points in kilometers using the Haversine algorithm.
    /// </summary>
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    /// <returns>A double indicating the distance between the points in KM.</returns>
    public static double GetDistanceKM(double Longitude1, double Latitude1, double Longitude2, double Latitude2)
    {
        double dLat = ToRad(Latitude2 - Latitude1);
        double dLon = ToRad(Longitude2 - Longitude1);

        double a = System.Math.Pow(System.Math.Sin(dLat / 2), 2) +
                   System.Math.Cos(ToRad(Latitude1)) * System.Math.Cos(ToRad(Latitude2)) *
                   System.Math.Pow(System.Math.Sin(dLon / 2), 2);

        double c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));

        double distance = EARTH_RADIUS_KM * c;
        return distance;
    }


    public static double GetUtmZoneOrigin(int Longitude, int Latitude)
    {
        Longitude = GetUTMMerridian(Longitude);
        //Debug.LogError ("UTM MERRIDIAN: " + Longitude);
        var Out2 = coordsystem.transformToUTMDouble(Longitude, Latitude);
        return Out2[0];
    }

     public static double GetUtmZoneHalfWidth(int Longitude, int Latitude)
    {
        double origin = GetUtmZoneOrigin(Longitude, Latitude);
        int Side = GetUTMMerridian(Longitude) + 3;
        var End = coordsystem.transformToUTMDouble(Side, Latitude);
        return System.Math.Abs(End[0] - origin);
    }

    // Get the halfway area of the UTM Zone
    public static int GetUTMMerridian(int Longitude)
    {
        int remainder = Longitude % 6;
        Longitude = Longitude - remainder + 3;
        return Longitude;
    }

    const int ZONE_ORIGIN = 500000;

    /// <summary>
    /// This only works for when you are in one hemisphere north or south but not both
    /// </summary>
    /// <param name="Longitude1"></param>
    /// <param name="Latitude1"></param>
    /// <param name="Longitude2"></param>
    /// <param name="Latitude2"></param>
    ///  THIS DOES NOT WORK!!!!
    /// <returns></returns>
    public static Vector2 GetXYDistance(float Longitude1,float Latitude1,float Longitude2,float Latitude2)
    {
        int refcordzone = coordsystem.GetZone(Latitude1, Longitude1);
        int othercordzone = coordsystem.GetZone(Latitude2, Longitude2);
        Debug.LogError("Ref coord zone: " + refcordzone);
        Debug.LogError("other coord zone: " + othercordzone);
        Vector2 DirectionVector = new Vector2(Longitude1, Latitude1) - new Vector2(Longitude2, Latitude2);
        DirectionVector.Normalize();

        // One of the two places
        var Start1 = coordsystem.transformToUTM(Longitude1, Latitude1);
        var Start2 = coordsystem.transformToUTM(Longitude2, Latitude2);
        double offsetx = 0;
        float offsety = 0;

        if(refcordzone > othercordzone)
        {
            var movedstart =  Longitude1 - 6;
            var newstart = coordsystem.transformToUTM(movedstart,Latitude1);
            Debug.LogError("A: " + GetUtmZoneHalfWidth((int)Longitude1, (int)Latitude1));
            Debug.LogError("B: " + GetUtmZoneHalfWidth((int)Longitude2, (int)Latitude2));
            offsetx += 2*GetUtmZoneHalfWidth((int)Longitude1, (int)Latitude1) - Mathf.Abs(Start1.x);
            offsetx += 2*GetUtmZoneHalfWidth((int)Longitude2, (int)Latitude2) - Mathf.Abs(Start2.x + ZONE_ORIGIN);
            if(Mathf.Abs(refcordzone -othercordzone) > 1)
            {
                int zone = Mathf.Abs(refcordzone - othercordzone) - 1;
                offsetx += zone * 2 * GetUtmZoneHalfWidth((int)Longitude2, (int)Latitude2);
            }
            offsety = Mathf.Abs(Start2.y - newstart.y);


        }
        else if(refcordzone < othercordzone)
        {
            var movedstart = Longitude1 + 6;
            var newstart = coordsystem.transformToUTM(movedstart, Latitude1);
            offsety = Mathf.Abs(Start2.y - newstart.y);
            Debug.LogError("A: " + GetUtmZoneHalfWidth((int)Longitude1, (int)Latitude1));
            Debug.LogError("B: " + GetUtmZoneHalfWidth((int)Longitude2, (int)Latitude2));
            offsetx += 2*GetUtmZoneHalfWidth((int)Longitude1, (int)Latitude1) - Mathf.Abs(Start1.x + ZONE_ORIGIN);
            offsetx += 2*GetUtmZoneHalfWidth((int)Longitude2, (int)Latitude2) - Mathf.Abs(Start2.x);
            if (Mathf.Abs(refcordzone - othercordzone) > 1)
            {
                int zone = Mathf.Abs(refcordzone - othercordzone) - 1;
                offsetx += zone * 2 * GetUtmZoneHalfWidth((int)Longitude2, (int)Latitude2);
            }
        }
        return new Vector2((float)offsetx, (float)offsety);
    }

    public static int GetZone(double latitude, double longitude)
    {


        return (int)(Mathf.Floor(((float)longitude + 180.0f) / 6) + 1);
    }

    public static Vector2 transformToUTM(float longitude, float latitude,ref int zone)
    {
        //int zone = GetZone(latitude, longitude);
        zone = GetZone(latitude, longitude);

        //Transform to UTM
        ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctfac = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
        ProjNet.CoordinateSystems.ICoordinateSystem wgs84geo = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
        ProjNet.CoordinateSystems.ICoordinateSystem utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, latitude > 0);
        ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84geo, utm);
        double[] pUtm = trans.MathTransform.Transform(new double[] { longitude, latitude });

        return new Vector2((float)pUtm[0], (float)pUtm[1]);
    }

    public static double[] transformToUTMDouble(float longitude, float latitude)
    {
        int zone = GetZone(latitude, longitude);

        ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctfac = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
        ProjNet.CoordinateSystems.ICoordinateSystem wgs84geo = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
        ProjNet.CoordinateSystems.ICoordinateSystem utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, latitude > 0);
        ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84geo, utm);

        double[] pUtm = trans.MathTransform.Transform(new double[] { longitude, latitude });

        return pUtm;
    }


}
