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
    public Text downloadTextBox;
    public Text selectedVariableTextBox;
    string selectedVariableString;
    
    // Local Variables
    Vector2 NormalizedPoint = Vector2.zero;
    public static int TOTAL = 0;
    //Rect BoundingBox;
    bool WMS = false;
    List<Frame> Reel = new List<Frame>();
    Simulator simulator = new Simulator();
    string selectedModelRun = "";
    float frameRatio = 0.0f;
    int count = 10;
    float point = 0.0f;
    //transform tran;
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
    }

    /// <summary>
    /// This is used to replace the update of new data. This will set the projector, and shader data.
    /// </summary>
    /// <param name="BoundingBox">New bounding box.</param>
    /// <param name="projection">Projection code.</param>
    /// <param name="wms">Yes/No if is a WMS type.</param>
    public void UpdateData(Rect BoundingBox, string projection, bool wms, string variable, string uuid)
    {
        // Set projector
        Vector2 BoundingScale;
        Utilities.PlaceProjector2(TimeProjector, BoundingBox, projection, out BoundingScale);
        WMS = wms;

        if (!WMS)
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

        // Set variable
        selectedVariableString = "Variable: " + variable;
        oldSelectedVariable = variable;
        selectedModelRun = uuid;
        modelrun = ModelRunManager.GetByUUID(selectedModelRun);

        // Set the bounding box to the trendgraph
        // Here is a patch.
        transform tran = new transform();
        //Debug.LogError("Coord System: " + record.projection);
        tran.createCoordSystem(projection); // Create a coordinate transform
                                                   //Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));

        // transfor a lat/long bounding box to UTM
        tran.setOrigin(coordsystem.WorldOrigin);
        Vector2 point = tran.transformPoint(new Vector2(BoundingBox.x, BoundingBox.y));
        Vector2 point2 = tran.transformPoint(new Vector2(BoundingBox.x + BoundingBox.width, BoundingBox.y - BoundingBox.height));

        if ((BoundingBox.x > -180 && BoundingBox.x < 180 && BoundingBox.y < 180 && BoundingBox.y > -180))
        {
            BoundingBox = new Rect(point.x, point.y, Math.Abs(point.x - point2.x), Math.Abs(point.y - point2.y));
        }

        trendGraph.Clear();
        trendGraph.SetBoundingBox(BoundingBox);
    }

    public void UpdateMinMax(float min, float max)
    {
        // Updates the max information across the necessary classes
        TimeProjector.material.SetFloat("_FloatMax", max);
        testImage.material.SetFloat("_FloatMax", max);
        colorPicker.SetMax(max);
        colorPicker.Mean = modelrun.GetVariable(oldSelectedVariable).Mean;
        colorPicker.frameCount = modelrun.GetVariable(oldSelectedVariable).frameCount;
        trendGraph.SetMax((int)max);

        TimeProjector.material.SetFloat("_FloatMin", min);
        testImage.material.SetFloat("_FloatMin", min);
        colorPicker.SetMin(min);
        colorPicker.Mean = modelrun.GetVariable(oldSelectedVariable).Mean;
        trendGraph.SetMin((int)min);
    }

    public void UpdateTimeDuration(DateTime start, DateTime end)
    {
        timeSlider.SetTimeDuration(start, end, Math.Min((float)(end - start).TotalHours, 30 * 24));
    }

    
    /// <summary>
    /// Updates the texture on the movie reel.
    /// </summary>
    public void ChangeTexture()
    {
        int currentcount = ActiveData.GetCount();
        textureIndex = ActiveData.FindNearestFrame(timeSlider.SimTime);
        if (textureIndex < 0)
        {
            textureIndex = 0;
        }
        else if (textureIndex >= currentcount)
        {
            textureIndex = currentcount - 1;
        }

        // Update the index on the trendgraph
        trendGraph.SetDataIndex(textureIndex);

        testImage.sprite = ActiveData.GetFrameAt(textureIndex).Picture;

        // Set projector image
        if (textureIndex == currentcount - 1 || currentcount < 2)
        {
            // Set both textures to last reel texture
            Frame setframe = ActiveData.GetFrameAt(currentcount);
            TimeProjector.material.SetTexture("_ShadowTex", setframe.Picture.texture);
            TimeProjector.material.SetTexture("_ShadowTex2", setframe.Picture.texture);

            testImage.material.SetTexture("_MainTex", setframe.Picture.texture);
            testImage.material.SetTexture("_MainTex2", setframe.Picture.texture);
        }
        else
        {
            // Set current texture
            testImage.material.SetTexture("_MainTex", ActiveData.GetFrameAt(textureIndex).Picture.texture);
            TimeProjector.material.SetTexture("_ShadowTex", ActiveData.GetFrameAt(textureIndex).Picture.texture);

            // Set future texture
            TimeProjector.material.SetTexture("_ShadowTex2", ActiveData.GetFrameAt(textureIndex + 1).Picture.texture);
            testImage.material.SetTexture("_MainTex2", ActiveData.GetFrameAt(textureIndex + 1).Picture.texture);
        }

    }
}
