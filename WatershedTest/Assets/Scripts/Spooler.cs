using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using VTL.SimTimeControls;
using System.Threading;
using VTL;
using VTL.TrendGraph;


public struct Frame
{
	public Sprite Picture;
	public DateTime starttime;
	public DateTime endtime;
	public float[,] Data;
};


public class FrameEndDateAscending : IComparer<Frame>
{
	public int Compare(Frame first, Frame second)
	{
		if (first.starttime > second.starttime) { return 1; }
		else if (first.starttime == second.starttime) { return 0; }
		else { return -1; }
	}
}


public class Spooler : MonoBehaviour
{
    // Public Variables
    static readonly object LOCK;
	public TrendGraphController trendGraph;
	public ModelRunVisualizer visual;
	public Image testImage;
	public Slider gridSlider;
	public TimeSlider timeSlider;
	public Queue<DataRecord> SliderFrames = new Queue<DataRecord>();
	public Projector TimeProjector;
    public Material colorWindow, colorProjector, slideProjector;
    private ColorPicker colorPicker;
    private ModelRun modelrun;
    public GameObject cursor;
    public Text downloadTextBox;
    public Text selectedVariableTextBox;
    public string selectedVariableString;
    
    // Local Variables
    Vector2 NormalizedPoint = Vector2.zero;
    public static int TOTAL = 0;
    Rect BoundingBox;
    bool WMS = false;
    List<Frame> Reel = new List<Frame>();
    Simulator simulator = new Simulator();
    string selectedModelRun = "";
    float frameRatio = 0.0f;
    int count = 10;
    float point = 0.0f;
    transform tran;
    int previ = 0, prevj = 0;
    string oldSelectedVariable;
    private int textureIndex = 0;
    int prevTextureIndex = 0;

	/// <summary>
	/// Used to determine the order the frames are in.
	/// </summary>
	/// <param name="x">The first frame.</param>
	/// <param name="y">The Second frame.</param>
	/// <returns></returns>
	private static int CompareFrames(Frame x, Frame y)
	{
		if (x.starttime == y.starttime)
		{
			return 0;
		}
		else if (x.starttime < y.starttime)
		{
			return -1;
		}
		return 1;
	}
	
	/// <summary>
	/// Starts the Spooler. Sets the colorPicker to the proper game object.
	/// </summary>
	void Start()
	{
        colorPicker = GameObject.Find("ColorSelector").GetComponent<ColorPicker>();
	}
	

