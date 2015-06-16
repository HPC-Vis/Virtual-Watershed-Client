using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class UpdateUISimstepText : MonoBehaviour {

    private GameObject slider;
    private SliderUpdate sliderUpdateScript;

    private Text text;

    // Use this for initialization
    void Start()
    {
        slider = GameObject.FindWithTag("TimeSlider");
        sliderUpdateScript = (SliderUpdate)slider.GetComponent(typeof(SliderUpdate));

        text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        text.text = sliderUpdateScript.SimStepProperty.ToString("D4");
	}
}
