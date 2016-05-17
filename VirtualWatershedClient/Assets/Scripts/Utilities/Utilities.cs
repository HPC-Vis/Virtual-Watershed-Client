using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;
using System.IO;

public static class Utilities
{
    // the train has left the station
    //public Terrain train;
    public static float[,] trainData;
    public static Texture2D terrainTex;
    public static Texture2D testor;
    public static Material projectorMaterial;
    public static List<Color> Palette;
    public static List<float> Ranges;
    public static bool texture = true;

    static float min, max;
    public static Sprite s;

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

	// Here are some projector building functions that need to be addressed
    public static GameObject buildProjector(DataRecord record, bool type = false)
	{
        // First Create a projector
        GameObject projector = GameObject.Instantiate(Resources.Load("Prefabs/SlideProjector/SlideProjector", typeof(GameObject))) as GameObject;
        projector.name = "SPAWNED PROJECTOR";
        var proj = projector.AddComponent<ProjectorObject>();
        proj.record = record;
        proj.buildProjector(record, type);
        ModelRunManager.sessionData.InsertSessionData(proj);

		return projector;
	}

	static float[,] rotateData(float[,] data)
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

    public static float[,] reflectData(float[,] data)
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
	

    // =========================================
    //         TERRAIN BUILDING FUNCTIONS
    // =========================================

    public static GameObject buildTerrain(DataRecord record)
    {
        TerrainData terrainData = new TerrainData();
        GameObject terrainGO = Terrain.CreateTerrainGameObject(terrainData);
        Terrain terrain = terrainGO.GetComponent<Terrain>();
        // Debug.Log("Giving data to terrain");
		record.Data[0] = reflectData (record.Data[0]);
        findMinMax(record.Data[0], ref Utilities.min, ref Utilities.max);

        float[,] normalizedData = trainData = normalizeData(record.Data[0]);

        // Set Terrain Resolution
        terrainData.heightmapResolution = record.Data[0].GetLength(0) + 1;

        // Set Terrain Data
        terrainData.SetHeights(0, 0, normalizedData);

        StreamWriter writar = new StreamWriter("heights.txt");
        writar.Write(trainData);
        writar.Close();
        

        //Find Min Max
        float min=float.MaxValue,max=float.MinValue;
        findMinMax(record.Data[0],ref  min, ref max);

        Logger.WriteLine("Building terrain with the data: MAX: " + max + " MIN: " + min + " Resolution: " + record.Resolution.x + " " + record.Resolution.y + " Width: " + record.width + " Height: " + record.height);

        //Set size of terrain
        terrainData.SetHeights(0, 0, normalizedData);

        // Resolution is the total height and total width
		terrainData.size = new Vector3(Mathf.Abs(record.Resolution.x  ), (max - min), Mathf.Abs(record.Resolution.y));
        
        terrain.basemapDistance = 0;

        // Set Gameobject Transform
        var tran = terrain.gameObject.AddComponent<WorldTransform>();
        tran.createCoordSystem(record.projection); // Create a coordinate transform
        // Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));

        Vector2 origin = tran.transformPoint(new Vector2(record.boundingBox.x + record.boundingBox.width, record.boundingBox.y));

        //tran.setOrigin(coordsystem.WorldOrigin);

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
        Utilities.Palette = new List<Color>{ first,second,third, fourth,fifth,sixth,seventh,eigth,ninth,Color.white};
        Ranges = new List<float> { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f };

        // Create and set Texture for terrain
        terrainTex = Utilities.buildGradientContourTexture(normalizedData, Utilities.Palette, Utilities.Ranges);
        testor = Utilities.CreateGradientTexture(Utilities.Palette, Utilities.Ranges);

        //makes the terrain ugly - find a fix for this
        Terrain.activeTerrain.heightmapMaximumLOD = 0;
        terrain.basemapDistance = 0;
        Debug.Log(record.Resolution.x + " " + record.Resolution.y);
        Debug.Log(record.boundingBox.x);

        return terrainGO;
    }



    // finds the minimum and maximum values from a heightmap
    public static void findMinMax(float[,] data, ref float min, ref float max)
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

    public static float[,] normalizeData(float[,] data)
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
    static float[,] interpolateValues(int dimension, int height, int width, float[,] data)
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
    static int nearestPowerOfTwo(int x)
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
    public static Texture2D buildTextures(float[,] data, Color high, Color low)
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

    static float NormalizeToRange(float Value, float Min, float Max)
    {
        return (Value - Min) / (Max - Min);
    }

    public static Texture2D CreateGradientTexture(List<Color> Colors, List<float> Ranges)
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

    public static Texture2D buildDiscreteContourTexture(float[,] Data, List<Color> Colors, List<float> Ranges)
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


