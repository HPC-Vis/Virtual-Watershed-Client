//UniStorm Weather System Editor JavaScript Version 1.8.3 @ Copyright
//Black Horizon Studios

@ExecuteInEditMode

@DrawGizmo (GizmoType.Selected)
@CustomEditor (WeatherZoneExample_JS)
class WeatherZoneExampleEditor_JS extends Editor {

var weatherType : String = "";

	  enum WeatherTypeDropDown
		{
			Foggy = 1, 
			LightRainOrLightSnowWinterOnly = 2, 
			ThunderStormOrSnowStormWinterOnly = 3, 
			PartlyCloud1 = 4, 
			PartlyCloud2 = 5, 
			PartlyCloud3 = 6, 
			Clear1 = 7,
			Clear2 = 8, 
			Cloudy = 9, 
			ButterfliesSummerOnly = 10,
			MostlyCloudy = 11, 
			HeavyRainNoThunder = 12,  
			FallingLeavesFallOnly = 13
		}
		
		public var editorWeatherType = WeatherTypeDropDown.PartlyCloud1;
	
		
 function OnInspectorGUI () {
    
    //Time Number Variables
    	EditorGUILayout.LabelField("UniStorm Zone Weather System", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("By: Black Horizon Studios", EditorStyles.label);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Zone Weather Options", EditorStyles.boldLabel);
		editorWeatherType = target.zoneWeather;
		editorWeatherType = EditorGUILayout.EnumPopup("Zone Weather Type", editorWeatherType);
    	target.zoneWeather = editorWeatherType;
    	
    	//var mostlyCloudy : boolean = !EditorUtility.IsPersistent (target);
        //target.uniStormSystem = EditorGUILayout.ObjectField ("UniStorm System", target.uniStormSystem, GameObject, mostlyCloudy);
    
    }
    
    
    function OnSceneGUI () {
    
    	if (editorWeatherType == WeatherTypeDropDown.Foggy)
		{
			weatherType = "Foggy";
		}
	
		if (editorWeatherType == WeatherTypeDropDown.LightRainOrLightSnowWinterOnly)
		{
			weatherType = "Light Rain/Light Snow";
		}
	
		if (editorWeatherType == WeatherTypeDropDown.ThunderStormOrSnowStormWinterOnly)
		{
			weatherType = "Thunder Storms/Heavy Snow";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.PartlyCloud1)
		{
			weatherType = "Partly Cloud 1";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.PartlyCloud2)
		{
			weatherType = "Partly Cloud 2";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.PartlyCloud3)
		{
			weatherType = "Partly Cloud 3";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.Clear1)
		{
			weatherType = "Clear 1";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.Clear2)
		{
			weatherType = "Clear 2";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.ButterfliesSummerOnly)
		{
			weatherType = "Butterflies (Summer Only)";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.FallingLeavesFallOnly)
		{
			weatherType = "Falling Leaves (Fall Only)";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.HeavyRainNoThunder)
		{
			weatherType = "Heavy Rain No Thunder";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.Cloudy)
		{
			weatherType = "Cloudy";
		}
		
		if (editorWeatherType == WeatherTypeDropDown.MostlyCloudy)
		{
			weatherType = "Mostly Cloudy";
		}
		

	    	Handles.color = Color.blue;
	 		Handles.Label(target.transform.position + Vector3.up*100, "Weather Type: " + weatherType);
	 		Handles.color = Color.blue;
	 		
	 		
	 		if (GUI.changed) 
			{ 
				EditorUtility.SetDirty(target); 
			}
	 		
	    }
	    
    
 }
