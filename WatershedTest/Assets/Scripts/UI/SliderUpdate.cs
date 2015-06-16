using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class SliderUpdate : MonoBehaviour
{
    
    private bool isPlaying;
    public bool IsPlayingProperty
    {
        get { return isPlaying; }
        set { isPlaying = value; }
    }

    private float playSpeed;
    public float PlaySpeedProperty
    {
        get { return playSpeed; }
        set { playSpeed = value; }
    }

    private bool LastPlayState;
    private DateTime LastUpdate;

    private DateTime StartTime = new DateTime(2010, 10, 1);
    public DateTime StartTimeProperty
    {
        get { return StartTime; }
        set { StartTime = value; }
    }

    private TimeSpan TimeDelta = new TimeSpan(1, 0, 0);
    public TimeSpan TimeDeltaProperty
    {
        get { return TimeDelta; }
        set { TimeDelta = value; }
    }


    private DateTime SimTime;
    public DateTime SimTimeProperty
    {
        get { return SimTime; }
        set { SimTime = value; }
    }

    private Int64 SimStep;
    public Int64 SimStepProperty
    {
        get { return SimStep; }
        set { SimStep = value; }
    }
	
	private GameObject target;
	private Slider targetSlider;

    public float maxSteps;
	
	// Use this for initialization
	void Start () {
		target = GameObject.FindWithTag("TimeSlider");
		targetSlider = target.GetComponent<Slider>();
        targetSlider.maxValue = maxSteps;
		
		isPlaying = true;
		SimTime = StartTime;
		LastUpdate = DateTime.UtcNow;

        PlaySpeedProperty = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaying) {
			if (LastPlayState)
			{
                targetSlider.value += (float)(DateTime.UtcNow - LastUpdate).TotalSeconds * PlaySpeedProperty;
			}
			LastUpdate = DateTime.UtcNow;
			LastPlayState = true;
			
			SimStep = (Int64)Math.Floor(targetSlider.value);
			SimTime = StartTime + TimeSpan.FromTicks( (Int64)((double)TimeDelta.Ticks * targetSlider.value));
		}
		else 
		{
			LastPlayState = false;
		}
		
        //if (false)  
        //    Debug.Log(String.Format("{0}, {1}, {2}, {3}, {4}", 
        //                            isPlaying,
        //                            Time.time,
        //                            targetSlider.value,
        //                            SimStep, SimTime));
	}
	
	void TogglePlaying() {
        isPlaying = !isPlaying;
	}
}