    /// <summary>
    /// The update of the spooler. Sets the color of the data for the terrain, 
    /// adds data to spooler, changes the frames, and gets a user click for trendgraphs.
    /// </summary>
    void Update()
    {

        // If needed this will set the colors of the data on the terrain in the shader
        if (!WMS && colorPicker.ColorBoxes.Count > 0)
        {
			// Add the colors to the timeprojector and image
			for(int i = 0; i < 6; i++)
			{
				TimeProjector.material.SetColor("_SegmentData00" + i.ToString(), colorPicker.ColorBoxes[i].GetComponent<Image>().color);
				testImage.material.SetColor("_SegmentData00" + i.ToString(), colorPicker.ColorBoxes[i].GetComponent<Image>().color);
			}

			// Add the ranges to the timeprojector and image
			for(int i = 0; i < 6; i++)
			{
				TimeProjector.material.SetFloat("_x" + i.ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
				testImage.material.SetFloat("_x" + i.ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
			}

            TimeProjector.material.SetInt("_NumLines", (int)gridSlider.value);
        }

        // Enable the point to be used.
        TimeProjector.material.SetInt("_UsePoint", 1);
        TimeProjector.material.SetVector("_Point", NormalizedPoint);
        
        // Called if there is data to be handled
        if (SliderFrames.Count > 0 && count > 0)
        {
			// Set a dequeue size for multiple dequeus in one update
            int dequeueSize = 5;
            if (SliderFrames.Count < dequeueSize)
            {
                dequeueSize = SliderFrames.Count;
            }
            
            // Run the dequeue dequeueSize times
            for (int i = 0; i < dequeueSize; i++)
            {
                // Get the new data record to add
                DataRecord record = SliderFrames.Dequeue();
                selectedVariableTextBox.text = selectedVariableString;
                trendGraph.SetUnit(oldSelectedVariable);
                
                // This is called as an initial setup
                if (Reel.Count == 0)
                {
                    // Set projector
                    Vector2 BoundingScale;
                    Utilities.PlaceProjector2(TimeProjector, record,out BoundingScale);

                    if (record.bbox2 != "" && record.bbox2 != null)
                    {
                        //Debug.LogError("We added BBox TWO.");
                    	BoundingBox = Utilities.bboxSplit(record.bbox2);
                        
                    }
                    else
                    {
                        //Debug.LogError("We added BBox ONE.");
						BoundingBox = Utilities.bboxSplit(record.bbox);
                    }

                    if(!WMS)
                    {
                        TimeProjector.material = colorProjector;
                        TimeProjector.material.SetFloat("_MaxX", BoundingScale.x);
                        TimeProjector.material.SetFloat("_MaxY", BoundingScale.y);
                        testImage.material = colorWindow;
                        testImage.material.SetFloat("_MaxX", BoundingScale.x);
                        testImage.material.SetFloat("_MaxY", BoundingScale.y);
                    }
                    else
                    {
                        TimeProjector.material = slideProjector;
                        TimeProjector.material.SetFloat("_MaxX", BoundingScale.x);
                        TimeProjector.material.SetFloat("_MaxY", BoundingScale.y);
                    }

                    tran = new transform();
                    //Debug.LogError("Coord System: " + record.projection);
                    tran.createCoordSystem(record.projection); // Create a coordinate transform
                    //Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));

                    // transfor a lat/long bounding box to UTM
                    tran.setOrigin(coordsystem.WorldOrigin);
                    Vector2 point = tran.transformPoint(new Vector2(BoundingBox.x, BoundingBox.y));
                    Vector2 point2 = tran.transformPoint(new Vector2(BoundingBox.x + BoundingBox.width, BoundingBox.y - BoundingBox.height));

                    // Here is a patch.
                    if ((BoundingBox.x > -180 && BoundingBox.x < 180 && BoundingBox.y < 180 && BoundingBox.y > -180))
                    {
                        BoundingBox = new Rect(point.x, point.y, Math.Abs(point.x - point2.x), Math.Abs(point.y - point2.y));
                    }
                     
                    Debug.LogError(BoundingBox);
                    Debug.LogError(BoundingBox.width);

                    // Set the bounding box to the trendgraph
                    trendGraph.SetBoundingBox(BoundingBox);
                }

                // Updates the count, and adds the record to the reel
                count--;
                textureBuilder(record);

                // Updates the max information across the necessary classes
                if(record.Max > modelrun.MinMax[oldSelectedVariable].y)
                {
                    float max = record.Max;
                    modelrun.MinMax[oldSelectedVariable] = new SerialVector2(new Vector2(modelrun.MinMax[oldSelectedVariable].x, max)); 
                    TimeProjector.material.SetFloat("_FloatMax", max);
                    testImage.material.SetFloat("_FloatMax", max);
                    colorPicker.SetMax(max);
                    trendGraph.SetMax((int)max);
                }
                if(record.Min < modelrun.MinMax[oldSelectedVariable].x)
                {
                    float min = record.Min;
                    modelrun.MinMax[oldSelectedVariable] = new SerialVector2(new Vector2(min, modelrun.MinMax[oldSelectedVariable].y));
                    TimeProjector.material.SetFloat("_FloatMin", min);
                    testImage.material.SetFloat("_FloatMin", min);
                    colorPicker.SetMin(min);
                    trendGraph.SetMin((int)min);
                }
            }

            // Sets the currently downloaded value
            if (downloadTextBox)
            {
                downloadTextBox.text = "Downloaded: " + ((float)Reel.Count / (float)TOTAL).ToString("P");
            }

            // Updates the time on the time slider view
			timeSlider.SetTimeDuration(Reel[0].starttime, Reel[Reel.Count - 1].endtime, Math.Min((float)(Reel[Reel.Count - 1].endtime-Reel[0].starttime).TotalHours,30*24));

        }
        
        // This will get a user click
		if (Input.GetMouseButtonDown (0) && cursor.GetComponent<mouselistener>().state == cursor.GetComponent<mouselistener>().states[1]) 
		{
			// Check if mouse is inside bounding box 
			Vector3 WorldPoint = coordsystem.transformToWorld(mouseray.CursorWorldPos);
			Vector2 CheckPoint = new Vector2(WorldPoint.x,WorldPoint.z);

			if( BoundingBox.Contains(CheckPoint) && !WMS)
			{
				// Debug.LogError("CONTAINS " + CheckPoint + " Width: " + BoundingBox.width + " Height: " +  BoundingBox.height);
                NormalizedPoint = TerrainUtils.NormalizePointToTerrain(WorldPoint, BoundingBox);
                trendGraph.SetCoordPoint(WorldPoint);
                int x = (int)Math.Min(Math.Round(Reel[textureIndex].Data.GetLength(0) * NormalizedPoint.x), (double)Reel[textureIndex].Data.GetLength(0) - 1);
                int y = (int)Math.Min(Math.Round(Reel[textureIndex].Data.GetLength(1) * NormalizedPoint.y), (double)Reel[textureIndex].Data.GetLength(1) - 1);
                
                trendGraph.SetPosition(Reel[textureIndex].Data.GetLength(1) - 1 - y, Reel[textureIndex].Data.GetLength(0) - 1 - x);
			}
		}

        // This if statement is used for debugging code
        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.LogError("The count / total: " + Reel.Count + " / " + TOTAL);
            trendGraph.SetPosition(50, 50);
            // trendGraph.PresetData();
        }
    }

    /// <summary>
    /// Takes the currently worked on record and adds it to the spooler.
    /// </summary>
    /// <param name="rec">The current record to add.</param>
    public void textureBuilder(DataRecord rec)
    {
        //return;
		// Caching 
       /* 
		if (!FileBasedCache.Exists (rec.id))  
		{
            //Debug.LogError("INSERTING INTO CACHE " + rec.id);
			FileBasedCache.Insert<DataRecord>(rec.id,rec);
		}
         */
		
		// This is used to check that the record is correct
        if(rec == null)
        {
            Debug.LogError("The RUN was invalid");
            return;
        }
		if (rec.modelRunUUID != selectedModelRun) 
		{
			// This is not the model run we want because something else was selected.
			return;
		}
        var TS = rec.end.Value - rec.start.Value;
        double totalhours = TS.TotalHours / rec.Data.Count;
        float max, min;
        if(rec.Data.Count > 1)
        {
            TOTAL = rec.Data.Count; // Patch
        }
        
        if (!rec.start.HasValue)
        {
            Debug.LogError("no start");
            rec.start = DateTime.MinValue;
        }
        if (!rec.end.HasValue)
        {
            Debug.LogError("no end");

            rec.end = DateTime.MaxValue;
        }

        for (int j = 0; j < rec.Data.Count; j++ )
        {
            // Build the Frame to pass in
            Frame frame = new Frame();


            if (rec.Data.Count == 1)
            {
                frame.starttime = rec.start.Value; 
                frame.endtime = rec.end.Value; 
            }
            else
            {
                frame.starttime = rec.start.Value + new TimeSpan((int)Math.Round((double)j*totalhours),0,0);
                frame.endtime = rec.end.Value + new TimeSpan((int)Math.Round((double)(j+1)*totalhours),0,0);
            }

            frame.Data = rec.Data[j];

            // Checks for NULL downloaded data
            if (rec.Data == null)
            {
                Debug.LogError("The data at UUID = " + rec.id + " was null.");
                return;
            }


            trendGraph.Add(rec.start.Value, 1.0f, rec.Data[0]);
            Logger.enable = true;
            Texture2D tex = new Texture2D(rec.width, rec.height);
            
            if (!WMS)
            {
                tex = Utilities.BuildDataTexture(rec.Data[j], out min, out max);
                rec.Min = Math.Min(min, rec.Min);
                rec.Max = Math.Max(max, rec.Max);
                //Debug.LogError("MIN AND MAX: " + rec.Min + " " + rec.Max);
            }
            else
            {
                // We need to change this ..... as this code is unreachable at the moment.
                tex.LoadImage(rec.texture);
            }

            // This will add a clear color on all the 
            for (int i = 0; i < tex.width; i++)
            {
                tex.SetPixel(i, 0, Color.clear);
                tex.SetPixel(i, tex.height - 1, Color.clear);
            }
            for (int i = 0; i < tex.height; i++)
            {
                tex.SetPixel(tex.width - 1, i, Color.clear);
                tex.SetPixel(0, i, Color.clear);
            }

            tex.Apply();
            frame.Picture = Sprite.Create(tex, new Rect(0, 0, 100, 100), Vector2.zero);
            Insert(frame);
            count++;
        }
	}
	
    /// <summary>
    /// Used for testing. Builds a spool of 10 images.
    /// </summary>
	void RandomMovie()
	{
		for (int i = 0; i < 10; i++)
		{
			AddRandomImage();
		}
	}
	
    /// <summary>
    /// Insert random image to the frame.
    /// </summary>
	public void AddRandomImage()
	{
		Texture2D image = new Texture2D(1000, 1000);
		Color[] colors = new Color[1000 * 1000];
		for (int i = 0; i < 1000; i++)
		{
			for (int j = 0; j < 1000; j++)
			{
				//Vector3 abc = UnityEngine.Random.onUnitSphere;
				float intensity = UnityEngine.Random.value;
				Color Set = Color.Lerp(Color.white,Color.black,UnityEngine.Random.value);
				Set.r = Set.r *intensity;
				Set.b = Set.b *intensity;
				Set.g = Set.g *intensity;
				colors[i * 1000 + j] = Set;
			}
		}
		image.SetPixels(colors);
		image.Apply();
		DateTime RandomTime = new DateTime(UnityEngine.Random.Range(1995, 2015), UnityEngine.Random.Range(1, 12), UnityEngine.Random.Range(1, 30));
		Debug.Log(RandomTime);
		Frame frame = new Frame();
		frame.Picture = Sprite.Create(image, new Rect(0, 0, 100, 100), Vector2.zero);
		frame.starttime = RandomTime;
		frame.endtime = RandomTime.AddHours(1.0);
		Insert(frame);
	}
	
    /// <summary>
    /// Inserts the frame into the Reel.
    /// </summary>
    /// <param name="frame">The frame to add to the reel.</param>
	void Insert(Frame frame)
	{
		// Does this handle duplicates..
		int index = Reel.BinarySearch(frame,new FrameEndDateAscending());

        // This is to help pinpoint too many records added to the Reel
        if(Reel.Count >= TOTAL)
        {
            //Debug.LogError("Why is there more records being added to the Reel?");
            //Debug.LogError("Here is out frame starttime: " + frame.starttime + " and the count is: " + count);
        }

		//if index >= 0 there is a duplicate 
		if(index >= 0)
		{
			//handle the duplicate!
			//throw new Exception("Duplicate Handling not implemented!!!!!");
            TOTAL--;
		}
		else
		{
			// new item
			// Debug.LogError("INSERTTING FRAMME " + ~index);
			Reel.Insert(~index, frame);
		}
	}
	
	/// <summary>
	/// Takes the data and places it into the Reel if it is WMS.
	/// </summary>
	/// <param name="data">The datarecord being handled</param>
	/// <param name="FromData">Build the image from the data.</param>
	public void Insert(DataRecord data, bool FromData)
	{
		var frame = new Frame();
        Texture2D image = new Texture2D(data.width, data.height);
		if (!FromData)
		{
			image.LoadImage(data.texture);
		}
		else
		{
			// Build a color map from Raw Data...
			// Create a sprite
            frame.Picture = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 100, 100), new Vector2(0, 0));
		}

