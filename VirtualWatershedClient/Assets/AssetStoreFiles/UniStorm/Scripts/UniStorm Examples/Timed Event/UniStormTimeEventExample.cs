//Attach this script to a game object
//This script should automatically find the C# UniStorm editor
//Set your hour for the event for a printed debug log of a test event
//You will need to add a custom event yourself
//This is an example of how to access the time to do so

//UniStorm Time Event Script Example

//By: Black Horizon Studios

using UnityEngine;
using System.Collections;

public class UniStormTimeEventExample : MonoBehaviour {

	private GameObject uniStormSystem;
	public float hourOfEvent;
	public float endHourOfEvent;
	public bool eventTestBool;
	public Light lightSource;
	public int Hour;
	
	void Awake () {

			//Find the UniStorm Weather System Editor, this must match the UniStorm Editor name
			uniStormSystem = GameObject.Find("UniStormSystemEditor");
	
	}
	
	void Start () {
		
		if (uniStormSystem == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Null Reference:</color> You must have the UniStorm Editor in your scene and named 'UniStormSystemEditor'. Make sure your C# UniStorm Editor has this name. ");
		}

		Hour = uniStormSystem.GetComponent<UniStormWeatherSystem_C>().hourCounter;

		if (Hour > 6 && Hour < 19)
		{
			
			lightSource.intensity = 0;
			lightSource.enabled = false;

		}
				
	}
	
	void Update () {

		
	if (uniStormSystem != null)
	{
			Hour = uniStormSystem.GetComponent<UniStormWeatherSystem_C>().hourCounter;

			if (Hour >= 19 && Hour < 24 && eventTestBool == false || Hour >= 0 && Hour < 5 && eventTestBool == false)
		{
				lightSource.intensity += Time.deltaTime;
				lightSource.enabled = true;

				if (lightSource.intensity >= 2)
				{
					eventTestBool = true;
				}
			
			
		}

			if (Hour > 5 && Hour < 19 && eventTestBool == true)
		{
				
				lightSource.intensity -= Time.deltaTime; 
				
				if (lightSource.intensity <= 0)
				{
					lightSource.enabled = false;
					eventTestBool = false;
				}

			
		}
		
	}
		
  }
	

}
