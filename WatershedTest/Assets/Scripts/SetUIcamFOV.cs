using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetUIcamFOV : MonoBehaviour 
{
	private Canvas uiCanvas;
	private RectTransform canvasRectTransform;
	
	private float aspect_ratio;
	private float size;
	
	// Use this for initialization
	void Start () 
	{
//		uiCanvas = GameObject.FindGameObjectWithTag ("TheUICanvas");
//		canvasRectTransform = uiCanvas.transform as RectTransform;
		
		aspect_ratio = (float)Screen.width / (float)Screen.height;
		size = -40.855f * aspect_ratio + 126.16f;
		GetComponent<Camera>().orthographicSize = size;
		
//		canvasRectTransform.sizeDelta (1080f * aspect_ratio, 1080f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		aspect_ratio = (float)Screen.width / (float)Screen.height;
		size = -40.855f * aspect_ratio + 126.16f;
		GetComponent<Camera>().orthographicSize = size;
		
	}
}