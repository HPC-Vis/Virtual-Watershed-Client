using UnityEngine;
using UnityEngine.UI;

public class WatershedcatalogSearch : MonoBehaviour {

    public Image terrainimage;

	// Use this for initialization
	void Start ()
    {
        terrainimage.sprite = GlobalConfig.terrainImage;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        GameObject terrains = GameObject.Find("Terrains");
        foreach(Transform terrain in terrains.GetComponentInChildren<Transform>())
        {
            terrain.gameObject.SetActive(false);
        }
    }
}
