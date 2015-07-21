
using UnityEditor;
using UnityEngine;
public class UniStormMenu : MonoBehaviour {
	
		
    // Add UniStorm to the menu bar.
    [MenuItem ("Window/UniStorm/Create Weather System/JavaScript")]
    static void InstantiateJavaScriptVersion () {
        
	   Selection.activeObject = SceneView.currentDrawingSceneView;
		
	   GameObject codeInstantiatedPrefab = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/UniStorm/Prefabs/UniStormPrefabs/UniStormPrefab_JS_Basic.prefab", typeof(GameObject))) as GameObject;
		
       codeInstantiatedPrefab.name = "UniStormPrefab_JS_Basic";
       Selection.activeGameObject = codeInstantiatedPrefab;
    }
	
	[MenuItem ("Window/UniStorm/Create Weather System/C#")]
    static void InstantiateCVersion () {	
	   Selection.activeObject = SceneView.currentDrawingSceneView;
		
	   GameObject codeInstantiatedPrefab = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/UniStorm/Prefabs/UniStormPrefabs/UniStormPrefab_C_Basic.prefab", typeof(GameObject))) as GameObject;
		
       codeInstantiatedPrefab.name = "UniStormPrefab_C_Basic";
       Selection.activeGameObject = codeInstantiatedPrefab;
    }
	
	[MenuItem ("Window/UniStorm/Create Weather Zone/JavaScript")]
    static void InstantiateWeatherZoneJS () {

		Selection.activeObject = SceneView.currentDrawingSceneView;
        Camera sceneCam = SceneView.currentDrawingSceneView.camera;
        Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,10f));
		
		GameObject codeInstantiatedPrefab = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/UniStorm/Prefabs/UniStormPrefabs/WeatherZone_JS.prefab", typeof(GameObject))) as GameObject;

		codeInstantiatedPrefab.transform.position = new Vector3 (spawnPos.x, spawnPos.y, spawnPos.z);
		
		codeInstantiatedPrefab.name = "WeatherZone_JS";
    }
	
	[MenuItem ("Window/UniStorm/Create Weather Zone/C#")]
    static void InstantiateWeatherZoneC () {
		
		Selection.activeObject = SceneView.currentDrawingSceneView;
        Camera sceneCam = SceneView.currentDrawingSceneView.camera;
        Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,10f));
		
		GameObject codeInstantiatedPrefab = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/UniStorm/Prefabs/UniStormPrefabs/WeatherZone_C.prefab", typeof(GameObject))) as GameObject;
		codeInstantiatedPrefab.transform.position = new Vector3 (spawnPos.x, spawnPos.y, spawnPos.z);
		
		codeInstantiatedPrefab.name = "WeatherZone_C";
    }
	
	
	[MenuItem ("Window/UniStorm/Create Time Event/JavaScript")]
    static void InstantiateTimeEventJS () {
		GameObject codeInstantiatedPrefab = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/UniStorm/Prefabs/UniStormPrefabs/TimeEvent_JS.prefab", typeof(GameObject))) as GameObject;
		codeInstantiatedPrefab.name = "TimeEvent_JS";
    }
	
	[MenuItem ("Window/UniStorm/Create Time Event/C#")]
    static void InstantiateTimeEventC () {
		GameObject codeInstantiatedPrefab = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/UniStorm/Prefabs/UniStormPrefabs/TimeEvent_C.prefab", typeof(GameObject))) as GameObject;
		codeInstantiatedPrefab.name = "TimeEvent_C";
    }
	
   
    [MenuItem ("Window/UniStorm/Documentation/Home")]
    static void Home ()
    {
		Application.OpenURL("http://unistorm-weather-system.wikia.com/wiki/Home");
    }
	
	[MenuItem ("Window/UniStorm/Documentation/Introduction")]
    static void Introduction ()
    {
		Application.OpenURL("http://unistorm-weather-system.wikia.com/wiki/Introduction");
    }
	
	[MenuItem ("Window/UniStorm/Documentation/Tutorials")]
    static void Tutorials ()
    {
		Application.OpenURL("http://unistorm-weather-system.wikia.com/wiki/Tutorials");
    }
	
	[MenuItem ("Window/UniStorm/Documentation/Code References")]
    static void CodeReferences ()
    {
		Application.OpenURL("http://unistorm-weather-system.wikia.com/wiki/Code_References");
    }
	
	[MenuItem ("Window/UniStorm/Documentation/Example Scripts")]
    static void ExampleScripts ()
    {
		Application.OpenURL("http://unistorm-weather-system.wikia.com/wiki/Example_Scripts");
    }
	
	[MenuItem ("Window/UniStorm/Documentation/Solutions to Possible Issues")]
    static void Solutions ()
    {
		Application.OpenURL("http://unistorm-weather-system.wikia.com/wiki/Solutions_to_Possible_Issues");
    }

	[MenuItem ("Window/UniStorm/Documentation/Realease Notes")]
    static void PatchNotes ()
    {
		Application.OpenURL("http://unistorm-weather-system.wikia.com/wiki/UniStorm_Patch_Notes");
    }

	[MenuItem ("Window/UniStorm/Beta Features/Test Beta Features")]
	static void DynamicWind ()
	{
		Application.OpenURL("http://unistorm-weather-system.wikia.com/wiki/Beta_Features");
	}

	[MenuItem ("Window/UniStorm/Documentation/Forums")]
	static void Forums ()
	{
		Application.OpenURL("http://forum.unity3d.com/threads/unistorm-v2-0-dynamic-day-night-weather-system-released-now-with-playable-demo.121021/");
	}
	
	[MenuItem ("Window/UniStorm/Customer Service")]
    static void CustomerService ()
    {
		Application.OpenURL("http://blackhorizonstudios.webs.com/customersupport.htm");
    }
	
}

/* 
	   Selection.activeObject = SceneView.currentDrawingSceneView;
       Camera sceneCam = SceneView.currentDrawingSceneView.camera;
       Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,10f));
		
	   GameObject codeInstantiatedPrefab = GameObject.Instantiate( Resources.LoadAssetAtPath("Assets/UniStorm/Prefabs/UniStormPrefabs/UniStormPrefab_JS_New.prefab", typeof(GameObject)), spawnPos, Quaternion.identity) as GameObject;
		
       //GameObject _go = Instantiate (Resources.Load ("Assets/UniStorm/Prefabs/UniStormPrefabs/UniStormPrefab_JS.prefab"), spawnPos, Quaternion.identity) as GameObject;
       codeInstantiatedPrefab.name = "UniStormPrefab_JS";
       Selection.activeGameObject = codeInstantiatedPrefab;
        //GameObject _go = Instantiate (Resources.Load ("Assets/UniStorm/Prefabs/UniStormPrefabs/UniStormPrefab_JS.prefab"), spawnPos, Quaternion.identity) as GameObject;
       */