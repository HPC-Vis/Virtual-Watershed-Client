using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class sampleManager : MonoBehaviour {

    public sampleData sd = new sampleData();
    public List<DataRecord> manager = new List<DataRecord>();
    public VWClient client = new VWClient();

	// Use this for initialization
	void Start () {
        search(0, 30);
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(manager.Count);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void search(int offset, int limit)
    {
        Debug.Log("started");
        string trackedJobs = client.RequestRecords(offset, limit);
        StartCoroutine(CheckStatus(trackedJobs));
        
    }

    public IEnumerator CheckStatus(string url)
    {
        DataTracker.CheckStatus(url);
        while (DataTracker.CheckStatus(url) != "Finished")
        {
            Debug.Log("looping");
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("done!");
        manager = DataTracker.JobFinished(url);
        sd.sortingAvailable = true;
        sd.setData(manager);
        yield return null;
        
    }

}
