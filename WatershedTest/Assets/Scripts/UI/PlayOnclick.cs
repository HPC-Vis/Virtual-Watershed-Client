using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayOnclick : MonoBehaviour 
{
    public Text playBtnText;

    private GameObject slider;
    private SliderUpdate sliderUpdateScript;

	// Use this for initialization
    void Start()
    {
        playBtnText = GameObject.FindGameObjectWithTag("PlayButton")
                                .transform
                                .FindChild("Text")
                                .GetComponent<Text>();

        slider = GameObject.FindWithTag("TimeSlider"); 
        sliderUpdateScript = (SliderUpdate)slider.GetComponent(typeof(SliderUpdate));

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TogglePlayMode()
    {
        sliderUpdateScript.IsPlayingProperty =  !sliderUpdateScript.IsPlayingProperty;

        if (sliderUpdateScript.IsPlayingProperty)
            playBtnText.text = "Pause";
        else
            playBtnText.text = "Play";
    }
}
