using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class ColorSetter : MonoBehaviour {

	public HSVPicker picker;
	Image image;
	Button button;
	// Use this for initialization
	void Start () {
		image = gameObject.GetComponent<Image>();
		button = gameObject.GetComponent<Button>();
		button.onClick.AddListener( () =>
		{
		   SetColorImage();
		}
		);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void SetColorImage()
	{
		
		if(picker == null)
	 	{
			picker = GameObject.Find("Picker").GetComponent<HSVPicker>();
			if(picker== null)
			{
				return;
			}
		}
		picker.colorImage = image;
		
		gameObject.transform.parent.GetComponent<ColorPicker>().SetSelected(gameObject.transform.GetSiblingIndex());
	}
}
