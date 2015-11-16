using UnityEngine;
using System.Collections;

public class MinimapPlayer : MonoBehaviour {

    private CharacterController characterController;
    private string characterControllerTag = "CharacterController";

    public float xmin = 0;
    public float xmax = 2000;
    public float zmin = 0;
    public float zmax = 2000;

    private float xrange, zrange;

    private RectTransform parentRectTransform;
    private RectTransform thisRectTransform;

	// Use this for initialization
	void Start () {
        xrange = xmax - xmin;
        zrange = zmax - zmin;

        characterController = 
            GameObject.FindGameObjectWithTag(characterControllerTag)
                      .GetComponent<CharacterController>();

        parentRectTransform = transform.parent.transform as RectTransform;
        thisRectTransform = transform as RectTransform;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = characterController.transform.position;

        Vector3 newPos = new Vector3(400 * (pos.x - xmin) / xrange - 200,
                                     400 * (pos.z - zmin) / zrange - 200, 0f);

        thisRectTransform.localPosition = newPos;


        float rot = -characterController.transform.rotation.eulerAngles.y;
        Vector3 newRot= new Vector3(0f, 0f, rot);
        
        thisRectTransform.localRotation = Quaternion.Euler(newRot);
        
	}
}
