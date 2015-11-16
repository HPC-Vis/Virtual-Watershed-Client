//UniStorm Weather System C# Version 1.8.5 @ Copyright
//Black Horizon Studios

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class UniStormWeatherSystem_C : MonoBehaviour {

	//Gets all our components on start and stores them here
	Renderer starSphereComponent;
	Renderer heavyCloudsComponent;
	Renderer moonObjectComponent;

	//Dyanmic Clouds
	Renderer lightClouds1Component;
	Renderer lightClouds1aComponent;
	Renderer heavyCloudsLayerLightComponent;
	Renderer partlyCloudyClouds1Component;
	Renderer partlyCloudyClouds2Component;
	Renderer mostlyCloudyClouds1Component;
	Renderer mostlyCloudyClouds2Component;

	//Non-Dynamic Clouds
	Renderer heavyCloudsLayer1Component;
	Renderer lightClouds2Component;
	Renderer lightClouds3Component;
	Renderer lightClouds4Component;
	Renderer lightClouds5Component;
	Renderer highClouds1Component;
	Renderer highClouds2Component;
	Renderer mostlyCloudyCloudsComponent;

	Light sunComponent;
	Light moonComponent;
	Light lightSourceComponent;

	AudioSource soundComponent;
	AudioSource rainSoundComponent;
	AudioSource windSoundComponent;
	AudioSource windSnowSoundComponent;

	ParticleEmitter mistFogComponent;

	Camera cameraObjectComponent;

	public bool useInstantStartingWeather;

	public float sunCalculator;

	public int environmentType;

	public float realStartTime;
	public int realStartTimeMinutes;
	public float realStartTimeMinutesFloat;

	public float sunHeight = 0.95f;
	public float mostlyCloudyFader;
	public float clearFader;
	public float colorFader;
	public float sunPosition = 90;

	public Color skyColorMorning; 
	public Color skyColorDay; 
	public Color skyColorEvening; 
	public Color skyColorNight;

	public string weatherString;
	public bool menuEnabled = false;
	bool moonChanger = false;

	public GameObject menuObject;

	//Added 1.8.4
	public float stormyMoonLightIntensity;

	public bool timeOptions = true;
	public bool caledarOptions = true;
	public bool skyOptions = true;
	public bool atmosphereOptions = true;
	public bool fogOptions = true;
	public bool lightningOptions = true;
	public bool temperatureOptions = true;
	public bool sunOptions = true;
	public bool moonOptions = true;
	public bool precipitationOptions = true;
	public bool GUIOptions = true;
	public bool soundManagerOptions = true;
	public bool colorOptions = true;
	public bool objectOptions = true;
	public bool helpOptions = true;
	public bool WindOptions = true;

	public bool terrainDetection = false;

	public float nightLength;
	public int nightLengthHour;
	public int dayLengthHour;
	public int nightLengthMinute;
	public int dayLengthMinute;

	public float stormGrassWavingSpeed;
	public float stormGrassWavingAmount;
	public float stormGrassWavingStrength;
	public float normalGrassWavingSpeed;
	public float normalGrassWavingAmount;
	public float normalGrassWavingStrength;

	public float HeavyRainSunIntensity;

	public string timeOfDayDisplay;
	public GameObject sunObject;
	
	public int calendarType = 0;
	public int numberOfDaysInMonth = 31;
	public int numberOfMonthsInYear = 12;
	
	public GameObject partlyCloudyClouds1;
	public GameObject partlyCloudyClouds2;

	public GameObject mostlyCloudyClouds1;
	public GameObject mostlyCloudyClouds2;

	public float partlyCloudyFader;

	public int cloudType = 1;
	public bool UseRainSplashes = true;
	public bool UseRainMist = true;
	public ParticleSystem rainSplashes;
	public ParticleSystem snow; 
	public ParticleSystem snowMistFog; 
	public float sunOffSetX;
	public float sunOffSetY;
	public Vector2 sunOffSet = new Vector2( 0.0f, 0.0f );

	public float dynamicPartlyCloudyFloat1;
	public float dynamicPartlyCloudyFloat2;
	
	public GameObject dynamicPartlyCloudy1;
	public GameObject dynamicPartlyCloudy2;
	
	public int springTemp = 0;
	public int summerTemp = 0;
	public int fallTemp = 0;
	public int winterTemp = 0;
	
	public bool weatherShuffled = false;
	
	public float minHeavyRainMistIntensity = 0;
	public int maxHeavyRainMistIntensity = 20;
	
	public ParticleSystem rainMist;
	
	public int moonSize = 4;
	public float moonRotationY = 4;
	public bool customMoonSize = false;
	public bool customMoonRotation = false;
	public Quaternion moonRotation = Quaternion.identity;
	
	public Color cloudColorMorning; 
	public Color cloudColorDay; 
	public Color cloudColorEvening; 
	public Color cloudColorNight;
	
	public Color stormyFogColorMorning; 
	public Color stormyFogColorDay; 
	public Color stormyFogColorEvening; 
	public Color stormyFogColorNight; 
	
	public Color originalFogColorMorning; 
	public Color originalFogColorDay; 
	public Color originalFogColorEvening; 
	public Color originalFogColorNight;
	
	public float fader = 0;
	public float fader2 = 0;
	
	public bool weatherHappened = false;
	
	public float moonFade = 0;
	public float moonFade2 = 0;
	
	public Color moonColorFade;
	
	public int temperatureType = 1;
	public int temperature_Celsius = 1;
	
	public bool stormControl = true;
	
	public int forceWeatherChange = 0;

	public int randomizedRainIntensity;
	public int currentGeneratedIntensity;
	public int lastWeatherType;
	public bool randomizedPrecipitation = false;
	public int moonShadowQuality = 2;
	
	public bool useSunFlare = false;
	public bool useRainStreaks = true;
	public Color sunFlareColor;
	
	public int timeToWaitMin;
	public int timeToWaitMax;
	public int timeToWaitCurrent = 3;
	
	public float TODSoundsTimer;
	public bool playedSound = false;
	public bool getDelay = false;
	public int amountOfSounds;
	public bool useMorningSounds = false;
	public bool useDaySounds = false;
	public bool useEveningSounds = false;
	public bool useNightSounds = false;
	
	public Material tempMat;
	public Color lightningColor;
	
	public int morningSize;
	public int daySize;
	public int eveningSize;
	public int nightSize;
	public List<AudioClip> ambientSoundsMorning = new List<AudioClip>();
	public List<AudioClip> ambientSoundsDay = new List<AudioClip>();
	public List<AudioClip> ambientSoundsEvening = new List<AudioClip>();
	public List<AudioClip> ambientSoundsNight = new List<AudioClip>();
	public List<bool> foldOutList = new List<bool>();
	
	public bool shadowsDuringDay = true;
	public float dayShadowIntensity;
	public int dayShadowType = 1;
	
	public bool shadowsDuringNight;
	public float nightShadowIntensity;
	public int nightShadowType = 1;
	
	public bool shadowsLightning;
	public float lightningShadowIntensity;
	public int lightningShadowType = 1;
	
	public int fogMode = 1;
	
	public string version;
	
	public int materialIndex = 0;
	
	public Vector2 uvAnimationRateA = new Vector2( 1.0f, 0.0f );
	public string CloudA = "_MainTex1";
	
	public Vector2 uvAnimationRateB = new Vector2( 1.0f, 0.0f );
	public string CloudB = "_MainTex2";
	
	public Vector2 uvAnimationRateC = new Vector2( 1.0f, 0.0f );
	public string CloudC = "_MainTex3";
	
	public Vector2 uvAnimationRateHeavyA = new Vector2( 1.0f, 0.0f );
	public Vector2 uvAnimationRateHeavyB = new Vector2( 1.0f, 0.0f );
	public Vector2 uvAnimationRateHeavyC = new Vector2( 1.0f, 0.0f );
	
	Vector2 uvOffsetA = Vector2.zero;
	Vector2 uvOffsetB = Vector2.zero;
	Vector2 uvOffsetC = Vector2.zero;
	
	Vector2 uvOffsetHeavyA = Vector2.zero;
	Vector2 uvOffsetHeavyB = Vector2.zero;
	Vector2 uvOffsetHeavyC = Vector2.zero;
	
	public bool scale = false;
	
	public float scaleX;
	public float scaleY;	
	
	public int cloudDensity;
	
	public AudioClip customRainSound;
	public AudioClip customRainWindSound;
	public AudioClip customSnowWindSound;
	
	public bool useCustomPrecipitationSounds = false;
	
	public bool useUnityFog = true;
	
	public float moonLightIntensity;
	
	public Light moonLight;
	public Color moonColor;

	//Time keeping variables
	public int minuteCounter = 0; 
	public int minuteCounterNew = 0;
	public string minute;
	public int hourCounter = 0; 
	public int hourCounter2 = 0;
	public int dayCounter = 0; 
	public float monthCounter = 0; 
	public float yearCounter = 0; 
	public int temperature = 0; 
	public float dayLength;
	public int cloudSpeed;
	public int heavyCloudSpeed;
	public int starSpeed;
	public float starSpeedCalculator;
	public float starRotationSpeed;
	public bool timeStopped = false;
	public bool staticWeather = false;
	public bool timeScrollBar = false;
	public bool horizonToggle  = true;
	public bool dynamicSnowEnabled = true;
	public bool weatherCommandPromptUseable = false;
	public bool timeScrollBarUseable = false;
	public float startTime;
	public int startTimeNumber;
	public float moonPhaseChangeTime;
	public int weatherForecaster = 0;
	private string stringToEdit = "0";
	
	public float SnowAmount;
	
	//Sun intensity control
	public float sunIntensity;
	public float maxSunIntensity; 
	
	//Sun angle control
	public float sunAngle;
	
	//Ambient light colors
	public Color MorningAmbientLight;
	public Color MiddayAmbientLight;
	public Color DuskAmbientLight;
	public Color NightAmbientLight;
	
	//Sun colors
	public Color SunMorning;
	public Color SunDay;
	public Color SunDusk;
	public Color SunNight;
	
	//Storm color variables Global Fog
	public Color stormyFogColorDay_GF;
	public Color stormyFogColorDuskDawn_GF;
	public Color stormyFogColorNight_GF;
	
	//Fog colors
	public Color fogMorningColor;
	public Color fogDayColor;
	public Color fogDuskColor;
	public Color fogNightColor;
	
	public float fogDensity;
	
	//Skyboxes
	public Material SkyBoxMaterial;
	
	//Atmospheric colors
	public Color MorningAtmosphericLight;
	public Color MiddayAtmosphericLight;
	public Color DuskAtmosphericLight;
	
	//Star System game objects
	public GameObject starSphere;
	public Color starBrightness;
	public GameObject moonObject;
	
	public int moonPhaseCalculator;
	public Color moonFadeColor;
	public Material moonPhase1;
	public Material moonPhase2;
	public Material moonPhase3;
	public Material moonPhase4;
	public Material moonPhase5;
	public Material moonPhase6;
	public Material moonPhase7;
	public Material moonPhase8;
	public Material moonPhase9;
	
	//Clouds game objects
	public GameObject lightClouds1;
	public GameObject lightClouds1a;
	public GameObject lightClouds2;
	public GameObject lightClouds3;
	public GameObject lightClouds4;
	public GameObject lightClouds5;
	public GameObject highClouds1;
	public GameObject highClouds2;
	public GameObject mostlyCloudyClouds;
	public GameObject heavyClouds;
	public GameObject heavyCloudsLayer1;
	public GameObject heavyCloudsLayerLight;
	
	//Max rain particles
	public int maxLightRainIntensity = 400;
	public int maxLightRainMistCloudsIntensity = 4;
	public int maxStormRainIntensity = 1000;
	public int maxStormMistCloudsIntensity = 15;
	
	public int maxLightSnowIntensity = 400;
	public int maxLightSnowDustIntensity = 4;
	public int maxSnowStormIntensity = 1000;
	public int maxHeavySnowDustIntensity = 15;
	
	//Weather game objects
	public ParticleSystem rain;
	public ParticleSystem butterflies;
	public GameObject mistFog;
	public ParticleSystem windyLeaves;
	public GameObject windZone;
	public GameObject snowObject;
	
	public Light sun;
	public Light moon;
	
	//Storm sound effects
	public GameObject rainSound;
	public GameObject windSound;
	public GameObject windSnowSound;
	//public Color nightSound : GameObject;
	
	public GameObject cameraObject;
	
	public float random;
	public float random2;
	
	//Our fade number values
	private float fadeHorizonController = 0;
	private float stormClouds = 0;
	private float topStormCloudFade = 0;
	private float fade2 = 0;
	private float butterfliesFade = 0;
	private float windyLeavesFade = 0;
	private float fadeStormClouds  = 0;
	private float fadeStars = 0;
	private float time = 0;
	private float sunShaftFade = 0;
	private float windControl = 0;
	private float windControlUp = 0;

	private float dynamicSnowFade = 0;
	private bool overrideFog = false;

	public float stormCounter = 0.0f;
	private float forceStorm = 0;  
	private float changeWeather = 0;  
	
	//1.6 weather commands
	private string foggy = "01";
	private string lightRain_lightSnow = "02";
	private string rainStorm_snowStorm = "03";
	private string partlyCloudy1 = "04";
	private string partlyCloudy2 = "05";
	private string partlyCloudy3 = "06";
	private string clear1 = "07";
	private string clear2 = "08";
	private string cloudy = "09";
	private string mostlyCloudy = "001";
	private string heavyRain = "002";
	private string fallLeaves = "003";
	private string butterfliesSummer = "004";
	private bool commandPromptActive = false;
	
	//Rain particle density controls
	public float minRainIntensity;
	private float minFogIntensity;
	
	//Snow particle density controls
	public float minSnowIntensity;
	public float minSnowFogIntensity;
	
	//Priavte vars
	private float calculate2;
	public float Hour;
	public float minuteCounterCalculator = 0; 
	private float cloudSpeedY;
	private float cloudSpeedHighY;
	public SunShafts sunShaftScript;
	public Color globalFogColor;

	//Global Fog is still used via the camera, 
	//but isn't alter through script. This is due to 
	//Unity making the Global Fog function hidden
	//which casues a fog error issue. So, we have 
	//removed it until they make it public by default.
	//public GlobalFog fogScript; 
	
	public int weatherOdds = 0;
	public int weatherChanceSpring = 0;
	public bool isSpring;
	public int weatherChanceSummer = 0;
	public bool isSummer;
	public int weatherChanceFall = 0;
	public bool isFall;
	public int weatherChanceWinter = 0;
	public bool isWinter;
	public int weatherUpdate = 0;
	public bool weatherUpdateActive;
	public bool nightTime;
	
	public int stormyFogDistance;
	public int stormyFogDistanceStart;
	
	public int fogStartDistance;
	public int fogEndDistance;
	
	public Transform lightningSpawn;
	public int lightningNumber;
	
	public GameObject lightningBolt1;
	
	public bool fadeLightning;
	public bool lightingGenerated;
	
	public Light lightSource;
	
	public AudioClip thunderSound1;
	public AudioClip thunderSound2;
	public AudioClip thunderSound3;
	public AudioClip thunderSound4;
	public AudioClip thunderSound5;
	
	//Temperature variables
	public int minSpringTemp;
	public int maxSpringTemp;
	public int minSummerTemp;
	public int maxSummerTemp;
	public int minFallTemp;
	public int maxFallTemp;
	public int minWinterTemp;
	public int maxWinterTemp;
	public int startingSpringTemp;
	public int startingSummerTemp;
	public int startingFallTemp;
	public int startingWinterTemp;
	public bool loadSpringTemp;
	public bool loadSummerTemp;
	public bool loadFallTemp;
	public bool loadWinterTemp;
	
	//Weather Timers	
	private float timer;
	public float onTimer;
	public float lightningOdds;
	public float lightningFlashLength;
	public int lightningMinChance = 5;
	public int lightningMaxChance = 10;
	
	public float minIntensity;
	public float maxIntensity;
	public float lightningIntensity;
	
	public Color MorningGradientLight;
	public Color DayGradientLight;
	public Color DuskGradientLight;
	public Color NightGradientLight;
	
	//Gradient Light Colors
	public Color MorningGradientContrastLight;
	public Color DayGradientContrastLight;
	public Color DuskGradientContrastLight;
	public Color NightGradientContrastLight;
	
	public bool isFalse = false;
	public float timerNew;
	
	public bool hour1 = false;
	public bool hour2 = false;
	public bool hour3 = false;
	public bool hour4 = false;
	public bool hour5 = false;
	public bool hour6 = false;
	public bool hour7 = false;
	public bool hour8 = false;
	public bool hour9 = false;	
	public bool hour10 = false;
	public bool hour11 = false;
	public bool hour12 = false;
	public bool hour13 = false;
	public bool hour14 = false;
	public bool hour15 = false;
	public bool hour16 = false;
	public bool hour17 = false;
	public bool hour18 = false;
	public bool hour19 = false;
	public bool hour20 = false;
	public bool hour21 = false;
	public bool hour22 = false;
	public bool hour23 = false;
	public bool hour0 = false;
	
	public float sunSize = 0.02f;
	public Color skyTintColor;
	public Color groundColor;
	public float atmosphereThickness;
	public float exposure;
	
	void Awake () 
	{
		if (useCustomPrecipitationSounds)
		{
			rainSound.GetComponent<AudioSource>().clip = customRainSound;
			rainSound.GetComponent<AudioSource>().enabled = false;
			
			windSound.GetComponent<AudioSource>().clip = customRainWindSound;
			windSound.GetComponent<AudioSource>().enabled = false;
			
			windSnowSound.GetComponent<AudioSource>().clip = customSnowWindSound;
			windSnowSound.GetComponent<AudioSource>().enabled = false;
		}	
	}
	
	void Start ()
	{
		if (useCustomPrecipitationSounds)
		{
			rainSound.GetComponent<AudioSource>().enabled = true;
			windSound.GetComponent<AudioSource>().enabled = true;
			windSnowSound.GetComponent<AudioSource>().enabled = true;
		}

		GetAllComponents();

		RenderSettings.fog = true;
		RenderSettings.skybox = SkyBoxMaterial;
		SkyBoxMaterial.SetColor("_SkyTint", skyTintColor);
		SkyBoxMaterial.SetColor("_GroundColor", groundColor);
		SkyBoxMaterial.SetFloat("_AtmosphereThickness", atmosphereThickness);
		SkyBoxMaterial.SetFloat("_Exposure", exposure);
		SkyBoxMaterial.SetFloat("_SunSize", 0);
		useSunFlare = true;

		realStartTimeMinutesFloat = (int)realStartTimeMinutes;

		//Calculates our start time based off the user's input
		startTime = realStartTime / 24 + realStartTimeMinutesFloat / 1440;

		if (Terrain.activeTerrain == null)
		{	
			terrainDetection = false;
		}
		
		if (Terrain.activeTerrain != null)
		{	
			terrainDetection = true;
		}

		if (terrainDetection)
		{
			if (Terrain.activeTerrain.terrainData.wavingGrassSpeed <= normalGrassWavingSpeed || Terrain.activeTerrain.terrainData.wavingGrassSpeed >= normalGrassWavingSpeed)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
			}
			
			if (Terrain.activeTerrain.terrainData.wavingGrassAmount <= normalGrassWavingStrength || Terrain.activeTerrain.terrainData.wavingGrassAmount >= normalGrassWavingStrength)
			{
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
			}
			
			if (Terrain.activeTerrain.terrainData.wavingGrassStrength <= normalGrassWavingAmount || Terrain.activeTerrain.terrainData.wavingGrassStrength >= normalGrassWavingAmount)
			{
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}
		
		
		if (cloudType == 1 && weatherForecaster == 4 || cloudType == 1 && weatherForecaster == 5 || cloudType == 1 && weatherForecaster == 6)
		{
			partlyCloudyFader = 1;
			colorFader = 1;
			mostlyCloudyFader = 0;
		}

		if (cloudType == 1 && weatherForecaster == 7)
		{
			partlyCloudyFader = 0;
			colorFader = 1;
			mostlyCloudyFader = 0;
		}

		if (cloudType == 1 && weatherForecaster == 8)
		{
			partlyCloudyFader = 0;
			colorFader = 0;
			mostlyCloudyFader = 0;
		}

		if (cloudType == 1 && weatherForecaster == 11)
		{
			partlyCloudyFader = 1;
			colorFader = 1;
			mostlyCloudyFader = 1;
		}

		
		weatherUpdate = 0;
		timeScrollBar = false;
		fader2 = 1;
		
		LogErrorCheck ();

		if (UseRainSplashes)
		{
			rainSplashes.gameObject.SetActive(true);
		}
		
		if (!UseRainSplashes)
		{
			rainSplashes.gameObject.SetActive(false);
		}

		originalFogColorMorning = fogMorningColor;
		originalFogColorDay = fogDayColor;
		originalFogColorEvening = fogDuskColor;
		originalFogColorNight = fogNightColor;

		if (customMoonSize)
		{
			moonObject.transform.localScale = new Vector3(moonSize, moonSize, moonSize);
		}

		//Added 1.8.5
		if (customMoonRotation)
		{
			moonObject.transform.localEulerAngles = new Vector3(0, moonRotationY, 0);
		}

		if (useUnityFog)
		{
			RenderSettings.fog = true;
		}
		
		if (fogMode == 1)
		{
			RenderSettings.fogMode = FogMode.Linear;
		}
		
		if (fogMode == 2)
		{
			RenderSettings.fogMode = FogMode.Exponential;
		}
		
		if (fogMode == 3)
		{
			RenderSettings.fogMode = FogMode.ExponentialSquared;
		}
		
		sunIntensity = maxSunIntensity;
		
		lightSource.color = lightningColor;
		
		//If user chooses to use standard clouds
		if (cloudType == 2)
		{
			//Fixed obsolete error
			heavyCloudsLayerLight.SetActive(false);
			heavyCloudsLayer1.SetActive(true);
			lightClouds1.SetActive(false);
			lightClouds1a.SetActive(false);
			lightClouds2.SetActive(true);
			lightClouds3.SetActive(true);
			lightClouds4.SetActive(true);
			lightClouds5.SetActive(true);
			highClouds1.SetActive(true);
			highClouds2.SetActive(true);
			mostlyCloudyClouds.SetActive(true);
		}
		
		//If user chooses to use dynamic clouds
		if (cloudType == 1)
		{
			//Updated 1.8.1
			lightClouds1.SetActive(true);
			heavyCloudsLayerLight.SetActive(true);
			heavyCloudsLayer1.SetActive(false);
			lightClouds2.SetActive(false);
			lightClouds3.SetActive(false);
			lightClouds4.SetActive(false);
			lightClouds5.SetActive(false);
			highClouds1.SetActive(false);
			highClouds2.SetActive(false);
			mostlyCloudyClouds.SetActive(false);

			partlyCloudyClouds1.SetActive(true);
			
			if (cloudDensity == 1)
			{
				//Warning Obsolete Message Update
				lightClouds1a.SetActive(false);
				partlyCloudyClouds2.SetActive(false);
			}
			
			if (cloudDensity == 2)
			{
				//Warning Obsolete Message Update
				lightClouds1a.SetActive(true);
				partlyCloudyClouds2.SetActive(true);
			}
		}
		
		//Set dynamic cloud values
		uvAnimationRateA = new Vector2(0.001f, 0.0f);
		uvAnimationRateB = new Vector2(0.001f, 0.001f);
		uvAnimationRateC = new Vector2(0.0001f, 0.0f);
		
		uvAnimationRateHeavyA = new Vector2(0.005f, 0.001f);
		uvAnimationRateHeavyB = new Vector2(0.004f, 0.0035f);
		uvAnimationRateHeavyC = new Vector2(0.0001f, 0.0f);

		if (useSunFlare)
		{	
			sunObject = GameObject.Find("SunGlow");
			sunObject.transform.localScale = new Vector3(sunSize, sunSize, sunSize);
			sunObject.SetActive(true);
		}
		
		if (useRainStreaks)
		{
			mistFog.SetActive(true);
		}
		
		if (!useRainStreaks)
		{
			mistFog.SetActive(false);
		}

		if (RenderSettings.fogMode == FogMode.Linear)
		{
			RenderSettings.fogStartDistance = fogStartDistance;
			RenderSettings.fogEndDistance = fogEndDistance;
		}

		moonLight = moon.GetComponent<Light>();
		moonLight.intensity = moonLightIntensity;

		if (useInstantStartingWeather)
		{
			InstantWeather();
		}

	}

	//Gets all our needed components on Start
	void GetAllComponents ()
	{
		//Renderer Components
		heavyCloudsLayer1Component = heavyCloudsLayer1.GetComponent<Renderer>();
		lightClouds2Component = lightClouds2.GetComponent<Renderer>();
		lightClouds3Component = lightClouds3.GetComponent<Renderer>();
		lightClouds4Component = lightClouds4.GetComponent<Renderer>();
		lightClouds5Component = lightClouds5.GetComponent<Renderer>();
		highClouds1Component = highClouds1.GetComponent<Renderer>();
		highClouds2Component = highClouds2.GetComponent<Renderer>();
		mostlyCloudyCloudsComponent = mostlyCloudyClouds.GetComponent<Renderer>();

		heavyCloudsComponent = heavyClouds.GetComponent<Renderer>();; 
		starSphereComponent = starSphere.GetComponent<Renderer>();
		lightClouds1Component = lightClouds1.GetComponent<Renderer>();
		lightClouds1aComponent = lightClouds1a.GetComponent<Renderer>();
		heavyCloudsLayerLightComponent = heavyCloudsLayerLight.GetComponent<Renderer>();
		partlyCloudyClouds1Component = partlyCloudyClouds1.GetComponent<Renderer>();
		partlyCloudyClouds2Component = partlyCloudyClouds2.GetComponent<Renderer>();
		mostlyCloudyClouds1Component = mostlyCloudyClouds1.GetComponent<Renderer>();
		mostlyCloudyClouds2Component = mostlyCloudyClouds2.GetComponent<Renderer>();

		moonObjectComponent = moonObject.GetComponent<Renderer>();

		//Light Component
		sunComponent = sun.GetComponent<Light>();
		lightSourceComponent = lightSource.GetComponent<Light>();
		moonComponent = moon.GetComponent<Light>();

		//Sound Components
		soundComponent = GetComponent<AudioSource>();
		rainSoundComponent = rainSound.GetComponent<AudioSource>();
		windSoundComponent = windSound.GetComponent<AudioSource>();
		windSnowSoundComponent = windSnowSound.GetComponent<AudioSource>();

		//Particle Components
		mistFogComponent = mistFog.GetComponent<ParticleEmitter>();

		//Other
		cameraObjectComponent = cameraObject.GetComponent<Camera>();

		//fogScript = cameraObjectComponent.GetComponent<GlobalFog>();
		sunShaftScript = cameraObjectComponent.GetComponent<SunShafts>();

		/*
		if (fogScript == null)
		{
			Debug.LogError("Please apply a C# Global Fog Script to your camera GameObject.");
		}
		*/
		
		if (sunShaftScript == null)
		{
			Debug.LogError("Please apply a C# Sun Shaft Script to your camera GameObject.");
		}
	}

	void Update () 
	{
		if (weatherForecaster == 2 && useRainStreaks)
		{
			if (minFogIntensity <= 0)
			{
				mistFog.gameObject.SetActive(false);
			}
		}

		if (weatherForecaster == 3 && useRainStreaks || weatherForecaster == 12 && useRainStreaks)
		{
			if (minFogIntensity >= 1)
			{
				mistFog.gameObject.SetActive(true);
			}
		}

		if (weatherForecaster == 2 && UseRainMist)
		{
			if (minHeavyRainMistIntensity <= 0)
			{
				rainMist.gameObject.SetActive(false);
			}
		}

		if (weatherForecaster == 3 && UseRainMist || weatherForecaster == 12 && UseRainMist)
		{
			if (minHeavyRainMistIntensity >= 1)
			{
				rainMist.gameObject.SetActive(true);
			}
		}

		if (minuteCounter <= 5)
		{
			weatherShuffled = false;
		}
		
		//Rewrote weather generator
		//Weather now generates properly according to season
		//Weather will also not longer get stuck generating endlessly when using the slider
		//This allows for reliable consistant weather generation
		
		if (minuteCounter > 58 && !weatherShuffled)
		{
			weatherShuffled = true;
			
			if (weatherUpdate == 1 && isSpring == true)
			{	
				
				//80% chance for percipitation			
				if (weatherChanceSpring == 80)
				{
					weatherOdds = Random.Range (80,100);
				}
				
				//60% chance for percipitation			
				if (weatherChanceSpring == 60)
				{
					weatherOdds = Random.Range (60,100);
				}
				
				//40% chance for percipitation			
				if (weatherChanceSpring == 40)
				{
					weatherOdds = Random.Range (40,100);
				}	
				
				//20% chance for percipitation			
				if (weatherChanceSpring == 20)
				{
					weatherOdds = Random.Range (20,100);
				}
			}
			
			//Summer Weather Odds
			if (weatherUpdate == 1 && isSummer == true)
			{	
				
				//80% chance for percipitation			
				if (weatherChanceSummer == 80)
				{
					weatherOdds = Random.Range (80,100);
				}
				
				//60% chance for percipitation			
				if (weatherChanceSummer == 60)
				{
					weatherOdds = Random.Range (60,100);
				}
				
				//40% chance for percipitation			
				if (weatherChanceSummer == 40)
				{
					weatherOdds = Random.Range (40,100);
				}	
				
				//20% chance for percipitation			
				if (weatherChanceSummer == 20)
				{
					weatherOdds = Random.Range (20,100);
				}
			}
			
			//Fall Weather Odds
			if (weatherUpdate == 1 && isFall == true)
			{	
				
				//80% chance for percipitation			
				if (weatherChanceFall == 80)
				{
					weatherOdds = Random.Range (80,100);
				}
				
				//60% chance for percipitation			
				if (weatherChanceFall == 60)
				{
					weatherOdds = Random.Range (60,100);
				}
				
				//40% chance for percipitation			
				if (weatherChanceFall == 40)
				{
					weatherOdds = Random.Range (40,100);
				}	
				
				//20% chance for percipitation			
				if (weatherChanceFall == 20)
				{
					weatherOdds = Random.Range (20,100);
				}
			}
			
			//Winter Weather Odds
			if (weatherUpdate == 1 && isWinter == true)
			{	
				
				//80% chance for percipitation			
				if (weatherChanceWinter == 80)
				{
					weatherOdds = Random.Range (80,100);
				}
				
				//60% chance for percipitation			
				if (weatherChanceWinter == 60)
				{
					weatherOdds = Random.Range (60,100);
				}
				
				//40% chance for percipitation			
				if (weatherChanceWinter == 40)
				{
					weatherOdds = Random.Range (40,100);
				}	
				
				//20% chance for percipitation			
				if (weatherChanceWinter == 20)
				{
					weatherOdds = Random.Range (20,100);
				}
			}
		}
		
		hourCounter = (int)Hour;

		stormCounter += Time.deltaTime * .5f;
		
		minuteCounterCalculator = Hour*60f;	
		minuteCounter = (int)minuteCounterCalculator;
		minuteCounterNew = minuteCounter;
		
		float cloudScrollSpeedCalculator = cloudSpeed * .001f;
		float heavCloudScrollSpeedCalculator = heavyCloudSpeed * .001f;
		
		starSpeedCalculator += starSpeed * 0.1f;

		cloudSpeedY = .003f;
		float starSpeedY5 = starSpeed * 0.001f;	
		
		float offsetY5a = Time.time * cloudScrollSpeedCalculator;
		float offsetY5b = Time.time * heavCloudScrollSpeedCalculator;
		
		float offsetY5 = Time.time * cloudSpeedY;
		float offsetYHigh5 = Time.time * cloudScrollSpeedCalculator;
		float offsetX5 = Time.time * starSpeedY5; 
		
		float starRotationSpeedCalc = Time.time * starRotationSpeed * 0.005f;

		if (cloudType == 2)
		{
			heavyCloudsLayer1Component.sharedMaterial.mainTextureOffset = new Vector2 (offsetY5b,0); 
			lightClouds2Component.sharedMaterial.mainTextureOffset = new Vector2 (offsetY5a,offsetY5a);
			lightClouds3Component.sharedMaterial.mainTextureOffset = new Vector2 (0,offsetY5a);
			lightClouds4Component.sharedMaterial.mainTextureOffset = new Vector2 (offsetY5a,offsetY5a);
			lightClouds5Component.sharedMaterial.mainTextureOffset = new Vector2 (0,offsetY5a);
			highClouds1Component.sharedMaterial.mainTextureOffset = new Vector2 (0,offsetYHigh5);
			highClouds2Component.sharedMaterial.mainTextureOffset = new Vector2 (0,offsetYHigh5);
			mostlyCloudyCloudsComponent.sharedMaterial.mainTextureOffset = new Vector2 (0,offsetY5);
		}


		starSphereComponent.sharedMaterial.mainTextureOffset = new Vector2 (offsetX5,0 + .02f);
		heavyCloudsComponent.sharedMaterial.mainTextureOffset = new Vector2 (offsetY5b,offsetY5b); 

		starSphere.transform.eulerAngles = new Vector3(270, starRotationSpeedCalc, 0);
		
		//Controls shadows for both day and night light sources
		if (shadowsDuringDay)
		{
			if (dayShadowType == 1)
			{
				sunComponent.shadows = LightShadows.Hard;
			}
			
			if (dayShadowType == 2)
			{
				sunComponent.shadows = LightShadows.Soft;
			}
			
			sunComponent.shadowStrength = dayShadowIntensity;
		}
		
		if (!shadowsDuringDay)
		{
			sunComponent.shadows = LightShadows.None;
		}
		
		if (shadowsDuringNight)
		{
			if (nightShadowType == 1)
			{
				moonComponent.shadows = LightShadows.Hard;
			}
			
			if (nightShadowType == 2)
			{
				moonComponent.shadows = LightShadows.Soft;
			}
			
			moonComponent.shadowStrength = nightShadowIntensity;
		}

		if (!shadowsDuringNight)
		{
			moonComponent.shadows = LightShadows.None;
		}
		
		if (shadowsLightning)
		{
			if (lightningShadowType == 1)
			{
				lightSourceComponent.shadows = LightShadows.Hard;
			}
			
			if (lightningShadowType == 2)
			{
				lightSourceComponent.shadows = LightShadows.Soft;
			}
			
			lightSourceComponent.shadowStrength = lightningShadowIntensity;
		}
		
		if (!shadowsLightning)
		{
			lightSourceComponent.shadows = LightShadows.None;
		}

		//Calculates our seasons
		if (monthCounter >= 2 && monthCounter <= 4)
		{
			isSpring = true;
			isSummer = false;
			isFall = false;
			isWinter = false;
			WeatherForecaster ();
			
		}
		
		//Calculates our seasons
		if (monthCounter >= 5 && monthCounter <= 7)
		{
			isSummer = true;
			isSpring = false;
			isFall = false;
			isWinter = false;
			WeatherForecaster ();
		}
		
		//Calculates our seasons
		if (monthCounter >= 8 && monthCounter <= 10)
		{
			isSummer = false;
			isSpring = false;
			isFall = true;
			isWinter = false; 
			WeatherForecaster ();
		}
		
		//Calculates our seasons
		if (monthCounter == 11 || monthCounter == 12 || monthCounter == 1)
		{
			isSummer = false;
			isSpring = false;
			isFall = false;
			isWinter = true;
			WeatherForecaster ();
		}
		
		//Controls wether the weather command prompt is enabled or disabled	
		if (weatherCommandPromptUseable == true)
		{
			if(Input.GetKeyDown(KeyCode.F12))
			{
				commandPromptActive = !commandPromptActive;
			}
		} 
		
		if (timeScrollBarUseable == true)
		{
			if(Input.GetKeyDown(KeyCode.F12))
			{
				timeScrollBar = !timeScrollBar;
			}
		} 
		
		if (weatherCommandPromptUseable == false)
		{ 
			commandPromptActive = false;
		}

		if (monthCounter >= 2 && monthCounter <= 4)
		{
			summerTemp = 0;
			winterTemp = 0;
			fallTemp = 0;
			
			if (springTemp == 0)
			{
				springTemp = 1;
			}
		}
		
		if (monthCounter >= 5 && monthCounter <= 7)
		{
			winterTemp = 0;
			fallTemp = 0;
			springTemp = 0;
			
			if (summerTemp == 0)
			{
				summerTemp = 1;
			}
		}
		
		if (monthCounter >= 8 && monthCounter <= 10)
		{
			summerTemp = 0;
			winterTemp = 0;
			springTemp = 0;
			
			if (fallTemp == 0)
			{
				fallTemp = 1;
			}
		}
		
		if (monthCounter == 11 || monthCounter == 12 || monthCounter == 1)
		{
			summerTemp = 0;
			fallTemp = 0;
			springTemp = 0;
			
			if (winterTemp == 0)
			{
				winterTemp = 1;
			}
		}
		
		if (monthCounter >= 2 && monthCounter <= 4)
		{
			if (springTemp == 1)
			{
				temperature = startingSpringTemp;
				springTemp = 2;	
			}
			
			if (temperature <= minSpringTemp && springTemp == 2)
			{
				temperature = minSpringTemp;
			}
			
			if (temperature >= maxSpringTemp && springTemp == 2)
			{
				temperature = maxSpringTemp;
			}
		}
		
		//Generates the temperature for Summer
		if (monthCounter >= 5 && monthCounter <= 7)
		{
			
			if (summerTemp == 1)
			{
				temperature = startingSummerTemp;
				summerTemp = 2;	
			}
			
			if (temperature <= minSummerTemp && summerTemp == 2)
			{
				temperature = minSummerTemp;
			}
			
			if (temperature >= maxSummerTemp && summerTemp == 2)
			{
				temperature = maxSummerTemp;
			}
		}
		
		//Generates the temperature for Fall
		if (monthCounter >= 8 && monthCounter <= 10)
		{
			
			if (fallTemp == 1)
			{
				temperature = startingFallTemp;
				fallTemp = 2;
			}
			
			if (temperature <= minFallTemp && fallTemp == 2)
			{
				temperature = minFallTemp;
			}
			
			if (temperature >= maxFallTemp && fallTemp == 2)
			{
				temperature = maxFallTemp;
			}
		}
		
		//Generates the temperature for Winter
		if (monthCounter >= 11 || monthCounter >= 12 || monthCounter <= 1)
		{
			if (winterTemp == 1)
			{
				temperature = startingWinterTemp;
				winterTemp = 2;
			}
			
			if (temperature <= minWinterTemp && winterTemp == 2)
			{
				temperature = minWinterTemp;
			}
			
			if (temperature >= maxWinterTemp && winterTemp == 2)
			{
				temperature = maxWinterTemp;
			}
		}	
		
		if (monthCounter == -1)
		{
			monthCounter = 11;
		}

		if (Hour >= 20)
		{
			moonComponent.enabled = true;
		}
		
		if (Hour >= 0 && Hour < 4)
		{
			moonComponent.enabled = true;
		}
		
		if (Hour > 4 && Hour < 20)
		{
			moonComponent.enabled = false;
		}
		
		//Calculates our moon phases
		if (moonPhaseCalculator == 1)
		{
			moonObjectComponent.sharedMaterial = moonPhase1;	
		}
		
		if (moonPhaseCalculator == 2)
		{
			moonObjectComponent.sharedMaterial = moonPhase2;	
		}
		
		if (moonPhaseCalculator == 3)
		{
			moonObjectComponent.sharedMaterial = moonPhase3;	
		}
		
		if (moonPhaseCalculator == 4)
		{
			moonObjectComponent.sharedMaterial = moonPhase4;	
		}
		
		if (moonPhaseCalculator == 5)
		{
			moonObjectComponent.sharedMaterial = moonPhase5;	
		}
		
		if (moonPhaseCalculator == 6)
		{
			moonObjectComponent.sharedMaterial = moonPhase6;	
		}
		
		if (moonPhaseCalculator == 7)
		{
			moonObjectComponent.sharedMaterial = moonPhase7;	
		}
		
		if (moonPhaseCalculator == 8)
		{
			moonObjectComponent.sharedMaterial = moonPhase8;	
		}
		
		if (moonPhaseCalculator == 9)
		{
			moonPhaseCalculator = 1;	
		}

		//Rotates our sun using quaternion rotations so the angles don't coincide (sunAngle angles the sun based off the user's input)	
		sun.transform.eulerAngles = new Vector3(startTime * 360 - 90, sunAngle, 180);

		//Fluctuates realistic temperatures
		if (hourCounter == 1 && hour1 == false)
		{
			temperature -= Random.Range (1,3);
			hour1 = true;
		}
		
		if (hourCounter == 2 && hour2 == false)
		{
			temperature -= Random.Range (1,3);
			hour2 = true;
		}
		
		if (hourCounter == 3 && hour3 == false)
		{
			temperature -= Random.Range (1,3);
			hour3 = true;
		}
		
		if (hourCounter == 4 && hour4 == false)
		{
			temperature -= Random.Range (1,3);
			hour4 = true;
		}
		
		if (hourCounter == 5 && hour5 == false)
		{
			temperature -= Random.Range (1,3);
			hour5 = true;
		}
		
		if (hourCounter == 6 && hour6 == false)
		{
			temperature += Random.Range (1,3);
			hour6 = true;
		}
		
		if (hourCounter == 7 && hour7 == false)
		{
			temperature += Random.Range (1,3);
			hour7 = true;
		}
		
		if (hourCounter == 8 && hour8 == false)
		{
			temperature += Random.Range (1,3);
			hour8 = true;
		}
		
		if (hourCounter == 9 && hour9 == false)
		{
			temperature += Random.Range (1,3);
			hour9 = true;
		}
		
		if (hourCounter == 10 && hour10 == false)
		{
			temperature += Random.Range (1,3);
			hour10 = true;
		}
		
		if (hourCounter == 11 && hour11 == false)
		{
			temperature += Random.Range (1,3);
			hour11 = true;
		}
		
		if (hourCounter == 12 && hour12 == false)
		{
			temperature += Random.Range (1,3);
			hour12 = true;
		}
		
		if (hourCounter == 13 && hour13 == false)
		{
			temperature += Random.Range (1,3);
			hour13 = true;
		}
		
		if (hourCounter == 14 && hour14 == false)
		{
			temperature += Random.Range (1,3);
			hour14 = true;
		}
		
		if (hourCounter == 15 && hour15 == false)
		{
			temperature += Random.Range (1,3);
			hour15 = true;
		}
		
		if (hourCounter == 16 && hour16 == false)
		{
			temperature += Random.Range (1,3);
			hour16 = true;
		}
		
		if (hourCounter == 17 && hour17 == false)
		{
			temperature -= Random.Range (1,3);
			hour17 = true;
		}
		
		if (hourCounter == 18 && hour18 == false)
		{
			temperature -= Random.Range (1,3);
			hour18 = true;
		}
		
		if (hourCounter == 19 && hour19 == false)
		{
			temperature -= Random.Range (1,3);
			hour19 = true;
		}
		
		if (hourCounter == 20 && hour20 == false)
		{
			temperature -= Random.Range (1,3);
			hour20 = true;
		}
		
		if (hourCounter == 21 && hour21 == false)
		{
			temperature -= Random.Range (1,3);
			hour21 = true;
		}
		
		if (hourCounter == 22 && hour22 == false)
		{
			temperature -= Random.Range (1,3);
			hour22 = true;
		}
		
		if (hourCounter == 23 && hour23 == false)
		{
			temperature -= Random.Range (1,3);
			hour23 = true;
		}
		
		if (hourCounter == 24 && hour0 == false)
		{
			temperature -= Random.Range (1,3);
			hour0 = true;
		}	

		if (Hour >= 12 && !moonChanger)
		{
			moonPhaseCalculator += 1;
			moonChanger = true;
		}

		if (Hour <= 1)
		{
			moonChanger = false;
		}
		
		//Calculates our minutes into hours
		if(minuteCounter >= 60)
		{	
			minuteCounter = (int)minuteCounterCalculator % 60;
		}
		
		if(minuteCounter == 59)
		{	
			weatherUpdate += 1;
		}
		
		if(minuteCounter == 1)
		{
			weatherUpdate = 0;
		}

		if (calendarType == 1)
		{				
			//Calculates our days into months
			if(dayCounter >= 32 && monthCounter == 1 || dayCounter >= 32 && monthCounter == 3 || dayCounter >= 32 && monthCounter == 5 || dayCounter >= 32 && monthCounter == 7 || dayCounter >= 32 && monthCounter == 8 || dayCounter >= 32 && monthCounter == 10 || dayCounter >= 32 && monthCounter == 12)
			{
				dayCounter = dayCounter % 32;
				dayCounter += 1;
				monthCounter += 1;
			}
			
			if(dayCounter == 31 && monthCounter == 4 || dayCounter == 31 && monthCounter == 6 || dayCounter == 31 && monthCounter == 9 || dayCounter == 31 && monthCounter == 11)
			{
				dayCounter = dayCounter % 31;
				dayCounter += 1;
				monthCounter += 1;
			}
			
			//Calculates Leap Year
			if(dayCounter >= 30 && monthCounter == 2 && (yearCounter % 4 == 0 && yearCounter % 100 != 0) || (yearCounter % 400 == 0))
			{
				dayCounter = dayCounter % 30;
				dayCounter += 1;
				monthCounter += 1;
			}
			
			else if (dayCounter >= 29 && monthCounter == 2 && yearCounter % 4 != 0)
			{
				dayCounter = dayCounter % 29;
				dayCounter += 1;
				monthCounter += 1;
			}
			
			//Calculates our months into years
			if (monthCounter > 12)
			{
				monthCounter = monthCounter % 13;
				yearCounter += 1;
				monthCounter += 1;
			}
		}
		
		if (calendarType == 2)
		{
			//Calculates our custom days into months
			if(dayCounter > numberOfDaysInMonth)
			{
				dayCounter = dayCounter % numberOfDaysInMonth - 1;
				dayCounter += 1;
				monthCounter += 1;
			}
			
			//Calculates our custom months into years
			if (monthCounter > numberOfMonthsInYear)
			{
				monthCounter = monthCounter % numberOfMonthsInYear - 1;
				yearCounter += 1;
				monthCounter += 1;
			}
		}
		
		//If staticWeather is true, the weather will never change
		if (staticWeather == false)
		{			
			//20% Chance of weather change
			if (weatherOdds == 20 && weatherChanceSpring == 20 || weatherOdds == 20 && weatherChanceSummer == 20 || weatherOdds == 20 && weatherChanceFall == 20 || weatherOdds == 20 && weatherChanceWinter == 20)
			{
				//Controls our storms from switching too often
				if (stormCounter >= 13)
				{
					weatherForecaster = Random.Range(1,13);
					weatherOdds = 1;
					stormCounter = Random.Range (0,7);
				}
			}
			
			//40% Chance of weather change
			if (weatherOdds == 40 && weatherChanceSpring == 40 && weatherOdds == 40 && weatherChanceSummer == 40 || weatherOdds == 40 && weatherChanceFall == 40 || weatherOdds == 40 && weatherChanceWinter == 40)
			{
				//Controls our storms from switching too often
				if (stormCounter >= 13)
				{
					weatherForecaster = Random.Range(1,13);
					weatherOdds = 1;
					stormCounter = Random.Range (0,7);
				}
			}
			
			//60% Chance of weather change
			if (weatherOdds == 60 && weatherChanceSpring == 60 && weatherOdds == 60 && weatherChanceSummer == 60 || weatherOdds == 60 && weatherChanceFall == 60 || weatherOdds == 60 && weatherChanceWinter == 60)
			{
				//Controls our storms from switching too often
				if (stormCounter >= 13)
				{
					weatherForecaster = Random.Range(1,13);
					weatherOdds = 1;
					stormCounter = Random.Range (0,7);
				}
			}
			
			//80% Chance of weather change
			if (weatherOdds == 80 && weatherChanceSpring == 80 && weatherOdds == 80 && weatherChanceSummer == 80 || weatherOdds == 80 && weatherChanceFall == 80 || weatherOdds == 80 && weatherChanceWinter == 80)
			{
				//Controls our storms from switching too often
				if (stormCounter >= 13)
				{
					weatherForecaster = Random.Range(1,13);
					weatherOdds = 1;
					stormCounter = Random.Range (0,7);
				}
			}		
		}
		
		//Calculates our day length so it stays consistent	
		Hour = startTime * 24;

		//This adds support for night length
		//If timeStopped is checked, time doesn't flow
		if (timeStopped == false)
		{	
			if (Hour >= dayLengthHour && Hour < nightLengthHour) 
			{
				startTime = startTime + Time.deltaTime/dayLength/60;
			}
			
			if (Hour >= nightLengthHour || Hour < dayLengthHour)
			{
				startTime = startTime + Time.deltaTime/nightLength/60;
			}
		}

		sun.intensity = (calculate2-0.1f) * sunIntensity;
		
		if (sunIntensity <= 0)
		{
			sunIntensity = 0;
			sun.enabled = false;
		}
		
		if (sunIntensity >= .01)
		{
			sun.enabled = true;
		}
		
		if (sunIntensity >= maxSunIntensity)
		{
			sunIntensity = maxSunIntensity;	
			lightSourceComponent.enabled = false;	
		}

		sunCalculator = Hour / 24;

		if(sunCalculator < 0.5)
		{
			calculate2 = sunCalculator;
		}
		if(sunCalculator > 0.5)
		{
			calculate2 = (1-sunCalculator);
		}

		//Added 1.8.5
		if (startTime >= 1.0f)
		{
			startTime = 0;
			CalculateDays();
		}
		
		//Forces precipitation if none has happened in an in-game week, prevents drouts
		if (forceStorm >= 7)
		{
			if (staticWeather == false)
			{	
				weatherForecaster = Random.Range(2,3);
				forceStorm = 0;
			}
		}
		
		//Changes our weather type if there has been precipitation for more than 3 in-game days
		if (changeWeather >= forceWeatherChange && stormControl == true)
		{
			if (staticWeather == false)
			{	
				weatherForecaster = Random.Range(4,11);
				changeWeather = 0;
			}
		}
		
		if (weatherForecaster == 1 || weatherForecaster == 2 || weatherForecaster == 3 || weatherForecaster == 12 || weatherForecaster == 9)
		{
			fader += 0.002f; 
			fader2 -= 0.01f; 
			
			if (fader2 <= 0)
			{
				fader2 = 0;
			}
			
			if (fader >= 1)
			{
				fader = 1;
				weatherHappened = true;
			}
			
			fogMorningColor = Color.Lerp (originalFogColorMorning, stormyFogColorMorning, fader);
			fogDayColor = Color.Lerp (originalFogColorDay, stormyFogColorDay, fader);
			fogDuskColor = Color.Lerp (originalFogColorEvening, stormyFogColorEvening, fader);
			fogNightColor = Color.Lerp (originalFogColorNight, stormyFogColorNight, fader);
		}
	
		if (weatherForecaster == 4 || weatherForecaster == 5 || weatherForecaster == 6 || weatherForecaster == 7 || weatherForecaster == 8)
		{
			fader2 += 0.0005f; 
			fader -= 0.0025f; 
			
			fogMorningColor = Color.Lerp (stormyFogColorMorning, originalFogColorMorning, fader2);
			fogDayColor = Color.Lerp (stormyFogColorDay, originalFogColorDay, fader2);
			fogDuskColor = Color.Lerp (stormyFogColorEvening, originalFogColorEvening, fader2);
			fogNightColor = Color.Lerp (stormyFogColorNight, originalFogColorNight, fader2);
			
			if (fader2 >= 1)
			{
				weatherHappened = false;
				fader2 = 1;
			}
			
			if (fader <= 0)
			{
				fader = 0;
			}
		}

		TimeOfDayCalculator ();
		DynamicCloudFormations();
		DynamicTimeOfDaySounds ();
	}


	void TimeOfDayCalculator ()
	{
		if(Hour > 2 && Hour < 4)
		{
			moon.color = Color.Lerp (moonColor, moonFadeColor, (Hour/2)-1);
		}
		
		//Calculates morning fading in from night
		if(Hour > 4 && Hour < 6)
		{
			RenderSettings.ambientLight = Color.Lerp (NightAmbientLight, MorningAmbientLight, (Hour/2)-2);
			sun.color = Color.Lerp (SunNight, SunMorning, (Hour/2)-2);
			
			RenderSettings.fogColor = Color.Lerp (fogNightColor, fogMorningColor, (Hour/2)-2);
			
			//Added 1.8.5
			SkyBoxMaterial.SetColor("_SkyTint", Color.Lerp (skyColorNight, skyColorMorning, (Hour/2)-2));
			
			lightClouds1Component.material.SetColor("_Color", Color.Lerp (cloudColorNight, cloudColorMorning, (Hour/2)-2) );
			lightClouds1aComponent.material.SetColor("_Color", Color.Lerp (cloudColorNight, cloudColorMorning, (Hour/2)-2) );
			
			moonObjectComponent.sharedMaterial.SetFloat("_FloatMin", (Hour/2)-2);
			moonFade2 = moonObjectComponent.sharedMaterial.GetFloat("_FloatMin") * -50000.0f;
			moonObjectComponent.sharedMaterial.SetFloat("_FogAmountMin", (moonFade2));
			
			//Added 1.8.5
			moonObjectComponent.sharedMaterial.SetColor ("_MoonColor", Color.Lerp (moonColorFade, moonFadeColor, (Hour/2)-2) );

			if (sunShaftScript != null)
			{
				sunShaftScript.sunColor = Color.Lerp (DuskAtmosphericLight, MorningAtmosphericLight, (Hour/2)-2);
			}
			
			moon.color = moonFadeColor;
			
			starSphereComponent.sharedMaterial.SetColor ("_TintColor", Color.Lerp (starBrightness, moonFadeColor, (Hour/2)-2) );
		}
		
		//Calculates Morning
		if(Hour > 6 && Hour < 8)
		{
			RenderSettings.ambientLight = Color.Lerp (MorningAmbientLight, MiddayAmbientLight, (Hour/2)-3);
			sun.color = Color.Lerp (SunMorning, SunDay, (Hour/2)-3);
			starSphereComponent.sharedMaterial.SetColor ("_TintColor",  starBrightness * fadeStars);
			RenderSettings.fogColor = Color.Lerp (fogMorningColor, fogDayColor, (Hour/2)-3);
			
			//Added 1.8.5
			SkyBoxMaterial.SetColor("_SkyTint", Color.Lerp (skyColorMorning, skyColorDay, (Hour/2)-3));
			
			lightClouds1Component.material.SetColor("_Color", Color.Lerp (cloudColorMorning, cloudColorDay, (Hour/2)-3) );
			lightClouds1aComponent.material.SetColor("_Color", Color.Lerp (cloudColorMorning, cloudColorDay, (Hour/2)-3) );

			if (sunShaftScript != null)
			{
				sunShaftScript.sunColor = Color.Lerp (MorningAtmosphericLight, MiddayAtmosphericLight, (Hour/2)-3);
			}
			
			starSphereComponent.sharedMaterial.SetColor ("_TintColor",  Color.black * fadeStars);
			
			fadeStars = 0;
		}
		
		//Calculates Day
		if(Hour > 8 && Hour < 16)
		{
			RenderSettings.ambientLight = Color.Lerp (MiddayAmbientLight, MiddayAmbientLight, (Hour/2)-4);
			sun.color = Color.Lerp (SunDay, SunDay, (Hour/2)-4);
			starSphereComponent.sharedMaterial.SetColor ("_TintColor",  starBrightness * fadeStars);
			RenderSettings.fogColor = Color.Lerp (fogDayColor, fogDayColor, (Hour/2)-4);
			
			//Added 1.8.5
			SkyBoxMaterial.SetColor("_SkyTint", Color.Lerp (skyColorDay, skyColorDay, (Hour/2)-4));
			
			lightClouds1Component.material.SetColor("_Color", Color.Lerp (cloudColorDay, cloudColorDay, (Hour/2)-4) );
			lightClouds1aComponent.material.SetColor("_Color", Color.Lerp (cloudColorDay, cloudColorDay, (Hour/2)-4) );

			if (sunShaftScript != null)
			{
				sunShaftScript.sunColor = Color.Lerp (MiddayAtmosphericLight, MiddayAtmosphericLight, (Hour/2)-4);
			}
			
			starSphereComponent.sharedMaterial.SetColor ("_TintColor",  Color.black * fadeStars);
			
			fadeStars = 0;
			
		}
		
		//Calculates dusk fading in from day
		if(Hour > 16 && Hour < 18)
		{
			RenderSettings.ambientLight = Color.Lerp (MiddayAmbientLight, DuskAmbientLight, (Hour/2)-8);
			sun.color = Color.Lerp (SunDay, SunDusk, (Hour/2)-8);
			RenderSettings.fogColor = Color.Lerp (fogDayColor, fogDuskColor, (Hour/2)-8);
			
			//Added 1.8.5
			SkyBoxMaterial.SetColor("_SkyTint", Color.Lerp (skyColorDay, skyColorEvening, (Hour/2)-8));
			
			lightClouds1Component.material.SetColor("_Color", Color.Lerp (cloudColorDay, cloudColorEvening, (Hour/2)-8) );
			lightClouds1aComponent.material.SetColor("_Color", Color.Lerp (cloudColorDay, cloudColorEvening, (Hour/2)-8) );
			
			//Added 1.8.5
			moonObjectComponent.sharedMaterial.SetColor ("_MoonColor", Color.Lerp (moonFadeColor, moonFadeColor, (Hour/2)-8) );

			if (sunShaftScript != null)
			{
				sunShaftScript.sunColor = Color.Lerp (MiddayAtmosphericLight, DuskAtmosphericLight, (Hour/2)-8);
			}
			
			starSphereComponent.sharedMaterial.SetColor ("_TintColor",  Color.black * fadeStars);
			
			fadeStars = 0; 		
		}
		
		
		
		
		//Calculates night fading in from dusk
		if(Hour > 18 && Hour < 20)
		{
			RenderSettings.ambientLight = Color.Lerp (DuskAmbientLight, NightAmbientLight, (Hour/2)-9);
			sun.color = Color.Lerp (SunDusk, SunNight, (Hour/2)-9);
			RenderSettings.fogColor = Color.Lerp (fogDuskColor, fogNightColor, (Hour/2)-9);
			
			//Added 1.8.5
			SkyBoxMaterial.SetColor("_SkyTint", Color.Lerp (skyColorEvening, skyColorNight, (Hour/2)-9));
			
			lightClouds1Component.material.SetColor("_Color", Color.Lerp (cloudColorEvening, cloudColorNight, (Hour/2)-9) );
			lightClouds1aComponent.material.SetColor("_Color", Color.Lerp (cloudColorEvening, cloudColorNight, (Hour/2)-9) );
			
			//Added 1.8.5
			moonObjectComponent.sharedMaterial.SetColor ("_MoonColor", Color.Lerp (moonFadeColor, starBrightness, (Hour/2)-9) );
			
			moonObjectComponent.sharedMaterial.SetFloat("_FloatMax", (Hour/2)-9);

			if (sunShaftScript != null)
			{
				sunShaftScript.sunColor = Color.Lerp (DuskAtmosphericLight, DuskAtmosphericLight, (Hour/2)-9);
			}
			
			starSphereComponent.sharedMaterial.SetColor ("_TintColor", Color.Lerp (Color.black, starBrightness, (Hour/2)-9) );
			
			if (fadeStars >= 1)
			{
				fadeStars = 1;
			}
			
		}
		
		//Calculates Night
		if(Hour > 20)
		{
			RenderSettings.ambientLight = NightAmbientLight;
			sun.color = Color.Lerp (SunNight, SunNight, (Hour/2)-10);	
			starSphereComponent.sharedMaterial.SetColor ("_TintColor",  starBrightness * fadeStars);
			
			//Added 1.8.5
			lightClouds1Component.material.SetColor("_Color", cloudColorNight);
			lightClouds1aComponent.material.SetColor("_Color", cloudColorNight);
			
			//Added 1.8.5
			moonObjectComponent.sharedMaterial.SetColor ("_MoonColor", Color.Lerp (starBrightness, starBrightness, (Hour/2)-10) );
			
			//Added 1.8.5
			SkyBoxMaterial.SetColor("_SkyTint", Color.Lerp (skyColorNight, skyColorNight, (Hour/2)-10));
			
			moonObjectComponent.sharedMaterial.SetFloat("_FogAmountMin", (0));
			
			moonObjectComponent.sharedMaterial.SetFloat("_FloatMax", (Hour/2)-10);
			moonFade = moonObjectComponent.sharedMaterial.GetFloat("_FloatMax") * 50000.0f;
			moonObjectComponent.sharedMaterial.SetFloat("_FogAmountMax", (moonFade));
			
			
			RenderSettings.fogColor = Color.Lerp (fogNightColor, fogNightColor, (Hour/2)-10);

			moon.color = Color.Lerp (moonFadeColor, moonColor, (Hour/2)-10);
			
			sun.enabled = false;
			
			fadeStars = 1;
		}
		
		if (Hour >= 0 && Hour <= 4)
		{
			lightClouds1Component.material.SetColor("_Color", cloudColorNight);
			lightClouds1aComponent.material.SetColor("_Color", cloudColorNight);
		}
		
		if (Hour >= 0 && Hour <= 2)
		{
			moonLight.color = moonColor;
		}
		

		if(Hour < 4)
		{
			RenderSettings.ambientLight = NightAmbientLight;
			sun.color = Color.Lerp (SunNight, SunNight, (Hour/2)-2);	
			starSphereComponent.sharedMaterial.SetColor ("_TintColor",  starBrightness * fadeStars);
			
			RenderSettings.fogColor = Color.Lerp (fogNightColor, fogNightColor, (Hour/2)-2);
			
			//Added 1.8.5
			lightClouds1Component.material.SetColor("_Color", cloudColorNight);
			lightClouds1aComponent.material.SetColor("_Color", cloudColorNight);
			
			//Added 1.8.5
			SkyBoxMaterial.SetColor("_SkyTint", Color.Lerp (skyColorNight, skyColorNight, (Hour/2)-10));
			
			//Added 1.8.5
			moonObjectComponent.sharedMaterial.SetColor ("_MoonColor", Color.Lerp (starBrightness, starBrightness, (Hour/2)-10) );
			
			moonObjectComponent.sharedMaterial.SetFloat("_FogAmountMin", (0));
			moonObjectComponent.sharedMaterial.SetFloat("_FogAmountMax", (50000));
			
			fadeStars = 1;	
		}
	}


	//Puts all fading in and out weather types into 2 function then picks by weather type to control individual weather factors
	void WeatherForecaster () 
	{	
		//Foggy 
		if (weatherForecaster == 1)
		{
			FadeInPrecipitation();
			weatherString = "Foggy";
		}

		//Light Snow / Light Rain
		if (weatherForecaster == 2)
		{
			FadeInPrecipitation();

			if (temperature >= 33)
			{
				weatherString = "Light Rain";
			}

			if (temperature <= 32)
			{
				weatherString = "Light Snow";
			}
		}

		//Heavy Snow
		if (weatherForecaster == 3)
		{
			FadeInPrecipitation();

			if (temperature >= 33)
			{
				weatherString = "Heavy Rain & Thunder Storm";
			}
			
			if (temperature <= 32)
			{
				weatherString = "Heavy Snow";
			}
		}

		//Partly Cloudy
		if (weatherForecaster == 4)
		{
			FadeOutPrecipitation ();

			weatherString = "Partly Cloudy";
		}

		//Partly Cloudy
		if (weatherForecaster == 5)
		{
			FadeOutPrecipitation ();
			//weatherString = "Partly Cloudy";
			weatherForecaster = 3;
		}
		
		//Partly Cloudy
		if (weatherForecaster == 6)
		{
			FadeOutPrecipitation ();
			weatherForecaster = 7;
			//weatherString = "Partly Cloudy";
		}
		
		//Mostly Clear
		if (weatherForecaster == 7)
		{
			FadeOutPrecipitation();
			weatherString = "Mostly Clear";
		}
		
		//Clear
		if (weatherForecaster == 8)
		{
			FadeOutPrecipitation ();
			weatherString = "Clear";
		}
		
		//Mostly Cloudy
		if (weatherForecaster == 11)
		{
			FadeOutPrecipitation ();
			weatherString = "Mostly Cloudy";
		}
		
		//Cloudy aka Foggy
		if (weatherForecaster == 9)
		{
			weatherForecaster = 1;
		}
		
		//Butterflies (Summer Only)
		if (weatherForecaster == 10)
		{
			FadeOutPrecipitation ();

			weatherString = "Lighning Bugs";
		}	
		
		//Heavy Rain (No Thunder)
		if (weatherForecaster == 12)
		{
			FadeInPrecipitation ();

			weatherString = "Heavy Rain (No Thunder)";
		}
		
		//Falling Fall Leaves
		if (weatherForecaster == 13)
		{
			FadeOutPrecipitation ();

			weatherString = "Falling Fall Leaves";
		}
	}

	
	void OnGUI () 
	{
		if (timeScrollBar == true)
		{
			//Allows a scrolling GUI bar that controls the time of day by the user
			startTime = GUI.HorizontalSlider(new Rect(25, 25, 200, 30), startTime, 0.0F, 0.99F);	
		}
		
		if (commandPromptActive == true)
		{
			stringToEdit = GUI.TextField (new Rect (10, 430, 40, 20), stringToEdit, 10);
			
			if(GUI.Button(new Rect(10, 450, 60, 40), "Change"))
			{
				weatherCommandPrompt ();
			}
		}
	}
	
	void weatherCommandPrompt ()
	{
		//Calculates our weather command prompts
		if (stringToEdit == foggy)
		{
			weatherForecaster = 1;
			print ("Weather Forced: Foggy");
		}
		
		if (stringToEdit == lightRain_lightSnow)
		{
			weatherForecaster = 2;
			print ("Weather Forced: Light Rain/Light Snow (Winter Only)");
		}
		
		if (stringToEdit == rainStorm_snowStorm)
		{
			weatherForecaster = 3;
			print ("Weather Forced: Tunder Storm/Snow Storm (Winter Only)");
		}
		
		if (stringToEdit == partlyCloudy1)
		{
			weatherForecaster = 4;
			print ("Weather Forced: Partly Cloudy");
		}
		
		if (stringToEdit == partlyCloudy2)
		{
			weatherForecaster = 4;
			print ("Weather Forced: Partly Cloudy");
		}
		
		if (stringToEdit == partlyCloudy3)
		{
			weatherForecaster = 4;
			print ("Weather Forced: Partly Cloudy");
		}
		
		if (stringToEdit == clear1)
		{
			weatherForecaster = 7;
			print ("Weather Forced: Mostly Clear");
		}
		
		if (stringToEdit == clear2)
		{
			weatherForecaster = 8;
			print ("Weather Forced: Sunny");
		}
		
		if (stringToEdit == cloudy)
		{
			weatherForecaster = 7;
			print ("Weather Forced: Sunny");
		}
		
		if (stringToEdit == butterfliesSummer)
		{
			weatherForecaster = 10;
			print ("Weather Forced: Lightning Bugs (Summer Only)");
		}
		
		if (stringToEdit == mostlyCloudy)
		{
			weatherForecaster = 11;
			print ("Weather Forced: Mostly Cloudy");
		}
		
		if (stringToEdit == heavyRain)
		{
			weatherForecaster = 12;
			print ("Weather Forced: Heavy Rain (No Thunder)");
		}
		
		if (stringToEdit == fallLeaves)
		{
			weatherForecaster = 13;
			print ("Weather Forced: Falling Fall Leaves (Fall Only)");
		}
	}	
	
	
	void Lightning () 
	{
			timer += Time.deltaTime;
			
			if (timer >= lightningOdds && lightingGenerated == false)
			{
				lightingGenerated = true;
				lightSourceComponent.enabled = true;
				
				lightningNumber = Random.Range(1,5);
				
				if (lightningNumber == 1)
				{
					soundComponent.PlayOneShot(thunderSound1);
				}
				
				if (lightningNumber == 2)
				{
					soundComponent.PlayOneShot(thunderSound2);
				}
				
				if (lightningNumber == 3)
				{
					soundComponent.PlayOneShot(thunderSound3);
				}
				
				if (lightningNumber == 4)
				{
					soundComponent.PlayOneShot(thunderSound4);
				}
				
				if (lightningNumber == 5)
				{
					soundComponent.PlayOneShot(thunderSound5);
				}
				
				Instantiate(lightningBolt1, lightningSpawn.position, lightningSpawn.rotation);
			}
			
			if (lightingGenerated == true)
			{
				if (fadeLightning == false)
				{
					lightSourceComponent.intensity += .22f;
				}

				//

				
				if (lightSourceComponent.intensity >= lightningIntensity && fadeLightning == false)
				{
					lightSourceComponent.intensity = lightningIntensity;
					fadeLightning = true;
				}
			}
			
			if (fadeLightning == true)
			{
				
				onTimer += Time.deltaTime;

				
				
				if (onTimer >= lightningFlashLength)
				{
					lightSourceComponent.intensity -= .14f;
				}
				
				
				if (lightSourceComponent.intensity <= 0)
				{
					lightSourceComponent.intensity = 0;
					fadeLightning = false;
					lightingGenerated = false;
					timer = 0;
					onTimer = 0;
					lightSourceComponent.enabled = false;
					lightSourceComponent.transform.rotation = Quaternion.Euler (50, Random.Range(0,360), 0);
					
					lightningOdds = Random.Range(lightningMinChance, lightningMaxChance);
					lightningIntensity = Random.Range(minIntensity, maxIntensity);
				}
			}
	}
	
	
	void LogErrorCheck () 
	{
		//Check Null and Log Errors for weather effects
		if (rain == null)
		{
			//Error Log if object is not found unable to find UniStorm Editor
			Debug.LogError("<color=red>Rain System Null Reference:</color> There is no Rain Particle System, make sure there is one assigned to the Rain Particle System slot of the UniStorm Editor. ");
		}
		
		if (snow == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Snow System Null Reference:</color> There is no Snow Particle System, make sure there is one assigned to the Snow Particle System slot of the UniStorm Editor. ");
		}
		
		if (butterflies == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Butterflies System Null Reference:</color> There is no Butterflies Particle System, make sure there is one assigned to the Butterflies Particle System slot of the UniStorm Editor. ");
		}
		
		if (mistFog == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Mist System Null Reference:</color> There is no Mist Particle System, make sure there is one assigned to the Mist Particle System slot of the UniStorm Editor. ");
		}
		
		if (snowMistFog == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Snow Dust System Null Reference:</color> There is no Snow Dust Particle System, make sure there is one assigned to the Snow Dust Particle System slot of the UniStorm Editor. ");
		}
		
		if (windyLeaves == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Windy Leaves System Null Reference:</color> There is no Windy Leaves Particle System, make sure there is one assigned to the Windy Leaves Particle System slot of the UniStorm Editor. ");
		}
		
		if (windZone == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Wind Zone Null Reference:</color> There is no Wind Zone System, make sure there is one assigned to the Wind Zone System slot of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the SkyBox Mateirals that UniStorm uses	
		if (SkyBoxMaterial == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Sky Box Material Null Reference:</color> There is a missing Sky Box Material, make sure there is one assigned to each of the Sky Box Material slots of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the Moon Phase Material that UniStorm uses	
		if (moonPhase1 == null || moonPhase2 == null || moonPhase3 == null || moonPhase4 == null || moonPhase5 == null || moonPhase6 == null || moonPhase7 == null || moonPhase8 == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Moon Phase Material Null Reference:</color> There is a missing Moon Phase Material, make sure there is one assigned to each of the Moon Phase Material slots of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the Cloud GameObjects that UniStorm uses
		if (lightClouds1 == null || lightClouds2 == null || lightClouds3 == null || lightClouds4 == null || lightClouds5 == null || heavyClouds == null || heavyCloudsLayer1 == null || heavyCloudsLayerLight == null || mostlyCloudyClouds == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Cloud GameObject Null Reference:</color> There is a missing Cloud GameObject, make sure there is one assigned to each of the Cloud GameObject slots of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the Sky Sphere GameObjects that UniStorm uses	
		if (starSphere == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Sky Sphere GameObject Null Reference:</color> There is a missing Sky Sphere GameObject, make sure there is one assigned to both the Star Sphere and Gradient Sphere slots of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the Cloud GameObjects that UniStorm uses
		if (moonObject == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Moon GameObject Null Reference:</color> The Moon GameObject is missing, make sure there it is assigned to the Moon GameObject slot of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the Sun GameObjects that UniStorm uses
		if (sun == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Sun GameObject Null Reference:</color> The Sun GameObject is missing, make sure it is assigned to the Sun GameObject slot of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the Moon GameObjects that UniStorm uses
		if (moon == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Moon GameObject Null Reference:</color> The Moon GameObject is missing, make sure it is assigned to the Moon GameObject slot of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the lightning GameObjects that UniStorm uses
		if (lightSource == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Lightning Light GameObject Null Reference:</color> The Lightning Light GameObject is missing, make sure it is assigned to the Lightning Light GameObject slot of the UniStorm Editor. ");
		}
		
		//Check Null and Log Errors for the Weather Sound GameObjects that UniStorm uses
		if (rainSound == null || windSound == null || windSnowSound == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Weather Sound Effect Null Reference:</color>  There is a missing Weather Sound Effect, make sure there is one assigned to each of the Weather Sound Effect slots of the UniStorm Editor.");
		}
		
		//Check Null and Log Errors for the thunder Sound GameObjects that UniStorm uses
		if (thunderSound1 == null || thunderSound2 == null || thunderSound3 == null || thunderSound4 == null || thunderSound5 == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Weather Sound Effect Null Reference:</color>  There is a missing Thunder Sound Effect, make sure there is one assigned to each of the Thunder Sound Effect slots of the UniStorm Editor.");
		}
		
		if (lightningBolt1 == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Lightning Bolt Null Reference:</color> The Lightning Bolt is missing, make sure there is one attached to the UniStorm UniStorm Editor.");
		}
		
		if (lightningSpawn == null)
		{
			//Error Log if script is unable to find UniStorm Editor
			Debug.LogError("<color=red>Lightning Bolt Spawn Null Reference:</color> The Lightning Bolt Spawn is missing, make sure there is one attached to the UniStorm UniStorm Editor.");
		}
	}
	
	void DynamicCloudFormations()
	{
		if (cloudType == 1)
		{
			uvOffsetA += (uvAnimationRateA * Time.deltaTime * cloudSpeed * 0.1f);
			uvOffsetB += (uvAnimationRateB * Time.deltaTime * cloudSpeed * 0.1f);
			uvOffsetC += (uvAnimationRateB * Time.deltaTime * cloudSpeed * 0.1f);
			
			lightClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudA, uvOffsetB );
			lightClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudB, uvOffsetB );
			lightClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudC, uvOffsetC );
			partlyCloudyClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudA, uvOffsetB );
			partlyCloudyClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudB, uvOffsetB );
			partlyCloudyClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudC, uvOffsetB );
			mostlyCloudyClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudA, uvOffsetB );
			mostlyCloudyClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudB, uvOffsetB );
			mostlyCloudyClouds1Component.materials[ materialIndex ].SetTextureOffset( CloudC, uvOffsetB );
			mostlyCloudyClouds2Component.materials[ materialIndex ].SetTextureOffset( CloudA, uvOffsetB );
			mostlyCloudyClouds2Component.materials[ materialIndex ].SetTextureOffset( CloudB, uvOffsetB );
			mostlyCloudyClouds2Component.materials[ materialIndex ].SetTextureOffset( CloudC, uvOffsetB );
			
			if (cloudDensity == 2)
			{
				lightClouds1aComponent.materials[ materialIndex ].SetTextureOffset( CloudA, uvOffsetB );
				lightClouds1aComponent.materials[ materialIndex ].SetTextureOffset( CloudB, uvOffsetB );
				lightClouds1aComponent.materials[ materialIndex ].SetTextureOffset( CloudC, uvOffsetC );
				partlyCloudyClouds2Component.materials[ materialIndex ].SetTextureOffset( CloudA, uvOffsetB );
				partlyCloudyClouds2Component.materials[ materialIndex ].SetTextureOffset( CloudB, uvOffsetB );
				partlyCloudyClouds2Component.materials[ materialIndex ].SetTextureOffset( CloudC, uvOffsetB );
			}
			
			uvOffsetHeavyA += (uvAnimationRateHeavyA * Time.deltaTime * heavyCloudSpeed * 0.1f);
			uvOffsetHeavyB += (uvAnimationRateHeavyB * Time.deltaTime * heavyCloudSpeed * 0.1f);
			uvOffsetHeavyC += (uvAnimationRateHeavyB * Time.deltaTime * heavyCloudSpeed * 0.1f);
			
			heavyCloudsLayerLightComponent.materials[ materialIndex ].SetTextureOffset( CloudA, uvOffsetHeavyA );
			heavyCloudsLayerLightComponent.materials[ materialIndex ].SetTextureOffset( CloudB, uvOffsetHeavyB );
			heavyCloudsLayerLightComponent.materials[ materialIndex ].SetTextureOffset( CloudC, uvOffsetHeavyC );
		}
		
	}
	
	void DynamicTimeOfDaySounds () 
	{
		
		TODSoundsTimer += Time.deltaTime;

		if (TODSoundsTimer >= timeToWaitCurrent && Hour > 4 && Hour < 8 && playedSound == false && useMorningSounds)
		{
			soundComponent.PlayOneShot(ambientSoundsMorning[Random.Range(0,ambientSoundsMorning.Count)]);
			playedSound = true;
		}
		
		if (TODSoundsTimer > timeToWaitCurrent && Hour > 8 && Hour < 16 && playedSound == false && useDaySounds)
		{
			soundComponent.PlayOneShot(ambientSoundsDay[Random.Range(0,ambientSoundsDay.Count)]);
			playedSound = true;
		}
		
		if (TODSoundsTimer > timeToWaitCurrent && Hour > 16 && Hour < 20 && playedSound == false && useEveningSounds)
		{
			soundComponent.PlayOneShot(ambientSoundsEvening[Random.Range(0,ambientSoundsEvening.Count)]);
			playedSound = true;
		}
		
		if (TODSoundsTimer > timeToWaitCurrent && Hour > 20 && Hour < 25 && playedSound == false && useNightSounds)
		{	
			soundComponent.PlayOneShot(ambientSoundsNight[Random.Range(0,ambientSoundsNight.Count)]);
			playedSound = true;
		}
		
		if (TODSoundsTimer > timeToWaitCurrent && Hour > 0 && Hour < 4 && playedSound == false && useNightSounds)
		{	
			soundComponent.PlayOneShot(ambientSoundsNight[Random.Range(0,ambientSoundsNight.Count)]);
			playedSound = true;
		}

		if (TODSoundsTimer >= timeToWaitCurrent+2)
		{
			timeToWaitCurrent = Random.Range(timeToWaitMin,timeToWaitMax);
			TODSoundsTimer = 0;
			playedSound = false;
		}
		
	}

	//Puts all fading out weather types into one function then picks by weather type to control individual weather factors
	void FadeOutPrecipitation ()
	{
			topStormCloudFade -= Time.deltaTime * .04f; 
			fade2 -= Time.deltaTime * .06f; 
			fadeStormClouds -= Time.deltaTime * .04f;
			windControl -= Time.deltaTime;
			time -= Time.deltaTime * .14f;
			sunShaftFade += Time.deltaTime * .14f;
			minRainIntensity -= 1;
			minFogIntensity -= .04f;
			minHeavyRainMistIntensity -= .08f;
			minSnowFogIntensity -= .024f;	
			minSnowIntensity -= .9f;
			stormClouds -= Time.deltaTime * .011f;
			sunIntensity += .0015f;
			sun.enabled = true;
			dynamicSnowFade -= Time.deltaTime * .0095f; 
			overrideFog = false;
			
			heavyCloudsComponent.material.color = new Color(.35f,.35f,.35f,topStormCloudFade);
			heavyCloudsLayer1Component.material.color = new Color(0,0,0,fade2);	
			heavyCloudsLayerLightComponent.material.color = new Color(.5f,.5f,.5f,fade2);

			moonLight.intensity += 0.001f;
			
			if (moonLight.intensity >= moonLightIntensity)
			{
				moonLight.intensity = moonLightIntensity;
			}
			
			//Fade in and out leaves for Fall weather type
			if (weatherForecaster == 13 && monthCounter >= 8 && monthCounter <= 10)
			{
				windyLeavesFade += .04f;

				if (windyLeavesFade >= 6)
				{
					windyLeavesFade = 6;
				}
			}
			else
			{
				windyLeavesFade -= .04f;
			
				if (windyLeavesFade <= 0)
				{
					windyLeavesFade = 0;
				}
			}

			//Fade in and out butterflies (lightning bugs) for Summer weather type
			if (weatherForecaster == 10 && monthCounter >= 5 && monthCounter <= 7)
			{
				butterfliesFade += .04f;

				if (butterfliesFade >= 8)
				{
					butterfliesFade = 8;
				}
			}
			else
			{
				butterfliesFade -= .04f;

				if (butterfliesFade <= 0)
				{
					butterfliesFade = 0;
				}
			}
			
		if (terrainDetection)
		{
			Terrain.activeTerrain.terrainData.wavingGrassSpeed -= 0.00025f;
			Terrain.activeTerrain.terrainData.wavingGrassAmount -= 0.00025f;
			Terrain.activeTerrain.terrainData.wavingGrassStrength -= 0.00025f;
			
			if (Terrain.activeTerrain.terrainData.wavingGrassSpeed <= normalGrassWavingSpeed)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
			}
			
			if (Terrain.activeTerrain.terrainData.wavingGrassAmount <= normalGrassWavingStrength)
			{
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
			}
			
			if (Terrain.activeTerrain.terrainData.wavingGrassStrength <= normalGrassWavingAmount)
			{
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}
			
			if (RenderSettings.fogMode == FogMode.Linear)
			{
				RenderSettings.fogStartDistance += 0.75f; //Was 0.75f
				RenderSettings.fogEndDistance += 0.75f;
				
				if (RenderSettings.fogStartDistance >= fogStartDistance)
				{
					RenderSettings.fogStartDistance = fogStartDistance;
				}
				
				if (RenderSettings.fogEndDistance >= fogEndDistance)
				{
					RenderSettings.fogEndDistance = fogEndDistance;
				}
			}
			
			if (fade2 <= 0)
			{
				fade2 = 0;
			}
			
			if (cloudType == 2 && weatherForecaster == 4)
			{		
				lightClouds2Component.enabled = true;
				
				lightClouds3Component.enabled = false;
				
				lightClouds4Component.enabled = false;
				
				lightClouds5Component.enabled = false;
				
				highClouds1Component.enabled = true;
				
				highClouds2Component.enabled = false;
				
				mostlyCloudyCloudsComponent.enabled = false;
			}

			if (cloudType == 2 && weatherForecaster == 5)
			{		
				lightClouds2Component.enabled = false;
				
				lightClouds3Component.enabled = true;
				
				lightClouds4Component.enabled = true;
				
				lightClouds5Component.enabled = false; 
				
				highClouds1Component.enabled = false;
				
				highClouds2Component.enabled = true;
				
				mostlyCloudyCloudsComponent.enabled = false;
			}

			if (cloudType == 2 && weatherForecaster == 5)
			{
				lightClouds2Component.enabled = false;
				
				lightClouds3Component.enabled = false;
				
				lightClouds4Component.enabled = false;
				
				lightClouds5Component.enabled = true;
				
				highClouds1Component.enabled = false;
				
				highClouds2Component.enabled = false;
				
				mostlyCloudyCloudsComponent.enabled = false;
			}

		//Mostly Cloudy
		if (weatherForecaster == 11)
		{
			partlyCloudyFader += 0.0015f;
			mostlyCloudyFader += 0.0015f;
			
			colorFader += 0.0015f;
			cloudColorMorning.a = colorFader;
			cloudColorDay.a = colorFader;
			cloudColorEvening.a = colorFader;
			cloudColorNight.a = colorFader;
			
			if (cloudType == 1)
			{
				partlyCloudyClouds1Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, partlyCloudyFader);
				partlyCloudyClouds2Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, partlyCloudyFader);
				mostlyCloudyClouds1Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, mostlyCloudyFader);
				mostlyCloudyClouds2Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, mostlyCloudyFader);

				if (mostlyCloudyFader >= 1)
				{
					mostlyCloudyFader = 1;
				}
				
				if (partlyCloudyFader >= 1)
				{
					partlyCloudyFader = 1;
				}
				
				if (colorFader >= 1)
				{
					colorFader = 1;
				}
			}
		}
			
		//Partly Cloudy
		if (weatherForecaster == 4 || weatherForecaster == 5 || weatherForecaster == 6)
		{
			partlyCloudyFader += 0.0015f;
			mostlyCloudyFader -= 0.0015f;
			colorFader += 0.0015f;
			cloudColorMorning.a = colorFader;
			cloudColorDay.a = colorFader;
			cloudColorEvening.a = colorFader;
			cloudColorNight.a = colorFader;

			if (cloudType == 1)
			{
				partlyCloudyClouds1Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, partlyCloudyFader);
				partlyCloudyClouds2Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, partlyCloudyFader);
				mostlyCloudyClouds1Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, mostlyCloudyFader);
				mostlyCloudyClouds2Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, mostlyCloudyFader);

				if (mostlyCloudyFader <= 0)
				{
					mostlyCloudyFader = 0;
				}
				
				if (partlyCloudyFader >= 1)
				{
					partlyCloudyFader = 1;
				}

				if (colorFader >= 1)
				{
					colorFader = 1;
				}
			}
		}

		//Mostly Clear
		if (cloudType == 2 && weatherForecaster == 7)
		{
			lightClouds2Component.enabled = false;
			
			lightClouds3Component.enabled = false;
			
			lightClouds5Component.enabled = false;
			
			lightClouds4Component.enabled = false;
			
			highClouds1Component.enabled = true;
			
			highClouds2Component.enabled = false;
			
			mostlyCloudyCloudsComponent.enabled = false;
		}

			if(weatherForecaster == 7)
			{
				partlyCloudyFader -= 0.0025f;
				mostlyCloudyFader -= 0.0015f;
				colorFader += 0.0025f;
				cloudColorMorning.a = colorFader;
				cloudColorDay.a = colorFader;
				cloudColorEvening.a = colorFader;
				cloudColorNight.a = colorFader;

				if (cloudType == 1)
				{
					partlyCloudyClouds1Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, partlyCloudyFader);
					partlyCloudyClouds2Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, partlyCloudyFader);
					mostlyCloudyClouds1Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, mostlyCloudyFader);
					mostlyCloudyClouds2Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, mostlyCloudyFader);

					if (mostlyCloudyFader <= 0)
					{
						mostlyCloudyFader = 0;
					}
					
					if (partlyCloudyFader <= 0)
					{
						partlyCloudyFader = 0;
					}

					if (colorFader >= 1)
					{
						colorFader = 1;
					}
				}
			}

		//Sunny
		if (cloudType == 2 && weatherForecaster == 8)
		{
			lightClouds2Component.enabled = false;
			
			lightClouds3Component.enabled = false;
			
			lightClouds5Component.enabled = false;
			
			lightClouds4Component.enabled = false;
			
			highClouds1Component.enabled = false;
			
			highClouds2Component.enabled = false;
			
			mostlyCloudyCloudsComponent.enabled = false;
		}
		
		if(weatherForecaster == 8)
		{
			partlyCloudyFader -= 0.0025f;
			mostlyCloudyFader -= 0.0015f;
			colorFader -= 0.0025f;
			cloudColorMorning.a = colorFader;
			cloudColorDay.a = colorFader;
			cloudColorEvening.a = colorFader;
			cloudColorNight.a = colorFader;
			
			if (cloudType == 1)
			{
				partlyCloudyClouds1Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, partlyCloudyFader);
				partlyCloudyClouds2Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, partlyCloudyFader);
				mostlyCloudyClouds1Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, mostlyCloudyFader);
				mostlyCloudyClouds2Component.material.color = new Color(lightClouds1Component.material.color.r, lightClouds1Component.material.color.g, lightClouds1Component.material.color.b, mostlyCloudyFader);

				if (mostlyCloudyFader <= 0)
				{
					mostlyCloudyFader = 0;
				}
				
				if (partlyCloudyFader <= 0)
				{
					partlyCloudyFader = 0;
				}

				if (colorFader <= 0)
				{
					colorFader = 0;
				}
			}
		}

			butterflies.emissionRate = butterfliesFade;
			
			snow.emissionRate = minSnowIntensity;
			
			snowMistFog.emissionRate = minSnowFogIntensity;
			
			rain.emissionRate = minRainIntensity;
			rainMist.emissionRate = minHeavyRainMistIntensity;
			
			mistFogComponent.minEmission = minFogIntensity;
			mistFogComponent.maxEmission = minFogIntensity;	
			
			windyLeaves.emissionRate = windyLeavesFade;
			
			//Fades our fog

		/*
		if (fogScript != null)
		{
			fogScript.heightDensity -= .0005f * Time.deltaTime;
			fogScript.startDistance += 5 * Time.deltaTime;
		}
		*/
			
			
			if (dynamicSnowFade <= .25)
			{
				dynamicSnowFade = .25f;
			}

		/*
		if (fogScript != null)
		{
			if (fogScript.heightDensity <= 0)
			{
				fogScript.heightDensity = 0;
			}
			
			if (fogScript.startDistance >= 300)
			{
				fogScript.startDistance = 300;
			}
		}
		*/
			
			//Keep snow from going below 0
			if (minSnowIntensity <= 0)
			{
				minSnowIntensity = 0;
			}
			
			//Keep snow fog from going below 0
			if (minSnowFogIntensity <= 0)
			{
				minSnowFogIntensity = 0;
			}
			
			rainSoundComponent.volume -= Time.deltaTime * .07f;
			windSoundComponent.volume -= Time.deltaTime * .04f;
			windSnowSoundComponent.volume -= Time.deltaTime * .04f;
			
			if (stormClouds <= 0)
			{
				stormClouds = 0;
			}
			
			//Calculates our wind making it lessen		
			if (minRainIntensity <= 0)
			{
				minRainIntensity = 0;

				//Keeps our wind at 0 if there hasn't been a storm
				windZone.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);

				windZone.gameObject.SetActive(false);
				
				if (windZone.transform.eulerAngles.y <= 1)
				{
					windZone.transform.eulerAngles = new Vector3(0, 0, 0);
					windZone.gameObject.SetActive(false);
				}
				
			}

			
			if (minRainIntensity >= 1)
			{
				//Makes our wind weaker for when the storm ends
				windZone.transform.Rotate(-Vector3.up * Time.deltaTime * 12);
				
				if (windZone.transform.eulerAngles.y <= 1)
				{
					windZone.transform.eulerAngles = new Vector3(0, 0, 0);
				}
				
			}

			
			//Keeps our fade numbers from going below 0
			if (minFogIntensity <= 0)
			{
				minFogIntensity = 0;
			}
			
			if (minHeavyRainMistIntensity <= 0)
			{
				minHeavyRainMistIntensity = 0;
			}
			
			if (topStormCloudFade <= 0)
			{
				topStormCloudFade = 0;
				
			}
			
			if (fadeStormClouds <= 0)
			{
				fadeStormClouds = 0;
			}
			
			if (time <= 0)
			{
				time = 0;
			}
		    
			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity += .005f;
			}
			
			if (sunShaftScript != null)
			{
				if (sunShaftScript.sunShaftIntensity >= 2)
				{
					sunShaftScript.sunShaftIntensity = 2;
					sun.enabled = true;
					RenderSettings.fogDensity += .00012f * Time.deltaTime;

					if (RenderSettings.fogDensity >= fogDensity)
					{
						RenderSettings.fogDensity = fogDensity;
					}
				}
			}
			
			//If the game speed is fast fade clouds instantly	
			if (dayLength >= 0 && dayLength <=15 || nightLength >= 0 && nightLength <=15) 
			{
				topStormCloudFade = 0;
			}
	}

	//Puts all fading in weather types into one function then picks by weather type to control individual weather factors
	void FadeInPrecipitation()
	{
			topStormCloudFade += Time.deltaTime * .015f;
			fade2 += Time.deltaTime * .015f;
			butterfliesFade -= .04f;
			windyLeavesFade -= .04f;
			fadeHorizonController += Time.deltaTime * .04f;
			fadeStormClouds += Time.deltaTime * .04f;
			time += Time.deltaTime * .0014f;
			windControlUp += 1;
			sunShaftFade -= Time.deltaTime * .14f;
			stormClouds += Time.deltaTime * .011f;
			sunIntensity -= .002f;
			dynamicSnowFade -= Time.deltaTime * .0095f; 


			//Fix 1.8.5
			if (weatherForecaster == 1)
			{
				minRainIntensity -= .2f;
				minFogIntensity -= .008f;
				minHeavyRainMistIntensity -= .008f;
				minSnowFogIntensity -= .024f;	
				minSnowIntensity -= .9f;

				rainSoundComponent.volume -= Time.deltaTime * .07f;
				windSoundComponent.volume -= Time.deltaTime * .04f;
				windSnowSoundComponent.volume -= Time.deltaTime * .04f;
				
				//Keeps our fade numbers from going below 0
				if (minFogIntensity <= 0)
				{
					minFogIntensity = 0;
				}
				
				if (minHeavyRainMistIntensity <= 0)
				{
					minHeavyRainMistIntensity = 0;
				}

				//Keep snow from going below 0
				if (minSnowIntensity <= 0)
				{
					minSnowIntensity = 0;
				}
				
				//Keep snow fog from going below 0
				if (minSnowFogIntensity <= 0)
				{
					minSnowFogIntensity = 0;
				}
			}
		
			//Light Rain
			if (temperature >= 33 && temperatureType == 1 && weatherForecaster == 2 || temperatureType == 2 && temperature >= 1 && weatherForecaster == 2)
			{
				//Fixed 1.8.5
				rainSoundComponent.volume += Time.deltaTime * .01f;
			
				if (fade2 >= 0.3f)
				{
					minRainIntensity += .2f;
				}

				minFogIntensity -= .008f;
				
				if (minRainIntensity >= maxLightRainIntensity)
				{
					minRainIntensity = maxLightRainIntensity;
				}
				
				if (minFogIntensity <= 0)
				{
					minFogIntensity = 0;
				}
				
				if (minHeavyRainMistIntensity <= 0)
				{
					minHeavyRainMistIntensity = 0;
				}
				
				//This keeps the sound levels low because this is just a little rain	
				if (windSoundComponent.volume >= .0)
				{
					windSoundComponent.volume = .0f;
				}
				
				if (rainSoundComponent.volume >= .3)
				{
					rainSoundComponent.volume = .3f;
				}

				//If generated precipitation is eqaul to last roll, regenerate intensity (If randomized rain is true)
				//Light Rain
				if (lastWeatherType != weatherForecaster && randomizedPrecipitation)
				{
					randomizedRainIntensity = Random.Range(100,maxLightRainIntensity);
					currentGeneratedIntensity = randomizedRainIntensity;
					lastWeatherType = weatherForecaster;
				}

				if (!randomizedPrecipitation)
				{
					if (minRainIntensity >= maxLightRainIntensity)
					{
						minRainIntensity = maxLightRainIntensity;
					}
				}
				
				if (randomizedPrecipitation)
				{
					if (minRainIntensity >= currentGeneratedIntensity)
					{
						minRainIntensity = currentGeneratedIntensity;
					}
				}
			}

			//Thunder Storm or Heavy Rain (No Thunder)
			if (temperature >= 33 && temperatureType == 1  && weatherForecaster == 3 || temperatureType == 2 && temperature >= 1 && weatherForecaster == 3 || temperature >= 33 && temperatureType == 1  && weatherForecaster == 12 || temperatureType == 2 && temperature >= 1 && weatherForecaster == 12)
			{
				if (fade2 >= 0.3f)
				{
					minRainIntensity += .2f;
					minFogIntensity += .008f;
					minHeavyRainMistIntensity += .008f;
				}

				minSnowFogIntensity -= .024f;	
				minSnowIntensity -= .9f;

				rainSoundComponent.volume += Time.deltaTime * .01f;
				windSoundComponent.volume += Time.deltaTime * .01f;

				if (weatherForecaster == 3 && minRainIntensity >= 150)
				{
					Lightning ();
				}

				if (!randomizedPrecipitation)
				{
					//Gradually fades our rain effects in
					if (minRainIntensity >= maxStormRainIntensity)
					{
						minRainIntensity = maxStormRainIntensity;
					}
				}
				
				if (randomizedPrecipitation)
				{
					if (minRainIntensity >= currentGeneratedIntensity)
					{
						minRainIntensity = currentGeneratedIntensity;
					}
				}

				//If generated precipitation is eqaul to last roll, regenerate intensity (If randomized rain is true)
				//Heavy Rain
				if (lastWeatherType != weatherForecaster && randomizedPrecipitation)
				{
					randomizedRainIntensity = Random.Range(400,maxStormRainIntensity);
					currentGeneratedIntensity = randomizedRainIntensity;
					lastWeatherType = weatherForecaster;
				}
				
				//Gradually fades our rain effects in
				if (minRainIntensity >= maxStormRainIntensity)
				{
					minRainIntensity = maxStormRainIntensity;
				}
				
				if (minFogIntensity >= maxStormMistCloudsIntensity)
				{
					minFogIntensity = maxStormMistCloudsIntensity;
				}
				
				if (minHeavyRainMistIntensity >= maxHeavyRainMistIntensity)
				{
					minHeavyRainMistIntensity = maxHeavyRainMistIntensity;
				}

				//Keep snow from going below 0
				if (minSnowIntensity <= 0)
				{
					minSnowIntensity = 0;
				}
				
				//Keep snow fog from going below 0
				if (minSnowFogIntensity <= 0)
				{
					minSnowFogIntensity = 0;
				}
			}

		//Snow Storm
		if (temperature <= 32 && temperatureType == 1  && weatherForecaster == 3 || temperatureType == 2 && temperature <= 0  && weatherForecaster == 3)
		{
			if (fade2 >= 0.3f)
			{
				minSnowIntensity += .2f;
				minSnowFogIntensity += .008f;
			}

			minRainIntensity -= 1;
			minFogIntensity -= .04f;
			minHeavyRainMistIntensity -= .08f;

			windSnowSoundComponent.volume += Time.deltaTime * .01f;
			windSoundComponent.volume -= Time.deltaTime * .04f;

			//If generated precipitation is eqaul to last roll, regenerate intensity (If randomized rain is true)
			//Light Snow
			if (lastWeatherType != weatherForecaster && randomizedPrecipitation)
			{
				randomizedRainIntensity = Random.Range(100,maxSnowStormIntensity);
				currentGeneratedIntensity = randomizedRainIntensity;
				lastWeatherType = weatherForecaster;
			}

			if (!randomizedPrecipitation)
			{
				//Keeps our snow level maxed at users input
				if (minSnowIntensity >= maxSnowStormIntensity)
				{
					minSnowIntensity = maxSnowStormIntensity;
				}
			}

			if (randomizedPrecipitation)
			{
				if (minSnowIntensity >= currentGeneratedIntensity)
				{
					minSnowIntensity = currentGeneratedIntensity;
				}
			}
			
			//Keeps our snow level maxed at users input
			if (minSnowIntensity >= maxSnowStormIntensity)
			{
				minSnowIntensity = maxSnowStormIntensity;
			}
			
			//Keeps our snow fog level maxed at users input
			if (minSnowFogIntensity >= maxHeavySnowDustIntensity)
			{
				minSnowFogIntensity = maxHeavySnowDustIntensity;
			}
			
			//Keeps our fade numbers from going below 0
			if (minRainIntensity <= 0)
			{
				minRainIntensity = 0;
			}

			if (minFogIntensity <= 0)
			{
				minFogIntensity = 0;
			}
			
			if (minHeavyRainMistIntensity <= 0)
			{
				minHeavyRainMistIntensity = 0;
			}
		}

		//Light Snow
		if (temperature <= 32 && temperatureType == 1  && weatherForecaster == 2 || temperatureType == 2 && temperature <= 0  && weatherForecaster == 2)
		{
			if (fade2 >= 0.3f)
			{
				minSnowIntensity += .2f;
				minSnowFogIntensity += .008f;	
			}

			minRainIntensity -= 1;
			minFogIntensity -= .04f;
			minHeavyRainMistIntensity -= .08f;

			windSnowSoundComponent.volume += Time.deltaTime * .01f;

			//If generated precipitation is eqaul to last roll, regenerate intensity (If randomized rain is true)
			//Light Snow
			if (lastWeatherType != weatherForecaster && randomizedPrecipitation)
			{
				randomizedRainIntensity = Random.Range(100,maxLightSnowIntensity);
				currentGeneratedIntensity = randomizedRainIntensity;
				lastWeatherType = weatherForecaster;
			}

			if (!randomizedPrecipitation)
			{
				//Keeps our snow level maxed at users input
				if (minSnowIntensity >= maxLightSnowIntensity)
				{
					minSnowIntensity = maxLightSnowIntensity;
				}
			}

			if (randomizedPrecipitation)
			{
				if (minSnowIntensity >= currentGeneratedIntensity)
				{
					minSnowIntensity = currentGeneratedIntensity;
				}
			}

			//Keeps our snow level maxed at users input
			if (minSnowIntensity >= maxLightSnowIntensity)
			{
				minSnowIntensity = maxLightSnowIntensity;
			}
			
			//Keeps our snow fog level maxed at users input
			if (minSnowFogIntensity >= maxLightSnowDustIntensity)
			{
				minSnowFogIntensity = maxLightSnowDustIntensity;
			}

			//Keeps our fade numbers from going below 0
			if (minRainIntensity <= 0)
			{
				minRainIntensity = 0;
			}
			
			if (minFogIntensity <= 0)
			{
				minFogIntensity = 0;
			}
			
			if (minHeavyRainMistIntensity <= 0)
			{
				minHeavyRainMistIntensity = 0;
			}

			if (windSnowSoundComponent.volume >= .3)
			{
				windSnowSoundComponent.volume = .3f;
			}
		}
			
		if (terrainDetection)
		{
			//Fades in our Dynamic Wind, but only if a terrain is present
			if (weatherForecaster == 3 || weatherForecaster == 12)
			{
				//Added 1.8.4
				Terrain.activeTerrain.terrainData.wavingGrassSpeed += 0.00008f;
				Terrain.activeTerrain.terrainData.wavingGrassAmount += 0.00008f;
				Terrain.activeTerrain.terrainData.wavingGrassStrength += 0.00008f;
				
				if (Terrain.activeTerrain.terrainData.wavingGrassSpeed >= stormGrassWavingSpeed)
				{
					Terrain.activeTerrain.terrainData.wavingGrassSpeed = stormGrassWavingSpeed;
				}
				
				if (Terrain.activeTerrain.terrainData.wavingGrassAmount >= stormGrassWavingStrength)
				{
					Terrain.activeTerrain.terrainData.wavingGrassAmount = stormGrassWavingStrength;
				}
				
				if (Terrain.activeTerrain.terrainData.wavingGrassStrength >= stormGrassWavingAmount)
				{
					Terrain.activeTerrain.terrainData.wavingGrassStrength = stormGrassWavingAmount;
				}
			}

			//Fades in our Dynamic Wind
			if (weatherForecaster == 1 || weatherForecaster == 2)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed -= 0.00025f;
				Terrain.activeTerrain.terrainData.wavingGrassAmount -= 0.00025f;
				Terrain.activeTerrain.terrainData.wavingGrassStrength -= 0.00025f;
				
				if (Terrain.activeTerrain.terrainData.wavingGrassSpeed <= normalGrassWavingSpeed)
				{
					Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				}
				
				if (Terrain.activeTerrain.terrainData.wavingGrassAmount <= normalGrassWavingStrength)
				{
					Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				}
				
				if (Terrain.activeTerrain.terrainData.wavingGrassStrength <= normalGrassWavingAmount)
				{
					Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
				}
			}
		}
			
			if (sunIntensity <= HeavyRainSunIntensity)
			{
				sunIntensity = HeavyRainSunIntensity;
			}
			
			moonLight.intensity -= 0.001f;
			
			if (moonLight.intensity <= stormyMoonLightIntensity)
			{
				moonLight.intensity = stormyMoonLightIntensity;
			}
			
			if (RenderSettings.fogMode == FogMode.Linear)
			{
				RenderSettings.fogStartDistance -= 0.75f;
				RenderSettings.fogEndDistance -= 0.75f;
				
				if (RenderSettings.fogStartDistance <= stormyFogDistanceStart)
				{
					RenderSettings.fogStartDistance = stormyFogDistanceStart;
				}
				
				if (RenderSettings.fogEndDistance <= stormyFogDistance)
				{
					RenderSettings.fogEndDistance = stormyFogDistance;
				}
			}

			//Slowly increases the wind to make it stronger for the storm
			if (minRainIntensity >= 1)
			{
				//Makes our wind stronger for the storm
				windZone.transform.Rotate(Vector3.up * Time.deltaTime * 3);	
				windZone.gameObject.SetActive(true);
			}
			
			if (windZone.transform.eulerAngles.y >= 180)
			{
				windZone.transform.eulerAngles = new Vector3(0, 180, 0);
			}
			
			
			if (dynamicSnowFade <= .25)
			{
				dynamicSnowFade = .25f;
			}

			//Fades in storm clouds
			heavyCloudsComponent.material.color = new Color(.35f,.35f,.35f,topStormCloudFade);
			heavyCloudsLayer1Component.material.color = new Color(0,0,0,fade2);	
			heavyCloudsLayerLightComponent.material.color = new Color(.5f,.5f,.5f,fade2);
			
			if (fade2 >= .75)
			{
				fade2 = .75f;
			}
			
			if (butterfliesFade <= 0)
			{
				butterfliesFade = 0;
			}
			
			butterflies.emissionRate = butterfliesFade;
			
			//Calculates fading in our particles
			snow.emissionRate = minSnowIntensity;
			
			snowMistFog.emissionRate = minSnowFogIntensity;

			rain.emissionRate = minRainIntensity;
			rainMist.emissionRate = minHeavyRainMistIntensity;
			
			mistFogComponent.minEmission = minFogIntensity;
			mistFogComponent.maxEmission = minFogIntensity;	
			
			windyLeaves.emissionRate = windyLeavesFade;
			
			//Fades our fog in	
			RenderSettings.fogDensity -= .00012f * Time.deltaTime;
			
			if (RenderSettings.fogDensity <= .0006)
			{
				RenderSettings.fogDensity = .0006f;
				
				//fogScript.heightDensity += .0008f * Time.deltaTime;
				//fogScript.startDistance -= 5 * Time.deltaTime;
				
				/*
				if (fogScript.startDistance <= 30)
				{
					fogScript.startDistance = 30;
					fogScript.heightDensity -= .0005f * Time.deltaTime;
				}
				*/
				
				if (overrideFog == false)
				{
					
					//fogScript.heightDensity += .0005f * Time.deltaTime;
					
					/*
					if (fogScript.heightDensity >= .0038)
					{   			    
						fogScript.heightDensity = .0038f;
					}
					*/
				}
			}
			
			if (overrideFog == true)
			{
				//fogScript.heightDensity -= .001f * Time.deltaTime;
				
				/*
				if (fogScript.heightDensity <= .0038)
				{   			    
					fogScript.heightDensity = .0038f;
				}
				*/
			}
			
			if (stormClouds >= .55)
			{
				stormClouds = .55f;
			}
			
			if (fade2 >= .75)
			{
				fade2 = .75f;
			}
			
			if (topStormCloudFade >= 1)
			{
				topStormCloudFade = 1;
			}
			
			if (fadeStormClouds >= 1)
			{
				fadeStormClouds = 1;
			}
			
			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity -= .0015f;
				
				if (sunShaftScript.sunShaftIntensity <= .1)
				{
					sunShaftScript.sunShaftIntensity = 0;
				}
			}
			
			if (time >= 1)
			{
				time = 1;
			}
			
			//If the game speed is fast forward fade clouds instantly	
			if (dayLength >= 0 && dayLength <=15 || nightLength >= 0 && nightLength <=15) 
			{
				topStormCloudFade = 1;
			}
	}

	public void CalculateDays()
	{	
		sunCalculator = 0;
		
		Hour = 0;

		dayCounter += 1;
		forceStorm += 1;
		
		hour1 = false;
		hour2 = false;
		hour3 = false;
		hour4 = false;
		hour5 = false;
		hour6 = false;
		hour7 = false;
		hour8 = false;
		hour9 = false;
		hour10 = false;
		hour11 = false;
		hour12 = false;
		hour13 = false;
		hour14 = false;
		hour15 = false;
		hour16 = false;
		hour17 = false;
		hour18 = false;
		hour19 = false;
		hour20 = false;
		hour21 = false;
		hour22 = false;
		hour23 = false;
		hour0 = false;
		
		
		if (weatherForecaster == 3 || weatherForecaster == 2 || weatherForecaster == 12) 
		{
			changeWeather += 1; 
		}
	}

	//Can be called to load user's saved time or to change the time from an external script
	public void LoadTime ()
	{
		realStartTimeMinutesFloat = (int)realStartTimeMinutes;
		
		//Aded 1.8.5
		startTime = realStartTime / 24 + realStartTimeMinutesFloat / 1440;
	}

	//Instant Weather can be called if you want the weather to change instantly. This can be done for quests, loading a player's game, events, etc. 
	//It will set all needed values to be fully faded in. This also goes for the starting weather, if desired.
	public void InstantWeather ()
	{


		if(weatherForecaster == 1)
		{
			topStormCloudFade = 1;
			fade2 = 0.75f;
			minRainIntensity = 0;
			minSnowIntensity = 0;
			minSnowFogIntensity = 0;
			minHeavyRainMistIntensity = 0;
			minFogIntensity = 0;
			rainSoundComponent.volume = 0;
			windSoundComponent.volume = 0;
			windSnowSoundComponent.volume = 0;
			sunShaftFade = 0;
			sunIntensity = HeavyRainSunIntensity;
			RenderSettings.fogEndDistance = stormyFogDistance;
			RenderSettings.fogStartDistance = stormyFogDistanceStart;
			butterfliesFade = 0;
			windyLeavesFade = 0;
			
			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 0;
			}
			
			fader2 = 0;
			fader = 1;

			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}

		if(weatherForecaster == 2 && temperature > 32)
		{
			topStormCloudFade = 1;
			fade2 = 0.75f;
			minRainIntensity = maxLightRainIntensity;
			minSnowIntensity = 0;
			minSnowFogIntensity = 0;
			rainSoundComponent.volume = 0f;
			windSoundComponent.volume = 0.3f;
			windSnowSoundComponent.volume = 0;
			sunShaftFade = 0;
			sunIntensity = HeavyRainSunIntensity;
			RenderSettings.fogEndDistance = stormyFogDistance;
			RenderSettings.fogStartDistance = stormyFogDistanceStart;
			butterfliesFade = 0;
			windyLeavesFade = 0;

			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 0;
			}

			fader2 = 0;
			fader = 1;

			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}

		if(weatherForecaster == 2 && temperature <= 32)
		{
			topStormCloudFade = 1;
			fade2 = 0.75f;
			minRainIntensity = 0;
			minFogIntensity = 0;
			minSnowIntensity = maxLightSnowIntensity;
			minSnowFogIntensity = maxLightSnowDustIntensity;
			rainSoundComponent.volume = 0;
			windSoundComponent.volume = 0;
			windSnowSoundComponent.volume = .3f;
			sunShaftFade = 0;
			sunIntensity = HeavyRainSunIntensity;
			RenderSettings.fogEndDistance = stormyFogDistance;
			RenderSettings.fogStartDistance = stormyFogDistanceStart;
			butterfliesFade = 0;
			windyLeavesFade = 0;

			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 0;
			}

			fader2 = 0;
			fader = 1;

			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}

		if(weatherForecaster == 3 && temperature > 32 || weatherForecaster == 12 && temperature > 32)
		{
			topStormCloudFade = 1;
			fade2 = 0.75f;
			currentGeneratedIntensity = 1000;
			minRainIntensity = 1000;
			minSnowIntensity = 0;
			minSnowFogIntensity = 0;
			minHeavyRainMistIntensity = maxHeavyRainMistIntensity;
			minFogIntensity = maxStormMistCloudsIntensity;
			rainSoundComponent.volume = 1.0f;
			windSoundComponent.volume = 1.0f;
			windSnowSoundComponent.volume = 0;
			sunShaftFade = 0;
			sunIntensity = HeavyRainSunIntensity;
			RenderSettings.fogEndDistance = stormyFogDistance;
			RenderSettings.fogStartDistance = stormyFogDistanceStart;
			butterfliesFade = 0;
			windyLeavesFade = 0;

			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 0;
			}

			fader2 = 0;
			fader = 1;

			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = stormGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = stormGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = stormGrassWavingAmount;
			}
		}
		
		if(weatherForecaster == 3 && temperature <= 32)
		{
			topStormCloudFade = 1;
			fade2 = 0.75f;
			minRainIntensity = 0;
			minFogIntensity = 0;
			currentGeneratedIntensity = 1000;
			minSnowIntensity = maxSnowStormIntensity;
			minSnowFogIntensity = maxHeavySnowDustIntensity;
			rainSoundComponent.volume = 0.0f;
			windSoundComponent.volume = 0.0f;
			windSnowSoundComponent.volume = 1.0f;
			sunShaftFade = 0;
			sunIntensity = HeavyRainSunIntensity;
			RenderSettings.fogEndDistance = stormyFogDistance;
			RenderSettings.fogStartDistance = stormyFogDistanceStart;
			butterfliesFade = 0;
			windyLeavesFade = 0;
			
			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 0;
			}
			
			fader2 = 0;
			fader = 1;

			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = stormGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = stormGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = stormGrassWavingAmount;
			}
		}

		if(weatherForecaster == 4)
		{
			topStormCloudFade = 0;
			fade2 = 0;
			minRainIntensity = 0;
			minHeavyRainMistIntensity = 0;
			minFogIntensity = 0;
			minSnowIntensity = 0;
			minSnowFogIntensity = 0;
			rainSoundComponent.volume = 0;
			windSoundComponent.volume = 0;
			windSnowSoundComponent.volume = 0;
			sunShaftFade = 0;
			sunIntensity = maxSunIntensity;
			RenderSettings.fogEndDistance = fogEndDistance;
			RenderSettings.fogStartDistance = fogStartDistance;
			butterfliesFade = 0;
			windyLeavesFade = 0;

			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 2;
			}

			fader2 = 1;
			fader = 0;
			partlyCloudyFader = 1;
			mostlyCloudyFader = 0;
			clearFader = 1;

			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}

		if(weatherForecaster == 7 || weatherForecaster == 8)
		{
			topStormCloudFade = 0;
			fade2 = 0;
			minRainIntensity = 0;
			minHeavyRainMistIntensity = 0;
			minFogIntensity = 0;
			minSnowIntensity = 0;
			minSnowFogIntensity = 0;
			rainSoundComponent.volume = 0;
			windSoundComponent.volume = 0;
			windSnowSoundComponent.volume = 0;
			sunShaftFade = 0;
			sunIntensity = maxSunIntensity;
			RenderSettings.fogEndDistance = fogEndDistance;
			RenderSettings.fogStartDistance = fogStartDistance;
			butterfliesFade = 0;
			windyLeavesFade = 0;

			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 2;
			}

			fader2 = 1;
			fader = 0;

			if (weatherForecaster == 7)
			{
				partlyCloudyFader = 0;
				clearFader = 1;
				mostlyCloudyFader = 0;
			}

			if (weatherForecaster == 9)
			{
				partlyCloudyFader = 0;
				clearFader = 0;
				mostlyCloudyFader = 0;
			}

			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}

		if(weatherForecaster == 11)
		{
			topStormCloudFade = 0;
			fade2 = 0;
			minRainIntensity = 0;
			minHeavyRainMistIntensity = 0;
			minFogIntensity = 0;
			minSnowIntensity = 0;
			minSnowFogIntensity = 0;
			rainSoundComponent.volume = 0;
			windSoundComponent.volume = 0;
			windSnowSoundComponent.volume = 0;
			sunShaftFade = 0;
			sunIntensity = maxSunIntensity;
			RenderSettings.fogEndDistance = fogEndDistance;
			RenderSettings.fogStartDistance = fogStartDistance;
			butterfliesFade = 0;
			windyLeavesFade = 0;
			
			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 2;
			}
			
			fader2 = 1;
			fader = 0;
			clearFader = 1;
			partlyCloudyFader = 1;
			mostlyCloudyFader = 1;

			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}

		if(weatherForecaster == 10 && monthCounter >= 5 && monthCounter <= 7)
		{
			topStormCloudFade = 0;
			fade2 = 0;
			minRainIntensity = 0;
			minHeavyRainMistIntensity = 0;
			minFogIntensity = 0;
			minSnowIntensity = 0;
			minSnowFogIntensity = 0;
			rainSoundComponent.volume = 0;
			windSoundComponent.volume = 0;
			windSnowSoundComponent.volume = 0;
			sunShaftFade = 0;
			sunIntensity = maxSunIntensity;
			RenderSettings.fogEndDistance = fogEndDistance;
			RenderSettings.fogStartDistance = fogStartDistance;
			butterfliesFade = 8;
			windyLeavesFade = 0;
			
			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 2;
			}
			
			fader2 = 1;
			fader = 0;
			partlyCloudyFader = 1;
			mostlyCloudyFader = 0;
			clearFader = 1;
			
			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}

		if(weatherForecaster == 13 && monthCounter >= 8 && monthCounter <= 10)
		{
			topStormCloudFade = 0;
			fade2 = 0;
			minRainIntensity = 0;
			minHeavyRainMistIntensity = 0;
			minFogIntensity = 0;
			minSnowIntensity = 0;
			minSnowFogIntensity = 0;
			rainSoundComponent.volume = 0;
			windSoundComponent.volume = 0;
			windSnowSoundComponent.volume = 0;
			sunShaftFade = 0;
			sunIntensity = maxSunIntensity;
			RenderSettings.fogEndDistance = fogEndDistance;
			RenderSettings.fogStartDistance = fogStartDistance;
			butterfliesFade = 0;
			windyLeavesFade = 6;
			
			if (sunShaftScript != null)
			{
				sunShaftScript.sunShaftIntensity = 2;
			}
			
			fader2 = 1;
			fader = 0;
			partlyCloudyFader = 1;
			mostlyCloudyFader = 0;
			clearFader = 1;
			
			if (terrainDetection)
			{
				Terrain.activeTerrain.terrainData.wavingGrassSpeed = normalGrassWavingSpeed;
				Terrain.activeTerrain.terrainData.wavingGrassAmount = normalGrassWavingStrength;
				Terrain.activeTerrain.terrainData.wavingGrassStrength = normalGrassWavingAmount;
			}
		}
	}
}











