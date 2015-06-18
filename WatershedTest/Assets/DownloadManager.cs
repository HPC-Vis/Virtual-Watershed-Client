using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class DownloadManager : MonoBehaviour {

    ///List<ModelRun> Downloaded = new List<ModelRun>();
    List<string> Downloaded = new List<string>();
	// Use this for initialization
	void Start () {
	
	}
    public Text DownloadedText;
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (ModelRunManager.Total > 0)
            DownloadedText.text = "Loaded " + ((float)(ModelRunManager.Counter) / (float)(ModelRunManager.Total)).ToString("p");
	}
    public void AddModelRun(string UUID)
    {
        //Debug.LogError("MWHWHWHWHHHAHAHHAHAHAHHAHAHHAHAHAHHAHAHHAHAHAHHAHAHAHHAHAHA");
		if (!Downloaded.Contains (UUID)) 
		{
			Downloaded.Add (UUID);
		}
    }

    public List<string> GetModelRuns()
    {
        Logger.WriteLine("<color=green>Number of model runs: " + Downloaded.Count + ".</color>");
        return Downloaded;
    }
    
}
