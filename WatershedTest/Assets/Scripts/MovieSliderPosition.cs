using UnityEngine;
using System.Collections;

public class MovieSliderPosition : MonoBehaviour {

    public GameObject vizWindow;
    public GameObject handle;


	// Use this for initialization
	void Start () {
        vizWindow = GameObject.Find("DataViz panel");
        handle = GameObject.Find("SliderHandle");
	}
	
	// Update is called once per frame
	void Update () {
        vizWindow.transform.position = new Vector3(handle.transform.position.x, handle.transform.position.y + 100);
	}
}
