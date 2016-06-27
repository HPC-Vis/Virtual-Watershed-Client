using UnityEngine;
using System.Collections;

/// <summary>
/// This is script is meant to demonstrate how to interact with filelistview.cs and the many game objects conencted to it.
/// </summary>
public class TestFileListView : MonoBehaviour {
    public FileListViewManager filelistview;

	// Use this for initialization
	void Start () {
        filelistview.SetTextToSave();
        filelistview.action = SaveSomething;
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.A) )
        {
            filelistview.SetTextToSave();
            filelistview.action = SaveSomething;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            filelistview.SetTextToLoad();
            filelistview.action = LoadSomething;
        }
	}

    public void SaveSomething()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.LogError("Now Saving: " + filelistview.GetCurrentSelection());
        }
    }

    public void LoadSomething()
    {
        Debug.LogError("Now Loading: " + filelistview.GetCurrentSelection());
    }
}
