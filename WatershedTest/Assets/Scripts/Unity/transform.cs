using UnityEngine;
using System.Collections;
using Proj4Net.Projection;
using Proj4Net;

/// <summary>
/// transform
/// This class is attached to all gameobjects that are georeferenced.
/// It will keep track of its own origin in the world and will be used to translated the georeferenced object based on some user or program selected origin.
/// 
/// </summary>
public class transform : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 originalOrigin;
    Vector2 origin;
    CoordinateReferenceSystem localCoords;
    BasicCoordinateTransform localTrans;
    float xRes, yRes;


    public Vector2 Origin
    {
        get
        {
            return origin;
        }
        set
        {
            origin = value;
        }
    }

    public void setOrigin(Vector2 org)
    {
        originalOrigin = origin = org;
    }

    public void moveOrigin()
    {
        origin = transformPoint(origin);
    }

    public CoordinateReferenceSystem LocalCoords
    {
        get
        {
            return localCoords;
        }
        set
        {
            localCoords = value;
        }
    }

    public bool createCoordSystem(string epsg)
    {
        localCoords = coordsystem.coordRefFactory.CreateFromName(epsg);
        localTrans = coordsystem.createUnityTransform(localCoords);
        Debug.Log(localCoords.Name);
        return true;
    }

    public Vector2 transformPoint(Vector2 point)
    {
        //Debug.LogError(point);
        GeoAPI.Geometries.Coordinate tempSrc = new GeoAPI.Geometries.Coordinate(point.x, point.y);
        GeoAPI.Geometries.Coordinate tempTgt = new GeoAPI.Geometries.Coordinate();
        localTrans.Transform(tempSrc, tempTgt);
		//Debug.LogError(point);
        //Debug.LogError(coordsystem.transformToUTM(point.x, point.y));
        //point.x = (float)tempTgt.X;
        //point.y = (float)tempTgt.Y;

        return coordsystem.transformToUTM(point.x, point.y);
    }

    public Vector2 translateToGlobalCoordinateSystem(Vector2 point)
    {
        return new Vector2(-(coordsystem.WorldOrigin.x - point.x), -(coordsystem.WorldOrigin.y - point.y)) / coordsystem.worldScaleX;
    }

    public void translateToGlobalCoordinateSystem()
    {
        gameObject.transform.position = new Vector3((coordsystem.WorldOrigin.x - Origin.x) / coordsystem.worldScaleX, gameObject.transform.position.y, (coordsystem.WorldOrigin.y - Origin.y) / coordsystem.worldScaleZ);
    }

    public float XRes
    {
        get
        {
            return xRes;
        }
        set
        {
            xRes = value;
        }
    }

    public float YRes
    {
        get
        {
            return yRes;
        }
        set
        {
            yRes = value;
        }
    }
}