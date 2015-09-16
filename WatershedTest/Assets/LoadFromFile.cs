using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using VTL.ListView;
using System;

/// <summary>
/// A GUI Interface class used for loading from a file.
/// </summary>
public class LoadFromFile : MonoBehaviour {

    public GameObject downloadListView, fileListView;
    public Text currentDirectory;
    public FileBrowse fileBrowser;
    public ListViewManager fileView;

	// Use this for initialization
	void Start () {
        fileBrowser = GameObject.Find("FileBrowser").GetComponent<FileBrowse>();
        fileView = fileListView.GetComponent<ListViewManager>();
        currentDirectory = GameObject.Find("CurrentDirectory").GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Open the file window for selecting files
    /// </summary>
    public void openFileWindow()
    {
        downloadListView.SetActive(false);
        fileListView.SetActive(true);
        populateFileWindow();
    }

    /// <summary>
    /// Open the window for downloading data from the virtual watershed.
    /// </summary>
    public void openDownloadWindow()
    {
        downloadListView.SetActive(true);
        fileListView.SetActive(false);
    }
    
    /// <summary>
    /// Populate data based on what is acquired from the filebrowse class.
    /// </summary>
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

    /// <summary>
    /// Change the directory to the relevant directory.
    /// </summary>
    public void ChangeDirectory()
    {
        var contents = fileView.GetSelectedRowContent();
        if (contents.Count > 0)
        {
            if (contents[0][2].ToString().ToLower() == "directory")
            {
                fileBrowser.SetDirectory((string)contents[0][0]);
                populateFileWindow();
                currentDirectory.text = "Current Directory: " + fileBrowser.CurrentDirectory;

            }
        }

    }

    /// <summary>
    /// Load a selected file which is only netcdfs at the moment.
    /// </summary>
    public void LoadFile()
    {
        var contents = fileView.GetSelectedRowContent();
        if (contents.Count > 0)
        {
            Debug.LogError("LOADING: " + contents[0][2] ) ;

            if (contents[0][2].ToString().ToLower() == "file")
            {
                string FileName = RasterDataset.GetGdalPath(fileBrowser.CurrentDirectory + @"\" + contents[0][0]);
                RasterDataset modelData = new RasterDataset(FileName);
                if (modelData.Open())
                {
                    // 
                    //Debug.LogError("ITS ALIVE");

                    // Create a new random modelrun.
                    ModelRun mr = new ModelRun(contents[0][0].ToString(), Guid.NewGuid().ToString(), "");
                    mr.Location = GlobalConfig.Location;
                    mr.isFile = true;
                    
                    ModelRunManager.InsertModelRun(mr.ModelRunUUID, mr);

                    // Get any subdatasets associate to this file
                    List<string> subSets = modelData.GetSubDatasets();

                    // Populate datarecords for each subdatasets
                    foreach (String str in subSets)
                    {
                        Debug.LogError("PROCESSING THIS STR: " + str);
                        RasterDataset rd = new RasterDataset(str);
                        if(rd.Open())
                        {

                            DateTime tempTime = new DateTime();
                            TimeSpan tempSpan = new TimeSpan();
                            DataRecord rec = new DataRecord(str);
                            rec.variableName = str.Contains("NETCDF")?str.Replace(FileName+":",""):str;
                            //rec.Data = rd.GetData();
                            rec.modelname = mr.Name;
                            rec.modelRunUUID = mr.ModelRunUUID;
                            rec.id = Guid.NewGuid().ToString();
                            rec.location = GlobalConfig.Location;
                            rec.Temporal = rd.IsTemporal();//(rec.Data.Count > 1);
                            rec.Type = "DEM";
                            rd.GetTimes(out tempTime, out tempSpan);
                            rec.start = tempTime;
                            rec.end = tempTime + tempSpan;
                            rec.bbox = rd.GetBoundingBox(); 
                            rec.projection = rd.ReturnProjection();
                            rec.numbands = rd.GetRasterCount();
							rec.services["file"] = str;
                            ModelRunManager.InsertDataRecord(rec, new List<DataRecord>() );
                        }

                    }

                    // Adding a modelrun to the startup cache record
                    var startupruns = FileBasedCache.Get<Dictionary<string,ModelRun>>("startup");
                    startupruns.Add(mr.ModelRunUUID, mr);
                    Debug.LogError("ADDED: " + mr.ModelRunUUID);
                    FileBasedCache.Insert<Dictionary<string, ModelRun>>("startup",startupruns);
                }
            }
        }

       
    }
}
