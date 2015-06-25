using UnityEngine;
using System.Collections;
using Proj4Net.Projection;
using Proj4Net;
using ProjNet;

public class StartupConfiguration : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {

	    // Setup global Coordinate System -- hard coding
        coordsystem.baseCoordSystem = coordsystem.coordRefFactory.CreateFromName("epsg:" + GlobalConfig.GlobalProjection.ToString());
        coordsystem.WorldOrigin = new Vector2(GlobalConfig.BoundingBox.center.x, GlobalConfig.BoundingBox.y - GlobalConfig.BoundingBox.height/2.0f);
        Logger.Log("Got the bounding box size of: " + GlobalConfig.BoundingBox.width + " " + GlobalConfig.BoundingBox.height);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

	static public void LoadConfig()
	{
		// Debug.LogError ("LOAD CONFIG");
		// Setup global Coordinate System -- hard coding
	    //coordsystem.UnityOrigin
		coordsystem.baseCoordSystem = coordsystem.coordRefFactory.CreateFromName("epsg:" + GlobalConfig.GlobalProjection.ToString());
		coordsystem.WorldOrigin = new Vector2(GlobalConfig.BoundingBox.center.x, GlobalConfig.BoundingBox.y - GlobalConfig.BoundingBox.height/2.0f);
		// Debug.LogError(GlobalConfig.BoundingBox.width + " " + GlobalConfig.BoundingBox.height);
		Logger.Log ("Loaded Bounding Box: " + GlobalConfig.BoundingBox.center.x + " " + GlobalConfig.BoundingBox.center.y);
	}

}
