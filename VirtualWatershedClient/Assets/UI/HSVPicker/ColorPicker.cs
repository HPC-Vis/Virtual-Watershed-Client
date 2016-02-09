#define OLDWAY
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour {

	public List<GameObject> ColorBoxes = new List<GameObject>();
	public Slider slider;

    private Color[,] DefinedColors;
	// Use this for initialization
	void Start () {
        DefinedColors = new Color[4,5];

        DefinedColors[0,0] = new Color32(239, 251, 205, 255);
        DefinedColors[0,1] = new Color32(220, 231, 189, 255);
        DefinedColors[0,2] = new Color32(85, 81, 82, 255);
        DefinedColors[0,3] = new Color32(46, 38, 51, 255);
        DefinedColors[0,4] = new Color32(158, 41, 59, 255);

        DefinedColors[1, 0] = new Color32(1, 91, 100, 255);
        DefinedColors[1, 1] = new Color32(255, 249, 213, 255);
        DefinedColors[1, 2] = new Color32(255, 207, 71, 255);
        DefinedColors[1, 3] = new Color32(226, 155, 47, 255);
        DefinedColors[1, 4] = new Color32(196, 72, 46, 255);

        DefinedColors[2, 0] = new Color32(238, 81, 74, 255);
        DefinedColors[2, 1] = new Color32(146, 51, 47, 255);
        DefinedColors[2, 2] = new Color32(138, 227, 225, 255);
        DefinedColors[2, 3] = new Color32(87, 190, 187, 255);
        DefinedColors[2, 4] = new Color32(255, 255, 255, 255);

        DefinedColors[3, 0] = new Color32(92, 57, 63, 255);
        DefinedColors[3, 1] = new Color32(142, 154, 166, 255);
        DefinedColors[3, 2] = new Color32(193, 226, 243, 255);
        DefinedColors[3, 3] = new Color32(226, 241, 208, 255);
        DefinedColors[3, 4] = new Color32(43, 61, 75, 255);

        AddColors(5);
    }

	bool updateHeight = false;
	int selected = -1;
    bool started = false;
    float Min, Max;
    public float Mean;
    public int frameCount = 1;
	
	// Update is called once per frame
	void Update () {

        
		if(updateHeight)
		{
			float height=0;
			
			foreach (Transform child in gameObject.transform) 
			{
				height += child.GetComponent<RectTransform>().sizeDelta.y;
			}
			height += 12.5f;
			//gameObject.GetComponent<RectTransform>()
			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x,height);
			updateHeight = false;
		}
        /*
		if(!started)
		{
			AddColors(5);
            started = true;
		}
        */
	}
	
	public void SetSelected(int index)
	{
		selected = index;
        float value_hold = float.Parse(ColorBoxes[selected].transform.GetChild(0).GetComponent<Text>().text);

        if (selected == 0)
        {
            slider.minValue = Min;
            slider.maxValue = float.Parse(ColorBoxes[selected+1].transform.GetChild(0).GetComponent<Text>().text);
        }
        else if (selected == 5)
        {
            slider.minValue = float.Parse(ColorBoxes[selected - 1].transform.GetChild(0).GetComponent<Text>().text);
            slider.maxValue = Max;
        }
        else
        {
            slider.minValue = float.Parse(ColorBoxes[selected - 1].transform.GetChild(0).GetComponent<Text>().text);
            slider.maxValue = float.Parse(ColorBoxes[selected + 1].transform.GetChild(0).GetComponent<Text>().text);
        }
        slider.value = value_hold;
	}

    //update to a normal distribution instead of a uniform distribution
    public void SetMin(float min)
    {
        Min = min;
#if OLDWAY
        float increment = (Max - Min) / (ColorBoxes.Count - 1);
        float assignment = Min;
        foreach (var box in ColorBoxes)
        {
            box.transform.GetChild(0).GetComponent<Text>().text = assignment.ToString();
            assignment += increment;
        }
#elif NEWWAY
        SetRanges();

#endif

        slider.minValue = Min;

    }

    public void SetMax(float max)
    {
        Max = max;
#if OLDWAY
        float increment = (Max - Min) / (ColorBoxes.Count - 1);
        float assignment = Min;
        foreach (var box in ColorBoxes)
        {
            box.transform.GetChild(0).GetComponent<Text>().text = assignment.ToString();
            assignment += increment;
        }
#elif NEWWAY
        SetRanges();
#endif
        slider.maxValue = Max;
    }


    public void ChangeColorPalette(int index)
    {
        for(int i = 0; i < ColorBoxes.Count; i++)
        {
            ColorBoxes[i].GetComponent<Image>().color = DefinedColors[index, i];
        }
    }
	

    public void SetRanges()
    {
        float stdDev = Mean / Mathf.Sqrt(frameCount);
        ColorBoxes[0].transform.GetChild(0).GetComponent<Text>().text = (Mean - stdDev * 2).ToString();
        ColorBoxes[1].transform.GetChild(0).GetComponent<Text>().text = (Mean - stdDev * 0.3).ToString();
        ColorBoxes[2].transform.GetChild(0).GetComponent<Text>().text = (Mean - stdDev * 0.05).ToString();
        ColorBoxes[3].transform.GetChild(0).GetComponent<Text>().text = (Mean + stdDev * 0.05).ToString();
        ColorBoxes[4].transform.GetChild(0).GetComponent<Text>().text = (Mean + stdDev * 0.3).ToString();
        ColorBoxes[5].transform.GetChild(0).GetComponent<Text>().text = (Mean + stdDev * 2).ToString();

    }

    public void setSelectedValue()
	{
		if(selected != -1 && selected != (ColorBoxes.Count - 1))
		{
            var highVal = float.Parse(ColorBoxes[selected+1].transform.GetChild(0).GetComponent<Text>().text);
            
            if( highVal > slider.value)
            {
                ColorBoxes[selected].transform.GetChild(0).GetComponent<Text>().text = slider.value.ToString();
            }
            else
            {
                ColorBoxes[selected].transform.GetChild(0).GetComponent<Text>().text = highVal.ToString();
                slider.value = highVal;
            }
            if (selected > 0)
            {
                var lowVal = float.Parse(ColorBoxes[selected - 1].transform.GetChild(0).GetComponent<Text>().text);
                if (lowVal < slider.value)
                {
                    ColorBoxes[selected].transform.GetChild(0).GetComponent<Text>().text = slider.value.ToString();
                }
                else
                {
                    slider.value = lowVal;
                }
            }
            // Ugly accesss statements that needs to be fixed at some point... easy fix though
			// ColorBoxes[selected].transform.GetChild(0).GetComponent<Text>().text = slider.value.ToString();
		}
	}
	
	// Lets add a number of colors to this window
	void AddColors(int numColors)
	{
		// clear children connect to this gameobject.
		foreach (Transform child in gameObject.transform) 
		{
			if(child.gameObject.name == "RangeAdjuster")
			{
				continue;
			}
			
			GameObject.Destroy(child.gameObject);
		}
		// Find the picker...
		var found = GameObject.Find("ColorPicker");
		
		if(found == null)
		{
			// Create a color picker here	
		}

		// spawn prefabs and attach them as children to this gameobject.
		for(int i =0; i < numColors; i++)
		{
			var go = Resources.Load<GameObject>("UI/ColorRow");
			//go.activeSelf()
			go = GameObject.Instantiate(go);
			go.transform.SetParent(gameObject.transform);
            go.transform.localScale = Vector3.one;
			//var picker = found.GetComponent<HSVPicker>();
			var setter = go.GetComponent<ColorSetter>();
			//setter.picker = picker;
            if(6 > i)
            {
                go.GetComponent<Image>().color = DefinedColors[0,i];
            }
            else
            {
                go.GetComponent<Image>().color = Color.black;
            }
            
			ColorBoxes.Add(go);
		}
		
		gameObject.transform.GetChild(0).SetAsLastSibling();
		updateHeight = true;


    }
	
}
