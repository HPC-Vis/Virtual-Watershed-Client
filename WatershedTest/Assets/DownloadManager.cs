using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DownloadManager : MonoBehaviour {

    ///List<ModelRun> Downloaded = new List<ModelRun>();
    List<string> Downloaded = new List<string>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void AddModelRun(string UUID)
    {
        Debug.LogError("MWHWHWHWHHHAHAHHAHAHAHHAHAHHAHAHAHHAHAHHAHAHAHHAHAHAHHAHAHA");
        Downloaded.Add(UUID);
    }

    public List<string> GetModelRuns()
    {
        Debug.LogError("RUNS!!!!: " + Downloaded.Count);
        return Downloaded;
    }
    
}
