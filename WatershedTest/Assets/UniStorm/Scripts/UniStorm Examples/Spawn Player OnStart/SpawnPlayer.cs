using UnityEngine;
using System.Collections;

public class SpawnPlayer : MonoBehaviour {


	public GameObject playerObject;


	// Use this for initialization
	void Awake () 
	{
			Instantiate(playerObject, transform.position, transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
