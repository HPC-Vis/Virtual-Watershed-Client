//Black Horizon Studios
//UniStorm Save Example

using UnityEngine;
using System.Collections;

public class SavePlayerData_C : MonoBehaviour {

	public Vector3 playerPosition;
	public Vector3 playerRotation;
	public GameObject UniStorm;
	public float currentHour;
	public Vector3 rotationToSet;
	public bool dataLoaded = false;

	void Start () 
	{
		UniStorm = GameObject.Find("UniStormSystemEditor");
	}
	

	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.O))
		{
			PlayerPrefs.SetInt("Current Minute", UniStorm.GetComponent<UniStormWeatherSystem_C>().minuteCounter);

			currentHour = (int)UniStorm.GetComponent<UniStormWeatherSystem_C>().Hour;

			PlayerPrefs.SetFloat("Current Hour", currentHour);

			PlayerPrefs.SetInt("Current Weather", UniStorm.GetComponent<UniStormWeatherSystem_C>().weatherForecaster);
			PlayerPrefs.SetInt("Current Day", UniStorm.GetComponent<UniStormWeatherSystem_C>().dayCounter);
			PlayerPrefs.SetFloat("Current Month", UniStorm.GetComponent<UniStormWeatherSystem_C>().monthCounter);
			PlayerPrefs.SetFloat("Current Year", UniStorm.GetComponent<UniStormWeatherSystem_C>().yearCounter);
			PlayerPrefs.SetInt("Current Temperature", UniStorm.GetComponent<UniStormWeatherSystem_C>().temperature);

			playerPosition = transform.position;
			playerRotation = transform.rotation.eulerAngles; 

			PlayerPrefs.SetFloat("Player Position X", playerPosition.x);
			PlayerPrefs.SetFloat("Player Position Y", playerPosition.y);
			PlayerPrefs.SetFloat("Player Position Z", playerPosition.z);

			PlayerPrefs.SetFloat("Player Rotation X", playerRotation.x);
			PlayerPrefs.SetFloat("Player Rotation Y", playerRotation.y);
			PlayerPrefs.SetFloat("Player Rotation Z", playerRotation.z);

			Debug.Log("Game Saved" + "\n" + " In-Game Time " + currentHour + ":" + UniStorm.GetComponent<UniStormWeatherSystem_C>().minuteCounter + " | "  + " In-Game Date " + UniStorm.GetComponent<UniStormWeatherSystem_C>().monthCounter + "/" + UniStorm.GetComponent<UniStormWeatherSystem_C>().dayCounter + "/" + UniStorm.GetComponent<UniStormWeatherSystem_C>().yearCounter + " |  Current Weather " + UniStorm.GetComponent<UniStormWeatherSystem_C>().weatherString + " | Current Temperature " + UniStorm.GetComponent<UniStormWeatherSystem_C>().temperature);
		}

		if(Input.GetKeyDown(KeyCode.L))
		{
			UniStorm.GetComponent<UniStormWeatherSystem_C>().realStartTimeMinutes = PlayerPrefs.GetInt("Current Minute");
			UniStorm.GetComponent<UniStormWeatherSystem_C>().realStartTime = PlayerPrefs.GetFloat("Current Hour");


			UniStorm.GetComponent<UniStormWeatherSystem_C>().LoadTime();

			UniStorm.GetComponent<UniStormWeatherSystem_C>().weatherForecaster = PlayerPrefs.GetInt("Current Weather");
			UniStorm.GetComponent<UniStormWeatherSystem_C>().dayCounter = PlayerPrefs.GetInt("Current Day");
			UniStorm.GetComponent<UniStormWeatherSystem_C>().monthCounter = PlayerPrefs.GetFloat("Current Month");
			UniStorm.GetComponent<UniStormWeatherSystem_C>().yearCounter = PlayerPrefs.GetFloat("Current Year");
			UniStorm.GetComponent<UniStormWeatherSystem_C>().temperature = PlayerPrefs.GetInt("Current Temperature");


			playerPosition.x = PlayerPrefs.GetFloat("Player Position X");
			playerPosition.y = PlayerPrefs.GetFloat("Player Position Y");
			playerPosition.z = PlayerPrefs.GetFloat("Player Position Z");

			playerRotation.x = PlayerPrefs.GetFloat("Player Rotation X");
			playerRotation.y = PlayerPrefs.GetFloat("Player Rotation Y");
			playerRotation.z = PlayerPrefs.GetFloat("Player Rotation Z");

			UniStorm.GetComponent<UniStormWeatherSystem_C>().InstantWeather();

			transform.position = new Vector3 (playerPosition.x, playerPosition.y, playerPosition.z);

			rotationToSet = new Vector3 (playerRotation.x, playerRotation.y, playerRotation.z);
			transform.eulerAngles = rotationToSet;

			Debug.Log("Game Loaded");

		}
	}
}
