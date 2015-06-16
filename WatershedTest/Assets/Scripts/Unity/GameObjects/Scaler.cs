using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour {

    GameObject currentController;
    public GameObject FirstPersonControler;
    public GameObject NoClipGhostPlayer;
    float OriginalHeight;
	// Use this for initialization
	void Start () {
        OriginalHeight = gameObject.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        ResizeObject();
	}

    void ResizeObject()
    {
        GameObject currentController;
        float distance;
        float yThresh, xzThresh;



        if (NoClipGhostPlayer.activeSelf)
        {
            currentController = NoClipGhostPlayer;
        }
        else
        {
            currentController = FirstPersonControler;
        }

        distance = (gameObject.transform.position - currentController.transform.position).magnitude;

        if (distance / 150 < 30.14)
        {
            yThresh = 30.14f;
        }
        else
        {
            yThresh = distance / 10;
        }

        if (distance / 150 < 5.14)
        {
            xzThresh = 5.14f;
        }
        else
        {
            xzThresh = distance / 75;
        }

        //OriginalHeight
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, OriginalHeight + (yThresh), gameObject.transform.position.z);
        gameObject.transform.localScale = new Vector3(xzThresh, yThresh, xzThresh);
    }
}
