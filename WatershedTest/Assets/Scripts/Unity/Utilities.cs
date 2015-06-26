using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;
using System.IO;
public class Utilities
{
    // the train has left the station
    //public Terrain train;
    public float[,] trainData;
    public Texture2D terrainTex;
    public Texture2D testor;
    public Material projectorMaterial;
    public List<Color> Palette;
    public List<float> Ranges;
    public bool texture = true; //texture true = contour, false = continous

    float min, max;
    Sprite s;

	public static Rect bboxSplit(string bbox)
	{
		bbox = bbox.Replace('[', ' ');
		bbox = bbox.Replace(']', ' ');
		bbox = bbox.Replace('\"', ' ');
		bbox = bbox.Replace(',', ' ');
		string[] coords = bbox.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		bbox = coords[0] + ',' + coords[1] + ',' + coords[2] + ',' + coords[3];
		float minx = float.Parse(coords[0]);
		float miny = float.Parse(coords[1]);
		float maxx = float.Parse(coords[2]);
		float maxy = float.Parse(coords[3]);
		return new Rect(Mathf.Min(minx,maxx),miny,Math.Abs(minx-maxx),Math.Abs(miny-maxy));
	}

    // A temporary patch
    public static Rect bboxSplit2(string bbox)
    {
        bbox = bbox.Replace('[', ' ');
        bbox = bbox.Replace(']', ' ');
        bbox = bbox.Replace('\"', ' ');
        bbox = bbox.Replace(',', ' ');
        string[] coords = bbox.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        bbox = coords[0] + ',' + coords[1] + ',' + coords[2] + ',' + coords[3];
        float minx = float.Parse(coords[0]);
        float miny = float.Parse(coords[1]);
        float maxx = float.Parse(coords[2]);
        float maxy = float.Parse(coords[3]);
        return new Rect(minx, maxy, Math.Abs(minx - maxx), Math.Abs(miny - maxy));
    }
	// WCS bbox
	public static Rect bboxSplit3(string bbox)
	{
		bbox = bbox.Replace('[', ' ');
		bbox = bbox.Replace(']', ' ');
		bbox = bbox.Replace('\"', ' ');
		bbox = bbox.Replace(',', ' ');
		string[] coords = bbox.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		bbox = coords[0] + ',' + coords[1] + ',' + coords[2] + ',' + coords[3];
		float minx = float.Parse(coords[0]);
		float miny = float.Parse(coords[1]);
		float maxx = float.Parse(coords[2]);
		float maxy = float.Parse(coords[3]);
		return new Rect(minx, maxy, Math.Abs(minx - maxx), Math.Abs(miny - maxy));
	}

	public static Rect bboxSplit4(string bbox)
	{
		bbox = bbox.Replace('[', ' ');
		bbox = bbox.Replace(']', ' ');
		bbox = bbox.Replace('\"', ' ');
		bbox = bbox.Replace(',', ' ');
		string[] coords = bbox.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		bbox = coords[0] + ',' + coords[1] + ',' + coords[2] + ',' + coords[3];
		float minx = float.Parse(coords[0]);
		float miny = float.Parse(coords[1]);
		float maxx = float.Parse(coords[2]);
		float maxy = float.Parse(coords[3]);
		return new Rect(minx, miny, Math.Abs(minx - maxx), Math.Abs(miny - maxy));
	}
	
