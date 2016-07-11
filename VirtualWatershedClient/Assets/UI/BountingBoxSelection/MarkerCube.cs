using UnityEngine;
using System.Collections;

public class MarkerCube : MonoBehaviour {
    // public Bou
    public GameObject marker;

	// Use this for initialization
	void Start () {
	
	}
    public void addPin(Vector3 pos)
    {
        marker.transform.position = new Vector3(pos.x, pos.y, pos.z);
        return;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
