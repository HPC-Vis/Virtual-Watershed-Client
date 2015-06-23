using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class ColorPicker : MonoBehaviour {

	List<GameObject> ColorBoxes = new List<GameObject>();
	public Slider slider;
	// Use this for initialization
	void Start () {
	
	}
	
	bool updateHeight = false;
	int selected = -1;
	
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
			Debug.LogError(height);
			gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x,height);
			updateHeight = false;
		}
		if(Input.GetKeyDown(KeyCode.Space))
		{
			AddColors(5);
		}
	}
	
	public void SetSelected(int index)
	{
		selected = index;
		Debug.LogError("selected: " + selected);
	}
	
	public void setSelectedValue()
	{
		if(selected != -1)
		{
			Debug.LogError(slider.value.ToString());
			// Ugly accesss statements that needs to be fixed at some point... easy fix though
			ColorBoxes[selected].transform.GetChild(0).GetComponent<Text>().text = slider.value.ToString();
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
		var found = GameObject.Find("Picker");
		
		if(found == null)
		{
			// Create a color picker here	
		}
		
		
		// spawn prefabs and attach them as children to this gameobject.
		for(int i =0; i < numColors; i++)
		{
			Debug.LogError(i);
			var go = Resources.Load<GameObject>("UI/ColorRow");
			//go.activeSelf()
			go = GameObject.Instantiate(go);
			go.transform.SetParent(gameObject.transform);
			var picker = found.GetComponent<HSVPicker>();
			var setter = go.GetComponent<ColorSetter>();
			setter.picker = picker;
			ColorBoxes.Add(go);
		}
		
		gameObject.transform.GetChild(0).SetAsLastSibling();
		updateHeight = true;
	}
	
}
