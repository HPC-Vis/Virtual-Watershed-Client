//UniStorm Weather System Editor C# Version 1.8.5 @ Copyright
//Black Horizon Studios

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

[System.Serializable]
[CustomEditor(typeof(UniStormWeatherSystem_C))] 
public class UniStormEditor_C : Editor 
{
	
	//Spring Weather Odds
	enum WeatherChanceDropDown1
	{
		_20 = 20, 
		_40 = 40, 
		_60 = 60, 
		_80 = 80
	}
	
	//Summer Weather Odds
	enum WeatherChanceDropDown2
	{
		_20 = 20, 
		_40 = 40, 
		_60 = 60, 
		_80 = 80
	}
	
	//Fall Weather Odds
	enum WeatherChanceDropDown3
	{
		_20 = 20, 
		_40 = 40, 
		_60 = 60, 
		_80 = 80
	}
	
	//Winter Weather Odds
	enum WeatherChanceDropDown4
	{
		_20 = 20, 
		_40 = 40, 
		_60 = 60, 
		_80 = 80
	}
	
	enum MonthDropDown
	{
		January = 1,
		February = 2,
		March = 3,
		April = 4,
		May = 5,
		June = 6,
		July = 7,
		August = 8,
		September = 9,
		October = 10,
		November = 11,
		December = 12
	}
	
	enum WeatherTypeDropDown
	{
		Foggy = 1, 
		LightRainOrLightSnowWinterOnly = 2, 
		ThunderStormOrSnowStormWinterOnly = 3, 
		PartlyCloudy = 4, 
		//PartlyCloud2 = 5, 
		//PartlyCloud3 = 6, 
		MostlyClear = 7,
		Sunny = 8, 
		//Cloudy = 9, 
		LightningBugsSummerOnly = 10,
		MostlyCloudy = 11, 
		HeavyRainNoThunder = 12,  
		FallingLeavesFallOnly = 13
	}
	
	enum MoonPhaseDropDown
	{
		NewMoon = 1,
		WaxingCresent = 2,
		FirstQuarter = 3,
		WaxingGibbous = 4,
		FullMoon = 5,
		WaningGibbous = 6,
		ThirdQuater = 7,
		WaningCresent = 8
	}
	
	enum FogModeDropDown
	{
		linear = 1,
		exponential = 2,
		exp2 = 3
	}
	
	enum CloudDensityDropDown
	{
		low = 1,
		high = 2
	}
	
	enum DayShadowTypeDropDown
	{
		Hard = 1,
		Soft = 2
	}
	
	enum NightShadowTypeDropDown
	{
		Hard = 1,
		Soft = 2
	}
	
	enum LightningShadowTypeDropDown
	{
		Hard = 1,
		Soft = 2
	}
	
	enum CloudTypeDropDown
	{
		Dynamic = 1,
		NonDynamic = 2
	}
	
	enum TemperatureDropDown
	{
		Fahrenheit = 1,
		Celsius = 2
	}

	enum CalendarDropDown
	{
		Standard = 1,
		Custom = 2
	}

