import System.Collections.Generic;
import UnityStandardAssets.ImageEffects;


	var unistorm : GameObject;
	var unistormCamera : GameObject;
	var rain : ParticleSystem;
	var snow : ParticleSystem;
	var lightningBugs : ParticleSystem;
	var rainMist : ParticleSystem;
	var snowDust : ParticleSystem;
	var rainStreaks : GameObject;
	var windyLeaves : ParticleSystem;
	var lightningBolt1 : GameObject;
	var rainSplash : ParticleSystem;
	var lightningPosition : Transform;
	var weathers : Object[];
	
	function Awake ()	
	{
		unistorm = GameObject.FindGameObjectWithTag("UniStorm");

		if (!ReferenceEquals (unistorm,null))	
		{
			weathers = unistorm.GetComponentsInChildren(UniStormWeatherSystem_JS);	
			//weather = weathers[0];
			
			/*if (!ReferenceEquals (weather,null))	
			{	
				/*weather.rain = rain;	
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
			}*/
		}	
	}