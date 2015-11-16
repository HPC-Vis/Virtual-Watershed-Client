	var playerPosition : Vector3;
	var playerRotation : Vector3;
	var UniStorm : GameObject;
	var currentHour : int;
	var rotationToSet : Vector3;
	var dataLoaded : boolean = false;

	function Start () 
	{
		UniStorm = GameObject.Find("UniStormSystemEditor");
	}

	function Update () 
	{
		if(Input.GetKeyDown(KeyCode.O))
		{
			PlayerPrefs.SetInt("Current Minute", UniStorm.GetComponent(UniStormWeatherSystem_JS).minuteCounter);

			currentHour = UniStorm.GetComponent(UniStormWeatherSystem_JS).Hour;

			PlayerPrefs.SetFloat("Current Hour", currentHour);

			PlayerPrefs.SetInt("Current Weather", UniStorm.GetComponent(UniStormWeatherSystem_JS).weatherForecaster);
			PlayerPrefs.SetInt("Current Day", UniStorm.GetComponent(UniStormWeatherSystem_JS).dayCounter);
			PlayerPrefs.SetFloat("Current Month", UniStorm.GetComponent(UniStormWeatherSystem_JS).monthCounter);
			PlayerPrefs.SetFloat("Current Year", UniStorm.GetComponent(UniStormWeatherSystem_JS).yearCounter);
			PlayerPrefs.SetInt("Current Temperature", UniStorm.GetComponent(UniStormWeatherSystem_JS).temperature);

			playerPosition = transform.position;
			playerRotation = transform.rotation.eulerAngles; 

			PlayerPrefs.SetFloat("Player Position X", playerPosition.x);
			PlayerPrefs.SetFloat("Player Position Y", playerPosition.y);
			PlayerPrefs.SetFloat("Player Position Z", playerPosition.z);

			PlayerPrefs.SetFloat("Player Rotation X", playerRotation.x);
			PlayerPrefs.SetFloat("Player Rotation Y", playerRotation.y);
			PlayerPrefs.SetFloat("Player Rotation Z", playerRotation.z);
			
			Debug.Log("Game Saved" + "\n" + " In-Game Time " + currentHour + ":" + UniStorm.GetComponent(UniStormWeatherSystem_JS).minuteCounter + " | "  + " In-Game Date " + UniStorm.GetComponent(UniStormWeatherSystem_JS).monthCounter + "/" + UniStorm.GetComponent(UniStormWeatherSystem_JS).dayCounter + "/" + UniStorm.GetComponent(UniStormWeatherSystem_JS).yearCounter + " |  Current Weather " + UniStorm.GetComponent(UniStormWeatherSystem_JS).weatherString + " | Current Temperature " + UniStorm.GetComponent(UniStormWeatherSystem_JS).temperature);
		}

		if(Input.GetKeyDown(KeyCode.L))
		{
			UniStorm.GetComponent(UniStormWeatherSystem_JS).realStartTimeMinutes = PlayerPrefs.GetInt("Current Minute");
			UniStorm.GetComponent(UniStormWeatherSystem_JS).realStartTime = PlayerPrefs.GetFloat("Current Hour");


			UniStorm.GetComponent(UniStormWeatherSystem_JS).LoadTime();

			UniStorm.GetComponent(UniStormWeatherSystem_JS).weatherForecaster = PlayerPrefs.GetInt("Current Weather");
			UniStorm.GetComponent(UniStormWeatherSystem_JS).dayCounter = PlayerPrefs.GetInt("Current Day");
			UniStorm.GetComponent(UniStormWeatherSystem_JS).monthCounter = PlayerPrefs.GetFloat("Current Month");
			UniStorm.GetComponent(UniStormWeatherSystem_JS).yearCounter = PlayerPrefs.GetFloat("Current Year");
			UniStorm.GetComponent(UniStormWeatherSystem_JS).temperature = PlayerPrefs.GetInt("Current Temperature");


			playerPosition.x = PlayerPrefs.GetFloat("Player Position X");
			playerPosition.y = PlayerPrefs.GetFloat("Player Position Y");
			playerPosition.z = PlayerPrefs.GetFloat("Player Position Z");

			playerRotation.x = PlayerPrefs.GetFloat("Player Rotation X");
			playerRotation.y = PlayerPrefs.GetFloat("Player Rotation Y");
			playerRotation.z = PlayerPrefs.GetFloat("Player Rotation Z");

			UniStorm.GetComponent(UniStormWeatherSystem_JS).InstantWeather();

			transform.position = new Vector3 (playerPosition.x, playerPosition.y, playerPosition.z);

			rotationToSet = new Vector3 (playerRotation.x, playerRotation.y, playerRotation.z);
			transform.eulerAngles = rotationToSet;

			Debug.Log("Game Loaded");

		}
	}	