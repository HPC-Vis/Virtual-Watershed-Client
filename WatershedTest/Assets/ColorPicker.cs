using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class ColorPicker : MonoBehaviour {

	public List<GameObject> ColorBoxes = new List<GameObject>();
	public Slider slider;
	// Use this for initialization
	void Start () {
        //AddColors(5);
	}

	bool updateHeight = false;
	int selected = -1;
    bool enabled = false;
    float Min, Max;
	
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
		if(!enabled)
		{
			AddColors(6);
            enabled = true;
		}
	}
	
	public void SetSelected(int index)
	{
		selected = index;
        slider.value = float.Parse(ColorBoxes[selected].transform.GetChild(0).GetComponent<Text>().text);
	}

    public void SetMinMax(float min, float max)
    {
        Min = min;
        Max = max;
        float increment = (Max - Min) / (ColorBoxes.Count - 1);
        float assignment = Min;
        foreach (var box in ColorBoxes)
        {
            box.transform.GetChild(0).GetComponent<Text>().text = assignment.ToString();
            assignment += increment;
        }
        slider.minValue = Min;
        slider.maxValue = Max;
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

        Color[] setValues = new Color[6];
        setValues[0] = Color.red;
        setValues[1] = Color.blue;
        setValues[2] = Color.green;
        setValues[3] = Color.yellow;
        setValues[4] = Color.gray;
        setValues[5] = Color.black;
		// spawn prefabs and attach them as children to this gameobject.
		for(int i =0; i < numColors; i++)
		{
			var go = Resources.Load<GameObject>("UI/ColorRow");
			//go.activeSelf()
			go = GameObject.Instantiate(go);
			go.transform.SetParent(gameObject.transform);
			var picker = found.GetComponent<HSVPicker>();
			var setter = go.GetComponent<ColorSetter>();
			setter.picker = picker;
            if(6 > i)
            {
                go.GetComponent<Image>().color =setValues[i];
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
