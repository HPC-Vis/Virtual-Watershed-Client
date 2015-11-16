using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeatherZoneExample_C : MonoBehaviour {
	
	public GameObject uniStormSystem;
	public int zoneWeather = 1;

	void Start () {
	
		uniStormSystem = GameObject.Find("UniStormSystemEditor");
		
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			uniStormSystem.GetComponent<UniStormWeatherSystem_C>().weatherForecaster = zoneWeather;
		}
	}
}
