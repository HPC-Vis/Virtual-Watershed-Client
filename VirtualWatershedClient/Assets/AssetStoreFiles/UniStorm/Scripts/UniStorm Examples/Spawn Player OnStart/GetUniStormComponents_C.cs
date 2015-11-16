using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class GetUniStormComponents_C : MonoBehaviour
{
	#region fields
	public GameObject unistorm;
	public GameObject unistormCamera;
	public ParticleSystem rain;
	public ParticleSystem snow;
	public ParticleSystem lightningBugs;
	public ParticleSystem rainMist;
	public ParticleSystem snowDust;
	public GameObject rainStreaks;
	public ParticleSystem windyLeaves;
	public GameObject lightningBolt1;
	public ParticleSystem rainSplash;
	public Transform lightningPosition;

	#endregion
	
	void Awake ()	
	{
		unistorm = GameObject.FindGameObjectWithTag("UniStorm");

		if (!ReferenceEquals (unistorm,null))	
		{	
			UniStormWeatherSystem_C[] weathers = unistorm.GetComponentsInChildren<UniStormWeatherSystem_C>();	
			UniStormWeatherSystem_C weather = weathers[0];
			
			if (!ReferenceEquals (weather,null))	
			{	
				weather.rain = rain;	
				weather.snow = snow;	
				weather.butterflies = lightningBugs;		
				weather.rainMist = rainMist;
				weather.rainSplashes = rainSplash;
				weather.lightningSpawn = lightningPosition;	
				weather.snowMistFog = snowDust;		
				weather.mistFog = rainStreaks;		
				weather.windyLeaves = windyLeaves;		
				weather.lightningBolt1 = lightningBolt1;	
				weather.cameraObject = unistormCamera;	
			}
		}	
	}
}