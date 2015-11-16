using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode()] 
[DrawGizmo (GizmoType.Selected)]

[CustomEditor(typeof(WeatherZoneExample_C))] 
public class WeatherZoneEditor_C : Editor {

	public string weatherType = "";

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
		
		WeatherTypeDropDown editorWeatherType = WeatherTypeDropDown.PartlyCloud1;
	
		
 		public override void OnInspectorGUI () {
		
		WeatherZoneExample_C self = (WeatherZoneExample_C)target;
    
    	//Time Number Variables
    	EditorGUILayout.LabelField("UniStorm Zone Weather System", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("By: Black Horizon Studios", EditorStyles.label);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Zone Weather Options", EditorStyles.boldLabel);
		editorWeatherType = (WeatherTypeDropDown)self.zoneWeather;
		editorWeatherType = (WeatherTypeDropDown)EditorGUILayout.EnumPopup("Zone Weather Type", editorWeatherType);
    	self.zoneWeather = (int)editorWeatherType;
    	
		if (GUI.changed) 
		{ 
			EditorUtility.SetDirty(self); 
		}
    
    }
    
    /*
    void OnSceneGUI () {
    
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
	 		Handles.Label(self.transform.position + Vector3.up*100, "Weather Type: " + weatherType);
	 		Handles.color = Color.blue;
	 		
	 		
	    }
	    */
}
