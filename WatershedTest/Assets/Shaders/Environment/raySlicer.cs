using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class raySlicer : MonoBehaviour
{
    int height, width;
    float[,] soilDrawArea;
    Mesh mesh;
    public Texture2D guit;
    public Material screenMaterial;
    
    bool display_soil_visualizer;
    RenderTexture test;
    public int marginHorizontal = 15;
    public int marginVertical = 11;
    public float min;
    public float max;
    public string units = "m";
    public GameObject cursor;

    public float displayMin = 1.0f;
    public float displayMax = 1.0f;
    public Vector2 firstPoint;
    public Vector2 secondPoint;
    public Texture3D environmentTex;
    public Texture2D slicerMap; //= new Texture2D(100, 100, TextureFormat.ARGB32, false);
    public SpriteRenderer spriteRend;
    public Sprite sliceSprite;
    //public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../Images";
    
	public ComputeShader CS;
	public MinMaxShader MinMax;

    public Texture2D clear2d;
    public Texture3D clear3d;

#if UNITY_EDITOR
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../Images";
#else
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Images";
#endif

    public static string ImageLoc = DirectoryLocation + "/" + GlobalConfig.Location + "terrainMap.png";

    // Used for knowing when min and max is to be calculated
    bool calc_min_max = true;

    // Use this for initialization

    void Start()
    {
		slicerMap = new Texture2D(100, 100, TextureFormat.ARGB32, false);
		MinMax = new MinMaxShader (CS);
		if (!Directory.Exists (DirectoryLocation))
		{
			Directory.CreateDirectory(DirectoryLocation);
		}

        height = 100;
        width = 100;
        
        setFirstPoint(Vector2.zero);
        setSecondPoint(new Vector2(1f, 1f));


        spriteRend.sprite = sliceSprite;
        //texas = GetComponent<UITexture>();
        spriteRend.material = screenMaterial;
        spriteRend.sprite = sliceSprite;
        guit = new Texture2D(height, width);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                guit.SetPixel(i, j, new Color(1,0,0,(float)i/(float)height));
            }

        }

        //create clear textures 
        
        //screenMaterial = texas.drawCall.dynamicMaterial;
        guit.Apply();
        generate3DTexture();
        screenMaterial.SetTexture("_MainTex2", guit);
        //Terrain.activeTerrains.Length
        // Stitch multiple terrains together 
        clear2d = new Texture2D(1, 1);
        clear2d.SetPixel(1, 1, new Color(0, 0, 0, 0));

        clear3d = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false);
        clear3d.SetPixels(new Color []{new Color(0, 0, 0, 0)});

            if (Terrain.activeTerrains.Length > 1)
            {
                // Debug.LogError("STITCHING TIME");
                List<Rect> Rects = new List<Rect>();
                List<float[,]> HeightMaps = new List<float[,]>();
                int count = 0;
                foreach (var i in Terrain.activeTerrains)
                {
                    HeightMaps.Add(i.terrainData.GetHeights(0, 0, i.terrainData.heightmapWidth, i.terrainData.heightmapHeight));
                    Rects.Add(new Rect(i.transform.position.x, i.transform.position.z, i.terrainData.heightmapWidth, i.terrainData.heightmapHeight));
                }

                // Assume all terrain are near each other.... otherwise fail

                // Time to stitch these terrains...
                for (int i = 0; i < Terrain.activeTerrains.Length; i++)
                {
                    for (int j = i + 1; j < Terrain.activeTerrains.Length; j++)
                    {
                        // Determine if they can all be stitch together
                        // Debug.LogError(Terrain.activeTerrains[i].s);
                        // Debug.LogError(i + " " + j);
                        // Debug.LogError(Rects[i].Contains(Rects[j].position));
                        if (Rects[i].Contains(Rects[j].position))
                            count++;
                    }

                }
                if (count == Terrain.activeTerrains.Length - 1)
                {
                    float upperLeftX = float.MaxValue;
                    float upperLeftY = float.MaxValue;
                    int x = -1;
                    int y = -1;
                    float[,] heights = new float[Terrain.activeTerrain.terrainData.heightmapWidth * 2, Terrain.activeTerrain.terrainData.heightmapHeight * 2];

                    // Time to stitch them all ... first find the correct order for things 
                    for (int i = 0; i < Terrain.activeTerrains.Length; i++)
                    {
                        // Debug.LogError(Rects[i].yMin);
                        // Debug.LogError(Rects[i].xMin);
                        if (upperLeftX > Rects[i].xMin)
                        {
                            x = i;

                            upperLeftX = Rects[i].xMin;
                        }

                        if (upperLeftY > Rects[i].yMin)
                        {
                            y = i;
                            upperLeftY = Rects[i].yMin;
                        }
                    }

                    //Build the heightmap texture for the terrain slicer
                    buildHeightMap(heights, Rects, x, y, HeightMaps);
                    
                    screenMaterial.SetTexture("_MainTex2", slicerMap);
                    MinMax.SetDataArray(slicerMap);
                    // Debug.LogError("SETTING HEIGHTS");
                }

                float max = 0.0f;
                foreach (var terrain in Terrain.activeTerrains)
                {
                    max = Mathf.Max(max, terrain.terrainData.size.y);
                }

                MinMax.SetMax(max);

            }
            else
            {
                GlobalConfig.TerrainBoundingBox = new Rect(Terrain.activeTerrain.transform.position.x, Terrain.activeTerrain.transform.position.z, GlobalConfig.BoundingBox.width, GlobalConfig.BoundingBox.height);
                slicerMap = TerrainUtils.GetHeightMapAsTexture(Terrain.activeTerrain);
                screenMaterial.SetTexture("_MainTex2", slicerMap);
                float max = Terrain.activeTerrain.terrainData.size.y;
                MinMax.SetDataArray(slicerMap);
                MinMax.SetMax(max);
            }

            //screenMaterial.SetFloat("_Min", min);
            //screenMaterial.SetFloat("_Max", MinMax.max);
        
    }

    public void buildHeightMap(float[,] heights, List<Rect> Rects, int x, int y, List<float[,]> HeightMaps)
    {

        byte[] terrainBytes;
        float xRes = 1.0f;// / (float)(Terrain.activeTerrain.terrainData.heightmapWidth*2);
        float yRes = 1.0f;// / (float)(Terrain.activeTerrain.terrainData.heightmapHeight*2);

        // Fill in the heightmap at the appropriate place
        if (File.Exists(ImageLoc))
        {
            terrainBytes = File.ReadAllBytes(ImageLoc);
            Debug.LogError("RETRIEVING slicer map");
            slicerMap.LoadImage(terrainBytes);
            GlobalConfig.TerrainBoundingBox = Rects[y];
            GlobalConfig.TerrainBoundingBox.x = Rects[x].x;
            GlobalConfig.TerrainBoundingBox.height = GlobalConfig.TerrainBoundingBox.height * 2;
            GlobalConfig.TerrainBoundingBox.width = GlobalConfig.TerrainBoundingBox.width * 2;
            screenMaterial.SetTexture("_MainTex2", slicerMap);
            MinMax.SetDataArray(slicerMap);
        }
        else
        {
            for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth * 2; i++)
            {
                for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight * 2; j++)
                {
                    float xpos = Rects[x].xMin + i * xRes;
                    float ypos = Rects[y].yMin + j * yRes;
                    float val = 0.0f;

                    for (int k = 0; k < Terrain.activeTerrains.Length; k++)
                    {
                        if (Rects[k].Contains(new Vector2(xpos, ypos)))
                        {
                            int row = (int)((float)(xpos - Rects[k].xMin) / (float)(Rects[k].xMax - Rects[k].xMin) * (float)Rects[k].width);
                            int col = (int)((float)(ypos - Rects[k].yMin) / (float)(Rects[k].yMax - Rects[k].yMin) * (float)Rects[k].height);
                            //row = Mathf.Abs(row);
                            //col = Mathf.Abs(col);
                            val = HeightMaps[k][row, col];

                            break;
                        }

                    }
                    if ((i > Terrain.activeTerrain.terrainData.heightmapWidth && j < Terrain.activeTerrain.terrainData.heightmapHeight) || (i < Terrain.activeTerrain.terrainData.heightmapWidth && j > Terrain.activeTerrain.terrainData.heightmapHeight))
                    {
                        int ival = i, jval = j;
                        if (i > Terrain.activeTerrain.terrainData.heightmapWidth)
                        {
                            ival -= Terrain.activeTerrain.terrainData.heightmapWidth;
                        }
                        else if (i < Terrain.activeTerrain.terrainData.heightmapWidth)
                        {
                            ival += Terrain.activeTerrain.terrainData.heightmapWidth;
                        }
                        if (j > Terrain.activeTerrain.terrainData.heightmapWidth)
                        {
                            jval -= Terrain.activeTerrain.terrainData.heightmapWidth;
                        }
                        else if (j < Terrain.activeTerrain.terrainData.heightmapWidth)
                        {
                            jval += Terrain.activeTerrain.terrainData.heightmapWidth;
                        }
                        /*if (i == Terrain.activeTerrain.terrainData.heightmapWidth)
                        {
                            ival = 0;
                        }
                        if (j == Terrain.activeTerrain.terrainData.heightmapWidth)
                        {
                            jval = 0;
                        }*/
                        heights[ival, jval] = val;
                    }
                    else
                    {
                        heights[i, j] = val;//(float)(i * j) /(float) (Terrain.activeTerrain.terrainData.heightmapWidth * 2 * Terrain.activeTerrain.terrainData.heightmapHeight * 2);
                    }
                    GlobalConfig.TerrainBoundingBox = Rects[y];
                    GlobalConfig.TerrainBoundingBox.x = Rects[x].x;
                    GlobalConfig.TerrainBoundingBox.height = GlobalConfig.TerrainBoundingBox.height * 2;
                    GlobalConfig.TerrainBoundingBox.width = GlobalConfig.TerrainBoundingBox.width * 2;
                }

            }
            Debug.LogError("WRITING slicer map");
            slicerMap = TerrainUtils.GetHeightMapAsTexture(heights);
            screenMaterial.SetTexture("_MainTex2", slicerMap);
            terrainBytes = slicerMap.EncodeToPNG();
            File.WriteAllBytes(ImageLoc, terrainBytes);
        }
    }

    public void generate3DTexture()
    {
        Texture3D randomTexture = new Texture3D(128, 128, 128, TextureFormat.ARGB32, false);

        Color[] colors = new Color[128 * 128 * 128];
        for (int i = 0; i < 128 * 128 * 128; i++)
        {
        colors[i] = Color.green;
        }
        randomTexture.SetPixels(colors);
        randomTexture.Apply();
        load3DTexture(randomTexture);
        setMax(1.0f);
        setMin(1.0f);

    }


    // Set 3D Texture
    public void load3DTexture(Texture3D environmentData)
    {
        environmentTex = environmentData;
        screenMaterial.SetTexture("_3DTex",environmentData);
    }

    public void WriteSlicerToFile()
    {
        MinMax.WriteSlicerToFile();
    }

    // Set cutting points into the world
    // The x and y coords must be in the range between 0 and 1.
    public void setFirstPoint(Vector2 one)
    {
        firstPoint = one;
		if(MinMax != null)
		MinMax.SetFirstPoint (one);
    }

    public void setSecondPoint(Vector2 two)
    {
        secondPoint = two;
		if(MinMax != null)
		MinMax.SetSecondPoint (two);
    }

    public void setMin(float Min)
    {
        min = Min;
    }

    public void setMax(float Max)
    {
        max = Max;
    }

    public void setUnits(string unit)
    {
        units = unit;
    }

    bool circle(int i, int j, int center, int center2, float radius)
    {
        return radius * radius >= (center - i) * (center - i) + (center2 - j) * (center2 - j);
    }


    static Rect windowRect = new Rect(0, 2 * Screen.height / 3 - 20, Screen.height / 3 + 100, Screen.height / 3 + 20);

    //void OnGUI()
    //{
    //}
    // This function needs to be renamed.
    void DoMyWindow(int windowID)
    {

        GUI.DragWindow();
        if (Event.current.type.Equals(EventType.Repaint))
        {
            Graphics.DrawTexture(new Rect(marginHorizontal, marginVertical, Screen.height / 3, Screen.height / 3), guit, screenMaterial);
        }
        GUI.TextArea(new Rect(Screen.height / 3 + 20, 10, 60, 20), max.ToString() + " " + units);
        GUI.TextArea(new Rect(Screen.height / 3 + 20, Screen.height / 3 - 10, 60, 20), min.ToString() + " " + units);

    }
    void OnRender()
    {
    }
    void setPropertices(Material m)
    {
        // get 2 point vects
        Vector2 fv = firstPoint;
        Vector2 sv = secondPoint;

        // flip dimensions of point vects, pass to shader
        m.SetVector("_Point1", new Vector2(fv.x, fv.y));
        m.SetVector("_Point2", new Vector2(sv.x, sv.y));
        //m.SetTexture("_MainTex2", guit);
        //m.SetTexture("_3DTex", environmentTex);
    }

    // Update is called once per frame
    void Update()
    {
        if (cursor.GetComponent<mouseray>().marker1.activeSelf == true && cursor.GetComponent<mouseray>().marker2.activeSelf == true)
        {
            //Debug.LogError(slicerMap != null);
            screenMaterial.SetTexture("_MainTex2", slicerMap);
            screenMaterial.SetTexture("_3DTex", environmentTex);

            // ensure display min/max do not exceed global min/max
            if (displayMax > max)
                displayMax = max;
            if (displayMin < min)
                displayMin = min;
            max = 1;
            min = 0;

            // get 2 point vects
            Vector2 fv = firstPoint;
            Vector2 sv = secondPoint;
            MinMax.SetFirstPoint(fv);
            MinMax.SetSecondPoint(sv);

            // Do a minmax update only when marker is down once, or marker is in movement
            if (cursor.GetComponent<mouseray>().mark1highlighted || cursor.GetComponent<mouseray>().mark2highlighted || calc_min_max)
            {
                // Used to either run cpu minmax or gpu
			#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
				MinMax.FindMinMaxCPU();
			#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				MinMax.FindMinMax();
            #endif
            }

            //Debug.LogError ("MAX: " + MinMax.max);
            //Debug.LogError ("MIN: " + MinMax.min);

            // flip dimensions of point vects, pass to shader
            float Range = MinMax.max - MinMax.min;
            screenMaterial.SetFloat("_Min", MinMax.min - Mathf.Min(Range / 2.0f, .2f));
            screenMaterial.SetFloat("_Max", MinMax.max + Mathf.Min(Range / 2.0f, .2f));

            screenMaterial.SetVector("_Point1", new Vector2(fv.x, fv.y));
            screenMaterial.SetVector("_Point2", new Vector2(sv.x, sv.y));
            Rect f = windowRect;
            f.y = 0;
            spriteRend.sprite = sliceSprite;
        }
        else
        {
            screenMaterial.SetTexture("_MainTex2", clear2d);
            screenMaterial.SetTexture("_3DTex", clear3d);
            calc_min_max = true;
        }
    }
}