using UnityEngine;
using System.Collections;

public class fastTravel : MonoBehaviour {

    public Camera miniMap;
    public GameObject player;

    private Ray ray;
    private RaycastHit hit;

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
        
	}
}
