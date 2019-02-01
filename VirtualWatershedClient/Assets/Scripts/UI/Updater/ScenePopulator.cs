using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScenePopulator : MonoBehaviour {

	// Use this for initialization
	void Start () {
        OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DATA", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\data");
        System.Environment.SetEnvironmentVariable("GDAL_DATA", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\data\");
	}
	
	// Update is called once per frame
	void Update () {
        // Check to quit the application
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // This is a test variable;
    //public Scene generalScene;


    public void LoadGeneralScene()
    {
        SceneManager.LoadScene(6);
    }

    public void LoadDryCreek() {
        Debug.Log("Load Dry Creek");
        GlobalConfig.Location = "Dry Creek";
        GlobalConfig.State = "Idaho";
        GlobalConfig.GlobalProjection = 26911;
        GlobalConfig.BoundingBox = new Rect(565889.360f, 4844940.560f, 569986.360f - 565889.360f, 4844940.560f - 4840843.560f);
        GlobalConfig.Zone = 11;
        GlobalConfig.terrainImage = Resources.Load<Sprite>("Images/DryCreekSelect");
        SceneManager.LoadScene(4);
    }

    public void LoadReynoldsCreek() {
        Debug.Log("Load Reynolds Creek");
        GlobalConfig.Location = "Reynolds Creek";
        GlobalConfig.State = "Idaho";
        GlobalConfig.GlobalProjection = 26911;
        GlobalConfig.BoundingBox = new Rect(512660.020f, 4778377.741f, 523839.382f - 512660.020f, 4778377.741f - 4767198.380f);
        GlobalConfig.Zone = 11;
        GlobalConfig.terrainImage = Resources.Load<Sprite>("Images/ReynoldCreekSelect");
        SceneManager.LoadScene(2);
    }

    public void LoadLehmanCreek() {
        Debug.Log("Load Lehman Creek");
        GlobalConfig.Location = "Lehman Creek";
        GlobalConfig.State = "Nevada";
        GlobalConfig.GlobalProjection = 26911;
        GlobalConfig.BoundingBox = new Rect(731653.41f, 4323643.51f, 741254.90f - 731653.41f, 4323643.51f - 4318466.366f); // 4318741.06f
        GlobalConfig.Zone = 11;
        GlobalConfig.terrainImage = Resources.Load<Sprite>("Images/LehmanCreekSelect");
        SceneManager.LoadScene(4);
    }

    public void LoadJemezCaynon() {
        Debug.Log("Load Jemez Caynon");
        GlobalConfig.Location = "Jemez Caldera";
        GlobalConfig.State = "New Mexico";
        GlobalConfig.GlobalProjection = 26913;
        GlobalConfig.BoundingBox = new Rect(324093.21f, 3987729.00f, 381077.39f - 324093.21f, 3987729.00f - 3935123.25f);
        GlobalConfig.Zone = 13;
        GlobalConfig.terrainImage = Resources.Load<Sprite>("Images/JemezSelect");
        SceneManager.LoadScene(3);
    }

    public void LoadSelect()
    {
        Debug.Log("Load Select Page");
        SceneManager.LoadScene(0);
    }
}
