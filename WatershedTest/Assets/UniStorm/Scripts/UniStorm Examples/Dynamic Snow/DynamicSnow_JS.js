
	private var uniStormSystem : GameObject;
	private var uniStormSystemScript : UniStormWeatherSystem_JS;
	var snowAmount : float;

	function Start () 
	{
		//Find the UniStorm Weather System Editor, this must match the UniStorm Editor name
		uniStormSystem = GameObject.Find("UniStormSystemEditor");
		uniStormSystemScript = uniStormSystem.GetComponent(UniStormWeatherSystem_JS);
	}
	

	function Update () 
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