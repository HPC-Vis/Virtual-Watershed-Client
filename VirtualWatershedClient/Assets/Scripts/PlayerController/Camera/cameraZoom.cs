using UnityEngine;
using System.Collections;

public class cameraZoom : MonoBehaviour {
    public Camera cam;
    public GameObject cursor;
    public float zoomSpeed = 20f;
    public float minZoomFOV = 5f;
    public float maxZoomFOV = 60f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(cursor.GetComponent<mouselistener>().State == cursor.GetComponent<mouselistener>().states[1])
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                ZoomIn();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                ZoomOut();
            }
        }
	        
	}

    public void ZoomIn()
    {
        cam.fieldOfView -= zoomSpeed / 4;
        if (cam.fieldOfView < minZoomFOV)
        {
            cam.fieldOfView = minZoomFOV;
        }
    }

    public void ZoomOut()
    {
        cam.fieldOfView += zoomSpeed / 4;
        if (cam.fieldOfView > maxZoomFOV)
        {
            cam.fieldOfView = maxZoomFOV;
        }
    }

}
