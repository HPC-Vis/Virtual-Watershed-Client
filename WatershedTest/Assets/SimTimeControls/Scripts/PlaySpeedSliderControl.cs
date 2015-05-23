/*
 * Copyright (c) 2014, Roger Lew (rogerlew.gmail.com)
 * Date: 2/5/2015
 * License: BSD (3-clause license)
 * 
 * The project described was supported by NSF award number IIA-1301792
 * from the NSF Idaho EPSCoR Program and by the National Science Foundation.
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VTL.SimTimeControls
{
	public class PlaySpeedSliderControl : MonoBehaviour
	{
		public TimeSlider timeSlider;

	    private Slider speedControlSlider;

	    private Text speedText;

		// Use this for initialization
		void Start () 
	    {
	        speedControlSlider = gameObject.GetComponent<Slider>();

			foreach (Transform child in transform) 
			{
				if (child.name == "Speed Text")
					speedText = child.gameObject.GetComponent<Text>();
			}
	        

	        Invoke("OnValueChange", 1f/60f);
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
	        timeSlider.PlaySpeed = Mathf.Pow(2f, value) / 8f;
	        float spd = timeSlider.PlaySpeed;
	        if (spd >= 1)
	            speedText.text = string.Format("1s:{0}h", (int)spd);
	        else
	            speedText.text = string.Format("1s:{0}m", (float)(spd*60f));

	    }

	}
}