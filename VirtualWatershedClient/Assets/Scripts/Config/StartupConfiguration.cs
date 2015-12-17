using UnityEngine;
using System.Collections;
using Proj4Net.Projection;
using Proj4Net;
using ProjNet;

public class StartupConfiguration : MonoBehaviour {

	// Use this for initialization
	void Awake () 
    {
        
        //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DATA", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\data\");
        Debug.LogError(OSGeo.GDAL.Gdal.GetConfigOption("GDAL_DATA",""));
	    // Setup global Coordinate System -- Currently hard coded, add ability to set these at runtime 
		//coordsystem.baseCoordSystem = new OSGeo.OSR.SpatialReference ("");//coordsystem.coordRefFactory.CreateFromName("epsg:" + GlobalConfig.GlobalProjection.ToString());
		//coordsystem.baseCoordSystem.ImportFromEPSG (GlobalConfig.GlobalProjection);
        coordsystem.WorldOrigin = new Vector2(GlobalConfig.BoundingBox.center.x, GlobalConfig.BoundingBox.y - GlobalConfig.BoundingBox.height/2.0f);
        Logger.Log("Got the bounding box size of: " + GlobalConfig.BoundingBox.width + " " + GlobalConfig.BoundingBox.height);
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
		coordsystem.WorldOrigin = new Vector2(GlobalConfig.BoundingBox.center.x, GlobalConfig.BoundingBox.y - GlobalConfig.BoundingBox.height/2.0f);
		Logger.Log ("Loaded Bounding Box: " + GlobalConfig.BoundingBox.center.x + " " + GlobalConfig.BoundingBox.center.y);
	}

}
