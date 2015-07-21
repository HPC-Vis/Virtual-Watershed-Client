//Attach this script to a game object
//Apply 3 Unity UI Texts for food, dryness, and warmth
//This script should automatically find the C# UniStorm editor
//It will then simulate vitals according to UniStorm and its factors
//You will need an a tag of 

//UniStorm Vitals Example

//By: Black Horizon Studios

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerVitalsExample : MonoBehaviour {

	private GameObject uniStormSystem;
	private UniStormWeatherSystem_C uniStormSystemScript;
	public int food = 100;
	public float dryness = 100;
	public float warmth = 100;

	private bool isSheltered = false;
	private bool foodCalculated = false;

	public string shelterTag = "Shelter";

	public Text foodText;
	public Text drynessText;
	public Text warmthText;
	public Text shelteredText;

	void Awake () 
	{
			//Find the UniStorm Weather System Editor, this must match the UniStorm Editor name
			uniStormSystem = GameObject.Find("UniStormSystemEditor");
			uniStormSystemScript = uniStormSystem.GetComponent<UniStormWeatherSystem_C>();
	}
	
	void Start () {
		
		if (uniStormSystem == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Null Reference:</color> You must have the UniStorm Editor in your scene and named 'UniStormSystemEditor'. Make sure your C# UniStorm Editor has this name. ");
		}
				
	}
	
	void Update () {
		
	if (uniStormSystem != null)
	{
		if (uniStormSystemScript.temperature <= 50 && !isSheltered)	
		{
			warmth -= Time.deltaTime * 0.5f;
		}

		if (uniStormSystemScript.minuteCounter >= 59 && !foodCalculated)
		{
			food -= 1;
			foodCalculated = true;
		}

		if (uniStormSystemScript.minuteCounter <= 58)
		{
			foodCalculated = false;
		}
		
		if (uniStormSystemScript.temperature >= 51 && !isSheltered && uniStormSystemScript.minRainIntensity <= 0)
		{
			warmth += Time.deltaTime * 0.5f;

				if (warmth >= 100)
				{
					warmth = 100;
				}
		}

			if (uniStormSystemScript.minRainIntensity >= 50 && !isSheltered || uniStormSystemScript.minSnowIntensity >= 50 && !isSheltered)	
			{
				dryness -= Time.deltaTime * 0.5f;
				warmth -= Time.deltaTime * 0.5f;
			}
			else
			{
				dryness += Time.deltaTime * 0.5f;

				if (dryness >= 100)
				{
					dryness = 100;
				}
			}

			if (food >= 1)
			{
				foodText.text = "Food " + food.ToString() + "/100";
			}
			else
			{
				foodText.text = "Dead";
			}

			if (warmth >= 1)
			{
				warmthText.text = "Warmth " + warmth.ToString("F1") + "/100";
			}
			else
			{
				warmthText.text = "Dead";
			}

			if (dryness >= 1)
			{
				drynessText.text = "Dryness " + dryness.ToString("F1") + "/100";
			}
			else
			{
				drynessText.text = "Dead";
			}

			if (dryness <= 0)
			{
				dryness = 0;
			}
			
			if (warmth <= 0)
			{
				warmth = 0;
			}

			
			
			
			if (isSheltered)
			{
				warmth += Time.deltaTime * 0.5f;

				if (warmth >= 100)
				{
					warmth = 100;
				}

				dryness += Time.deltaTime * 0.5f;
				
				if (dryness >= 100)
				{
					dryness = 100;
				}

				shelteredText.text = "Sheltered";
				shelteredText.color = Color.green;
			}

			if (!isSheltered)
			{
				shelteredText.text = "Not Sheltered";
				shelteredText.color = Color.red;
			}


	}
		
  }

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.tag == shelterTag)
		{
			isSheltered = true;
		}
	}

	void OnTriggerExit(Collider other) 
	{
		if (other.gameObject.tag == shelterTag)
		{
			isSheltered = false;
		}
	}
	

}
