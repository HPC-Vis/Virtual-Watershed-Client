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
    static WorldCoordinateSystem cs;
    
    //public static CoordinateReferenceSystemFactory coordRefFactory = new CoordinateReferenceSystemFactory();
    //public static CoordinateReferenceSystem baseCoordSystem = coordRefFactory.CreateFromName("epsg:4326");
	public static SpatialReference baseCoordSystem = new SpatialReference("");

    // For now lets assume everything with respect to our zone :D.
    static public int localzone = 11;


    public static WorldCoordinateSystem CS
    {
        set
        {
            cs = value;
        }
    }

    public static Vector2 WorldOrigin
    {
        get
        {
            return cs.WorldOrigin;
        }
        set
        {
            cs.WorldOrigin = value;
        }
    }

    public static Vector2 UnityOrigin
    {
        get
        {
            return cs.UnityOrigin;
        }
        set
        {
            cs.UnityOrigin = value;
        }
    }

    public static float XUnityRes
    {
        get
        {
            return cs.worldScaleX;
        }
        set
        {
            cs.worldScaleX = value;
        }
    }

    public static float YUnityRes
    {
        get
        {
            return cs.worldScaleY;
        }
        set
        {
            cs.worldScaleY = value;
        }
    }

    public static CoordinateTransformation createTransform(SpatialReference reference, SpatialReference target)
    {
        return new BasicCoordinateTransform(reference, target);
    }

    public static CreateUnityTransform(SpatialReference target)
    {
        return cs.createUnityTransform(target);
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
		int zone = GetZone(latitude, longitude);

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