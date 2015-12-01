using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using Proj4Net.Projection;
using Proj4Net;

using OSGeo.OSR;


/// <summary>
/// transform
/// This class is attached to all gameobjects that are georeferenced.
/// It will keep track of its own origin in the world and will be used to translated the georeferenced object based on some user or program selected origin.
/// 
/// </summary>
public class transform : MonoBehaviour
{
    Vector2 origin;
    SpatialReference localCoords = new SpatialReference("");
    CoordinateTransformation localTrans;
    float xRes, yRes;

    public transform(string epsg) : base()
    {
        createCoordSystem(epsg);
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


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    // Original Origin in world coordinates...
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

    /// <summary>
    /// createCoordSystem creates our transform.
    /// </summary>
    /// <param name="epsg">The coordinate systems supported by our application are things that have an epsg value attached to it. </param>
    /// <returns></returns>
    public bool createCoordSystem(string epsg)
    {
		// Get the numeric part of the string
        if ("" != Regex.Match(epsg, @"(epsg:[0-9]+$)|(EPSG:[0-9][0-9][0-9][0-9]$)").Value)
        {
            var resultString = Regex.Match(epsg, @"\d+").Value;

            int EPSG = int.Parse(resultString);
            localCoords = new SpatialReference("");
            localCoords.ImportFromEPSG(EPSG);

        }
        else
        {
            localCoords = new SpatialReference(epsg);
        }
        localTrans = coordsystem.createUnityTransform(localCoords);

        
        return true;
    }

    public Vector2 transformPoint(Vector2 point)
    {
		double[] pointed = new double[]{point.x,point.y};
		localTrans.TransformPoint (pointed);
        string tts2="";
        localCoords.ExportToWkt(out tts2);
        return coordsystem.transformToUTM(point.x, point.y);
    }


    public void translateToGlobalCoordinateSystem()
    {
        // Call transform function
        Vector2 TransformedPoint = Vector2.zero;
        Vector3 pos = coordsystem.transformToWorld(Origin);
        gameObject.transform.position = new Vector3(pos.x,gameObject.transform.position.y,pos.y);//new Vector3((coordsystem.WorldOrigin.x - Origin.x) / coordsystem.worldScaleX, gameObject.transform.position.y, (coordsystem.WorldOrigin.y - Origin.y) / coordsystem.worldScaleZ);
    }

    public Vector2 translateToGlobalCoordinateSystem(Vector2 point)
    {
        // This is for more than one point
        Vector2 TransformedPoint = Vector2.zero;

        return TransformedPoint;//Vector2(-(coordsystem.WorldOrigin.x - point.x), -(coordsystem.WorldOrigin.y - point.y)) / coordsystem.worldScaleX;
    }

}