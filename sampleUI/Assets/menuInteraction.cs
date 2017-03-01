using UnityEngine;
using System.Collections;



public class menuInteraction : MonoBehaviour {

    public SteamVR_TrackedObject trackedObj;
    private bool isTouched = false;
    private bool held = false;
    private Light changeMe;
    private Color inactive = Color.green;
    private Color interactable = Color.red;
    private Color active = Color.yellow;

    // Use this for initialization
    void Start ()
    {
	
	}


    // Update is called once per frame
    void FixedUpdate ()
    {

        var device = SteamVR_Controller.Input((int)trackedObj.index);


        if (isTouched == device.GetTouch(SteamVR_Controller.ButtonMask.Grip) && isTouched != false)
        {
            held = true;
        }
        else
        {
            held = false;
        }

        isTouched = device.GetTouch(SteamVR_Controller.ButtonMask.Grip);

	}

    void OnTriggerEnter (Collider other)
    {
       if (other.gameObject.CompareTag("menuToucher"))
        {
            changeMe = other.gameObject.GetComponent<Light>();
            if (changeMe.color != active)
            {
                changeMe.color = interactable;
            }
        }
    }

    void OnTriggerStay (Collider other)
    {
        if (isTouched)
        {
            if (other.gameObject.CompareTag("menuToucher"))
            {
                changeMe = other.gameObject.GetComponent<Light>();

                if (changeMe.color == interactable && !held)
                {
                    activateChildren(other.gameObject);
                    changeMe.color = active;
                }
                else if (changeMe.color == active && !held)
                {
                    deactivateChildren(other.gameObject);
                    changeMe.color = interactable;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("menuToucher"))
        {
            changeMe = other.gameObject.GetComponent<Light>();

            //if the color is red then the menu isnt active
            if (changeMe.color == interactable)
            {
                changeMe.color = inactive;
            }
        }
    }

    private void activateChildren(GameObject parent)
    {
        GameObject child;
        for (int counter = 0; counter < parent.transform.childCount; counter++)
        {
            child = parent.transform.GetChild(counter).gameObject;
            child.SetActive(true);
        }
    }

    private void deactivateChildren(GameObject parent)
    {
        GameObject child;
        for (int counter = 0; counter < parent.transform.childCount; counter++)
        {
            child = parent.transform.GetChild(counter).gameObject;
            child.SetActive(false);
        }
    }
}
