using UnityEngine;
using System.Collections;
using Gavaghan.Geodesy;
public static class CoordinateUtils
{


    static GeodeticCalculator geoCalc = new GeodeticCalculator();

    // select a reference elllipsoid
    static Gavaghan.Geodesy.Ellipsoid reference = Gavaghan.Geodesy.Ellipsoid.WGS84;

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


    // Based on this reference http://stackoverflow.com/questions/7222382/get-lat-long-given-current-point-distance-and-bearing
    // Can be derived from haversine.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="LongLat">Must be in degrees. </param>
    /// <param name="angle">Must be in degrees. </param>
    /// <param name="distance">Must be in meters.</param>
    /// <returns></returns>
    public static Vector2 CalculateProjectedPointHaversine(Vector2 LongLat, float angle,float distance)
    {
        float lat1 = LongLat.y;
        float long1 = LongLat.x;
        float lat2, long2;
        angle *= Mathf.Deg2Rad;
        lat1 *= Mathf.Deg2Rad;
        long1 *= Mathf.Deg2Rad;
        float R = 6378.1f*1000; // Earth Radius in meters
        lat2 = Mathf.Asin(Mathf.Sin(lat1) * Mathf.Cos(distance / R) +
        Mathf.Cos(lat1) * Mathf.Sin(distance / R) * Mathf.Cos(angle));

        long2 = long1 + Mathf.Atan2(Mathf.Sin(angle) * Mathf.Sin(distance / R) * Mathf.Cos(lat1),
        Mathf.Cos(distance / R) - Mathf.Sin(lat1) * Mathf.Sin(lat2));

        lat2 = Mathf.Rad2Deg * lat2;
        long2 = Mathf.Rad2Deg * long2;
        return new Vector2(long2,lat2);
    }

    // Using Gavaghan's library -- 
    public static Vector2 CalculateProjectedPointVincenty(Vector2 LongLat, float bearing, float distance)
    {

        float lat1 = LongLat.y;
        float long1 = LongLat.x;

        // set Lincoln Memorial coordinates
        GlobalCoordinates PointOne;
        PointOne = new GlobalCoordinates(
            new Angle(lat1), new Angle(long1)
        );

        // set the direction and distance
        Angle startBearing = new Angle(bearing);

        // find the destination
        Angle endBearing;
        GlobalCoordinates dest = geoCalc.CalculateEndingGlobalCoordinates(reference, PointOne, startBearing, distance, out endBearing);

        return new Vector2((float)dest.Longitude.Degrees,(float)dest.Latitude.Degrees);
    }


    public static float CalculateBearing(float Longitude1, float Latitude1, float Longitude2, float Latitude2)
    {
        // Calaculate bearing
        Vector2 one = new Vector2(Longitude1, Latitude1);
        Vector2 two = new Vector2(Longitude2, Latitude2);
        Vector2 heading = two - one;
        heading.Normalize();
        float angle = Vector2.Angle(heading, Vector2.up);
        if (heading.y < 0)
        {
            angle = -angle;
        }
        return angle;
    }


    // Using Gavaghan's library --
    public static double VincentyDistanceKM(double Longitude1, double Latitude1, double Longitude2, double Latitude2)
    {

        // set Lincoln Memorial coordinates
        GlobalCoordinates pointOne;
        pointOne = new GlobalCoordinates(
            new Angle(Latitude1), new Angle(Longitude1)
        );

        // set Eiffel Tower coordinates
        GlobalCoordinates pointTwo;
        pointTwo = new GlobalCoordinates(
            new Angle(Latitude2), new Angle(Longitude2)
        );

        // calculate the geodetic curve
        GeodeticCurve geoCurve = geoCalc.CalculateGeodeticCurve(reference, pointOne, pointTwo);
        double ellipseKilometers = geoCurve.EllipsoidalDistance / 1000.0;
        return ellipseKilometers;
    }

    public static Vector2 TransformPoint(OSGeo.OSR.CoordinateTransformation tran, Vector2 Point)
    {
        double[] Out = new double[2];
        Out[0] = (float)Point.x;
        Out[1] = (float)Point.y;
        tran.TransformPoint(Out);
        return new Vector2((float)Out[0], (float)Out[1]);
    }
}
