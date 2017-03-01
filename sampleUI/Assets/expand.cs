using UnityEngine;
using System.Collections;

public class expand : MonoBehaviour {

    public Light backLight;
    public SteamVR_TrackedController controller;
    public GameObject prefab;

	// Use this for initialization
	void Start () {
        
        backLight.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "menuToucher")
        {
            backLight.enabled = true;
            Debug.Log("onenter");
        }
    }

    void OnTriggerStay (Collider other)
    {
        if (other.tag == "menuToucher")
        {
            bool isPressed = false;
            Debug.Log("stay");

            if (controller.gripped && isPressed == false)
            {
                isPressed = true;
                Debug.Log("stay");
                for (int i = 0; i < 10; i++)
                {
                    Instantiate(prefab, new Vector3(i * .4f, .4f, 0.0f), Quaternion.identity);
                }
            }
        }
    }

    
}