    public static Texture2D buildGradientContourTexture(float[,] Data, List<Color> Colors, List<float> Ranges)
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
                        colors[i * height + j] = Color.Lerp(Colors[0], Colors[1], Utilities.NormalizeToRange(Data[i, j], min, max));
                        else
                        colors[i * height + j] = Color.Lerp(Colors[k], Colors[k+1], Utilities.NormalizeToRange(Data[i, j], min, max));
                        break;
                    }
                    else if (k == Ranges.Count - 1)
                    {
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

    public static void SwapTexture()
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

    // The buildShape function builds a bunch of shapes that remain attached to a parent gameobject.
    // This parent gameobject is used to move that shape around.
    public static GameObject buildShape(DataRecord record)
    {
        GameObject parent = new GameObject();
		var shape = parent.AddComponent<Shape> ();
		shape.buildShape (record);
        shape.record = record;
        ModelRunManager.sessionData.InsertSessionData(shape);
        

        return parent;
    }

    // Rebuild Shapes -- This will only take care of the gameobject case.... where we don't have an origin change.
    public static void rebuildShape(GameObject parent)
    {
        int childCount = parent.transform.childCount;
        for (int i = 0; i < childCount; i++ )
        {
            Vector3 pos =  parent.transform.GetChild(i).position ;
            parent.transform.GetChild(i).position = mouseray.raycastHitFurtherest(new Vector3(pos.x, 0, pos.z), Vector3.up); ;
        }
    }
    
    /// <summary>
    /// Floats to color32.
    /// </summary>
    /// <returns>The to color32.</returns>
    /// <param name="f">F.</param>
    public static Color32 floatToColor32(float f)
    {
    	// Pull bytes out of float
    	var byes = BitConverter.GetBytes(f);
    	
    	Color32 convertedFloat = new Color32(byes[3],byes[2],byes[1],byes[0]);
    	return convertedFloat;
    }
    
    // Creepy how good this automatic summaries are VVV -- I didn't write that.
    /// <summary>
    /// Builds the data texture.
    /// </summary>
    /// <returns>The data texture.</returns>
    /// <param name="data">Data.</param>
    public static Texture2D BuildDataTexture(float [,] data, out ValueContainer minimum, out ValueContainer maximum, out float mean)
   	{
        float min = float.MaxValue;
        float max = float.MinValue;
        minimum = new ValueContainer(min, new Vector2(0,0));
        maximum = new ValueContainer(max, new Vector2(0,0));
        mean = 0;
        float total = 0;
   		int width = data.GetLength(0);
   		int height = data.GetLength(1);
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
   		Color32[] colorData = new Color32[width*height];
   		for(int i = 0; i < width; i++)
   		{
   			for(int j = 0; j < height; j++)
   			{
   				colorData[(width-i-1)*height+(height-j-1)] = Utilities.floatToColor32(data[i,j]);
                total += data[i, j];
                if(data[i,j] > max)
                {
                    max = data[i,j];
                    maximum = new ValueContainer(max, new Vector2(i, j));
                    //Debug.LogError(data[i, j]);
                }
                if(data[i,j] < min)
                {
                    min = data[i,j];
                    minimum = new ValueContainer(min, new Vector2(i, j));
                    //Debug.LogError(data[i, j]);
                }
   			}
   		}

        // calculate mean 
        mean = total / (height * width);
        
        tex.wrapMode = TextureWrapMode.Clamp;
        //tex.filterMode = FilterMode.Point;
   		tex.SetPixels32(colorData);
   		tex.Apply();
   		return tex;
   	}

    /// <summary>
    /// Will take in a file name, and return a full file path to the desktop.
    /// Also will not overwrite any file names that currently exist.
    /// </summary>
    /// <param name="filename">The file name to be opened.</param>
    /// <returns>The full system path to open the file.</returns>
    public static String GetFilePath(String filename)
    {
        string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string[] filesplit = filename.Split('.');

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            string pathDownload = pathUser + "/";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        string pathDownload = pathUser + "\\";
#endif

        string bascase = pathDownload;
        pathDownload = pathDownload + filesplit[0] + "." + filesplit[1];
        int count = 1;
        while (File.Exists(pathDownload))
        {
            pathDownload = bascase + filesplit[0] + "(" + count + ")" + "." + filesplit[1];
            count += 1;
        }

        return pathDownload;
    }


    public static bool DataRecordGridToCsv(string filename, float[,] Data, string[,] headers=null)
    {
        String pathDownload = Utilities.GetFilePath(filename);
        using (StreamWriter file = new StreamWriter(@pathDownload))
        {
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                for (int j = 0; j < Data.GetLength(0); j++)
                {
                    file.Write(Data[i, j] + ", ");
                }
                file.Write("\n");
            }
        }

        return true;
    }

    

}