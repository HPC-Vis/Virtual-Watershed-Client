using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using VTL.SimTimeControls;
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
	public TrendGraphController trendGraph;
    string selectedModelRun="";
    public ModelRunVisualizer visual;
    Simulator simulator = new Simulator();
    // This will hold all of the Reel...
    List<Frame> Reel = new List<Frame>();
    public Image testImage;
    public Slider slider;
    public TimeSlider timeSlider;
    public Queue<DataRecord> SliderFrames = new Queue<DataRecord>();
	public Projector TimeProjector;

	// BoundingBox used for the time series graph...
	public Rect BoundingBox;

	public void GenerateTimeSeries()
	{

	}

	int TOTAL = 0;
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

	Vector2 NormalizedPoint=Vector2.zero;

    // Use this for initialization
    void Start()
    {
        //RandomMovie();
        
        //testImage.sprite = Reel[0].Picture;
        point = 0;
        
    }
    int count = 10;


    bool first = true;
    float point;
	public Text downloadTextBox;
    // Update is called once per frame
    void Update()
    {
        if (SliderFrames.Count > 0 && count > 0)
        {
			// Clear time series graph


			DataRecord record = SliderFrames.Dequeue();
			if(Reel.Count == 0)
			{
				// Set projector...
				Utilities utilites = new Utilities();

				utilites.PlaceProjector(TimeProjector,record);
				BoundingBox = Utilities.bboxSplit(record.bbox);
				Debug.LogError(BoundingBox);
				var tran = new transform();
				tran.createCoordSystem(record.projection); // Create a coordinate transform
				//Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));
				
				tran.setOrigin(coordsystem.WorldOrigin);
				Vector2 point = tran.transformPoint(new Vector2(BoundingBox.x, BoundingBox.y));
				Vector2 point2 = tran.transformPoint(new Vector2(BoundingBox.x+BoundingBox.width, BoundingBox.y-BoundingBox.height));
				BoundingBox = new Rect(point.x,point2.y,Math.Abs(point.x-point2.x),Math.Abs(point.y-point2.y));
				Debug.LogError(BoundingBox);
			}

            count--;
            textureBuilder(record);
			if(downloadTextBox)
				downloadTextBox.text = "Downloaded: " + ((float)Reel.Count/(float)TOTAL).ToString("P");
            timeSlider.SetTimeDuration(Reel[0].starttime, Reel[Reel.Count - 1].endtime, 1);

        }
		if (Input.GetMouseButtonDown (0)) 
		{
			// Check if mouse is inside bounding box 
			Debug.LogError(coordsystem.transformToWorld(mouseray.CursorWorldPos));
			Vector3 WorldPoint = coordsystem.transformToWorld(mouseray.CursorWorldPos);
			Vector2 CheckPoint = new Vector2(WorldPoint.x,WorldPoint.z);
			if(BoundingBox.Contains(CheckPoint))
			{
				Debug.LogError("CONTAINS");
				trendGraph.Clear();
				NormalizedPoint = TerrainUtils.NormalizePointToTerrain(WorldPoint,BoundingBox);

			}
		}
    }

    public void textureBuilder(DataRecord rec)
    {
		// Caching 
		if (!FileBasedCache.Exists (rec.id))  
		{
			FileBasedCache.Insert<DataRecord>(rec.id,rec);
		}

		// Else update the cache
		if (rec.modelRunUUID != selectedModelRun) 
		{
			// This is not the model run we want because sometime else was selected.
			return;
		}

        // Frame to pass in
        Frame frame = new Frame();
        Utilities utilities = new Utilities();


        frame.starttime = rec.start;
        frame.endtime = rec.end;
		frame.Data = rec.Data;
        //Debug.LogError(rec.start + " | " + rec.end);
		//Logger.enable = true;
        //frame.Picture = Sprite.Create(new Texture2D(100, 100), new Rect(0, 0, 100, 100), Vector2.zero);
		var tex = utilities.buildTextures (rec.Data, Color.clear, Color.red);
		for(int i = 0; i < tex.width; i++)
		{

			tex.SetPixel(i,0,Color.clear);
			tex.SetPixel(i,tex.height-1,Color.clear);
		}
		for(int i = 0; i < tex.height; i++)
		{
			tex.SetPixel(tex.width-1,i,Color.clear);
			tex.SetPixel(0,i,Color.clear);
		}

		tex.Apply ();
        frame.Picture = Sprite.Create(tex, new Rect(0, 0, 115, 115), Vector2.zero);//new Texture2D();// Generate Sprite

        // second hand to spooler
        Insert(frame);
        count++;


    }

    void RandomMovie()
    {
        for (int i = 0; i < 10; i++)
        {
            AddRandomImage();
        }
    }

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

        //Reel.Add(frame);
        // SortList();
        Insert(frame);
    }

    void SortList()
    {
        Reel.Sort(CompareFrames);
        timeSlider.SetStartTime(Reel[0].starttime);
        //timeSlider.si
    }


    void LoadModelRun()
    {
        
    }

    void Insert(Frame frame)
    {
        // Does this handle duplicates..
        int index = Reel.BinarySearch(frame,new FrameEndDateAscending());
        //if index >= 0 there is a duplicate 
        if(index >= 0)
        {
            //handle the duplicate!
            throw new Exception("Duplicate Handling not implemented!!!!!");
        }
        else
        {
            // new item
           // Debug.LogError("INSERTTING FRAMME " + ~index);
            Reel.Insert(~index, frame);
        }
   }

    
    // This can handle WMS Requests

    public void Insert(DataRecord data, bool FromData)
    {
        var frame = new Frame();
        Texture2D image = new Texture2D(100, 100);
        if (!FromData)
        {
            image.LoadImage(data.texture);


        }
        else
        {
            // Build a color map from Raw Data...
            // Create a sprite
            frame.Picture = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 100, 100), new Vector2(0, 0));
        }
        // Create a sprite
        frame.Picture = Sprite.Create(image, new Rect(0, 0, 100, 100), new Vector2(0, 0));
        // Attached an associate Date Time Object
        frame.starttime = data.start;
        frame.endtime = data.end;
        //Reel.Add(frame);
        //SortList();
        Insert(frame);
    }

    int FindNearestFrame(DateTime Time)
    {
        Frame temp = new Frame();
        temp.starttime = Time;
        int index = Reel.BinarySearch(temp, new FrameEndDateAscending());
        //Debug.LogError(index);
        return index < 0 ? ~index-1 : index; 
    }

    // This will handle wcs related requests..
    void HandDataToSpooler(List<DataRecord> Records)
    {

        // Handing to spooler 
        // first build a color map
        SliderFrames.Enqueue(Records[0]);
    }


	int previ=0,prevj=0;
    public void LoadSelected()
    {
		Debug.LogError ("LOADING SELECTED");

        // Load this 
        var temp = visual.listView.GetSelectedModelRuns();
        var seled = visual.listView.GetSelectedRowContent();
        string variable = seled[0][1].ToString();
		Debug.LogError (temp.Count);

		if (selectedModelRun != "") 
		{
			//ModelRunManager.RemoveFromDownloads(selectedModelRun);
			Reel.Clear();
			// Add some code here to handle the time slider.
			//timeslider.reset();
		}
        if(temp!= null)
        {
            
            // Time to load some things
            //ModelRunManager.
            //temp[0]
            SystemParameters sp = new SystemParameters();

            // This is not getting passed into WCS UGH! Right now width and height come out to equal 0!!!!!!
            sp.width = 115;
            sp.height = 115;
            sp.interpolation = "bilinear";
			Debug.LogError (temp[0].GetCount());
			var Records = temp[0].FetchVariableData(variable);
			TOTAL = Records.Count;
			selectedModelRun = temp[0].ModelRunUUID;
			Debug.LogError("NUMBER OF RECORDS: " + Records.Count);
            ModelRunManager.Download(Records, HandDataToSpooler, param: sp);
        }
        //visual.listView.GetSelectedModelRuns()[0];
    }


    private int textureIndex = 0;
	int prevTextureIndex = 0;

    public void ChangeTexture()
    {
        if (Reel.Count > 0)
        {
            textureIndex = FindNearestFrame(timeSlider.SimTime);//(int)slider.value;

            //Debug.Log("CHANGING");
            //Debug.Log(textureIndex);
            // Debug.Log(timeSlider.SimTime);
            //Debug.Log(textureIndex);
            if (textureIndex < 0)
            {
                textureIndex = 0;
            }
            else if (textureIndex >= Reel.Count)
            {
                textureIndex = Reel.Count - 1;
            }

            testImage.sprite = Reel[textureIndex].Picture;
			int i = (int)Math.Min(Math.Round(Reel[textureIndex].Data.GetLength(0)*NormalizedPoint.x),(double)Reel[textureIndex].Data.GetLength(0)-1);
			int j = (int)Math.Min(Math.Round(Reel[textureIndex].Data.GetLength(1)*NormalizedPoint.y),(double)Reel[textureIndex].Data.GetLength(1)-1);
			if(previ != i || prevj != j)
			{
				// Clear time series graph

				// set new previ and prevj
				previ = i;
				prevj = j;
			}

			if(textureIndex != prevTextureIndex)
			{
				// Send data to time series graph.
				trendGraph.Add(Reel[textureIndex].starttime,Reel[textureIndex].Data[i,j]);
			}

			// Set projector image
			if(textureIndex == Reel.Count - 1 || Reel.Count < 2)
			{
				//Debug.LogError("End of the Reel");
				// Set both textures to last reel texture
				TimeProjector.material.SetTexture("_ShadowTex",Reel[Reel.Count-1].Picture.texture);
				TimeProjector.material.SetTexture("_ShadowTex2",Reel[Reel.Count-1].Picture.texture);

			}
			else
			{
				//Debug.LogError("Reeling");
				// Set current texture
				TimeProjector.material.SetTexture("_ShadowTex",Reel[textureIndex].Picture.texture);

				// Set future texture
				TimeProjector.material.SetTexture("_ShadowTex2",Reel[textureIndex+1].Picture.texture);
			}
        }
    }
}
