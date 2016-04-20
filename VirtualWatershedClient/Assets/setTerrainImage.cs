using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class setTerrainImage : MonoBehaviour {

    public Image terrain;

	// Use this for initialization
	void Start () {
        terrain.sprite = GlobalConfig.terrainImage;    
	}
	
}
