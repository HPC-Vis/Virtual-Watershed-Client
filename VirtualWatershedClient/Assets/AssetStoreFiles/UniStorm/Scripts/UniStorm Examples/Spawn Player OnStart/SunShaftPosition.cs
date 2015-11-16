//This is needed to get the Sun Shaft Position for UniStorm's sun
//Attached this to the camera that is using UniStorm

using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class SunShaftPosition : MonoBehaviour {

	public SunShafts sunShaftScript;
	public GameObject uniStormSun;

	void Start () 
	{
		uniStormSun = GameObject.Find("SunGlow");
		sunShaftScript = GetComponent<Camera>().GetComponent<SunShafts>();
		sunShaftScript.sunTransform = uniStormSun.transform;
	}

	void Update () {
	
	}
}
