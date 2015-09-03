using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;
using System;
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
            fileView.AddRow(new object[] {i.filename,i.dateModified, i.IsDirectory?"Directory":"File", i.IsDirectory?"":i.bytes});
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

            if (contents[0][2].ToString().ToLower() == "file")
            {
                RasterDataset modelData = new RasterDataset(RasterDataset.GetGdalPath(fileBrowser.CurrentDirectory + @"\" + contents[0][0]));
                if (modelData.Open())
                {
                    ModelRun mr = new ModelRun(contents[0][0].ToString(), Guid.NewGuid().ToString(), "");
                    ModelRunManager.InsertModelRun(mr.ModelRunUUID, mr);
                    List<string> subSets = modelData.GetSubDatasets();
                    foreach (String str in subSets)
                    {
                        RasterDataset rd = new RasterDataset(str);
                        if(rd.Open())
                        {
                            DataRecord rec = new DataRecord(str);
                            rec.variableName = str;
                            rec.Data = rd.GetData();
                            rec.modelRunUUID = mr.ModelRunUUID;
                            rec.id = Guid.NewGuid().ToString();
                            ModelRunManager.InsertDataRecord(rec, new List<DataRecord>() { rec });
                        }

                    }
                    var startupruns = FileBasedCache.Get<Dictionary<string,ModelRun>>("startup");
                    startupruns.Add(mr.ModelRunUUID, mr);
                    FileBasedCache.Insert<Dictionary<string, ModelRun>>("startup",startupruns);
                }
            }
        }

       
    }
}
