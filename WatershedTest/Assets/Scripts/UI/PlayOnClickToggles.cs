using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayOnClickToggles : MonoBehaviour 
{
    ToggleGroup toggleGroup;

    private GameObject slider;
    private SliderUpdate sliderUpdateScript;

	// Use this for initialization
    void Start()
    {
        slider = GameObject.FindWithTag("TimeSlider"); 
        sliderUpdateScript = (SliderUpdate)slider.GetComponent(typeof(SliderUpdate));
	}
	
	// Update is called once per frame
	public void onValueChange(Toggle playToggle) {
        sliderUpdateScript.IsPlayingProperty = playToggle.isOn;
	}
}