	public void PlaceProjector(Projector projector, DataRecord record)
	{
		//projector.name = "SPAWNED PROJECTOR";
		record.boundingBox = new SerialRect (bboxSplit(record.bbox));

        projector.gameObject.GetComponent<transform> ();
		if (projector.gameObject.GetComponent<transform>() != null) {
			Component.Destroy(projector.gameObject.GetComponent<transform>());
		}
		var tran = projector.gameObject.AddComponent<transform>();
		tran.createCoordSystem(record.projection); // Create a coordinate transform
		//Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));
		
		tran.setOrigin(coordsystem.WorldOrigin);

		Vector2 point = tran.transformPoint(new Vector2(record.boundingBox.x, record.boundingBox.y));
		Vector2 upperLeft = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y)));
		Vector2 upperRight = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x+record.boundingBox.width,record.boundingBox.y)));
		Vector2 lowerRight = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x+record.boundingBox.width,record.boundingBox.y-record.boundingBox.height)));;
		Vector2 lowerLeft = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y-record.boundingBox.height)));
		
		point = upperLeft;
		Vector3 pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
		pos.y += 10;

		point = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y)));
		float dim = Math.Max(Math.Abs((upperLeft - upperRight).x) / 2.0f,Math.Abs((upperLeft-lowerLeft).y)/2.0f);

        pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
		pos.y += 3000;
		pos.x += dim;
		pos.z += dim;
		projector.transform.position = pos;
		
		var pro = projector.GetComponent<Projector> ();
		pro.farClipPlane = 10000;
		pro.orthographicSize = Math.Max(Math.Abs((upperLeft - upperRight).x) / 2.0f,Math.Abs((upperLeft-lowerLeft).y)/2.0f);

		float boundingAreaX = Mathf.Abs ((upperLeft.x - upperRight.x) / (2.0f*pro.orthographicSize));
		float boundingAreaY = Mathf.Abs ((upperLeft.y - lowerLeft.y) / (2.0f*pro.orthographicSize));
		// Debug.LogError ("MAX X: " + boundingAreaX + " MAX Y: " + boundingAreaY);
		// Debug.LogError ("MAX X: " + (upperLeft.x - upperRight.x) + " MAX Y: " + boundingAreaY);
		pro.material = Material.Instantiate (pro.material);
		pro.material.SetFloat ("_MaxX", boundingAreaX);
		pro.material.SetFloat ("_MaxY", boundingAreaY);
	}

	public void PlaceProjector2(Projector projector, DataRecord record)
	{
        Debug.LogError("<size=16>We Are Still In A Temp Patch</size>");
		// Debug.LogError ("WCS BBOX: " + record.bbox2);
		// Debug.LogError ("JSON BBOX: " + record.bbox);
		//projector.name = "SPAWNED PROJECTOR";
		if( record.bbox2 == "" || record.bbox2 == null || (Math.Abs(bboxSplit(record.bbox2).x) > 180  && Math.Abs(bboxSplit(record.bbox2).y) > 180) )
	    {
	    	record.boundingBox = new SerialRect (bboxSplit(record.bbox));
	    }
	    else
	    {
			record.boundingBox = new SerialRect (bboxSplit(record.bbox2));
	    }
		
		projector.gameObject.GetComponent<transform> ();
		if (projector.gameObject.GetComponent<transform>() != null) {
			Component.Destroy(projector.gameObject.GetComponent<transform>());
		}
		var tran = projector.gameObject.AddComponent<transform>();
		tran.createCoordSystem(record.projection); // Create a coordinate transform
		//Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));
		
		tran.setOrigin(coordsystem.WorldOrigin);
		
		Vector2 point = tran.transformPoint(new Vector2(record.boundingBox.x, record.boundingBox.y));
		Vector2 upperLeft = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y)));
		Vector2 upperRight = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x+record.boundingBox.width,record.boundingBox.y)));
		Vector2 lowerRight = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x+record.boundingBox.width,record.boundingBox.y-record.boundingBox.height)));;
		Vector2 lowerLeft = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y-record.boundingBox.height)));
		
		point = upperLeft;
		Vector3 pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
		pos.y += 10;
		
		point = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y)));
		float dim = Math.Max(Math.Abs((upperLeft - upperRight).x) / 2.0f,Math.Abs((upperLeft-lowerLeft).y)/2.0f);
		
		pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
		pos.y += 3000;
		pos.x += dim;
		pos.z += dim;
		projector.transform.position = pos;
		
		var pro = projector.GetComponent<Projector> ();
		pro.farClipPlane = 10000;
		pro.orthographicSize = Math.Max(Math.Abs((upperLeft - upperRight).x) / 2.0f,Math.Abs((upperLeft-lowerLeft).y)/2.0f);
		
		float boundingAreaX = Mathf.Abs ((upperLeft.x - upperRight.x) / (2.0f*pro.orthographicSize));
		float boundingAreaY = Mathf.Abs ((upperLeft.y - lowerLeft.y) / (2.0f*pro.orthographicSize));
		// Debug.LogError ("MAX X: " + boundingAreaX + " MAX Y: " + boundingAreaY);
		// Debug.LogError ("MAX X: " + (upperLeft.x - upperRight.x) + " MAX Y: " + (upperLeft.y - lowerLeft.y));
		pro.material = Material.Instantiate (pro.material);
		pro.material.SetFloat ("_MaxX", boundingAreaX);
		pro.material.SetFloat ("_MaxY", boundingAreaY);
	}

	// Here are some projector building functions that need to be addressed
	public GameObject buildProjector(DataRecord record,bool type=false)
	{
		// Debug.LogError ("PROJECTOR: " + record.boundingBox.x + " " + record.boundingBox.y);
		// Debug.LogError ("BOUNDING BOX: " + record.boundingBox.width + " " + record.boundingBox.height);
		// GameObject GO;
		// GameObject GO2;
		// GameObject GO3;
		// GameObject GO4;
		// First Create a projector
		GameObject projector = GameObject.Instantiate(Resources.Load("SlideProjector/Prefabs/SlideProjector",typeof(GameObject))) as GameObject;
		projector.name = "SPAWNED PROJECTOR";
		// Debug.LogError (projector.name);
		// GO = GameObject.CreatePrimitive(PrimitiveType.Sphere);//Resources.Load("SlideProjector/Prefabs/SlideProjector") as GameObject;
		// GO2 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		// GO3 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		// GO4 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		// Second Place the projector
		// Time to place this guy somewhere
		// Set Gameobject Transform
		var tran = projector.AddComponent<transform>();
		tran.createCoordSystem(record.projection); // Create a coordinate transform
		// Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));
		
		tran.setOrigin(coordsystem.WorldOrigin);
		
		//Vector2 origin = tran.transformPoint(new Vector2(record.boundingBox.x + record.boundingBox.width, record.boundingBox.y));
		
		// tran.setOrigin(origin);
		
		
		Vector2 point = tran.transformPoint(new Vector2(record.boundingBox.x, record.boundingBox.y));
		Vector2 upperLeft = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y)));
		Vector2 upperRight = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x+record.boundingBox.width,record.boundingBox.y)));
		Vector2 lowerRight = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x+record.boundingBox.width,record.boundingBox.y-record.boundingBox.height)));;
		Vector2 lowerLeft = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y-record.boundingBox.height)));
		
		point = upperLeft;
		Vector3 pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
		pos.y += 10;


		// GO.transform.position = pos;
		// GO.transform.localScale = new Vector3 (100, 100, 100);

		point = upperRight;
		pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);

		// GO2.transform.position = pos;
		// GO2.transform.localScale = new Vector3 (100, 100, 100);

		point = lowerRight;
		pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);

		// GO3.transform.position = pos;
		// GO3.transform.localScale = new Vector3 (100, 100, 100);

		point = lowerLeft;
		pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);

		// GO4.transform.position = pos;
		// GO4.transform.localScale = new Vector3 (100, 100, 100);


		// Projector placement code
		point = tran.translateToGlobalCoordinateSystem(tran.transformPoint(new Vector2(record.boundingBox.x,record.boundingBox.y)));
		float dim = Math.Max(Math.Abs((upperLeft - upperRight).x) / 2.0f,Math.Abs((upperLeft-lowerLeft).y)/2.0f);

		pos = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
		pos.y += 3000;
		pos.x += dim;
		pos.z -= dim;
		projector.transform.position = pos;
	
		var pro = projector.GetComponent<Projector> ();
		pro.farClipPlane = 10000;
		pro.orthographicSize = Math.Max(Math.Abs((upperLeft - upperRight).x) / 2.0f,Math.Abs((upperLeft-lowerLeft).y)/2.0f);

		float boundingAreaX = Mathf.Abs ((upperLeft.x - upperRight.x) / (2.0f*pro.orthographicSize));
		float boundingAreaY = Mathf.Abs ((upperLeft.y - lowerLeft.y) / (2.0f*pro.orthographicSize));
		// Debug.LogError ("MAX X: " + boundingAreaX + " MAX Y: " + boundingAreaY);
		// Debug.LogError ("MAX X: " + (upperLeft.x - upperRight.x) + " MAX Y: " + boundingAreaY);
		pro.material = Material.Instantiate (pro.material);
		pro.material.SetFloat ("_MaxX", boundingAreaX);
		pro.material.SetFloat ("_MaxY", boundingAreaY);
		pro.material.SetInt ("_UsePoint", 0);
		if (record.texture != null) {
			Texture2D image = new Texture2D (1024, 1024, TextureFormat.ARGB32, false);
			image.wrapMode = TextureWrapMode.Clamp;
			image.LoadImage (record.texture);
			for (int i = 0; i < image.width; i++) {
				image.SetPixel (0, i, Color.clear);
				image.SetPixel (i, 0, Color.clear);
				image.SetPixel (image.width - 1, i, Color.clear);
				image.SetPixel (i, image.height - 1, Color.clear);
			}
			image.Apply ();
			pro.material.SetTexture ("_ShadowTex", image);
		} 
		else if (record.Data != null) 
		{
			PlaceProjector2(pro,record);
			Texture2D image = buildTextures(normalizeData(record.Data),Color.grey,Color.blue);
			image.wrapMode = TextureWrapMode.Clamp;

			for (int i = 0; i < image.width; i++) 
			{
				image.SetPixel (i, 0, Color.clear);
				image.SetPixel (i, image.height - 1, Color.clear);
			}

			for(int i =0; i <image.height;i++)
			{
				image.SetPixel (0, i, Color.clear);
				image.SetPixel (image.width - 1, i, Color.clear);
			}
			image.Apply ();

			pro.material.SetTexture("_ShadowTex",image);
		}

		                        // Third what type of data are we visualizing...
		// Determine if the data is a square or rect
		// if square add it to the project material
		//else it is a rectangle....
		// pad the texture with the approriate bounds
		// add it to the projector or material
		
		// Return the object 
		return projector;
	}

	float[,] rotateData(float[,] data)
	{
		int width = data.GetLength (0);
		int height = data.GetLength (1);
		float[,] Data = new float[height, width];
		for(int i = 0; i < height; i++)
		{
			for(int j =0; j < width;j++)
			{
				Data[i,width-j-1] = data[j,i]; 
			}
		}
		return Data;
	}

	public float[,] reflectData(float[,] data)
	{
		int width = data.GetLength (0);
		int height = data.GetLength (1);
		float[,] Data = new float[width, height];
		for(int i = height-1; i >= 0; i--)
		{
			for(int j = width-1; j >= 0; j--)
			{
				Data[i,j] = data[width-1-i,height-1-j]; 
			}
		}
		return Data;
	}
	
    /// Temporary Gameobject variable
    GameObject SHARP;
    // =========================================
    //         TERRAIN BUILDING FUNCTIONS
    // =========================================

    public GameObject buildTerrain(DataRecord record)
    {
        TerrainData terrainData = new TerrainData();
        GameObject terrainGO = Terrain.CreateTerrainGameObject(terrainData);
        Terrain terrain = terrainGO.GetComponent<Terrain>();
        // Debug.Log("Giving data to terrain");
		record.Data = reflectData (record.Data);
        findMinMax(record.Data, ref this.min, ref this.max);

        float[,] normalizedData = trainData = normalizeData(record.Data);

        // Set Terrain Resolution
        terrainData.heightmapResolution = record.Data.GetLength(0) + 1;

        // Set Terrain Data
        terrainData.SetHeights(0, 0, normalizedData);

        StreamWriter writar = new StreamWriter("heights.txt");
        writar.Write(trainData);
        writar.Close();
        

        //Find Min Max
        float min=float.MaxValue,max=float.MinValue;
        findMinMax(record.Data,ref  min, ref max);

        Logger.WriteLine("Building terrain with the data: MAX: " + max + " MIN: " + min + " Resolution: " + record.Resolution.x + " " + record.Resolution.y + " Width: " + record.width + " Height: " + record.height);

        //Set size of terrain
        terrainData.SetHeights(0, 0, normalizedData);

        // Resolution is the total height and total width
		terrainData.size = new Vector3(Mathf.Abs(record.Resolution.x  / coordsystem.worldScaleZ), (max - min) / coordsystem.worldScaleY, Mathf.Abs(record.Resolution.y/ coordsystem.worldScaleX));
        
        terrain.basemapDistance = 0;

        // Set Gameobject Transform
        var tran = terrain.gameObject.AddComponent<transform>();
        tran.createCoordSystem(record.projection); // Create a coordinate transform
        // Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));

        Vector2 origin = tran.transformPoint(new Vector2(record.boundingBox.x + record.boundingBox.width, record.boundingBox.y));

        tran.setOrigin(coordsystem.WorldOrigin);

        // Set world origin
        //coordsystem.WorldOrigin = origin;
        
        // Add this guy to some data structure -- Need to figure this one out before moving forward......
        //tran.translateToGlobalCoordinateSystem();
        Color first = new Color(0f / 255f, 46f / 255f, 184f / 255f);
        Color second = new Color(46f / 255f, 0f / 255f, 184f / 255f);
        Color third = new Color(102f / 255f,51f / 255f, 255f / 255f);
        Color fourth = new Color(138f / 255f, 0f / 255f, 184f / 255f);
        Color fifth = new Color(184f / 255f, 46f / 255f, 0f / 255f);
        Color sixth = new Color(184f / 255f, 138f / 255f, 0f / 255f);
        Color seventh = new Color(245f / 255f, 184f / 255f, 0f / 255f);
        Color eigth = new Color(255f / 255f, 104f / 255f, 51f / 255f);
        Color ninth = new Color(184f / 255f, 138f / 255f, 0f / 255f);
        Palette = new List<Color>{ first,second,third, fourth,fifth,sixth,seventh,eigth,ninth,Color.white};
        Ranges = new List<float> { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f };
        // Create and set Texture for terrain
        terrainTex = buildGradientContourTexture(normalizedData, Palette, Ranges);
        //projectorMaterial.SetTexture("_ShadowTex", terrainTex);
        //s = Sprite.Create(CreateGradientTexture(Palette, Ranges), new Rect(0, 0, 1, 100), new Vector2(0, 0));
        //gradSet.sprite = s;
        testor = CreateGradientTexture(Palette, Ranges);//CreateGradientTexture(Palette, Ranges);


        // Debug.Log(" TERRAIN Long Lat : " + record.boundingBox.x + " " + record.boundingBox.y);


        //makes the terrain ugly - find a fix for this
        Terrain.activeTerrain.heightmapMaximumLOD = 0;
        terrain.basemapDistance = 0;
        //GameObject.Find("Main Camera").GetComponent<Camera>().transparencySortMode = TransparencySortMode.Perspective;
        Debug.Log(record.Resolution.x + " " + record.Resolution.y);
        Debug.Log(record.boundingBox.x);

        // Rebuild Shapes
        if (SHARP != null)
            rebuildShape(SHARP);

        return terrainGO;
    }

    public void Rebuild()
    {
 
    }


    // finds the minimum and maximum values from a heightmap
    public void findMinMax(float[,] data, ref float min, ref float max)
    {
    	float min2 = float.MaxValue;
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
				float val = data[i,j];
                if (val > max)
                {
                    max = val;
                }
                if (val < min && val != min)//&& min != 0)
                {
                    min = val;
                }
                if(min2 > min && min2 != min)
                {
                	min2 = min;
                }
            }
        }
        min = min2;
    }

    public float[,] normalizeData(float[,] data)
    {
        // (point - min) / (max-min)
        float max = float.MinValue;
        float min = float.MaxValue;
        float[,] outData = new float[0, 0];
        if( data != null)
        {
            outData = new float[data.GetLength(0), data.GetLength(1)];
        }
        //Debug.Log(data.GetLength(0) + data.GetLength(1));

        findMinMax(data, ref min, ref max);
		// Debug.LogError ("MIN!!!!! " + min);
		// Debug.LogError ("MAX!!!!! " + max);
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                float val = (data[i, j] - min) / (max - min);
                //if(val < 0)
                //{
                //    val = 0;
                //}
                outData[i, j] = val;
            }
        }

        return outData;
    }

    // Bilinear Interpolation -- interpolates height and width to the nearest square power of two.
    float[,] interpolateValues(int dimension, int height, int width, float[,] data)
    {
        // Push power of two to one above
        ++dimension;

        float[,] outData = new float[dimension, dimension];
        //print("DIMENSION " + dimension);
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                float xVal = ((float)x / (float)dimension) * (height - 1); //(mapHeight-1)*((float)i/(float)(gridWidth))+x;
                float yVal = ((float)y / (float)dimension) * (width - 1);//(mapWidth-1)*((float)j/(float)(gridHeight))+y;

                //heightSamples[x,y] = (float)(ds.grid[name].data[(int)yVal*mapWidth + (int)xVal]-min)/(float)(max-min);
                //continue;
                // Check to make sure xVa		
                float xRat = xVal - Mathf.Floor(xVal);
                float yRat = yVal - Mathf.Floor(yVal);
                float UL = data[(int)Mathf.Floor(xVal), (int)Mathf.Floor(yVal)];
                float UR = data[(int)Mathf.Ceil(xVal), (int)Mathf.Floor(yVal)];//arr[(int)Mathf.Floor(yVal)*width+(int)Mathf.Ceil(xVal)];
                float LL = data[(int)Mathf.Floor(xVal), (int)Mathf.Ceil(yVal)];//arr[(int)Mathf.Ceil(yVal)*width+(int)Mathf.Floor(xVal)];
                float LR = data[(int)Mathf.Ceil(xVal), (int)Mathf.Ceil(yVal)];//arr[(int)Mathf.Ceil(yVal)*width+(int)Mathf.Ceil(xVal)];

                float s1 = Mathf.Lerp(UL, LL, yRat); //UL * (1.0f - yRat) + LL * (yRat);
                float s2 = Mathf.Lerp(UR, LR, yRat); // UR * (1.0f - yRat) + LR * (yRat);
                float v = Mathf.Lerp(s1, s2, xRat);// s1 * (1.0f - xRat) + s2 * (xRat);

                if (v < 0)
                {
                    v = 0;
                }
                outData[x, y] = v;

            }
        }
        return outData;
    }

    // http://stackoverflow.com/questions/5525122/c-sharp-math-question-smallest-power-of-2-bigger-than-x
    // Finds the clost power of two
    int nearestPowerOfTwo(int x)
    {
        x--; // comment out to always take the next biggest power of two, even if x is already a power of two
        x |= (x >> 1);
        x |= (x >> 2);
        x |= (x >> 4);
        x |= (x >> 8);
        x |= (x >> 16);
        return (x + 1);
    }

    // MORE TO ADD FOR TERRAIN BUILDING //////////////////

    // =========================================
    //         TEXTURE BUILDING FUNCTIONS
    // =========================================

    /// <summary>
    /// Builds a texture dependent upon the heightmap data passed in
    /// </summary>
    /// <param name="data">The heightmap data that the texture is being created for</param>
    /// <param name="low">The color of low terrain</param>
    /// <param name="high">The color of high terrain</param>
    /// <returns>A texture shall be applied to the terrain calling this function, 
    /// interpolating between the high and low colors dependent upon terrain height</returns>
    public Texture2D buildTextures(float[,] data, Color high, Color low)
    {
        int width = data.GetLength(0);
        int height = data.GetLength(1);
        Texture2D heightMapTexture = new Texture2D(width, height,TextureFormat.ARGB32,false);
		heightMapTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] colors = new Color[width * height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(data[i,j] > 0)
                colors[(width - i - 1) * height + (height - j - 1)] = Color.Lerp(high, low, data[i, j]);
                else
                colors[(width - i - 1) * height + (height - j - 1)] = Color.clear;
            }
        }

        heightMapTexture.SetPixels(colors);
        heightMapTexture.Apply();
        return heightMapTexture;
    }

    float NormalizeToRange(float Value, float Min, float Max)
    {
        return (Value - Min) / (Max - Min);
    }

    public Texture2D CreateGradientTexture(List<Color> Colors, List<float> Ranges)
    {
        Texture2D gradTexture = new Texture2D(1, 100);
        Color[] colors = new Color[100];
        for(int i = 0; i < 100; i++)
        {
            for (int k = 0; k < Ranges.Count; k++)
            {
                if ((float)i/(float)100 < Ranges[k])
                {
                    colors[i] = Colors[k];
                    break;
                }
                else if (k == Ranges.Count - 1)
                {
                    colors[i] = Colors[k+1];
                }
            }
        }
        gradTexture.SetPixels(colors);
        gradTexture.Apply();

        // To be added somewhere else....
        //gradRanges.text = Math.Floor(((max - min) * Ranges[Ranges.Count - 1])).ToString() + "m - " + Math.Floor((max)).ToString() + "m \n";

        //for (int i = Ranges.Count - 1; i > 0; i--)
        //{
        //    gradRanges.text += Math.Floor(((max - min) * Ranges[i - 1])).ToString() + "m - " + Math.Floor(((max - min) * Ranges[i])).ToString() + "m \n";
        //}

        //gradRanges.text += Math.Floor(min).ToString() + "m - " + Math.Floor(((max - min) * Ranges[0])).ToString() + "m \n";


            return gradTexture;
    }

    public Texture2D buildDiscreteContourTexture(float[,] Data, List<Color> Colors, List<float> Ranges)
    {
        int width = Data.GetLength(0);
        int height = Data.GetLength(1);
        Texture2D heightMapContour = new Texture2D(width, height);
        Color[] colors = new Color[width * height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for(int k = 0; k < Ranges.Count; k++)
                {
                    if (Data[i, j] < Ranges[k] )
                    {
                        float max = Ranges[k];
                        float min = 0;
                        if (k == 0)
                        {
                            min = 0;
                        }
                        else
                        {
                            min = Ranges[k - 1];
                        }
                        if (NormalizeToRange(Data[i, j], min, max) > 0.9 || NormalizeToRange(Data[i, j], min, max) < 0.1)
                        {
                            colors[i * height + j] = Color.black;
                        }
                        else
                        {
                            colors[i * height + j] = Colors[k];
                        }

                        break;
                    }
                    else if(k == Ranges.Count-1)
                    {
                        colors[i * height + j] = Colors[k+1];
                    }
                }
            }
        }

        heightMapContour.SetPixels(colors);
        
        heightMapContour.Apply();
        return heightMapContour;
    }


    public Texture2D buildGradientContourTexture(float[,] Data, List<Color> Colors, List<float> Ranges)
    {
        int width = Data.GetLength(0);
        int height = Data.GetLength(1);
        Texture2D heightMapContour = new Texture2D(width, height);
        Color[] colors = new Color[width * height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < Ranges.Count; k++)
                {
                    if (Data[i, j] < Ranges[k])
                    {
                        float max = Ranges[k];
                        float min = 0;
                        if (k == 0)
                        {
                            min = 0;
                        }
                        else
                        {
                            min = Ranges[k - 1];
                        }
                        if(k==0)
                        colors[i * height + j] = Color.Lerp(Colors[0], Colors[1], NormalizeToRange(Data[i, j], min, max));
                        else
                        colors[i * height + j] = Color.Lerp(Colors[k], Colors[k+1], NormalizeToRange(Data[i, j], min, max));
                        break;
                    }
                    else if (k == Ranges.Count - 1)
                    {
                        float max = 1f;
                        float min = Ranges[k];
                        colors[i * height + j] = Colors[k + 1];
                        //colors[i * height + j] = Color.Lerp(Colors[k+1], Color.black, NormalizeToRange(Data[i, j], min, max));
                    }
                }
            }
        }

        heightMapContour.SetPixels(colors);
        heightMapContour.Apply();
        return heightMapContour;
    }

    public void SwapTexture()
    {
        Debug.Log("SWAPPING TEXTURES");
        if (texture == true)
        {
            terrainTex = buildDiscreteContourTexture(trainData, Palette, Ranges);
            //  GameObject.Find("Canvas").GetComponent<Image>().sprite = Sprite.Create(terrainTex, new Rect(0, 0, terrainTex.width, terrainTex.height), new Vector2());
            texture = false;
        }
        else
        {
            terrainTex = buildGradientContourTexture(trainData, Palette, Ranges);
            texture = true;
        }
        projectorMaterial.SetTexture("_ShadowTex", terrainTex);

    }

    // =========================================
    //          SHAPE BUILDING FUNCTIONS
    // =========================================
    public Color[] colors = new[] { Color.red, Color.red, Color.red, Color.red, new Color(.5f, .5f, .1f, 1f), Color.cyan, Color.magenta };
    int current = 0;

	GameObject addPoint( Vector2 point,transform tr)
	{
		GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		
		//determine world position of the point
		point = tr.transformPoint(point);
		
		point = tr.translateToGlobalCoordinateSystem(point);
		
		// determine position and size of cylinder using point information
		cylinder.transform.position = mouseray.raycastHitFurtherest(new Vector3(point.x, 0, point.y), Vector3.up);
		
		// set coloring variables of the cylinder
		cylinder.AddComponent<Light>();
		cylinder.GetComponent<Light>().range = 50.0f;
		cylinder.GetComponent<Light>().intensity = 100;
		Debug.LogError(-1*(GlobalConfig.TerrainBoundingBox.width/2));
		if (!GlobalConfig.TerrainBoundingBox.Contains(cylinder.transform.position) || (cylinder.transform.position.z < (-1*(GlobalConfig.TerrainBoundingBox.width/2))))
		{
			cylinder.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
			cylinder.GetComponent<Light>().color = Color.clear;
			cylinder.GetComponent<Renderer>().material.color = Color.clear;
		}
		else
		{
			cylinder.transform.localScale = new Vector3(.75f, 5.0f, .75f);
			cylinder.GetComponent<Light>().color = Color.red;
			cylinder.GetComponent<Renderer>().material.color = Color.red;
			var scaler = cylinder.AddComponent<Scaler>();
			scaler.FirstPersonControler = GameObject.Find("First Person Controller");
			scaler.NoClipGhostPlayer = GameObject.Find("NoClipFirstPersonController");
			
		}
		return cylinder;
	}

    public Material LineMaterial;

    GameObject addline(List<SerialVector2> Points,transform tr)
    {
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
        line.SetColors(colors[current % colors.Length], colors[current % colors.Length]);

        return lineObject;
    }

    // The buildShape function builds a bunch of shapes that remain attached to a parent gameobject.
    // This parent gameobject is used to move that shape around.
    public GameObject buildShape(DataRecord record)
    {
        GameObject parent = new GameObject();
        transform trans = parent.AddComponent<transform>();

        // Set Gameobject Transform
        trans.createCoordSystem("epsg:" + GlobalConfig.GlobalProjection.ToString()); // Create a coordinate transform
        
        //Vector2 origin = trans.transformPoint(new Vector2(record.boundingBox.x, record.boundingBox.y));

        // Set world origin
        //coordsystem.WorldOrigin = origin;
        Debug.Log("BOUNDING BOX: " + record.boundingBox.x + " " + record.boundingBox.y);
        trans.setOrigin(coordsystem.WorldOrigin);

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
        current++;
        SHARP = parent;
        return parent;
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
            /*var children = new List<GameObject>();
            foreach (Transform child in parent.transform)
            {
                children.Add(child.gameObject);
            }
            parent.transform.DetachChildren();
            children.ForEach(child => Destroy(child));*/
            

        //Destroy(parent);
        //buildShape(parent, shapes);
    }
    
    /// <summary>
    /// Floats to color32.
    /// </summary>
    /// <returns>The to color32.</returns>
    /// <param name="f">F.</param>
    public Color32 floatToColor32(float f)
    {
    	// Pull bytes out of float
    	var byes = BitConverter.GetBytes(f);
    	
    	Color32 convertedFloat = new Color32(byes[0],byes[1],byes[2],byes[3]);
    	return convertedFloat;
    }
    
    // Creepy how good this automatic summaries are VVV -- I didn't write that.
    /// <summary>
    /// Builds the data texture.
    /// </summary>
    /// <returns>The data texture.</returns>
    /// <param name="data">Data.</param>
    public Texture2D BuildDataTexture(float [,] data)
   	{
   		int width = data.GetLength(0);
   		int height = data.GetLength(1);
   		Texture2D tex = new Texture2D(width,height);
   		Color32[] colorData = new Color32[width*height];
   		for(int i = 0; i < width; i++)
   		{
   			for(int j = 0; j < height; j++)
   			{
   				colorData[i*height+j] = floatToColor32(data[i,j]);
   			}
   		}
   		tex.SetPixels32(colorData);
   		tex.Apply();
   		return tex;
   	}

}