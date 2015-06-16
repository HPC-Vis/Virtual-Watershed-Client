using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlaySpeedSelection : MonoBehaviour {

    private Button speedButton12;
    private Button speedButton24;
    private Button speedButton48;
    private Button speedButton96;

    private GameObject slider;
    private SliderUpdate sliderUpdateScript;

	// Use this for initialization
	void Start () {
        speedButton12 = GameObject.FindWithTag("SpeedButton12").GetComponent<Button>();
        speedButton24 = GameObject.FindWithTag("SpeedButton24").GetComponent<Button>();
        speedButton48 = GameObject.FindWithTag("SpeedButton48").GetComponent<Button>();
        speedButton96 = GameObject.FindWithTag("SpeedButton96").GetComponent<Button>();

        slider = GameObject.FindWithTag("TimeSlider"); 
        sliderUpdateScript = (SliderUpdate)slider.GetComponent(typeof(SliderUpdate));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetSpeed12()
    {
        speedButton12.enabled = false;
        speedButton24.enabled = true;
        speedButton48.enabled = true;
        speedButton96.enabled = true;

        sliderUpdateScript.PlaySpeedProperty = 12f;
    }

    public void SetSpeed24()
    {
        speedButton12.enabled = true;
        speedButton24.enabled = false;
        speedButton48.enabled = true;
        speedButton96.enabled = true;

        sliderUpdateScript.PlaySpeedProperty = 24f;
    }

    public void SetSpeed48()
    {
        speedButton12.enabled = true;
        speedButton24.enabled = true;
        speedButton48.enabled = false;
        speedButton96.enabled = true;

        sliderUpdateScript.PlaySpeedProperty = 48f;
    }

    public void SetSpeed96()
    {
        speedButton12.enabled = true;
        speedButton24.enabled = true;
        speedButton48.enabled = true;
        speedButton96.enabled = false;

        sliderUpdateScript.PlaySpeedProperty = 96f;
    }
}
