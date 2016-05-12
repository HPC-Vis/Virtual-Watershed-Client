using UnityEngine;
using System.Collections;

public class TerrainGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        GameObject terrains = GameObject.Find("Terrains");
        foreach (Transform terrain in terrains.GetComponentInChildren<Transform>())
        {
            terrain.gameObject.SetActive(true);
        }
    }
}
