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
public class WorldTransform : MonoBehaviour
{
    Vector2 origin;
    SpatialReference localCoords = new SpatialReference("");
    CoordinateTransformation localTrans;
    float xRes, yRes;

    public WorldTransform(string epsg="EPSG:4326") : base()
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


    /// <summary>
    /// transformPoint changes a point from its local earth coordinate system into a Lat Long.
    /// </summary>
    /// <param name="point">This must be a earth coordinate point</param>
    /// <returns></returns>
    public Vector2 transformPoint(Vector2 point)
    {
		double[] pointed = new double[]{point.x,point.y};
		localTrans.TransformPoint (pointed);
        string tts2="";
        localCoords.ExportToWkt(out tts2);
        return CoordinateUtils.transformToUTM(point.x, point.y);
    }

    // This is for translating the attached gameobject to a location.
    public void translateToGlobalCoordinateSystem()
    {
        Vector3 pos = coordsystem.transformToUnity(Origin);
        gameObject.transform.position = new Vector3(pos.x,gameObject.transform.position.y,pos.y);
    }


    // This is for more than one point -- points should be in 
    public Vector2 translateToGlobalCoordinateSystem(Vector2 point)
    {
        return coordsystem.transformToUnity(point);
    }

}