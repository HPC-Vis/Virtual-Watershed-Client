using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	[Range (-2f,2f)]
	public float rotateSpeed = 0.1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
	}
}