	enum DayHourDropDown
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10,
		_11,
		_12,
		_13,
		_14,
		_15,
		_16,
		_17,
		_18,
		_19,
		_20,
		_21,
		_22,
		_23
	}

	enum NightHourDropDown
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10,
		_11,
		_12,
		_13,
		_14,
		_15,
		_16,
		_17,
		_18,
		_19,
		_20,
		_21,
		_22,
		_23
	}

	enum NightMDropDown
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10,
		_11,
		_12,
		_13,
		_14,
		_15,
		_16,
		_17,
		_18,
		_19,
		_20,
		_21,
		_22,
		_23
	}

	enum DayMDropDown
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10,
		_11,
		_12,
		_13,
		_14,
		_15,
		_16,
		_17,
		_18,
		_19,
		_20,
		_21,
		_22,
		_23
	}

	enum StartTimeNew
	{
		_0 = 0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10,
		_11,
		_12,
		_13,
		_14,
		_15,
		_16,
		_17,
		_18,
		_19,
		_20,
		_21,
		_22,
		_23
	}
	
	bool showAdvancedOptions = true;

	WeatherTypeDropDown editorWeatherType = WeatherTypeDropDown.PartlyCloudy;
	MonthDropDown editorMonth = MonthDropDown.January;
	MoonPhaseDropDown editorMoonPhase = MoonPhaseDropDown.FullMoon;
	WeatherChanceDropDown1 editorWeatherChance1 = WeatherChanceDropDown1._40;
	WeatherChanceDropDown2 editorWeatherChance2 = WeatherChanceDropDown2._40;
	WeatherChanceDropDown3 editorWeatherChance3 = WeatherChanceDropDown3._40;
	WeatherChanceDropDown4 editorWeatherChance4 = WeatherChanceDropDown4._40;
	
	FogModeDropDown editorFogMode = FogModeDropDown.linear;
	CloudDensityDropDown editorCloudDensity = CloudDensityDropDown.high;
	DayShadowTypeDropDown editorDayShadowType = DayShadowTypeDropDown.Hard;
	NightShadowTypeDropDown editorNightShadowType = NightShadowTypeDropDown.Hard;
	LightningShadowTypeDropDown editorLightningShadowType = LightningShadowTypeDropDown.Hard;
	CloudTypeDropDown editorCloudType = CloudTypeDropDown.Dynamic;
	TemperatureDropDown editorTemperature = TemperatureDropDown.Fahrenheit;
	CalendarDropDown editorCalendarType = CalendarDropDown.Standard;

	DayHourDropDown editorDayHour = DayHourDropDown._6;
	NightHourDropDown editorNightHour = NightHourDropDown._18;

	StartTimeNew editorStartTimeNew = StartTimeNew._12;

	public override void OnInspectorGUI () 
	{
		serializedObject.Update ();

		UniStormWeatherSystem_C self = (UniStormWeatherSystem_C)target;

		//Time Number Variables
		EditorGUILayout.LabelField("UniStorm Weather System", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("By: Black Horizon Studios", EditorStyles.label);
		EditorGUILayout.Space();

		EditorGUILayout.Space();

		string showOrHide_TimeOptions = "Show";
		if(self.timeOptions)
			showOrHide_TimeOptions = "Hide";

		if(GUILayout.Button(showOrHide_TimeOptions + " Time Options"))
		{
			self.timeOptions = !self.timeOptions;
		}


		if (self.timeOptions)
		{
			EditorGUILayout.LabelField("Time Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The current UniStorm time is displayed with these variables. Setting the Starting Time will start UniStorm at that specific time of day according to the Hour and Minute. Time variables can be used to create events, quests, and effects at specific times.", MessageType.None, true);
			}
			
			editorStartTimeNew = (StartTimeNew)self.realStartTime;
			editorStartTimeNew = (StartTimeNew)EditorGUILayout.EnumPopup("Start Time Hour", editorStartTimeNew);
			self.realStartTime = (int)editorStartTimeNew;

			if (self.realStartTimeMinutes <= 9)
			{
				EditorGUILayout.LabelField("Your day will start at " + self.realStartTime + ":0" + self.realStartTimeMinutes, EditorStyles.miniButton); //objectFieldThumb
			}

			if (self.realStartTimeMinutes >= 10)
			{
				EditorGUILayout.LabelField("Your day will start at " + self.realStartTime + ":" + self.realStartTimeMinutes, EditorStyles.miniButton); //objectFieldThumb
			}

			self.realStartTimeMinutes = EditorGUILayout.IntSlider ("Start Time Minute", self.realStartTimeMinutes, 0, 59);

			
			EditorGUILayout.Space();

			self.minuteCounter = EditorGUILayout.IntField ("Minutes", self.minuteCounter);
			
			self.hourCounter = EditorGUILayout.IntField ("Hours", self.hourCounter);
			
			self.dayCounter = EditorGUILayout.IntField ("Days", self.dayCounter);
			
			if (self.calendarType == 1)
			{	
				editorMonth = (MonthDropDown)self.monthCounter;
				editorMonth = (MonthDropDown)EditorGUILayout.EnumPopup("Month", editorMonth);
				self.monthCounter = (int)editorMonth;
			}

			if (self.calendarType == 2)
			{
				EditorGUILayout.Space();	
				EditorGUILayout.HelpBox("While Custom Calendar is enabled, UniStorm will display numbers for months.", MessageType.Warning, true);
				
				self.monthCounter = EditorGUILayout.FloatField ("Months", self.monthCounter);
				
				EditorGUILayout.Space();
			}
			
			self.yearCounter = EditorGUILayout.FloatField ("Years", self.yearCounter);
			
			EditorGUILayout.Space();

			EditorGUILayout.Space();

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Day Legnth Hour determins what time UniStorm will start using the Day Length time. This allows you to make your days longer or short than nights, if desired.", MessageType.None, true);
			}

			editorDayHour = (DayHourDropDown)self.dayLengthHour;
			editorDayHour = (DayHourDropDown)EditorGUILayout.EnumPopup("Day Length Hour", editorDayHour);
			self.dayLengthHour = (int)editorDayHour;

			if (self.dayLengthHour > self.nightLengthHour || self.dayLengthHour == self.nightLengthHour)
			{
				EditorGUILayout.HelpBox("Your Starting Day Hour can't be higher than, or equal to, your Starting Night Hour.", MessageType.Warning, true);
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Your in-game Day will start at " + self.dayLengthHour + ":00", EditorStyles.miniButton); //objectFieldThumb

			EditorGUILayout.Space();

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The Day Length is calculated by how many real-time minutes pass until UniStorm switches to night, based on the hour you've set for Starting Night Hour. A value of 60 would give you 1 hour long days. This can be changed to any value that's desired.", MessageType.None, true);
			}


			self.dayLength = EditorGUILayout.FloatField ("Day Length", self.dayLength); 

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Night Length Hour determins what time UniStorm will start using the Night Length time. This allows you to make your nights longer or short than days, if desired.", MessageType.None, true);
			}

			editorNightHour = (NightHourDropDown)self.nightLengthHour;
			editorNightHour = (NightHourDropDown)EditorGUILayout.EnumPopup("Night Legnth Hour", editorNightHour);
			self.nightLengthHour = (int)editorNightHour;

			if (self.nightLengthHour < self.dayLengthHour)
			{
				EditorGUILayout.HelpBox("Your Starting Night Hour can't be lower than, or equal to, your Starting Day Hour.", MessageType.Warning, true);
			}

			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Your in-game Night will start at " + self.nightLengthHour + ":00", EditorStyles.miniButton); //objectFieldThumb
			
			EditorGUILayout.Space();

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The Night Length is calculated by how many real-time minutes pass until UniStorm switches to day, based on the hour you've set for Starting Day Hour. A value of 60 would give you 1 hour long nights. This can be changed to any value that's desired.", MessageType.None, true);
			}


			self.nightLength = EditorGUILayout.FloatField ("Night Length", self.nightLength); 

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Time Stopped will stop UniStorm's time and sun from moving, but will allow the clouds to keep animating.", MessageType.None, true);
			}
			
			self.timeStopped = EditorGUILayout.Toggle ("Time Stopped",self.timeStopped);

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			editorCalendarType = (CalendarDropDown)self.calendarType;
			editorCalendarType = (CalendarDropDown)EditorGUILayout.EnumPopup("Calendar Type", editorCalendarType);
			self.calendarType = (int)editorCalendarType;
			
			if (self.calendarType == 1)
			{
				if (self.helpOptions == true)
				{	
					EditorGUILayout.Space();
					
					EditorGUILayout.HelpBox("While the Calendar Type is set to Standard, UniStorm will have standard calendar calculations. This includes the automatic calculation of Leap Year.", MessageType.None, true);
				}
			}
			
			if (self.calendarType == 2)
			{	
				EditorGUILayout.Space();
				
				self.numberOfDaysInMonth = EditorGUILayout.IntField ("Days In Month", self.numberOfDaysInMonth);
				self.numberOfMonthsInYear = EditorGUILayout.IntField ("Months In Year", self.numberOfMonthsInYear);
				
				if (self.helpOptions == true)
				{	
					EditorGUILayout.Space();
					
					EditorGUILayout.HelpBox("While the Calendar Type is set to Custom, UniStorm will choose the values you set within the Editor to calculate Days, Months, and Years. The Month will be changed and listed as a number value.", MessageType.None, true);
				}
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		string showOrHide_SkyOptions = "Show";
		if(self.skyOptions)
			showOrHide_SkyOptions = "Hide";
		if(GUILayout.Button(showOrHide_SkyOptions + " Weather Options"))
		{
			self.skyOptions = !self.skyOptions;
		}

		if (self.skyOptions)
		{
			EditorGUILayout.LabelField("Weather Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The Weather Options allow you to control weather and the speed of sky related components. The Weather Type allows you to manually change UniStorm's weather to any weather that's listed in the drop down menu. This can be used for starting weather or be changed while testing out your scene. The Cloud Thickness controls how thick the dynamic clouds will render.", MessageType.None, true);
			}
			
			EditorGUILayout.Space();
			
			if (self.cloudType == 1)
			{
				editorCloudDensity = (CloudDensityDropDown)self.cloudDensity;
				editorCloudDensity = (CloudDensityDropDown)EditorGUILayout.EnumPopup("Cloud Thickness", editorCloudDensity);
				self.cloudDensity = (int)editorCloudDensity; 
			}

			EditorGUILayout.Space();

			if (self.cloudDensity == 1)
			{
				EditorGUILayout.HelpBox("Low Cloud Thickness will render less dense clouds. This option is for people who like the look of lighter and more faint looking clouds.", MessageType.Info, true);
			}
			
			if (self.cloudDensity == 2)
			{
				EditorGUILayout.HelpBox("High Cloud Thickness will render clouds more dense. This option is for people who like the look of thick noticable clouds.", MessageType.Info, true);
			}

			
			EditorGUILayout.Space();
			
			editorCloudType = (CloudTypeDropDown)self.cloudType;
			editorCloudType = (CloudTypeDropDown)EditorGUILayout.EnumPopup("Cloud Type", editorCloudType);
			self.cloudType = (int)editorCloudType; 
			
			EditorGUILayout.Space();
			
			if (self.cloudType == 1)
			{
				EditorGUILayout.HelpBox("While using Dynamic Cloud UniStorm's clouds will form dynamically. This allows no two clouds in the sky to look the same. Dynamic clouds also look more relalistic and have much more diveristy. This options also affects UniStorm's Storm Clouds.", MessageType.Info, true);
			}
			
			if (self.cloudType == 2)
			{
				EditorGUILayout.HelpBox("While using Non Dynamic clouds, UniStorm will revert back to version 1.6 clouds. Some features may also be disabled both in the UniStorm Editor and visually (Such as clouds being masked along the horizon). These clouds have more of a 'Skyrim' look compared to Dynamic Clouds. These clouds still animate. This option is availble for those who prefer this look over the Dynamic Clouds. ", MessageType.Info, true);
			}

			self.cloudSpeed = EditorGUILayout.IntSlider ("Cloud Speed", self.cloudSpeed, 0, 50);

			self.heavyCloudSpeed = EditorGUILayout.IntSlider ("Storm Cloud Speed", self.heavyCloudSpeed, 0, 50);

			EditorGUILayout.Space();
			
			self.starSpeed = EditorGUILayout.IntField ("Star Scroll Speed", self.starSpeed);
			
			self.starRotationSpeed = EditorGUILayout.FloatField ("Star Rotation Speed", self.starRotationSpeed);

			EditorGUILayout.Space();
			
			editorWeatherType = (WeatherTypeDropDown)self.weatherForecaster;
			editorWeatherType = (WeatherTypeDropDown)EditorGUILayout.EnumPopup("Weather Type", editorWeatherType);
			self.weatherForecaster = (int)editorWeatherType;

			EditorGUILayout.Space();

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Static Weather will stop the weather from ever changing making it static. However, you can still change it manually.", MessageType.None, true);
			}

			self.staticWeather = EditorGUILayout.Toggle ("Static Weather",self.staticWeather);

			EditorGUILayout.Space();

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("If Instant Starting Weather is enabled, weather will be instantly faded in on start bypassing the transitioning of weather. This function can also be called to bypass weather transitions for instance, loading a player's game or an event.", MessageType.None, true);
			}

			self.useInstantStartingWeather = EditorGUILayout.Toggle ("Instant Starting Weather",self.useInstantStartingWeather);

			EditorGUILayout.Space();

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The Weather Odds control the weather odds for each season. A value of 20% gives a 20% chance that the weather will change where 80% would give you an 80% chance. UniStorm's advanced algorithm handles the rest and generates you dynamic weather according to your weather odds for each season.", MessageType.None, true);
			}

			EditorGUILayout.LabelField("Weather Odds", EditorStyles.miniLabel);

			editorWeatherChance1 = (WeatherChanceDropDown1)self.weatherChanceSpring;
			editorWeatherChance1 = (WeatherChanceDropDown1)EditorGUILayout.EnumPopup("Spring %", editorWeatherChance1);
			self.weatherChanceSpring = (int)editorWeatherChance1;
			
			editorWeatherChance2 = (WeatherChanceDropDown2)self.weatherChanceSummer;
			editorWeatherChance2 = (WeatherChanceDropDown2)EditorGUILayout.EnumPopup("Summer %", editorWeatherChance2);
			self.weatherChanceSummer = (int)editorWeatherChance2;
			
			editorWeatherChance3 = (WeatherChanceDropDown3)self.weatherChanceFall;
			editorWeatherChance3 = (WeatherChanceDropDown3)EditorGUILayout.EnumPopup("Fall %", editorWeatherChance3);
			self.weatherChanceFall = (int)editorWeatherChance3;
			
			editorWeatherChance4 = (WeatherChanceDropDown4)self.weatherChanceWinter;
			editorWeatherChance4 = (WeatherChanceDropDown4)EditorGUILayout.EnumPopup("Winter %", editorWeatherChance4);
			self.weatherChanceWinter = (int)editorWeatherChance4;
		}

		EditorGUILayout.Space();		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_WindOptions = "Show";
		if(self.WindOptions)
			showOrHide_WindOptions = "Hide";
		if(GUILayout.Button(showOrHide_WindOptions + " Wind Options"))
		{
			self.WindOptions = !self.WindOptions;
		}
		
		if (self.WindOptions)
		{
			EditorGUILayout.LabelField("Wind Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Here you can adjust the wind settings for the terrain's grass. UniStorm will use the normal wind settings during none precipitation weather types and will slowly transition into stormy wind during precipitation weather types.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			self.normalGrassWavingAmount = EditorGUILayout.Slider ("Normal Grass Wind Speed", self.normalGrassWavingAmount, 0.1f, 1.0f);
			self.stormGrassWavingAmount = EditorGUILayout.Slider ("Stormy Grass Wind Speed", self.stormGrassWavingAmount, 0.1f, 1.0f);

			EditorGUILayout.Space();

			self.normalGrassWavingSpeed = EditorGUILayout.Slider ("Normal Grass Wind Size", self.normalGrassWavingSpeed, 0.1f, 1.0f);
			self.stormGrassWavingSpeed = EditorGUILayout.Slider ("Stormy Grass Wind Size", self.stormGrassWavingSpeed, 0.1f, 1.0f);

			EditorGUILayout.Space();

			self.normalGrassWavingStrength = EditorGUILayout.Slider ("Normal Grass Wind Bending", self.normalGrassWavingStrength, 0.1f, 1.0f);
			self.stormGrassWavingStrength = EditorGUILayout.Slider ("Stormy Grass Wind Bending", self.stormGrassWavingStrength, 0.1f, 1.0f);
		}


		EditorGUILayout.Space();		
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		string showOrHide_AtmosphereOptions = "Show";
		if(self.atmosphereOptions)
			showOrHide_AtmosphereOptions = "Hide";
		if(GUILayout.Button(showOrHide_AtmosphereOptions + " Atmosphere Options"))
		{
			self.atmosphereOptions = !self.atmosphereOptions;
		}
		
		if (self.atmosphereOptions)
		{
			EditorGUILayout.LabelField("Atmosphere Options", EditorStyles.boldLabel);			
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("UniStorm now uses a Physically Based Skybox shader. This shader allows you to adjust factors of the atmosphere that affect the color of the sky which changes according to the angle of the Sun.", MessageType.None, true);
			}

			self.skyColorMorning = EditorGUILayout.ColorField("Sky Tint Color Morning", self.skyColorMorning);
			self.skyColorDay = EditorGUILayout.ColorField("Sky Tint Color Day", self.skyColorDay);
			self.skyColorEvening = EditorGUILayout.ColorField("Sky Tint Color Evening", self.skyColorEvening);
			self.skyColorNight = EditorGUILayout.ColorField("Sky Tint Color Night", self.skyColorNight);
			
			EditorGUILayout.Space();
			
			self.groundColor = EditorGUILayout.ColorField("Ground Color", self.groundColor);

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Here you can adjust the Skybox Tint and Ground colors. The procedural skybox shader will accurately shade according to the time of day and angle of the sun.", MessageType.None, true);
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			self.atmosphereThickness = EditorGUILayout.Slider ("Atmosphere Thickness", self.atmosphereThickness, 0.0f, 5.0f);
			
			EditorGUILayout.Space();
			
			self.exposure = EditorGUILayout.Slider ("Exposure", self.exposure, 0.0f, 8.0f);

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Here you can adjust the Atmosphere Thickness and Exposure. These values allow you to control how thick the atosphere is and how much light is scattered.", MessageType.None, true);
			}
		}
	
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		string showOrHide_FogOptions = "Show";
		if(self.fogOptions)
			showOrHide_FogOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_FogOptions + " Fog Options"))
		{
			self.fogOptions = !self.fogOptions;
		}
		
		
		if (self.fogOptions)
		{
			EditorGUILayout.LabelField("Fog Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The Fog Options allow you to control all densities of Unity's fog. Unity has 3 fog modes; Linear, Exponential, and Exp2. UniStorm will adjust the options according to the fog mode you've selected. Auto Enable Fog will enable Unity's Fog automatically if the check box is checked.", MessageType.None, true);
			}
			
			EditorGUILayout.Space();
			
			self.useUnityFog = EditorGUILayout.Toggle ("Auto Enable Fog?",self.useUnityFog);
			
			EditorGUILayout.Space();
			
			editorFogMode = (FogModeDropDown)self.fogMode;
			editorFogMode = (FogModeDropDown)EditorGUILayout.EnumPopup("Fog Mode", editorFogMode);
			self.fogMode = (int)editorFogMode; 
			
			EditorGUILayout.Space();
			
			if (self.fogMode == 1)
			{
				self.stormyFogDistanceStart = EditorGUILayout.IntSlider ("Stormy Fog Start Distance", self.stormyFogDistanceStart, -400, 1000);
				self.stormyFogDistance = EditorGUILayout.IntSlider ("Stormy Fog End Distance", self.stormyFogDistance, 200, 2500);
				self.fogStartDistance = EditorGUILayout.IntSlider ("Regular Fog Start Distance", self.fogStartDistance, -400, 1000);
				self.fogEndDistance = EditorGUILayout.IntSlider ("Regular Fog End Distance", self.fogEndDistance, 200, 5000);
			}
			
			if (self.fogMode == 2 || self.fogMode == 3)
			{
				self.fogDensity = EditorGUILayout.FloatField ("Fog Density", self.fogDensity);
			}
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		string showOrHide_LightningOptions = "Show";
		if(self.lightningOptions)
			showOrHide_LightningOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_LightningOptions + " Lightning Options"))
		{
			self.lightningOptions = !self.lightningOptions;
		}
		
		
		if (self.lightningOptions)
		{
			EditorGUILayout.LabelField("Lightning Options", EditorStyles.boldLabel);
			EditorGUILayout.Space();
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("These settings allow you to adjust any lightning and thunder related options. These features will only happen during the Thunder Storm weather type.", MessageType.None, true);
			}
			
			self.shadowsLightning = EditorGUILayout.Toggle ("Shadows Enabled?",self.shadowsLightning);
			
			if (self.shadowsLightning)
			{
				EditorGUILayout.Space();
				
				editorLightningShadowType = (LightningShadowTypeDropDown)self.lightningShadowType;
				editorLightningShadowType = (LightningShadowTypeDropDown)EditorGUILayout.EnumPopup("Shadow Type", editorLightningShadowType);
				self.lightningShadowType = (int)editorLightningShadowType;
				
				EditorGUILayout.Space();
				
				self.lightningShadowIntensity = EditorGUILayout.Slider ("Shadow Intensity", self.lightningShadowIntensity, 0, 1.0f);
				
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.Space();
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The lighting intesity settings control the possible minimum and maximum light intensity of the lightning.", MessageType.None, true);
			}
			
			self.lightningColor = EditorGUILayout.ColorField("Lightning Color", self.lightningColor);
			
			EditorGUILayout.Space();
			
			self.minIntensity = EditorGUILayout.Slider ("Min Lightning Intensity", (float)self.minIntensity, 0.5f, 1.5f);
			self.maxIntensity = EditorGUILayout.Slider ("Max Lightning Intensity", self.maxIntensity, 0.5f, 1.5f);
			
			EditorGUILayout.Space();
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The minimum and maximum wait controls the seconds between each lightning strike.", MessageType.None, true);
			}
			
			self.lightningMinChance = EditorGUILayout.IntSlider ("Min Wait", (int)self.lightningMinChance, 1, 20);
			self.lightningMaxChance = EditorGUILayout.IntSlider ("Max Wait", (int)self.lightningMaxChance, 10, 40);
			
			EditorGUILayout.Space();
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The flash length controls how quickly the lightning flashes on and off.", MessageType.None, true);
			}
			
			self.lightningFlashLength = EditorGUILayout.Slider ("Lightning Flash Length", self.lightningFlashLength, 0.4f, 1.2f);
			
			EditorGUILayout.Space();
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Here you can add custom thunder sounds if desired. UniStorm will play them randomly each lightning strike.", MessageType.None, true);
			}
			
			
			bool thunderSound1 = !EditorUtility.IsPersistent (self);
			self.thunderSound1 = (AudioClip)EditorGUILayout.ObjectField ("Thunder Sound 1", self.thunderSound1, typeof(AudioClip), thunderSound1);
			bool thunderSound2 = !EditorUtility.IsPersistent (self);
			self.thunderSound2 = (AudioClip)EditorGUILayout.ObjectField ("Thunder Sound 2", self.thunderSound2, typeof(AudioClip), thunderSound2);
			bool thunderSound3 = !EditorUtility.IsPersistent (self);
			self.thunderSound3 = (AudioClip)EditorGUILayout.ObjectField ("Thunder Sound 3", self.thunderSound3, typeof(AudioClip), thunderSound3);
			bool thunderSound4 = !EditorUtility.IsPersistent (self);
			self.thunderSound4 = (AudioClip)EditorGUILayout.ObjectField ("Thunder Sound 4", self.thunderSound4, typeof(AudioClip), thunderSound4);
			bool thunderSound5 = !EditorUtility.IsPersistent (self);
			self.thunderSound5 = (AudioClip)EditorGUILayout.ObjectField ("Thunder Sound 5", self.thunderSound5, typeof(AudioClip), thunderSound5);
			
			EditorGUILayout.Space();
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("This Game Object controls where the lightning strikes happen and should be attached to the position axis of (0,0,0) of your player. UniStorm will randomly spawn lightning strikes around your player.", MessageType.None, true);
			}
			
			bool lightningSpawn = !EditorUtility.IsPersistent (self);
			self.lightningSpawn = (Transform)EditorGUILayout.ObjectField ("Lightning Bolt Spawn", self.lightningSpawn, typeof(Transform), lightningSpawn);
			
			EditorGUILayout.Space();
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("You can add a custom lightning strike here if desired and UniStorm will spawn random strikes according to your player's position.", MessageType.None, true);
			}
			
			bool lightningBolt1 = !EditorUtility.IsPersistent (self);
			self.lightningBolt1 = (GameObject)EditorGUILayout.ObjectField ("Lightning Bolt", self.lightningBolt1, typeof(GameObject), lightningBolt1);
		}


		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_TemperatureOptions = "Show";
		if(self.temperatureOptions)
			showOrHide_TemperatureOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_TemperatureOptions + " Temperature Options"))
		{
			self.temperatureOptions = !self.temperatureOptions;
		}
		
		
		if (self.temperatureOptions)
		{
			//Temperature Options
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Temperature Options", EditorStyles.boldLabel); 
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("With the Temperature Options you can see the current temperature and adjust many temperature related settings.", MessageType.None, true);
			}
			
			EditorGUILayout.Space();
			
			editorTemperature = (TemperatureDropDown)self.temperatureType;
			editorTemperature = (TemperatureDropDown)EditorGUILayout.EnumPopup("Temperature Type", editorTemperature);
			self.temperatureType = (int)editorTemperature;
			
			if (self.temperatureType == 1)
			{
				self.temperature = EditorGUILayout.IntField ("Current Temperature", self.temperature);

				EditorGUILayout.HelpBox("While using the Fahrenheit temperature type, UniStorm will snow at a temperature of 32 degrees or below.", MessageType.Info, true);
			}
			
			if (self.temperatureType == 2)
			{
				self.temperature = EditorGUILayout.IntField ("Current Temperature", self.temperature);

				EditorGUILayout.HelpBox("While using the Celsuis temperature type, UniStorm will snow at a temperature of 0 degrees or below.", MessageType.Info, true);
			}

			EditorGUILayout.Space();
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Here you can adjust the minimum and maximum temperature for each month. UniStorm will generate realistic temperature fluctuations according to your minimum and maximums. This is done both hourly and daily.", MessageType.None, true);
			}
			
			self.startingSpringTemp = EditorGUILayout.IntField ("Starting Spring Temp", self.startingSpringTemp);
			self.minSpringTemp = EditorGUILayout.IntField ("Min Spring", self.minSpringTemp);
			self.maxSpringTemp = EditorGUILayout.IntField ("Max Spring", self.maxSpringTemp);
			EditorGUILayout.Space();
			
			self.startingSummerTemp = EditorGUILayout.IntField ("Starting Summer Temp", self.startingSummerTemp);
			self.minSummerTemp = EditorGUILayout.IntField ("Min Summer", self.minSummerTemp);
			self.maxSummerTemp = EditorGUILayout.IntField ("Max Summer", self.maxSummerTemp);
			EditorGUILayout.Space();
			
			self.startingFallTemp = EditorGUILayout.IntField ("Starting Fall Temp", self.startingFallTemp);
			self.minFallTemp = EditorGUILayout.IntField ("Min Fall", self.minFallTemp);
			self.maxFallTemp = EditorGUILayout.IntField ("Max Fall", self.maxFallTemp);
			EditorGUILayout.Space();
			
			self.startingWinterTemp = EditorGUILayout.IntField ("Starting Winter Temp", self.startingWinterTemp);
			self.minWinterTemp = EditorGUILayout.IntField ("Min Winter", self.minWinterTemp);
			self.maxWinterTemp = EditorGUILayout.IntField ("Max Winter", self.maxWinterTemp);
		}


		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_SunOptions = "Show";
		if(self.sunOptions)
			showOrHide_SunOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_SunOptions + " Sun Options"))
		{
			self.sunOptions = !self.sunOptions;
		}
		
		
		if (self.sunOptions)
		{
			//Sun Intensity
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Sun Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("With the Sun Options you can control things like Sun Rotation, Sun Light and Sun Shadow Intensity, and whether or not you would like to enable or disable shadows for the Sun. Adjusting the Sun Rotation rotates the Sun's rising and setting posistions. You can rotate the Sun 360 degrees to perfectly suit your needs. The Sun Max Intensity is the max intesity the Sun will reach for the day. The Enable Shadows check box controls whether or not the Sun will use shadows.", MessageType.None, true);
			}
			
			EditorGUILayout.Space();		
			EditorGUILayout.Space();
			
			self.maxSunIntensity = EditorGUILayout.Slider ("Max Sun Intensity", self.maxSunIntensity, 0.5f, 4);
			
			EditorGUILayout.Space();
			
			self.sunSize = EditorGUILayout.Slider ("Sun Size", self.sunSize, 0, 0.05f);

			EditorGUILayout.Space();

			self.HeavyRainSunIntensity = EditorGUILayout.Slider ("Stormy Sun Intensity", self.HeavyRainSunIntensity, 0f, 4);

			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Stormy Sun Intensity controls how much sun is aloud during precipitation weather types, including Foggy and Cloudy. This allows the sun to still be shinning when it's raining, snowing, cloudy, or foggy. Keeping the sun on, during precipitation weather types, can improve the shading of objects and the terrain.", MessageType.None, true);
			}
			
			EditorGUILayout.Space();
			
			self.shadowsDuringDay = EditorGUILayout.Toggle ("Shadows Enabled?",self.shadowsDuringDay);
			
			if (self.shadowsDuringDay)
			{
				EditorGUILayout.Space();
				
				editorDayShadowType = (DayShadowTypeDropDown)self.dayShadowType;
				editorDayShadowType = (DayShadowTypeDropDown)EditorGUILayout.EnumPopup("Shadow Type", editorDayShadowType);
				self.dayShadowType = (int)editorDayShadowType;
				
				EditorGUILayout.Space();
				
				self.dayShadowIntensity = EditorGUILayout.Slider ("Shadow Intensity", self.dayShadowIntensity, 0, 1.0f);
			}

			EditorGUILayout.Space();

			self.sunHeight = EditorGUILayout.Slider ("Sun Height", self.sunHeight, 0.5f, 1.2f);
			self.sunAngle = EditorGUILayout.IntSlider ("Sun Rotation", (int)self.sunAngle, -180, 180);
		}


		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_MoonOptions = "Show";
		if(self.moonOptions)
			showOrHide_MoonOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_MoonOptions + " Moon Options"))
		{
			self.moonOptions = !self.moonOptions;
		}
		
		
		if (self.moonOptions)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Moon Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The Moon Options allow you to choose the starting moon phase. There are a total of 8 moon phases that are updated each day. The moon phase will continue to cycle and starts with the moon phase you choose. You can change the materials of the moon pahases and UniStorm will cycle throught them accordingly.", MessageType.None, true);
			}

			EditorGUILayout.Space();
			
			self.moonLightIntensity = EditorGUILayout.Slider ("Normal Moon Intensity", self.moonLightIntensity, 0, 1.0f);

			EditorGUILayout.Space();

			self.stormyMoonLightIntensity = EditorGUILayout.Slider ("Stormy Moon Intensity", self.stormyMoonLightIntensity, 0, 1.0f);

			EditorGUILayout.Space();

			self.moonColor = EditorGUILayout.ColorField("Moon Color", self.moonColor);

			EditorGUILayout.Space();

			self.shadowsDuringNight = EditorGUILayout.Toggle ("Shadows Enabled?",self.shadowsDuringNight);
			
			if (self.shadowsDuringNight)
			{
				EditorGUILayout.Space();
				
				editorNightShadowType = (NightShadowTypeDropDown)self.nightShadowType;
				editorNightShadowType = (NightShadowTypeDropDown)EditorGUILayout.EnumPopup("Shadow Type", editorNightShadowType);
				self.nightShadowType = (int)editorNightShadowType;
				
				EditorGUILayout.Space();
				
				self.nightShadowIntensity = EditorGUILayout.Slider ("Shadow Intensity", self.nightShadowIntensity, 0, 1.0f);
			}
			
			EditorGUILayout.Space();
			
			self.customMoonSize = EditorGUILayout.Toggle ("Customize Moon Size?",self.customMoonSize);
			
			EditorGUILayout.Space();
			
			if (self.customMoonSize)
			{
				self.moonSize = EditorGUILayout.IntSlider ("Moon Size", self.moonSize, 1, 15);
				
				EditorGUILayout.Space();
				
				EditorGUILayout.HelpBox("The Moon's size can be adjust on a scale of 1 to 15. This will change the default setting size of 3.5 to whatever value you use on with slider. ", MessageType.Info, true);
				
				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}
			
			self.customMoonRotation = EditorGUILayout.Toggle ("Customize Moon Rotation?",self.customMoonRotation);
			
			if (self.customMoonRotation)
			{
				self.moonRotationY = EditorGUILayout.Slider ("Moon Rotation", self.moonRotationY, 0, 360);
				
				EditorGUILayout.Space();
				
				EditorGUILayout.HelpBox("The Moon's rotation, on the Z Axis, can be adjust on a scale of 0 to 360. This will change the default setting rotation of 0 to whatever value you use on with slider. The Z Axis adjusts which direction the bright side of the moon faces. ", MessageType.Info, true);
				
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.Space();
			
			editorMoonPhase = (MoonPhaseDropDown)self.moonPhaseCalculator;
			editorMoonPhase = (MoonPhaseDropDown)EditorGUILayout.EnumPopup("Moon Phase", editorMoonPhase);
			self.moonPhaseCalculator = (int)editorMoonPhase;
		}
		

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_PrecipitationOptions = "Show";
		if(self.precipitationOptions)
			showOrHide_PrecipitationOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_PrecipitationOptions + " Precipitation Options"))
		{
			self.precipitationOptions = !self.precipitationOptions;
		}
		
		
		if (self.precipitationOptions)
		{
			//Weather Particle Slider Adjustments Rain
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Precipitation Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The Precipitation Options allow you to set a max number for weather that uses particles. This is useful for keeping draw calls low and keeping the frame rate high. Each game is different so these options are completely customizable.", MessageType.None, true);
			}
			
			EditorGUILayout.Space();
			
			self.randomizedPrecipitation = EditorGUILayout.Toggle ("Randomize Precipitation?",self.randomizedPrecipitation);
			
			EditorGUILayout.Space();
			
			if (self.randomizedPrecipitation)
			{
				EditorGUILayout.HelpBox("Selecting Randomize Precipitation generates new precipitation maxes for every storm. While Randomize Precipitation is selected the maxes below are the caps of the max possible precipitation generation for that weather type.", MessageType.Info, true);
			}
			
			EditorGUILayout.Space();
			
			self.useRainStreaks = EditorGUILayout.Toggle ("Use Rain Streaks?",self.useRainStreaks);
			
			if (self.useRainStreaks)
			{
				EditorGUILayout.HelpBox("While Use Rain Streaks is enabled UniStorm will use the rain streaks particle effect to simulate rain streaks during the heavy rain precipitation weather types.", MessageType.Info, true);
			}
			
			EditorGUILayout.Space();
			
			self.UseRainMist = EditorGUILayout.Toggle ("Use Rain Mist?",self.UseRainMist);
			
			if (self.UseRainMist)
			{
				EditorGUILayout.HelpBox("While Use Rain Mist is enabled UniStorm will use the rain mist particle effect to simulate windy rain during the heavy rain precipitation weather types.", MessageType.Info, true);
			}
			
			EditorGUILayout.Space();
			
			self.UseRainSplashes = EditorGUILayout.Toggle ("Use Rain Splashes?",self.UseRainSplashes);
			
			if (self.UseRainSplashes)
			{
				EditorGUILayout.HelpBox("When using Rain Splashes UniStorm will have splashes spawn where the rain collisions hit. This allows rain splashes to collide with objects and create splash effects.", MessageType.Info, true);
			}
			
			EditorGUILayout.Space();
			
			self.stormControl = EditorGUILayout.Toggle ("Use Precipitation Control?",self.stormControl);
			
			EditorGUILayout.Space();
			
			if (self.stormControl)
			{
				self.forceWeatherChange = EditorGUILayout.IntSlider ("Change Weather Days", (int)self.forceWeatherChange, 1, 7);
				
				EditorGUILayout.HelpBox("When using Precipitation Control UniStorm will change the weather after the set amount of consecutive stormy days has been reached. This is helpful to help control (in rare cases) it raining or snowing for too long.", MessageType.Info, true);
			}
			
			EditorGUILayout.Space();
			
			EditorGUILayout.Space();
			
			self.maxLightRainIntensity = EditorGUILayout.IntSlider ("Light Rain Intensity", (int)self.maxLightRainIntensity, 1, 500);
			self.maxLightRainMistCloudsIntensity = EditorGUILayout.IntSlider ("Light Rain Mist Intensity", (int)self.maxLightRainMistCloudsIntensity, 0, 6);
			self.maxStormRainIntensity = EditorGUILayout.IntSlider ("Heavy Rain Intensity", (int)self.maxStormRainIntensity, 1, 5000);
			self.maxStormMistCloudsIntensity = EditorGUILayout.IntSlider ("Heavy Rain Streaks Intensity", (int)self.maxStormMistCloudsIntensity, 0, 50);
			self.maxHeavyRainMistIntensity = EditorGUILayout.IntSlider ("Heavy Rain Mist Intensity", (int)self.maxHeavyRainMistIntensity, 0, 50);
			
			//Weather Particle Slider Adjustments Snow
			self.maxLightSnowIntensity = EditorGUILayout.IntSlider ("Light Snow Intensity", (int)self.maxLightSnowIntensity, 1, 500);
			self.maxLightSnowDustIntensity = EditorGUILayout.IntSlider ("Light Snow Dust Intensity", (int)self.maxLightSnowDustIntensity, 0, 20);
			self.maxSnowStormIntensity = EditorGUILayout.IntSlider ("Heavy Snow Intensity", (int)self.maxSnowStormIntensity, 1, 3000);
			self.maxHeavySnowDustIntensity = EditorGUILayout.IntSlider ("Heavy Snow Dust Intensity", (int)self.maxHeavySnowDustIntensity, 0, 50);
			
			EditorGUILayout.Space();
			
			self.useCustomPrecipitationSounds = EditorGUILayout.Toggle ("Use Custom Sounds?",self.useCustomPrecipitationSounds);
			
			if (self.useCustomPrecipitationSounds)
			{
				EditorGUILayout.Space();
				
				EditorGUILayout.HelpBox("While Use Custom Sounds is enabled UniStorm will use these sounds for the precipitation noises instead of UniStorm's default sounds. If the audio slots below are empty no sounds will play during precipiation weather types.", MessageType.Info, true);
				
				
				bool customRainSound = !EditorUtility.IsPersistent (self);
				self.customRainSound = (AudioClip)EditorGUILayout.ObjectField ("Rain Sound", self.customRainSound, typeof(AudioClip), customRainSound);
				
				bool customRainWindSound = !EditorUtility.IsPersistent (self);
				self.customRainWindSound = (AudioClip)EditorGUILayout.ObjectField ("Rain Wind Sound", self.customRainWindSound, typeof(AudioClip), customRainWindSound);
				
				bool customSnowWindSound = !EditorUtility.IsPersistent (self);
				self.customSnowWindSound = (AudioClip)EditorGUILayout.ObjectField ("Snow Wind Sound", self.customSnowWindSound, typeof(AudioClip), customSnowWindSound);
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_GUIOptions = "Show";
		if(self.GUIOptions)
			showOrHide_GUIOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_GUIOptions + " GUI Options"))
		{
			self.GUIOptions = !self.GUIOptions;
		}
		
		
		if (self.GUIOptions)
		{
			//GUI Options
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("GUI Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("GUI Options are useful for development and can be enabled and disabled in-game by pressing F12, or for mobile devices pressing 2 fingers on the screen and 3 for disabling it. The checkboxes below control what is turned on when the GUI Options are enabled. If you don't want either on unckeck both checkboxes.", MessageType.None, true);
			}
			
			self.timeScrollBarUseable = EditorGUILayout.Toggle ("Time Scroll Bar",self.timeScrollBarUseable);
			self.weatherCommandPromptUseable = EditorGUILayout.Toggle ("WCPS Enabled",self.weatherCommandPromptUseable);
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_SoundManagerOptions = "Show";
		if(self.soundManagerOptions)
			showOrHide_SoundManagerOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_SoundManagerOptions + " Sound Manager Options"))
		{
			self.soundManagerOptions = !self.soundManagerOptions;
		}
		
		
		if (self.soundManagerOptions)
		{
			//Sound Manager Options
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Sound Manager Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("The Sound Manager allows you to set an array of sounds that will play dynamically for each time of each day according to the min and max seconds set within the editor (One for morning, day, evening, and night) An example for this could be birds in the morning and evening, wind during the day, and crickets at night. UniStorm will pick from a selection of up to 20 sounds (for each time of day) that will play throughout the day and night. You can choose to enable or disable sounds for each time of day using the checkboxes below.", MessageType.None, true);
			}
			
			self.timeToWaitMin = EditorGUILayout.IntField ("Min Wait Time", self.timeToWaitMin);
			self.timeToWaitMax = EditorGUILayout.IntField ("Max Wait Time", self.timeToWaitMax);
			
			EditorGUILayout.Space();
			
			self.useMorningSounds = EditorGUILayout.Toggle ("Use Morning Sounds?",self.useMorningSounds);
			self.useDaySounds = EditorGUILayout.Toggle ("Use Day Sounds?",self.useDaySounds);
			self.useEveningSounds = EditorGUILayout.Toggle ("Use Evening Sounds?",self.useEveningSounds);
			self.useNightSounds = EditorGUILayout.Toggle ("Use Night Sounds?",self.useNightSounds);
			
			EditorGUILayout.Space();
			
			//Sound Manager Lists
			//Morning
			
			if (self.useMorningSounds)
			{
				
				EditorGUILayout.BeginVertical ();
				self.morningSize = EditorGUILayout.IntSlider("Morning Sound Size", self.morningSize, 1, 20);
				
				EditorGUILayout.Space();
				
				if(self.morningSize > self.foldOutList.Count)              //If the counter is greater then foldout count
				{
					var temp = (self.morningSize - self.foldOutList.Count);
					for(int jmorning = 0; jmorning < temp ; jmorning++)
						self.foldOutList.Add(true);                      
				}
				
				if(self.morningSize > self.ambientSoundsMorning.Count)               //If the Slider is higher add more elements.   
				{
					var temp1 = self.morningSize - self.ambientSoundsMorning.Count;
					for(int jmorning = 0; jmorning < temp1 ; jmorning++)
					{
						self.ambientSoundsMorning.Add(new AudioClip() );    //Add a new Audio Clip
					}
				}
				
				if(self.ambientSoundsMorning.Count > self.morningSize)
				{
					self.ambientSoundsMorning.RemoveRange( (self.morningSize), self.ambientSoundsMorning.Count - (self.morningSize)); // If the list is longer then the set morningSize         
					self.foldOutList.RemoveRange( (self.morningSize), self.foldOutList.Count-(self.morningSize));
				}
				
				for(int imorning = 0; imorning < self.ambientSoundsMorning.Count; imorning++)
				{                   
					self.ambientSoundsMorning[imorning] = (AudioClip)EditorGUILayout.ObjectField("Morning Sound " + imorning + ":" , self.ambientSoundsMorning[imorning], typeof(AudioClip), true );
					GUILayout.Space(10);
				}
				EditorGUILayout.EndVertical ();
			}
			
			if (self.useDaySounds)
			{
				//Day
				EditorGUILayout.BeginVertical ();
				self.daySize = EditorGUILayout.IntSlider("Day Sound Size", self.daySize, 1, 20);
				
				EditorGUILayout.Space();
				
				if(self.daySize > self.foldOutList.Count)              //If the counter is greater then foldout count
				{
					var temp2 = (self.daySize - self.foldOutList.Count);
					for(int jday = 0; jday < temp2 ; jday++)
						self.foldOutList.Add(true);                      
				}
				
				if(self.daySize > self.ambientSoundsDay.Count)               //If the Slider is higher add more elements.   
				{
					var temp3 = self.daySize - self.ambientSoundsDay.Count;
					for(int jday = 0; jday < temp3 ; jday++)
					{
						self.ambientSoundsDay.Add(new AudioClip() );    //Add a new Audio Clip
					}
				}
				
				if(self.ambientSoundsDay.Count > self.daySize)
				{
					self.ambientSoundsDay.RemoveRange( (self.daySize), self.ambientSoundsDay.Count - (self.daySize)); // If the list is longer then the set daySize         
					self.foldOutList.RemoveRange( (self.daySize), self.foldOutList.Count-(self.daySize));
				}
				
				for(int iday = 0; iday < self.ambientSoundsDay.Count; iday++)
				{                   
					self.ambientSoundsDay[iday] = (AudioClip)EditorGUILayout.ObjectField("Day Sound " + iday + ":" , self.ambientSoundsDay[iday], typeof(AudioClip), true );
					GUILayout.Space(10);
				}
				EditorGUILayout.EndVertical ();		
			}
			
			if (self.useEveningSounds)
			{
				//Evening
				EditorGUILayout.BeginVertical ();
				self.eveningSize = EditorGUILayout.IntSlider("Evening Sound Size", self.eveningSize, 1, 20);
				
				EditorGUILayout.Space();
				
				if(self.eveningSize > self.foldOutList.Count)              //If the counter is greater then foldout count
				{
					var temp4 = (self.eveningSize - self.foldOutList.Count);
					for(int jevening = 0; jevening < temp4 ; jevening++)
						self.foldOutList.Add(true);                      
				}
				
				if(self.eveningSize > self.ambientSoundsEvening.Count)               //If the Slider is higher add more elements.   
				{
					var temp5 = self.eveningSize - self.ambientSoundsEvening.Count;
					for(int jevening = 0; jevening < temp5 ; jevening++)
					{
						self.ambientSoundsEvening.Add(new AudioClip() );    //Add a new Audio Clip
					}
				}
				
				if(self.ambientSoundsEvening.Count > self.eveningSize)
				{
					self.ambientSoundsEvening.RemoveRange( (self.eveningSize), self.ambientSoundsEvening.Count - (self.eveningSize)); // If the list is longer then the set eveningSize         
					self.foldOutList.RemoveRange( (self.eveningSize), self.foldOutList.Count-(self.eveningSize));
				}
				
				for(int ievening = 0; ievening < self.ambientSoundsEvening.Count; ievening++)
				{                   
					self.ambientSoundsEvening[ievening] = (AudioClip)EditorGUILayout.ObjectField("Evening Sound " + ievening + ":" , self.ambientSoundsEvening[ievening], typeof(AudioClip), true );
					GUILayout.Space(10);
				}
				EditorGUILayout.EndVertical ();
			}
			
			if (self.useNightSounds)
			{
				//Night
				EditorGUILayout.BeginVertical ();
				self.nightSize = EditorGUILayout.IntSlider("Night Sound Size", self.nightSize, 1, 20);
				
				EditorGUILayout.Space();
				
				if(self.nightSize > self.foldOutList.Count)              //If the counter is greater then foldout count
				{
					var temp6 = (self.nightSize - self.foldOutList.Count);
					for(int jnight = 0; jnight < temp6 ; jnight++)
						self.foldOutList.Add(true);                      
				}
				
				if(self.nightSize > self.ambientSoundsNight.Count)               //If the Slider is higher add more elements.   
				{
					var temp7 = self.nightSize - self.ambientSoundsNight.Count;
					for(int jnight = 0; jnight < temp7 ; jnight++)
					{
						self.ambientSoundsNight.Add(new AudioClip() );    //Add a new Audio Clip
					}
				}
				
				if(self.ambientSoundsNight.Count > self.nightSize)
				{
					self.ambientSoundsNight.RemoveRange( (self.nightSize), self.ambientSoundsNight.Count - (self.nightSize)); // If the list is longer then the set nightSize         
					self.foldOutList.RemoveRange( (self.nightSize), self.foldOutList.Count-(self.nightSize));
				}
				
				for(int inight = 0; inight < self.ambientSoundsNight.Count; inight++)
				{                   
					self.ambientSoundsNight[inight] = (AudioClip)EditorGUILayout.ObjectField("Night Sound " + inight + ":" , self.ambientSoundsNight[inight], typeof(AudioClip), true );
					GUILayout.Space(10);
				}
				EditorGUILayout.EndVertical ();		
			}
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		string showOrHide_ColorOptions = "Show";
		if(self.colorOptions)
			showOrHide_ColorOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_ColorOptions + " Color Options"))
		{
			self.colorOptions = !self.colorOptions;
		}
		
		
		if (self.colorOptions)
		{
			EditorGUILayout.LabelField("Color Options", EditorStyles.boldLabel);
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Here you control every color component UniStorm uses. There is one for Morning, Day, Evening, and Night. UniStorm will seamlessly transition to each time of day using the colors you have set for the time of day.", MessageType.None, true);
			}
			
			self.cloudColorMorning = EditorGUILayout.ColorField("Clouds Morning", self.cloudColorMorning);
			self.cloudColorDay = EditorGUILayout.ColorField("Clouds Day", self.cloudColorDay);
			self.cloudColorEvening = EditorGUILayout.ColorField("Clouds Evening", self.cloudColorEvening);
			self.cloudColorNight = EditorGUILayout.ColorField("Clouds Night", self.cloudColorNight);

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			self.MorningAmbientLight = EditorGUILayout.ColorField("Ambient Morning", self.MorningAmbientLight);
			self.MiddayAmbientLight = EditorGUILayout.ColorField("Ambient Day", self.MiddayAmbientLight);
			self.DuskAmbientLight = EditorGUILayout.ColorField("Ambient Evening", self.DuskAmbientLight);
			self.NightAmbientLight = EditorGUILayout.ColorField("Ambient Night", self.NightAmbientLight);
			
			//Sun Colors
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			self.SunMorning = EditorGUILayout.ColorField("Sun Morning", self.SunMorning);
			self.SunDay = EditorGUILayout.ColorField("Sun Day", self.SunDay);
			self.SunDusk = EditorGUILayout.ColorField("Sun Evening", self.SunDusk);
			self.SunNight = EditorGUILayout.ColorField("Sun Night", self.SunNight);
			
			//Normal Fog Color
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			self.fogMorningColor = EditorGUILayout.ColorField("Fog Morning", self.fogMorningColor);
			self.fogDayColor = EditorGUILayout.ColorField("Fog Day", self.fogDayColor);
			self.fogDuskColor = EditorGUILayout.ColorField("Fog Evening", self.fogDuskColor);
			self.fogNightColor = EditorGUILayout.ColorField("Fog Night", self.fogNightColor);
			
			//Added 1.8.1
			//Storm Fog Color
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			self.stormyFogColorMorning = EditorGUILayout.ColorField("Stormy Fog Morning", self.stormyFogColorMorning);
			self.stormyFogColorDay = EditorGUILayout.ColorField("Stormy Fog Day", self.stormyFogColorDay);
			self.stormyFogColorEvening = EditorGUILayout.ColorField("Stormy Fog Evening", self.stormyFogColorEvening);
			self.stormyFogColorNight = EditorGUILayout.ColorField("Stormy Fog Night", self.stormyFogColorNight);
			
			//Atmospheric Light Color
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			self.MorningAtmosphericLight = EditorGUILayout.ColorField("Atmospheric Morning", self.MorningAtmosphericLight);
			self.MiddayAtmosphericLight = EditorGUILayout.ColorField("Atmospheric Day", self.MiddayAtmosphericLight);
			self.DuskAtmosphericLight = EditorGUILayout.ColorField("Atmospheric Evening", self.DuskAtmosphericLight);

			//Global Fog Colors
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			self.stormyFogColorDay_GF = EditorGUILayout.ColorField("Stormy Global Fog Day", self.stormyFogColorDay_GF);
			self.stormyFogColorDuskDawn_GF = EditorGUILayout.ColorField("Stormy Global Fog Morning/Evening", self.stormyFogColorDuskDawn_GF);
			self.stormyFogColorNight_GF = EditorGUILayout.ColorField("Stormy Global Fog Night", self.stormyFogColorNight_GF);
			
			//Star Brightness
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Fade Colors", EditorStyles.boldLabel);
			self.starBrightness = EditorGUILayout.ColorField("Star Brightness", self.starBrightness);
			self.moonFadeColor = EditorGUILayout.ColorField("Moon Fade Color", self.moonFadeColor);
			self.moonColorFade = EditorGUILayout.ColorField("Dark Side Moon", self.moonColorFade);
			
			EditorGUILayout.Space();
		}



		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_ObjectOptions = "Show";
		if(self.objectOptions)
			showOrHide_ObjectOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_ObjectOptions + " Object Options"))
		{
			self.objectOptions = !self.objectOptions;
		}
		
		
		if (self.objectOptions)
		{
			EditorGUILayout.LabelField("Object Fields", EditorStyles.boldLabel);
			
			
			if (showAdvancedOptions == false)
			{
				EditorGUILayout.HelpBox("The viewing of the Object Fields have been disabled. You can enabled them in the Editor Options of the UniStorm Editor.", MessageType.None, true);
			}
			
			if (self.helpOptions == true)
			{
				EditorGUILayout.HelpBox("Here is where all object related UniStorm objects are kept. All components are already applied. If you are missing a component you will be notified with Error Log that will tell you how to fix it. If you are using custom objects refer to UniStorm's Documentation.", MessageType.None, true);
			}
			
			//Sun Objects
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Light Components", EditorStyles.boldLabel);
			bool sunObject = !EditorUtility.IsPersistent (self);
			self.sun = (Light)EditorGUILayout.ObjectField ("Sun Light", self.sun, typeof(Light), sunObject);
			
			bool moonLight = !EditorUtility.IsPersistent (self);
			self.moon = (Light)EditorGUILayout.ObjectField ("Moon Light", self.moon, typeof(Light), moonLight);
			
			bool lightSource = !EditorUtility.IsPersistent (self);
			self.lightSource = (Light)EditorGUILayout.ObjectField ("Light Source", self.lightSource, typeof(Light), lightSource);
			
			//Weather Particle Effects
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Particle Systems", EditorStyles.boldLabel);
			
			bool rainObject = !EditorUtility.IsPersistent (self);
			self.rain = (ParticleSystem)EditorGUILayout.ObjectField ("Rain Particle System", self.rain, typeof(ParticleSystem), rainObject);
			
			bool splashObject = !EditorUtility.IsPersistent (self);
			self.rainSplashes = (ParticleSystem)EditorGUILayout.ObjectField ("Rain Splash System", self.rainSplashes, typeof(ParticleSystem), splashObject);
			
			bool mistFogObject = !EditorUtility.IsPersistent (self);
			self.mistFog = (GameObject)EditorGUILayout.ObjectField ("Rain Streaks Particle System", self.mistFog, typeof(GameObject), mistFogObject);
			
			bool windyRainObject = !EditorUtility.IsPersistent (self);
			self.rainMist = (ParticleSystem)EditorGUILayout.ObjectField ("Rain Mist Particle System", self.rainMist, typeof(ParticleSystem), windyRainObject);
			
			bool snowObject = !EditorUtility.IsPersistent (self);
			self.snow = (ParticleSystem)EditorGUILayout.ObjectField ("Snow Particle System", self.snow, typeof(ParticleSystem), snowObject);
			
			bool snowMistFogObject = !EditorUtility.IsPersistent (self);
			self.snowMistFog = (ParticleSystem)EditorGUILayout.ObjectField ("Snow Dust Particle System", self.snowMistFog, typeof(ParticleSystem), snowMistFogObject);
			
			bool butterflieObject = !EditorUtility.IsPersistent (self);
			self.butterflies = (ParticleSystem)EditorGUILayout.ObjectField ("Lightning Bugs Particle System", self.butterflies, typeof(ParticleSystem), butterflieObject);
			
			bool windyLeavesObject = !EditorUtility.IsPersistent (self);
			self.windyLeaves = (ParticleSystem)EditorGUILayout.ObjectField ("Windy Leaves Particle System", self.windyLeaves, typeof(ParticleSystem), windyLeavesObject);
			
			
			//Sound Objects
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Sound Components", EditorStyles.boldLabel);
			
			bool rainSoundObject = !EditorUtility.IsPersistent (self);
			self.rainSound = (GameObject)EditorGUILayout.ObjectField ("Rain Sound Object", self.rainSound, typeof(GameObject), rainSoundObject);
			
			bool windSoundObject = !EditorUtility.IsPersistent (self);
			self.windSound = (GameObject)EditorGUILayout.ObjectField ("Wind Rain Sound Object", self.windSound, typeof(GameObject), windSoundObject);
			
			bool windSnowSoundObject = !EditorUtility.IsPersistent (self);
			self.windSnowSound = (GameObject)EditorGUILayout.ObjectField ("Wind Snow Sound Object", self.windSnowSound, typeof(GameObject), windSnowSoundObject);
			
			//Sky Objects
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Sky Components", EditorStyles.boldLabel);
			
			bool starObject = !EditorUtility.IsPersistent (self);
			self.starSphere = (GameObject)EditorGUILayout.ObjectField ("Star Sphere", self.starSphere, typeof(GameObject), starObject);
			
			bool moon = !EditorUtility.IsPersistent (self);
			self.moonObject = (GameObject)EditorGUILayout.ObjectField ("Moon Object", self.moonObject, typeof(GameObject), moon);
			
			//Cloud Objects
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Cloud Objects", EditorStyles.boldLabel);
			
			bool cloud1  = !EditorUtility.IsPersistent (self);
			self.lightClouds1 = (GameObject)EditorGUILayout.ObjectField ("Dynamic Light Clouds 1", self.lightClouds1, typeof(GameObject), cloud1);
			
			bool cloud1a  = !EditorUtility.IsPersistent (self);
			self.lightClouds1a = (GameObject)EditorGUILayout.ObjectField ("Dynamic Light Clouds 2", self.lightClouds1a, typeof(GameObject), cloud1a);	

			bool partlyCloudy1  = !EditorUtility.IsPersistent (self);
			self.partlyCloudyClouds1 = (GameObject)EditorGUILayout.ObjectField ("Dynamic Partly Cloudy Clouds 1", self.partlyCloudyClouds1, typeof(GameObject), partlyCloudy1);

			bool partlyCloudy2  = !EditorUtility.IsPersistent (self);
			self.partlyCloudyClouds2 = (GameObject)EditorGUILayout.ObjectField ("Dynamic Partly Cloudy Clouds 2", self.partlyCloudyClouds2, typeof(GameObject), partlyCloudy2);

			bool mostlyCloudy1  = !EditorUtility.IsPersistent (self);
			self.mostlyCloudyClouds1 = (GameObject)EditorGUILayout.ObjectField ("Dynamic Mostly Cloudy Clouds 1", self.mostlyCloudyClouds1, typeof(GameObject), mostlyCloudy1);
			
			bool mostlyCloudy2  = !EditorUtility.IsPersistent (self);
			self.mostlyCloudyClouds2 = (GameObject)EditorGUILayout.ObjectField ("Dynamic Mostly Cloudy Clouds 2", self.mostlyCloudyClouds2, typeof(GameObject), mostlyCloudy2);
			
			bool cloud2  = !EditorUtility.IsPersistent (self);
			self.lightClouds2 = (GameObject)EditorGUILayout.ObjectField ("Non-Dynamic Light Clouds", self.lightClouds2, typeof(GameObject), cloud2);
			
			bool cloud3  = !EditorUtility.IsPersistent (self);
			self.lightClouds3 = (GameObject)EditorGUILayout.ObjectField ("Light Clouds 3", self.lightClouds3, typeof(GameObject), cloud3);
			
			bool cloud4  = !EditorUtility.IsPersistent (self);
			self.lightClouds4 = (GameObject)EditorGUILayout.ObjectField ("Light Clouds 4", self.lightClouds4, typeof(GameObject), cloud4);
			
			bool cloud5  = !EditorUtility.IsPersistent (self);
			self.lightClouds5 = (GameObject)EditorGUILayout.ObjectField ("Light Clouds 5", self.lightClouds5, typeof(GameObject), cloud5);
			
			bool highcloud1  = !EditorUtility.IsPersistent (self);
			self.highClouds1 = (GameObject)EditorGUILayout.ObjectField ("High Clouds 1", self.highClouds1, typeof(GameObject), highcloud1);
			
			bool highcloud2  = !EditorUtility.IsPersistent (self);
			self.highClouds2 = (GameObject)EditorGUILayout.ObjectField ("High Clouds 2", self.highClouds2, typeof(GameObject), highcloud2);
			
			bool mostlyCloudy  = !EditorUtility.IsPersistent (self);
			self.mostlyCloudyClouds = (GameObject)EditorGUILayout.ObjectField ("Mostly Cloudy Clouds", self.mostlyCloudyClouds, typeof(GameObject), mostlyCloudy);
			
			//Heavy Cloud Objects
			bool storm1  = !EditorUtility.IsPersistent (self);
			self.heavyClouds = (GameObject)EditorGUILayout.ObjectField ("Base Storm Clouds", self.heavyClouds, typeof(GameObject), storm1);
			
			bool storm2  = !EditorUtility.IsPersistent (self);
			self.heavyCloudsLayer1 = (GameObject)EditorGUILayout.ObjectField ("Non-Dynamic Storm Clouds", self.heavyCloudsLayer1, typeof(GameObject), storm2);
			
			bool storm3  = !EditorUtility.IsPersistent (self);
			self.heavyCloudsLayerLight = (GameObject)EditorGUILayout.ObjectField ("Dynamic Storm Clouds", self.heavyCloudsLayerLight, typeof(GameObject), storm3);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Unity Components", EditorStyles.boldLabel);
			
			//Camera Object
			bool cameraObjectObject = !EditorUtility.IsPersistent (self);
			self.cameraObject = (GameObject)EditorGUILayout.ObjectField ("Camera Object", self.cameraObject, typeof(GameObject), cameraObjectObject);
			
			bool windZoneObject = !EditorUtility.IsPersistent (self);
			self.windZone = (GameObject)EditorGUILayout.ObjectField ("Wind Zone", self.windZone, typeof(GameObject), windZoneObject);	
			
			//Skybox Materials
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Skybox Material", EditorStyles.boldLabel);
			
			bool SkyBoxMaterial  = !EditorUtility.IsPersistent (self);
			self.SkyBoxMaterial = (Material)EditorGUILayout.ObjectField ("Skybox Material", self.SkyBoxMaterial, typeof(Material), SkyBoxMaterial);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Moon Phase Materials", EditorStyles.boldLabel);
			
			bool moonPhaseMat1  = !EditorUtility.IsPersistent (self);
			self.moonPhase1 = (Material)EditorGUILayout.ObjectField ("Moon Phase Material 1", self.moonPhase1, typeof(Material), moonPhaseMat1);
			
			bool moonPhaseMat2  = !EditorUtility.IsPersistent (self);
			self.moonPhase2 = (Material)EditorGUILayout.ObjectField ("Moon Phase Material 2", self.moonPhase2, typeof(Material), moonPhaseMat2);
			
			bool moonPhaseMat3  = !EditorUtility.IsPersistent (self);
			self.moonPhase3 = (Material)EditorGUILayout.ObjectField ("Moon Phase Material 3", self.moonPhase3, typeof(Material), moonPhaseMat3);	
			
			bool moonPhaseMat4  = !EditorUtility.IsPersistent (self);
			self.moonPhase4 = (Material)EditorGUILayout.ObjectField ("Moon Phase Material 4", self.moonPhase4, typeof(Material), moonPhaseMat4);
			
			bool moonPhaseMat5  = !EditorUtility.IsPersistent (self);
			self.moonPhase5 = (Material)EditorGUILayout.ObjectField ("Moon Phase Material 5", self.moonPhase5, typeof(Material), moonPhaseMat5);
			
			bool moonPhaseMat6  = !EditorUtility.IsPersistent (self);
			self.moonPhase6 = (Material)EditorGUILayout.ObjectField ("Moon Phase Material 6", self.moonPhase6, typeof(Material), moonPhaseMat6);
			
			bool moonPhaseMat7  = !EditorUtility.IsPersistent (self);
			self.moonPhase7 = (Material)EditorGUILayout.ObjectField ("Moon Phase Material 7", self.moonPhase7, typeof(Material), moonPhaseMat7);
			
			bool moonPhaseMat8  = !EditorUtility.IsPersistent (self);
			self.moonPhase8 = (Material)EditorGUILayout.ObjectField ("Moon Phase Material 8", self.moonPhase8, typeof(Material), moonPhaseMat8);
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		string showOrHide_HelpOptions = "Show";
		if(self.helpOptions)
			showOrHide_HelpOptions = "Hide";
		
		if(GUILayout.Button(showOrHide_HelpOptions + " Help Options"))
		{
			self.helpOptions = !self.helpOptions;
		}
		
		
		if (self.helpOptions)
		{

		}
		
		GUILayout.BeginHorizontal();
		
		
		GUILayout.EndHorizontal();

		//Added 1.8.2
		//UniStorm will no longer revert to prefab settings
		if (GUI.changed) 
		{ 
			EditorUtility.SetDirty(self); 
		}

		serializedObject.ApplyModifiedProperties ();


	}
	
}