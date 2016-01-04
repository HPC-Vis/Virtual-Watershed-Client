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
    static WorldCoordinateSystem cs = new HaversineCoordinateSystem();
    
    //public static CoordinateReferenceSystemFactory coordRefFactory = new CoordinateReferenceSystemFactory();
    //public static CoordinateReferenceSystem baseCoordSystem = coordRefFactory.CreateFromName("epsg:4326");
	//public static SpatialReference baseCoordSystem = new SpatialReference("");

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
        return new CoordinateTransformation(reference, target);
    }

    public static CoordinateTransformation createUnityTransform(SpatialReference target)
    {
        return cs.createUnityTransform(target);
    }

	public static Vector2 transformToLatLong()
	{
		return new Vector2();
	}
    
    public static Vector2 transformToUnity(Vector2 world)
    {
        return cs.TranslateToUnity(world);
    }

	public static Vector2 transformToWorld(Vector2 world)
	{
        return  cs.TranslateToWorld(world);
	}
    
}