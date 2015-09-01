using UnityEngine;
using System.Collections;

public class LoadFromFile : MonoBehaviour {

    public GameObject downloadListView, fileListView;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void openFileWindow()
    {
        downloadListView.SetActive(false);
        fileListView.SetActive(true);
    }

    public void openDownloadWindow()
    {
        downloadListView.SetActive(true);
        fileListView.SetActive(false);
    }
}
