using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlaySpeedSliderControl : MonoBehaviour {

    private GameObject slider;
    private SliderUpdate sliderUpdateScript;

    private Slider speedControlSlider;

    private Text speedText;

	// Use this for initialization
	void Start () 
    {
        slider = GameObject.FindWithTag("TimeSlider");
        sliderUpdateScript = (SliderUpdate)slider.GetComponent(typeof(SliderUpdate));

        speedControlSlider = GameObject.FindWithTag("SpeedControlSlider").GetComponent<Slider>();
        speedText = GameObject.FindWithTag("SpeedText").GetComponent<Text>();

        OnValueChange(3);
	}
	
	// Update is called once per frame
	void Update () 
    {
	}
    
    public void OnValueChange()
    {
        OnValueChange(speedControlSlider.value);
    }

    public void OnValueChange(float value)
    {
        sliderUpdateScript.PlaySpeedProperty = Mathf.Pow(2f, value) / 8f;
        float spd = sliderUpdateScript.PlaySpeedProperty;
        if (spd >= 1)
            speedText.text = string.Format("1s:{0}h", (int)spd);
        else
            speedText.text = string.Format("1s:{0}m", (float)(spd*60f));

    }

}
