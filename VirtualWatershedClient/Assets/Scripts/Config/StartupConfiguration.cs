using UnityEngine;
using System.Collections;
using Proj4Net.Projection;
using Proj4Net;
using ProjNet;
using OSGeo.OSR;
public class StartupConfiguration : MonoBehaviour {

	// Use this for initialization
	void Awake () 
    {
        
        //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DATA", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\data\");
        LoadConfig();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

	static public void LoadConfig()
	{
		// Setup global Coordinate System -- hard coding
	    //coordsystem.UnityOrigin
		//coordsystem.baseCoordSystem = new OSGeo.OSR.SpatialReference ("");//coordsystem.coordRefFactory.CreateFromName("epsg:" + GlobalConfig.GlobalProjection.ToString());
		//coordsystem.baseCoordSystem.ImportFromEPSG (GlobalConfig.GlobalProjection);

        // Convert global config points to lat long
        SpatialReference sr = new SpatialReference("");
        sr.ImportFromEPSG(GlobalConfig.GlobalProjection);
        var transform = coordsystem.createUnityTransform(sr);
        //int zone = CoordinateUtils.GetZone(40, GlobalConfig.BoundingBox.center.x);

        // Set origin to configs transformed center
        double[] bboxcenter = { GlobalConfig.BoundingBox.center.x, GlobalConfig.BoundingBox.y - GlobalConfig.BoundingBox.height / 2.0f };
        transform.TransformPoint(bboxcenter);

        coordsystem.WorldOrigin = new Vector2((float)bboxcenter[0], (float)bboxcenter[1]);//new Vector2(GlobalConfig.BoundingBox.center.x, GlobalConfig.BoundingBox.y - GlobalConfig.BoundingBox.height/2.0f);
        coordsystem.UnityOrigin = Vector2.zero;
	}

}
