using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using VTL.SimTimeControls;


public class Spooler : MonoBehaviour
{
    // Public Variables
    static readonly object LOCK;
	public Image testImage;
	public Slider gridSlider;
	public TimeSlider timeSlider;
	public Projector TimeProjector;
    public Material colorWindow, colorProjector, slideProjector;
    private ColorPicker colorPicker;
    private GameObject colorbarObj;
    private Image colorbar;
    public Text selectedVariableTextBox;
    public GameObject minPin;
    public GameObject maxPin;

    // Local Variables
    private DateTime lastUpdateTime;
    Vector2 NormalizedPoint = Vector2.zero;
    bool WMS = false;
    string oldSelectedVariable;
    private FilterMode filtermode = FilterMode.Bilinear;

	
	/// <summary>
	/// Starts the Spooler. Sets the colorPicker to the proper game object.
	/// </summary>
	void Start()
	{
        colorPicker = GameObject.Find("ColorSelector").GetComponent<ColorPicker>();
        colorbarObj = GameObject.Find("Colorbar");
        colorbar = colorbarObj.transform.GetChild(0).GetComponent<Image>();
        UpdateMinMax(0.0f, 5.0f);
    }
	

    /// <summary>
    /// The update of the spooler. Sets the color of the data for the terrain.
    /// </summary>
    void Update()
    {
        // If needed this will set the colors of the data on the terrain in the shader
        if (!WMS && colorPicker.ColorBoxes.Count > 0)
        {
			// Add the colors to the timeprojector and image
			for(int i = 0; i < colorPicker.ColorBoxes.Count; i++)
			{
				TimeProjector.material.SetColor("_SegmentData00" + i.ToString(), colorPicker.ColorBoxes[i].GetComponent<Image>().color);
				testImage.material.SetColor("_SegmentData00" + i.ToString(), colorPicker.ColorBoxes[i].GetComponent<Image>().color);
                colorbar.material.SetColor("_SegmentData00" + i.ToString(), colorPicker.ColorBoxes[i].GetComponent<Image>().color);
            }

			// Add the ranges to the timeprojector and image
			for(int i = 0; i < colorPicker.ColorBoxes.Count; i++)
			{
				TimeProjector.material.SetFloat("_x" + (i + 1).ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
				testImage.material.SetFloat("_x" + (i + 1).ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
                colorbar.material.SetFloat("_x" + (i + 1).ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
            }

            TimeProjector.material.SetInt("_NumLines", (int)gridSlider.value);
        }

        // Enable the point to be used.
        TimeProjector.material.SetInt("_UsePoint", 1);
        TimeProjector.material.SetVector("_Point", NormalizedPoint);

        // Check last update time
        if(lastUpdateTime != timeSlider.SimTime)
        {
            ChangeTexture();
        }

       
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
        TimeProjector.GetComponent<ProjectorObject>().PlaceProjector(BoundingBox, projection, out BoundingScale);
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

        // Update title
        selectedVariableTextBox.text = "Variable: " + variable + ": " + VariableReference.GetDescription(variable);
        ChangeTexture();
    }

    public void UpdateMinMax(float min, float max)
    {
        // Updates the max information across the necessary classes
        TimeProjector.material.SetFloat("_FloatMax", max);
        testImage.material.SetFloat("_FloatMax", max);
        colorbar.material.SetFloat("_FloatMax", max);
        colorPicker.SetMax(max);
        colorbarObj.transform.GetChild(1).GetComponent<Text>().text = max.ToString();
        //colorPicker.Mean = modelrun.GetVariable(oldSelectedVariable).Mean;
        //colorPicker.frameCount = modelrun.GetVariable(oldSelectedVariable).frameCount;
        
        TimeProjector.material.SetFloat("_FloatMin", min);
        testImage.material.SetFloat("_FloatMin", min);
        colorbar.material.SetFloat("_FloatMin", min);
        colorPicker.SetMin(min);
        colorbarObj.transform.GetChild(2).GetComponent<Text>().text = min.ToString();
        //colorPicker.Mean = modelrun.GetVariable(oldSelectedVariable).Mean;
    }

    public void UpdateTimeDuration(DateTime start, DateTime end)
    {
        timeSlider.SetTimeDuration(start, end, Math.Min((float)(end - start).TotalHours, 30 * 24));
    }

    public void Clear()
    {
        colorPicker.SetMin(0);
        colorPicker.SetMax(1);
    }
    
    /// <summary>
    /// Updates the texture on the movie reel.
    /// </summary>
    public void ChangeTexture()
    {
        // Set the new time for update
        lastUpdateTime = timeSlider.SimTime;

        List<String> tempFrameRef = ActiveData.GetCurrentAvtive();
        if(tempFrameRef.Count < 1)
        {
            return;
        }
        int currentcount = ActiveData.GetCount(tempFrameRef[0]);
        int textureIndex = ActiveData.FindNearestFrame(timeSlider.SimTime);
        testImage.sprite = ActiveData.GetFrameAt(tempFrameRef[0], textureIndex).Picture;

        // Do nothing if there is no data
        if(currentcount < 1)
        {
            return;
        }
        
        // Set projector image
        if (textureIndex == currentcount - 1 || currentcount == 1)
        {
            // Set both textures to last reel texture
            Frame setframe = ActiveData.GetFrameAt(tempFrameRef[0], currentcount - 1);
            PlacePins(setframe, ActiveData.GetBoundingBox(tempFrameRef[0]));
            setframe.Picture.texture.filterMode = filtermode;
            TimeProjector.material.SetTexture("_ShadowTex", setframe.Picture.texture);
            TimeProjector.material.SetTexture("_ShadowTex2", setframe.Picture.texture);

            testImage.material.SetTexture("_MainTex", setframe.Picture.texture);
            testImage.material.SetTexture("_MainTex2", setframe.Picture.texture);
        }
        else
        {
            // Set current texture
            Frame setFrame = ActiveData.GetFrameAt(tempFrameRef[0], textureIndex);
            PlacePins(setFrame, ActiveData.GetBoundingBox(tempFrameRef[0]));
            setFrame.Picture.texture.filterMode = filtermode;
            testImage.material.SetTexture("_MainTex", setFrame.Picture.texture);
            TimeProjector.material.SetTexture("_ShadowTex", setFrame.Picture.texture);

            // Set future texture
            setFrame = ActiveData.GetFrameAt(tempFrameRef[0], textureIndex + 1);
            setFrame.Picture.texture.filterMode = filtermode;
            TimeProjector.material.SetTexture("_ShadowTex2", setFrame.Picture.texture);
            testImage.material.SetTexture("_MainTex2", setFrame.Picture.texture);
        }
    }

    /// <summary>
    /// Place the min and max pins at the correct locations.
    /// </summary>
    /// <param name="frame">The frame to refrence for min/max placement.</param>
    private void PlacePins(Frame frame, Rect bb)
    {
        // Normalize between 100
        Vector3 maxLoc = new Vector3((1.0f - frame.record._MaxContainer.location.y/100.0f), 0.0f, frame.record._MaxContainer.location.x/100.0f);
        Vector2 latLongMax = new Vector2(bb.x + maxLoc.x * bb.width, bb.y + (1 - maxLoc.z) * bb.height);
        Vector2 normalMax = coordsystem.transformToUnity(latLongMax);
        maxPin.transform.position = mouseray.raycastHitFurtherest(new Vector3(normalMax.x, -100, normalMax.y), new Vector3(0, 1, 0));


        Vector3 minLoc = new Vector3((1.0f - frame.record._MinContainer.location.y) / 100.0f, 0.0f, frame.record._MinContainer.location.x / 100.0f);
        Vector2 latLongMin = new Vector2(bb.x + minLoc.x * bb.width, bb.y + (1 - minLoc.z) * bb.height);
        Vector2 normalMin = coordsystem.transformToUnity(latLongMin);
        minPin.transform.position = mouseray.raycastHitFurtherest(new Vector3(normalMin.x, -100, normalMin.y), new Vector3(0, 1, 0));
    }

    // Change the filter mode
    public void interpolationMode(Text input)
    {
        if (input.text == "Point")
        {
            filtermode = FilterMode.Point;
        }
        else if(input.text == "Bilinnear")
        {
            filtermode = FilterMode.Bilinear;
        }

    }
}
