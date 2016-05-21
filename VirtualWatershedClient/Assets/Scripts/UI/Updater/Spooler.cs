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
    public Image testImage2;
    public Slider gridSlider;
	public TimeSlider timeSlider;
	public Projector TimeProjector;
    public Material colorWindow, colorProjector, slideProjector, colorWindow2;
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
                if(testImage2.gameObject.activeSelf)
                {
                    testImage2.material.SetColor("_SegmentData00" + i.ToString(), colorPicker.ColorBoxes[i].GetComponent<Image>().color);
                }
                colorbar.material.SetColor("_SegmentData00" + i.ToString(), colorPicker.ColorBoxes[i].GetComponent<Image>().color);
            }

			// Add the ranges to the timeprojector and image
			for(int i = 0; i < colorPicker.ColorBoxes.Count; i++)
			{
				TimeProjector.material.SetFloat("_x" + (i + 1).ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
				testImage.material.SetFloat("_x" + (i + 1).ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
                if (testImage2.gameObject.activeSelf)
                {
                    testImage2.material.SetFloat("_x" + (i + 1).ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
                }
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
    public void UpdateData(string variable)
    {
        // TODO: Handle multiple projections for a projector

        // Get the active variables
        List<string> tempRef = ActiveData.GetCurrentAvtive();

        // Set projector
        Vector2 BoundingScale;
        TimeProjector.GetComponent<ProjectorObject>().PlaceProjector(ActiveData.GetBoundingBox(tempRef[0]), ActiveData.GetProjection(tempRef[0]), out BoundingScale);
        WMS = ActiveData.GetWMS(tempRef[0]);

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

        // Check for the need of a comparison
        if(tempRef.Count > 1)
        {
            testImage2.gameObject.SetActive(true);
            testImage2.material = colorWindow2;
            testImage2.material.SetFloat("_MaxX", BoundingScale.x);
            testImage2.material.SetFloat("_MaxY", BoundingScale.y);
            TimeProjector.material.SetInt("_Compare", 1);
        }
        else
        {
            testImage2.gameObject.SetActive(false);
            TimeProjector.material.SetInt("_Compare", 0);
        }

        // Update title
        selectedVariableTextBox.text = "Variable: " + variable + ": " + VariableReference.GetDescription(variable);
        ChangeTexture();
    }

    public void UpdateMinMax()
    {
        // Get the min/max for each active
        float min, max;
        List<string> tempRef = ActiveData.GetCurrentAvtive();
        if(tempRef.Count > 1)
        {
            Vector2 d1 = ActiveData.GetMinMax(tempRef[0]);
            Vector2 d2 = ActiveData.GetMinMax(tempRef[1]);
            min = d1.x - d2.y;
            max = d1.y - d2.x;
            testImage.material.SetFloat("_FloatMax", d1.y);
            testImage.material.SetFloat("_FloatMin", d1.x);
            if(!testImage2.gameObject.activeSelf)
            {
                testImage2.gameObject.SetActive(true);
            }
            testImage2.material.SetFloat("_FloatMax", d2.y);
            testImage2.material.SetFloat("_FloatMin", d2.x);
        }
        else
        {
            Vector2 d1 = ActiveData.GetMinMax(tempRef[0]);
            min = d1.x;
            max = d1.y;
            testImage.material.SetFloat("_FloatMax", max);
            testImage.material.SetFloat("_FloatMin", min);
        }

        // Updates the max information across the necessary classes
        TimeProjector.material.SetFloat("_FloatMax", max);        
        colorbar.material.SetFloat("_FloatMax", max);
        colorPicker.SetMax(max);
        colorbarObj.transform.GetChild(1).GetComponent<Text>().text = max.ToString();
        //colorPicker.Mean = modelrun.GetVariable(oldSelectedVariable).Mean;
        //colorPicker.frameCount = modelrun.GetVariable(oldSelectedVariable).frameCount;
        
        TimeProjector.material.SetFloat("_FloatMin", min);        
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

        // Check the active data
        List<String> tempFrameRef = ActiveData.GetCurrentAvtive();
        if(tempFrameRef.Count < 1)
        {
            return;
        }

        // Call the texture update
        AddTextureIndex(tempFrameRef[0], "_MainTex", "_FloatMin", "_FloatMax", testImage);
        if(tempFrameRef.Count > 1)
        {
            Debug.LogError("There are two active");            
            AddTextureIndex(tempFrameRef[1], "_MainTex2", "_FloatMin2", "_FloatMax2", testImage2);
        }
    }

    private void AddTextureIndex(string variable, string textureName, string minName, string maxName, Image image)
    {
        int currentcount = ActiveData.GetCount(variable);
        int textureIndex = ActiveData.FindNearestFrame(variable, lastUpdateTime);
        image.sprite = ActiveData.GetFrameAt(variable, textureIndex).Picture;

        // Get the min and max for the shader
        Vector2 MinMax = ActiveData.GetMinMax(variable);

        // Do nothing if there is no data
        if (currentcount < 1)
        {
            return;
        }

        // Set projector image
        if (textureIndex == currentcount - 1 || currentcount == 1)
        {
            // Set both textures to last reel texture
            Frame setframe = ActiveData.GetFrameAt(variable, currentcount - 1);
            PlacePins(setframe, ActiveData.GetBoundingBox(variable));
            setframe.Picture.texture.filterMode = filtermode;
            TimeProjector.material.SetTexture(textureName, setframe.Picture.texture);
            image.material.SetTexture(textureName, setframe.Picture.texture);
        }
        else
        {
            // Set current texture
            Frame setFrame = ActiveData.GetFrameAt(variable, textureIndex);
            PlacePins(setFrame, ActiveData.GetBoundingBox(variable));
            setFrame.Picture.texture.filterMode = filtermode;
            image.material.SetTexture(textureName, setFrame.Picture.texture);
            TimeProjector.material.SetTexture(textureName, setFrame.Picture.texture);
        }
    }

    /// <summary>
    /// Place the min and max pins at the correct locations.
    /// </summary>
    /// <param name="frame">The frame to refrence for min/max placement.</param>
    private void PlacePins(Frame frame, Rect bb)
    {
        // Debug.LogError("Max: " + frame.record._MaxContainer.location + " " + frame.record._MaxContainer.value);
        // Debug.LogError("Min: " + frame.record._MinContainer.location + " " + frame.record._MinContainer.value);

        // Normalize between 100
        Vector2 maxLoc = new Vector2((1.0f - ((frame.record._MaxContainer.location.y) / 100.0f)), (1.0f - ((frame.record._MaxContainer.location.x + 3) / 100.0f)));
        Vector2 latLongMax = new Vector2(bb.x + maxLoc.x * bb.width, bb.y + (maxLoc.y) * bb.height);
        Vector2 normalMax = coordsystem.transformToUnity(latLongMax);
        maxPin.transform.position = mouseray.raycastHitFurtherest(new Vector3(normalMax.x, -100, normalMax.y), new Vector3(0, 1, 0));

        Vector2 minLoc = new Vector2((1.0f - ((frame.record._MinContainer.location.y) / 100.0f)), (1.0f - ((frame.record._MinContainer.location.x + 3) / 100.0f)));
        Vector2 latLongMin = new Vector2(bb.x + minLoc.x * bb.width, bb.y + (minLoc.y) * bb.height);
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
