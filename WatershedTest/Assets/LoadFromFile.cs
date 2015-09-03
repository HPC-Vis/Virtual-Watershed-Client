using UnityEngine;
using System.Collections;
using VTL.ListView;
public class LoadFromFile : MonoBehaviour {

    public GameObject downloadListView, fileListView;
    public FileBrowse fileBrowser;
    public ListViewManager fileView;

	// Use this for initialization
	void Start () {
        fileBrowser = GameObject.Find("FileBrowser").GetComponent<FileBrowse>();
        fileView = fileListView.GetComponent<ListViewManager>();
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void openFileWindow()
    {
        downloadListView.SetActive(false);
        fileListView.SetActive(true);
        populateFileWindow();
    }

    public void openDownloadWindow()
    {
        downloadListView.SetActive(true);
        fileListView.SetActive(false);
    }
    
    void populateFileWindow()
    {
        var Contents = fileBrowser.GetContents();
        fileView.Clear();
        DirectoryStruct ds = new DirectoryStruct();
        ds.filename = "..";
        ds.IsDirectory = true;
        Contents.Add(ds);
        foreach(var i in Contents)
        {
            fileView.AddRow(new object[] {i.filename,"", i.IsDirectory?"Directory":"File", ""});
        }
    }

    public void ChangeDirectory()
    {
        var contents = fileView.GetSelectedRowContent();
        if(contents.Count > 0)
        fileBrowser.SetDirectory((string)contents[0][0]);
        populateFileWindow();
    }

    public void LoadFile()
    {
        var contents = fileView.GetSelectedRowContent();
        if (contents.Count > 0)
        {
            Debug.LogError("LOADING: " + contents[0][2] ) ;
            //fileBrowser.SetDirectory((string)contents[0][0]);
        }
    }
}
