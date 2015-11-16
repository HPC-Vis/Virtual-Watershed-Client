//This script demonstrates UniStorm's dynamic snow shader (currently in beta)
//If it's snowing, the dynamic snow will start to form.
//If it's above 32 degrees, it will start to melt.
//This is globally altered so all materials using this shader will be affected
//Requires the reference of at least 1 material using the snow shader

//Black Horizon Studios

using UnityEngine;
using System.Collections;

public class DynamicSnow_C : MonoBehaviour {

	private GameObject uniStormSystem;
	private UniStormWeatherSystem_C uniStormSystemScript;
	public float snowAmount;

	void Start () 
	{
		//Find the UniStorm Weather System Editor, this must match the UniStorm Editor name
		uniStormSystem = GameObject.Find("UniStormSystemEditor");
		uniStormSystemScript = uniStormSystem.GetComponent<UniStormWeatherSystem_C>();
	}
	

	void Update () 
	{
		if (uniStormSystemScript.weatherForecaster == 3 && uniStormSystemScript.temperature <= 32 && uniStormSystemScript.temperatureType == 1 || uniStormSystemScript.weatherForecaster == 3 && uniStormSystemScript.temperature <= 0 && uniStormSystemScript.temperatureType == 2)
		{
			if (uniStormSystemScript.minSnowIntensity >= 50)
			{
				snowAmount += Time.deltaTime * 0.008f;
				Shader.SetGlobalFloat("_LayerStrength", snowAmount);

				if (snowAmount >= 0.6f)
				{
					snowAmount = 0.6f;
				}
			}
		}

		if (uniStormSystemScript.temperature > 32)
		{
			snowAmount -= Time.deltaTime * 0.008f;
			Shader.SetGlobalFloat("_LayerStrength", snowAmount);
			
			if (snowAmount <= 0)
			{
				snowAmount = 0;
			}
		}

		if (uniStormSystemScript.weatherForecaster == 2 && uniStormSystemScript.temperature <= 32 && uniStormSystemScript.temperatureType == 1 || uniStormSystemScript.weatherForecaster == 2 && uniStormSystemScript.temperature <= 0 && uniStormSystemScript.temperatureType == 2)
		{
			snowAmount += Time.deltaTime * 0.008f;
			Shader.SetGlobalFloat("_LayerStrength", snowAmount);
			
			if (snowAmount >= 0.6f)
			{
				snowAmount = 0.6f;
			}
		}
	}
}
