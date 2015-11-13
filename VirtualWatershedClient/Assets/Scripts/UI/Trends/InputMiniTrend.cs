using UnityEngine;
using System.Collections;

public class InputMiniTrend : MonoBehaviour 
{
    public string variableName = "m_pp";
    public float vmin = -18f;
    public float vmax = 32f;
    public float range = 3f;
    public float xvel = -0.5f;
    public float life0 = 20f;

    private GameObject inputPanel;
    private InputPanel inputPanelScript;

    private ParticleSystem.Particle[] points;

    private GameObject slider;
    private SliderUpdate sliderUpdateScript;

    private int laststep = -1;
    private bool lastisplaying = true;
    private Vector3 origin;

    private Vector3 curpos;
    private Vector3 velocity;
    public Color trendcolor = new Color(61f/255f, 43f/255f, 129f/255f);

    // Use this for initialization
    void Start()
    {
        slider = GameObject.FindWithTag("TimeSlider");
        sliderUpdateScript = (SliderUpdate)slider.GetComponent<SliderUpdate>();

        inputPanel = GameObject.FindWithTag("InputPanel");
        inputPanelScript = (InputPanel)inputPanel.GetComponent<InputPanel>();

        origin = GetComponent<ParticleSystem>().transform.position;
        curpos = origin;
        velocity = new Vector3(xvel, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () 
    {
        int step = (int)sliderUpdateScript.SimStepProperty;

        if (step != laststep)
        { 
            float value = inputPanelScript.valueAtStep(variableName, step);
            curpos = new Vector3(origin.x, 
                                 origin.y + range * (value - vmin) / (vmax - vmin), 
                                 origin.z);

            if (laststep > 0)
                GetComponent<ParticleSystem>().Emit(curpos, velocity, 0.3f, life0, trendcolor);
        }
        
        laststep = step;

        // was paused now playing
        if (sliderUpdateScript.IsPlayingProperty & !lastisplaying)
        {
            GetComponent<ParticleSystem>().Play();
        }
        // was playing now paused
        else if (!sliderUpdateScript.IsPlayingProperty & lastisplaying)
        {
            GetComponent<ParticleSystem>().Pause();
        }
        lastisplaying = sliderUpdateScript.IsPlayingProperty;
	}
}