		// Create a sprite
        frame.Picture = Sprite.Create(image, new Rect(0, 0, 100, 100), new Vector2(0, 0));

		// Attached an associate Date Time Object
		frame.starttime = data.start.Value;
		frame.endtime = data.end.Value;
		Insert(frame);
	}

	/// <summary>
	/// The next frame in the reel and updates the window texture.
	/// </summary>
	/// <param name="Time">The time on the time slider.</param>
	/// <returns>Location of the Reel the time is.</returns>
	int FindNearestFrame(DateTime Time)
	{
		Frame temp = new Frame();
		temp.starttime = Time;
		int index = Reel.BinarySearch(temp, new FrameEndDateAscending());
		return index < 0 ? ~index-1 : index; 
	}
	
    /// <summary>
    /// This will handle wcs related requests
    /// </summary>
    /// <param name="Records">The list of records to add.</param>
	public void HandDataToSpooler(List<DataRecord> Records)
	{
        //Debug.LogError("DOWNLOADING THIS NUMBER OF BANDS: " + Records[0].numbands + " " + Records[0].Data.Count);
		SliderFrames.Enqueue(Records[0]);
	}

    /// <summary>
    /// Used by the ModelRunComparison to buyild the Delta and add to spooler.
    /// </summary>
    public void SetupForDelta(string seled, string variable, int total, string uuid)
    {
        // Handles the clearing of previous data.
        if (selectedModelRun != "")
        {
            Reel.Clear();
            SliderFrames.Clear();
            trendGraph.Clear();
            modelrun.ClearData(oldSelectedVariable);
        }

        // Set the new model run
        selectedVariableString = "Current Model Run: " + seled + " Variable: " + variable;

        // Get the Model Run
        TOTAL = total;
        selectedModelRun = uuid;
        modelrun = ModelRunManager.GetByUUID(selectedModelRun);
        oldSelectedVariable = variable;

        // Set the download based on the doqq in description
        WMS = false;
    }
	
	/// <summary>
	/// Gets the selected model run, begins the record download, and updates page data.
	/// </summary>
    public void LoadSelected()
    {		
		// Load this 
		var temp = visual.listView.GetSelectedModelRuns();
		var seled = visual.listView.GetSelectedRowContent();
		string variable = seled[0][2].ToString();

        // Set the data of new model run
        selectedVariableString = "Current Model Run: " + seled[0][0].ToString() + " Variable: " + variable;

        // Only run if what was selected returned a value
		if(temp != null)
		{
            // Handles the clearing of previous data.
            if (selectedModelRun != "")
            {
                Reel.Clear();
                SliderFrames.Clear();
                trendGraph.Clear();
                modelrun.ClearData(oldSelectedVariable);
            }

			// Time to load some things
			SystemParameters sp = new SystemParameters();
			sp.interpolation = "bilinear";
            sp.width = 100;
            sp.height = 100;

            // Get the Model Run
			var Records = temp[0].FetchVariableData(variable);
			TOTAL = Records.Count;
			selectedModelRun = temp[0].ModelRunUUID;
            modelrun = ModelRunManager.GetByUUID(selectedModelRun);
            oldSelectedVariable = variable;
			Logger.WriteLine("Load Selected: " + variable + " with Number of Records: " + Records.Count);

            // Set the download based on the doqq in description
			if(temp[0].Description.ToLower().Contains("doqq"))
		    {
		    	WMS = true;
				ModelRunManager.Download(Records, HandDataToSpooler, param: sp, operation: "wms");
			}
			else
		    {
		    	WMS = false;
				ModelRunManager.Download(Records, HandDataToSpooler, param: sp);
			}
		}
	}
	
	/// <summary>
	/// Updates the texture on the movie reel.
	/// </summary>
	public void ChangeTexture()
	{
		if (Reel.Count > 0)
		{
			textureIndex = FindNearestFrame(timeSlider.SimTime);

            // Update the index on the trendgraph
            trendGraph.SetDataIndex(textureIndex);
			
			if (textureIndex < 0)
			{
				textureIndex = 0;
			}
			else if (textureIndex >= Reel.Count)
			{
				textureIndex = Reel.Count - 1;
			}
			
			testImage.sprite = Reel[textureIndex].Picture;
			int i = -1;
			int j = -1;
			if(!WMS)
			{
				i = (int)Math.Min(Math.Round(Reel[textureIndex].Data.GetLength(0)*NormalizedPoint.x),(double)Reel[textureIndex].Data.GetLength(0)-1);
				j = (int)Math.Min(Math.Round(Reel[textureIndex].Data.GetLength(1)*NormalizedPoint.y),(double)Reel[textureIndex].Data.GetLength(1)-1);
			}
			if(previ != i || prevj != j)
			{
				// set new previ and prevj
				previ = i;
				prevj = j;
			}
						
			// Set projector image
			if(textureIndex == Reel.Count - 1 || Reel.Count < 2)
			{
				//Debug.LogError("End of the Reel");
				// Set both textures to last reel texture
				TimeProjector.material.SetTexture("_ShadowTex",Reel[Reel.Count-1].Picture.texture);
				TimeProjector.material.SetTexture("_ShadowTex2",Reel[Reel.Count-1].Picture.texture);

                testImage.material.SetTexture("_MainTex", Reel[Reel.Count - 1].Picture.texture);
                testImage.material.SetTexture("_MainTex2", Reel[Reel.Count - 1].Picture.texture);
			}
			else
			{
				//Debug.LogError("Reeling");
				// Set current texture
				TimeProjector.material.SetTexture("_ShadowTex",Reel[textureIndex].Picture.texture);
				
				// Set future texture
				TimeProjector.material.SetTexture("_ShadowTex2",Reel[textureIndex+1].Picture.texture);
                testImage.material.SetTexture("_MainTex", Reel[textureIndex].Picture.texture);
                testImage.material.SetTexture("_MainTex2", Reel[textureIndex + 1].Picture.texture);
			}
		}
	}
}
