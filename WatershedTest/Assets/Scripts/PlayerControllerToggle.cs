using UnityEngine;
using System.Collections;

public class PlayerControllerToggle : MonoBehaviour {

    public bool ghost;
    public bool guiCursor;
    public GameObject firstPerson;
    public GameObject ghostPerson;
    Camera fpsCam, ghostCam; 

	// Use this for initialization
	void Start () {
        ghost = true;
        guiCursor = false;
		if (Application.loadedLevel == 1) {
            firstPerson.transform.position = mouseray.raycastHitFurtherest(Vector3.zero, Vector3.up, -10000);
			// firstPerson.transform.position = new Vector3 (400, 2000, 400);
		} else {
            firstPerson.transform.position = mouseray.raycastHitFurtherest(Vector3.zero, Vector3.up, -10000);
			// firstPerson.transform.position = new Vector3 (400, 575, 400);
		}
        fpsCam = firstPerson.GetComponentInChildren<Camera>();
        ghostCam = ghostPerson.GetComponentInChildren<Camera>();
        toggleGhosting();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            toggleGhosting();
        }
        if (ghost)
        {
            firstPerson.transform.position = ghostPerson.transform.position;
            fpsCam.transform.rotation = ghostCam.transform.rotation;
            fpsCam.transform.position = ghostCam.transform.position;

        }
        else
        {
            ghostPerson.transform.position = firstPerson.transform.position;
            ghostCam.transform.rotation = fpsCam.transform.rotation;
            ghostCam.transform.position = fpsCam.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (guiCursor)
            {
                ghostPerson.GetComponent<NoClipMouseLook>().enabled = (true);
                firstPerson.GetComponent<MouseLook>().enabled = (true);
                ghostCam.GetComponent<MouseLook>().enabled = (true);
                fpsCam.GetComponent<MouseLook>().enabled = (true);

                guiCursor = false;
            }
            else
            {
                ghostPerson.GetComponent<NoClipMouseLook>().enabled = (false);
                firstPerson.GetComponent<MouseLook>().enabled = (false);
                ghostCam.GetComponent<MouseLook>().enabled = (false);
                fpsCam.GetComponent<MouseLook>().enabled = (false);

                guiCursor = true;
            }
        }
    }

    void toggleGhosting()
    {
        if (ghost)
        {
            ghostCam.enabled = (false);
            fpsCam.enabled = (true);
            ghost = false;
        }
        else
        {
            fpsCam.enabled = (false);
            ghostCam.enabled = (true);
            ghost = true;
        }
    }
}
