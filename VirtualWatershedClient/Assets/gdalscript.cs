using UnityEngine;
using System.Collections;

public class gdalscript : MonoBehaviour {
    public UnityEngine.UI.Text test;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        test.text = OSGeo.GDAL.Gdal.GetDriverCount().ToString();
	}
}
