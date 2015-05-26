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
		Vector2 Point = TerrainUtils.NormalizePointToTerrain(this.transform.position, GlobalConfig.TerrainBoundingBox);
		Lat.text = Point.x.ToString ();
		Long.text = Point.y.ToString ();
		//Lat.text = Get from scene;
		//Long.text = Get From Scene;
		if (Terrain.activeTerrain != null) {
			Ele.text = ((mouseray.raycastHitFurtherest (this.transform.position, Vector3.up).y) + Terrain.activeTerrain.transform.position.y).ToString ();
		}
	}
}
