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
    public Text selectedVariableTextBox;

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
			}

			// Add the ranges to the timeprojector and image
			for(int i = 0; i < colorPicker.ColorBoxes.Count; i++)
			{
				TimeProjector.material.SetFloat("_x" + i.ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
				testImage.material.SetFloat("_x" + i.ToString(), (float.Parse(colorPicker.ColorBoxes[i].transform.GetChild(0).GetComponent<Text>().text)));
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

        // Change the filter mode
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(filtermode == FilterMode.Bilinear)
            {
                filtermode = FilterMode.Point;
            }
            else
            {
                filtermode = FilterMode.Bilinear;
            }
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
        colorPicker.SetMax(max);
        //colorPicker.Mean = modelrun.GetVariable(oldSelectedVariable).Mean;
        //colorPicker.frameCount = modelrun.GetVariable(oldSelectedVariable).frameCount;
        
        TimeProjector.material.SetFloat("_FloatMin", min);
        testImage.material.SetFloat("_FloatMin", min);
        colorPicker.SetMin(min);
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
}
