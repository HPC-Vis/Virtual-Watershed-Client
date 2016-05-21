using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class fastTravel : MonoBehaviour {

    public Camera miniMap;
    public GameObject player;
    public GameObject playerMarker;

    private Ray ray;
    private RaycastHit hit;

    // To be moved....
    public Text Elevation;
    public Text Northsouth;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (Terrain.activeTerrains.Length == 1)
        {
            // Orthographic does everything with respect to some center)..
            transform.position = new Vector3(0, 10000, 0);
            cam.orthographicSize = Mathf.Max(Mathf.Abs(Terrain.activeTerrain.transform.position.x), Mathf.Abs(Terrain.activeTerrain.transform.position.z) );
        }
        playerMarker.transform.localScale = new Vector3(cam.orthographicSize * .1f,1, cam.orthographicSize * .1f);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            ray = miniMap.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Clicked on Minimap");
                player.transform.position = hit.point+new Vector3(0,10,0);
            }

        }
        playerMarker.transform.position = new Vector3(player.transform.position.x, transform.position.y - 10, player.transform.position.z);

        /// lat long
        var LatLong = coordsystem.transformToWorld(player.transform.position);

        // Elevation
        float elevation = player.transform.position.y;
        Elevation.text = elevation + "m";
        Northsouth.text = LatLong.z.ToString() + " N , " + LatLong.x.ToString() + " E";

        //mouseray.raycastHitFurtherest(player.transform.position,Vector3.down)
	}
}
