using UnityEngine;
using System.Collections;

public class ScenePopulator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // Check to quit the application
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void LoadDryCreek() {
        Debug.LogError("Load Dry Creek");
        GlobalConfig.Location = "Dry Creek";
        GlobalConfig.State = "Idaho";
        GlobalConfig.GlobalProjection = 26911;
        GlobalConfig.BoundingBox = new Rect(565889.360f, 4844940.560f, 569986.360f - 565889.360f, 4844940.560f - 4840843.560f);
        GlobalConfig.Zone = 11;
        Application.LoadLevel(1);
    }

    public void LoadReynoldsCreek() {
        Debug.LogError("Load Reynolds Creek");
        GlobalConfig.Location = "Reynolds Creek";
        GlobalConfig.State = "Idaho";
        GlobalConfig.GlobalProjection = 26911;
        GlobalConfig.BoundingBox = new Rect(512660.020f, 4778377.741f, 523839.382f - 512660.020f, 4778377.741f - 4767198.380f);
        GlobalConfig.Zone = 11;
        Application.LoadLevel(2);
    }

    public void LoadLehmanCreek() {
        Debug.LogError("Load Lehman Creek");
        GlobalConfig.Location = "Lehman Creek";
        GlobalConfig.State = "Nevada";
        GlobalConfig.GlobalProjection = 26911;
        GlobalConfig.BoundingBox = new Rect(731653.41f, 4323643.51f, 741254.90f - 731653.41f, 4323643.51f - 4318466.366f); // 4318741.06f
        GlobalConfig.Zone = 11;
        Application.LoadLevel(4);
    }

    public void LoadJemezCaynon() {
        Debug.LogError("Load Jemez Caynon");
        GlobalConfig.Location = "Jemez Caldera";
        GlobalConfig.State = "New Mexico";
        GlobalConfig.GlobalProjection = 26911;
        GlobalConfig.BoundingBox = new Rect(324093.21f, 3987729.00f, 381077.39f - 324093.21f, 3987729.00f - 3935123.25f);
        GlobalConfig.Zone = 13;
        Application.LoadLevel(3);
    }

    public void LoadSelect()
    {
        Debug.LogError("Load Select Page");
        Application.LoadLevel(0);
    }
}
