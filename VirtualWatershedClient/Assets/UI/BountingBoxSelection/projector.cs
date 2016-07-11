using UnityEngine;
using System.Collections;

public class projector : MonoBehaviour {
    public Projector proj;

	// Use this for initialization
	void Start () {
        proj.GetComponent<Projector>();
        proj.farClipPlane = 10;
        	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
