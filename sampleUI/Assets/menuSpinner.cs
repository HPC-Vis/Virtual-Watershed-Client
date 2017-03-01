using UnityEngine;
using System.Collections;
//
//	1) In your scene you should have controllers attached to the camera rig, eg:
//	[CameraRig]
//	-- Controller (Left)
//
//	2) Ensure that controller has both a "SteamVR_TrackedObject" script AND "SteamVR_TrackedController" script
//
//	3) Add this script to the controller, and modify it as necessary
//
[RequireComponent(typeof(SteamVR_TrackedController))]
public class menuSpinner : MonoBehaviour
{

    public GameObject menu;
    private bool clicked = false;

	// Use this for initialization
	void OnEnable ()
    {
        SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();

        if (!clicked)
        {
            controller.PadClicked += OnPadClicked;
            clicked = true;
        }

        if (!controller.padPressed)
        {
            clicked = false;
        }

	}
	
	void OnDisable()
    {
		SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();
		controller.PadClicked -= OnPadClicked;
	}

	void OnPadClicked(object sender, ClickedEventArgs e)
    {
        if (e.padX < 0)
        {
            menu.transform.Rotate(0.0f, 90.0f, 0.0f);
        }
        else if (e.padX >= 0)
        {
            menu.transform.Rotate(0.0f, -90.0f, 0.0f);
        }
	}
	
}
