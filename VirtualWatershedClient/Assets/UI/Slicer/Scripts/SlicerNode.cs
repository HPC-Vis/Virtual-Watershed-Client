using UnityEngine;
using System.Collections;
using System;

public class SlicerNode : MonoBehaviour {
	public string Latinput;
	public string Longinput;
	public string Elevation;
	public TextMesh Lat;
	public TextMesh Long;
	public TextMesh Ele;
    private Vector3 previousPosition;

	// Use this for initialization
	void Start () {
		//TextMesh Text = (TextMesh)GetComponent (typeof(TextMesh));
		Lat.fontSize = 14;
		Lat.text = Latinput;

		Long.fontSize = 14;
		Long.text = Longinput;

		Ele.fontSize = 14;
		Ele.text = Elevation;
	}

    void OnActivate()
    {
        previousPosition = new Vector3(-999999.0f, -999999.0f, -999999.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(transform.position != previousPosition)
        {
            SetLatLong();
            SetElevation();
            previousPosition = transform.position;
        }
    }

    private void SetElevation()
    {
        if (Terrain.activeTerrain != null)
        {
            // The old way
            float elevation;
            if (Terrain.activeTerrain.transform.position.y > 0)
            {
                elevation = (((mouseray.raycastHitFurtherest(transform.position, Vector3.up).y) * Terrain.activeTerrain.terrainData.heightmapHeight) + Terrain.activeTerrain.transform.position.y);
            }
            else
            {
                elevation = (((mouseray.raycastHitFurtherest(transform.position, Vector3.up).y) * Terrain.activeTerrain.terrainData.heightmapHeight) + (-1) * Terrain.activeTerrain.transform.position.y);
            }

            //float[,] data = TerrainUtils.GetHeightmap(Terrain.activeTerrain);
            //Rect BoundingBox = GlobalConfig.BoundingBox;
            //Vector3 WorldPoint = coordsystem.transformToWorld(transform.position);
            //Vector2 MarkerPoint = Utilities.GetDataPointFromWorldPoint(data, BoundingBox, WorldPoint);
            //Debug.LogError("SlicerNode: " + MarkerPoint + " " + WorldPoint + " " + BoundingBox);
            //float elevation = data[(int)MarkerPoint.x, (int)MarkerPoint.y];
            //Ele.text = elevation.ToString();
        }
    }

    private void SetLatLong()
    {
        Vector3 Point = coordsystem.transformToWorld(transform.position);
        Lat.text = Point.z.ToString("#,##0");
        Long.text = Point.x.ToString("#,##0");
    }
}
