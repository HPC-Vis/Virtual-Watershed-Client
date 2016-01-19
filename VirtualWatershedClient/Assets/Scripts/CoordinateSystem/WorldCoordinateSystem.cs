using UnityEngine;
using System.Collections;
using OSGeo.OSR;
using OSGeo.GDAL;
/// <summary>
/// WorldCoordinateSystem 
/// Author: Chase Carthen
/// Description: A abstract class that describes a coordinate system.
/// This class is meant to be used as a class that other coordinate systems are used to inherit from.
/// This class will require everything to happen in Lat Long.
/// </summary>
public abstract class WorldCoordinateSystem
{
    Vector2 unityOrigin;
    Vector2 worldOrigin;

    // For scaling the world..
    // Assumption 1 meter resolution per 1 unity.
    public float worldScaleX = 1, worldScaleY = 1;

    public static SpatialReference baseCoordSystem = new SpatialReference("");

    public bool NorthHemisphere;


    
    public WorldCoordinateSystem()
    {
        //Debug.LogError(OSGeo.GDAL.Gdal.GetConfigOption("GDAL_DATA", ""));
        OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DATA", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\data");
        //Debug.LogError(OSGeo.GDAL.Gdal.GetConfigOption("GDAL_DATA", ""));
        baseCoordSystem = new SpatialReference("");
        worldOrigin = unityOrigin = Vector2.zero;

        // By Default we are in EPSG:4326
        baseCoordSystem.ImportFromEPSG(4326);
    }

    public Vector2 WorldOrigin
    {
        get
        {
            return worldOrigin;
        }
        set
        {
            worldOrigin = value;
            UpdateInternalOrigin();
            //Debug.LogError("NEW WORLD ORIGIN: " + worldOrigin);
            //Debug.LogError("Interal Origin: " + InternalOrigin);
        }
    }

    public Vector2 UnityOrigin
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

    public Vector2 InternalOrigin;
    // Our world translation functions --- When using these functions we will assume there are no conversions to be made.
    public abstract Vector2 TranslateToUnity(Vector2 World);
    public abstract Vector2 TranslateToWorld(Vector2 Unity);
    // This function must be called everytime the origin changes. -- This keeps a origin in the coordinate systems preference -- like utm
    public abstract void UpdateInternalOrigin();

    /// <summary>
    /// createUnityTransform generates a tranformation to be used for tranforming from one coordinates system to another.
    /// </summary>
    /// <param name="source">The source spatial reference to be created for a tranformation. </param>
    /// <returns></returns>
    public CoordinateTransformation createUnityTransform(SpatialReference source)
    {
        baseCoordSystem.ImportFromEPSG(4326);
        return new CoordinateTransformation(source, baseCoordSystem);
    }

}
