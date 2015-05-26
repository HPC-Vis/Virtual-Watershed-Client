using UnityEngine;
using System.Collections;

public class SlicerNode : MonoBehaviour {
	public string Latinput;
	public string Longinput;
	public string Elevation;
	public TextMesh Lat;
	public TextMesh Long;
	public TextMesh Ele;
	// Use this for initialization
	void Start () {
		//TextMesh Text = (TextMesh)GetComponent (typeof(TextMesh));
		Lat.text = Latinput;
		Long.text = Longinput;
		Ele.text = Elevation;
	}
	
	// Update is called once per frame
	void Update () {
		//Lat.text = Get from scene;
		//Long.text = Get From Scene;
		Ele.text = (mouseray.raycastHitFurtherest (this.transform.position, Vector3.up).y).ToString ();
		Debug.Log ((mouseray.raycastHitFurtherest (this.transform.position, Vector3.up).y).ToString ());
	}
}
