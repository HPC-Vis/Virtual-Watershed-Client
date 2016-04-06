using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;
using System.IO;

public class Shape : WorldObject {

	// Use this for initialization
	void Start () {
        IsRaster = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override bool saveData(string filename, string format = "")
    {
        return false;
    }

    public override bool moveObject(Vector3 displacement)
    {
        Offset = displacement;
        gameObject.transform.position += displacement;
        return true;
    }

    public override bool changeProjection(string projectionString)
    {
        return false;
    }

    public override void alterData()
    {

    }

    public override void getData()
    {

    }

    public override SessionObjectStructure saveSessionData()
    {
        SessionObjectStructure structure = new SessionObjectStructure();
        structure.Name = record.name;
        structure.GameObjectPosition = gameObject.transform.position;
        structure.GameObjectOffset += Offset;
        structure.Projection = record.projection;
        structure.Sources = record.services;
        //structure.Modified =
        //structure.Created = record.s Need to acquire created data for datarecord. --- In the json from the virtual watershed 
        return structure;
    }

	int current=0;
	GameObject addPoint( Vector2 point,WorldTransform tr)
	{
		GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        Debug.LogError("POINT: " + point);

		//determine world position of the point
		point = tr.transformPoint(point);
		
		//point = tr.translateToGlobalCoordinateSystem(point);
		
		// determine position and size of cylinder using point information
		cylinder.transform.position = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
		
		// set coloring variables of the cylinder
		cylinder.AddComponent<Light>();
		cylinder.GetComponent<Light>().range = 50.0f;
		cylinder.GetComponent<Light>().intensity = 100;
		Debug.LogError("TERRAIN BOUNDING BOX: " + GlobalConfig.TerrainBoundingBox);
		if (!GlobalConfig.TerrainBoundingBox.Contains(cylinder.transform.position) )
		{
			cylinder.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			cylinder.GetComponent<Light>().color = Color.clear;
			cylinder.GetComponent<Renderer>().material.color = Color.clear;
		}
		else
		{
			cylinder.transform.localScale = new Vector3(.75f, 5.0f, .75f);
			cylinder.GetComponent<Light>().color = Color.red;
			cylinder.GetComponent<Renderer>().material.color = Color.red;
			//var scaler = cylinder.AddComponent<Scaler>();
			//scaler.FirstPersonController = GameObject.Find("First Person Controller");
			
		}
		cylinder.layer = LayerMask.NameToLayer("Terrain");
		return cylinder;
	}

	GameObject addline(List<SerialVector2> Points, WorldTransform tr)
	{
		Material LineMaterial = new Material (Shader.Find ("Transparent/VertexLit with Z"));
		List<Vector2> points = SerialVector2.ToVector2Array(Points.ToArray()).ToList();
		GameObject lineObject = new GameObject();
		LineRenderer line = lineObject.AddComponent<LineRenderer>();
		
		line.SetWidth(30, 30);
		line.SetVertexCount(points.Count);
		for (int index = 0; index < points.Count; index++)
		{
			Vector2 point = tr.transformPoint(points[index]);
			
			// For now we use the zone hack.... assuming everything is in the same zone..
			// Same zone hack ----- 
			/*int zone = coordsystem.GetZone(points[index].y, points[index].x);

            if (zone != coordsystem.localzone)
            {
                // Thanks to https://www.maptools.com/tutorials/utm/details
                pnt.x += (zone - coordsystem.localzone) * 674000f;
            }*/
			
			point = tr.translateToGlobalCoordinateSystem(point);
			Vector3 pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
			pos.y += 2;
			if (GlobalConfig.TerrainBoundingBox.Contains(pos) && (pos.z > (-1*(GlobalConfig.TerrainBoundingBox.width/2))))
			{
				line.SetPosition(index, pos);
			}
		}
		
		line.material = LineMaterial;//= new Material(Shader.Find("Particles/Additive"));
		
		// Setting colors to some prefined scheme ... We should do this procedurely.
		Color[] colors = new[] { Color.red, Color.red, Color.red, Color.red, new Color(.5f, .5f, .1f, 1f), Color.cyan, Color.magenta };
		line.SetColors(colors[current % colors.Length], colors[current % colors.Length]);
		lineObject.layer = LayerMask.NameToLayer("Terrain");
		return lineObject;
	}
	
	// The buildShape function builds a bunch of shapes that remain attached to a parent gameobject.
	// This parent gameobject is used to move that shape around.
	public void buildShape(DataRecord record)
	{
        Debug.LogError("BUILD SHAPE: ");
		GameObject parent = this.gameObject;
		WorldTransform trans = parent.AddComponent<WorldTransform>();
		
		// Set Gameobject Transform
		trans.createCoordSystem(record.projection); // Create a coordinate transform
		
		//Vector2 origin = trans.transformPoint(new Vector2(record.boundingBox.x, record.boundingBox.y));
		
		// Set world origin
		//coordsystem.WorldOrigin = origin;
		Debug.Log("BOUNDING BOX: " + record.boundingBox.x + " " + record.boundingBox.y);
		//trans.setOrigin(coordsystem.WorldOrigin);
		
		foreach (var shape in record.Lines)
		{
			if (shape.Count == 1)
			{
				// Build cylinder
				addPoint(SerialVector2.ToVector2Array(shape.ToArray())[0], trans).transform.parent = parent.transform;
			}
			else
			{
				// Lets build some lines.
				addline(shape,trans).transform.parent = parent.transform;
			}
		}
	}
	
	// Rebuild Shapes -- This will only take care of the gameobject case.... where we don't have an origin change.
	public void rebuildShape(GameObject parent)
	{
		int childCount = parent.transform.childCount;
		for (int i = 0; i < childCount; i++ )
		{
			Vector3 pos =  parent.transform.GetChild(i).position ;
			parent.transform.GetChild(i).position = mouseray.raycastHitFurtherest(new Vector3(pos.x, 0, pos.z), Vector3.up); ;
		}
	}
}
