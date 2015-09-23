using UnityEngine;
using System.Collections;

public class ScenePopulator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadDryCreek() {
        Debug.LogError("Load Dry Creek");
        GlobalConfig.Location = "Dry Creek";
        GlobalConfig.State = "Idaho";
        GlobalConfig.GlobalProjection = 26911;
        GlobalConfig.BoundingBox = new Rect(565889.360f, 4844940.560f, 569986.360f - 565889.360f, 4844940.560f - 4840843.560f);
        Application.LoadLevel(0);
    }

    public void LoadReynoldsCreek() {
        Debug.LogError("Load Reynolds Creek");
        GlobalConfig.Location = "Reynolds Creek";
        GlobalConfig.State = "Idaho";
    }

    public void LoadLehmanCreek() {
        Debug.LogError("Load Lehman Creek");
        GlobalConfig.Location = "Lehman Creek";
        GlobalConfig.State = "Nevada";
    }

    public void LoadJemezCaynon() {
        Debug.LogError("Load Jemez Caynon");
        GlobalConfig.Location = "Jemez Caldera";
        GlobalConfig.State = "New Mexico";
    }
}
