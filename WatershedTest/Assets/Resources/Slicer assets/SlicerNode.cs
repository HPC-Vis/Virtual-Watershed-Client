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
		Lat.fontSize = 14;
		Lat.text = Latinput;

		Long.fontSize = 14;
		Long.text = Longinput;

		Ele.fontSize = 14;
		Ele.text = Elevation;


	}
	
	// Update is called once per frame
	void Update () {
		Vector3 Point = coordsystem.transformToWorld(this.transform.position);
		Lat.text = Point.x.ToString ();
		Long.text = Point.z.ToString ();
		//Lat.text = Get from scene;
		//Long.text = Get From Scene;
		if (Terrain.activeTerrain != null) {
			float elevation;
			if(Terrain.activeTerrain.transform.position.y > 0){
				elevation = ((mouseray.raycastHitFurtherest (this.transform.position, Vector3.up).y) +  Terrain.activeTerrain.transform.position.y);
			}
			else{
				elevation = ((mouseray.raycastHitFurtherest (this.transform.position, Vector3.up).y) +  (-1)*Terrain.activeTerrain.transform.position.y);
			}
			Ele.text = elevation.ToString ();
		}
	}
}
