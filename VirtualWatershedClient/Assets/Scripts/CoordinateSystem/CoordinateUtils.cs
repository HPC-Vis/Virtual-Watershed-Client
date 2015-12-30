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
        var Out2 = CoordinateUtils.transformToUTMDouble(Longitude, Latitude);
        return Out2[0];
    }

     public static double GetUtmZoneHalfWidth(int Longitude, int Latitude)
    {
        double origin = GetUtmZoneOrigin(Longitude, Latitude);
        int Side = GetUTMMerridian(Longitude) + 3;
        var End = CoordinateUtils.transformToUTMDouble(Side, Latitude);
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

    public static int GetZone(double latitude, double longitude)
    {
        return (int)(Mathf.Floor(((float)longitude + 180.0f) / 6) + 1);
    }

    public static Vector2 transformToUTM(float longitude, float latitude)
    {
        //int zone = GetZone(latitude, longitude);
        int zone = GetZone(latitude, longitude);

        //Transform to UTM
        ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctfac = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
        ProjNet.CoordinateSystems.ICoordinateSystem wgs84geo = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
        ProjNet.CoordinateSystems.ICoordinateSystem utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, latitude > 0);
        ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84geo, utm);
        double[] pUtm = trans.MathTransform.Transform(new double[] { longitude, latitude });

        return new Vector2((float)pUtm[0], (float)pUtm[1]);
    }

    public static Vector2 transformToUTMWithZone(float longitude, float latitude,int zone)
    {
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
