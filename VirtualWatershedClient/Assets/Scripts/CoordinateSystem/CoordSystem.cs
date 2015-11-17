using UnityEngine;
using System.Collections;
using Proj4Net.Projection;
//using Proj4Net;
using ProjNet;
using OSGeo.OSR;
//using System.Math;
/*
* Class: coordsystem
* Description: This class is meant to be a coordinate system that keeps world objects in terms of Unity's space.
 * Everything in this class must be format as such
 * Any 2D points must be Long, Lat i.e. East-West,North-South
 * Any 3D Points must be Long, Altitude, Lat
*/
public static class coordsystem
{
    // A Global projection meant to represent unitys overall understanding of the world coordinate system.
    static Projection globalProjection = new Projection();
    // Assumption 1 meter resolution per 1 unity.
    static public float worldScaleX = 1.0f, worldScaleY = 1.0f, worldScaleZ = 1.0f;
    static Vector2 worldOrigin;
    //public static CoordinateReferenceSystemFactory coordRefFactory = new CoordinateReferenceSystemFactory();
    //public static CoordinateReferenceSystem baseCoordSystem = coordRefFactory.CreateFromName("epsg:4326");
	public static SpatialReference baseCoordSystem = new SpatialReference("");

    static Vector2 unityOrigin;
    static float xUnityRes, yUnityRes;

    // For now lets assume everything with respect to our zone :D.
    static public int localzone = 11;


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
    //

    public static Vector2 WorldOrigin
    {
        get
        {
            return worldOrigin;
        }
        set
        {
            worldOrigin = value;
        }
    }

    public static Vector2 UnityOrigin
    {
        get
        {
            return unityOrigin;
        }
        set
        {
            unityOrigin = value;
        }
    }

    public static float XUnityRes
    {
        get
        {
            return worldScaleX;
        }
        set
        {
            xUnityRes = value;
        }
    }

    public static float YUnityRes
    {
        get
        {
            return worldScaleY;
        }
        set
        {
            yUnityRes = value;
        }
    }

    static void setWorldScaleX(float x)
    {
        worldScaleX = x;
    }

    static void setWorldScaleY(float y)
    {
        worldScaleY = y;
    }

    // xUnityRes and yUnityRes are a world to unity conversion
    static Vector2 worldToUnity(Vector2 a)
    {
        return new Vector2(((a.x - worldOrigin.x) * xUnityRes),
                           ((worldOrigin.y - a.y) * yUnityRes));
    }

    // reciprocal gives unity to world conversion
    static Vector2 unityToWorld(Vector2 a)
    {
        return new Vector2(((a.x - unityOrigin.x) / xUnityRes),
                           ((unityOrigin.y - a.y) / yUnityRes));
    }

    static void utmToLatLong()
    {

    }

    static void latlongToUtm()
    {

    }
    static string information()
    {
        return "THIS IS A DUMMY OUTPUT FOR NOW";
    }
    public static CoordinateTransformation createTransform(SpatialReference reference, SpatialReference target)
    {
		baseCoordSystem.ImportFromEPSG (4326);
		return new CoordinateTransformation (reference, target);//new BasicCoordinateTransform(reference, target);
    }
    public static int GetZone(double latitude, double longitude)
    {


        return (int)(Mathf.Floor(((float)longitude + 180.0f) / 6) + 1);
    }

    public static Vector2 transformToUTM(float longitude, float latitude)
    {
        // For now we are gonna do everything with respect to the "local zone". I still do not know how to handle across zone datasets.
        int zone = localzone;
        
        
        //int zone = GetZone(latitude, longitude);
        zone = GetZone(latitude, longitude);
        //Transform to UTM
        //Debug.LogError("ZONE: " + zone);
        ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctfac = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
        ProjNet.CoordinateSystems.ICoordinateSystem wgs84geo = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
        ProjNet.CoordinateSystems.ICoordinateSystem utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, latitude > 0);
        ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84geo, utm);
        double[] pUtm = trans.MathTransform.Transform(new double[] {longitude, latitude });

        return new Vector2((float)pUtm[0], (float)pUtm[1]);
    }


	public static double[] transformToUTMDouble(float longitude, float latitude)
	{
		// For now we are gonna do everything with respect to the "local zone". I still do not know how to handle across zone datasets.
		//int zone = localzone;
		
		int zone = GetZone(latitude, longitude);
		//Transform to UTM
		//Debug.LogError("ZONE: " + zone);
		ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctfac = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
		ProjNet.CoordinateSystems.ICoordinateSystem wgs84geo = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
		ProjNet.CoordinateSystems.ICoordinateSystem utm = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(zone, latitude > 0);
		ProjNet.CoordinateSystems.Transformations.ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84geo, utm);
        
		double[] pUtm = trans.MathTransform.Transform(new double[] {longitude, latitude });
		
		return pUtm;
	}

	public static Vector2 transformToLatLong()
	{
		return new Vector2();
	}

    public static Vector3 transformToUnity(Vector3 world)
    {
        //Debug.LogError(world);
        //Debug.LogError(new Vector3(worldOrigin.x - world.x * worldScaleX, 1, worldOrigin.y - world.z * worldScaleZ));
        return new Vector3(worldOrigin.x - world.x * worldScaleX, 0, worldOrigin.y - world.z * worldScaleZ);
    }

	public static Vector3 transformToWorld(Vector3 world)
	{
		//Debug.LogError(world);
		//Debug.LogError(new Vector3(worldOrigin.x - world.x * worldScaleX, 1, worldOrigin.y - world.z * worldScaleZ));
		return new Vector3(worldOrigin.x + world.x * worldScaleX, 0, worldOrigin.y + world.z * worldScaleZ);
	}

    public static CoordinateTransformation createUnityTransform(SpatialReference source)
    {
        //Debug.Log(baseCoordSystem.Name);
        //Debug.Log(source.Name);
        //ProjNet.CoordinateSystems.CoordinateSystemFactory cf = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
		baseCoordSystem.ImportFromEPSG (4326);
		return new CoordinateTransformation(source,baseCoordSystem);//new BasicCoordinateTransform(source, baseCoordSystem);
    }


}