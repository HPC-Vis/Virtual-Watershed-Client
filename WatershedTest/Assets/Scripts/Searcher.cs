using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// The ListView is contained in its own name space
using VTL.ListView;

public class Searcher : MonoBehaviour {
// This guy will give us the search data results

    // Current Menu Contents..
    List<DataRecord> Records = new List<DataRecord>();
    
    public ListViewManager listViewManager;

    bool NewSearch=false;
    int count = 0;
    //ModelRunManager ModelRunManager;
    VWClient vwc;
	NetworkManager nm;
    public DownloadManager downloadManager;
    int searchCounter = 0;
	bool firstPopulation = false;
	float UpdateTimer;
    public static string DirectoryLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../";

	// Use this for initialization
	void Start () {
	
		//Logger.enable = false;
	//	if (ModelRunManager.client == null) {
			nm = new NetworkManager ();
			vwc = new VWClient (new DataFactory (nm), nm);
			nm.Subscribe (vwc);
	ModelRunManager.client = vwc;
	//	} 
	//	else 
	//	 {
			//vwc = ModelRunManager.client;
		// }

    // Lets get all of the model runs out of the way!!!!!
    ModelRunManager.SearchForModelRuns();
	}

   /// <summary>
   /// EmitSelected a test function for showing that the datarecords have been selected.
   /// </summary>
   public  void EmitSelected()
   {
		Logger.WriteLine("<color=green>Selected: " + listViewManager.GetSelectedModelRuns().Count + "</color>");
        foreach (var i in listViewManager.GetSelectedModelRuns())
        {
            // Now to load datasets...
            ModelRunManager.PopulateModelRunData(i.ModelRunUUID);

            // Pass things to downloaded -- Beware of the change of reference bug!!!
            downloadManager.AddModelRun(i.ModelRunUUID);
        }
   }

   /// <summary>
   /// Refresh is used by a GUI button to refesh the model run list view.
   /// </summary>
   public void Refresh()
   {
       if(!Directory.Exists(DirectoryLocation + "Cache")){
           Directory.CreateDirectory(DirectoryLocation + "Cache");
       }
       firstPopulation = false;
   }

   public void ApplyWMSService()
   {
       // Apply WMS Service to data
       // and do something with it after it has finished based on the type.
       // Add selected to buildable list
   }
    public void ApplyWFSService()
   {
        // Apply WFS Service to data
        // build a gameobject or store it somewhere.
        // Add selected to buildable list
   }
    public void ApplyWCSService()
    {
        // Apply WCS Service to data
        // and do someting with after it has finished based on the type.
        // Add selected to buildable list
    }
    Rect testRect = new Rect(0, 0, 150, 150);
	// Update is called once per frame
	void Update () {
        
        if( Input.GetKey(KeyCode.R) )
        {
			Camera.main.backgroundColor = new Color(0,0,0,0);
            Refresh();
        }

		UpdateTimer += Time.deltaTime;
		if (!firstPopulation )//|| UpdateTimer > 25)
		{
            if (listViewManager.GetSelectedModelRuns().Count == 0 || !firstPopulation)
            {
                SystemParameters parameters = null; //new SystemParameters();
                List<ModelRun> Runs = ModelRunManager.QueryModelRuns(parameters, true, 15);
                if (Runs.Count > 0)
                {
                    // Debug.Log("The List has Been Populated");
                    listViewManager.Clear();
                    ApplyToUI(Runs);
                    firstPopulation = true;
                }
                UpdateTimer = 0;
            }
		}
	}
	
    /// <summary>
    /// ApplyToUI applys a list of datarecords to the UI 
    /// </summary>
    /// <param name="Records"></param>
	void ApplyToUI(List<DataRecord> Records)
    {
        foreach(var rec in Records)
        {
            Debug.LogError(rec.name);
            listViewManager.AddRow(new object[]{rec.name,
                                            rec.description,
                                            rec.location,
                                            rec.modelRunUUID,
                                            rec.variableName,
                                            rec.start,
                                            rec.end},rec);
        }
        
    }

    /// <summary>
    /// ApplyToUI applys a list of datarecords to the UI 
    /// </summary>
    /// <param name="Records"></param>
    void ApplyToUI(List<ModelRun> ModelRuns)
    {
        foreach (var mr in ModelRuns)
        {
            var StringList = mr.GetVariables();
            string Variables = "";
            foreach (var s in StringList)
            {
                Variables += s + ", ";
            }
                
            //Debug.LogError("Adding to list: " + mr.ModelName);
            listViewManager.AddRow(new object[]{mr.ModelName,
			mr.Description,
			mr.Location,
			mr.ModelRunUUID,
			Variables,
			mr.Start == null ? "" : mr.Start.ToString(),
			mr.End == null ? "" : mr.End.ToString()}, mr);
        }
    }

    /// <summary>
    /// MergeCollections does a list intersection operation where the combination of the two records are returned without dups.
    /// </summary>
    /// <param name="Records"></param>
    /// <param name="Records2"></param>
    /// <returns></returns>
	List<DataRecord> MergeCollections(List<DataRecord> Records,List<DataRecord> Records2)
    {
		if(Records==null)
		return Records2;
		else if(Records2==null)
	     return Records;
	     
		foreach(var i in Records2)
	    {
			if(!Records.Contains(i))
		    {
				Records.Add(i);
		    }
	    }
	    foreach(var i in Records)
	    {
	    Debug.Log ("SOMETHING WITH THIS" + i.name);
	    }
	    return Records;
    }
	
    /// <summary>
    /// A setter used as a callback function in Manager.getavaliable records to get current existing records.
    /// </summary>
    /// <param name="Setted"></param>
    /// <param name="records"></param>
	public void Setter(ref List<DataRecord> Setted, List<DataRecord> records)
    {
		if(records == null)
		{
			records = new List<DataRecord>();
		}
		
		Setted = records;
    }

    public void OnDestroy()
    {
        nm.Halt();
    }

	// We can turn this into a thread later.
	IEnumerator searchForAvailable(int Count, SystemParameters ps,bool or=true,int number=0)
    {
        List<DataRecord> Records=new List<DataRecord>();
        List<DataRecord> PassedRecords=null;
        // Lets make sure the manager exists!!!!
		/*if(ModelRunManager == null)
	    {
	      Debug.LogError("AFSDFSDF");
	      yield return null;
	    }*/
       // Lets do a local search!
       Records.InsertRange(0,ModelRunManager.Query(ps,or,number));
       Debug.Log("LOOK HERE " + Records.Count);
       
       // Lets do a Virtual Watershed Search
       ModelRunManager.getAvailable(ps,null,((records) => Setter(ref PassedRecords,records)));
       
       // Now we wait for this guy to finish
       while(PassedRecords == null)
       {
		  //Debug.LogError("TEST");
          yield return new WaitForEndOfFrame();
       }

       if(searchCounter==Count)
       {
            listViewManager.Clear();
		    ApplyToUI(MergeCollections(Records,PassedRecords));
       }
       else
       {
           Debug.Log("FAILURE");
       }
       yield return null;
    }

    IEnumerator SearchForModelRuns(int Count, SystemParameters ps, bool or=true, int number=0)
    {
        List<DataRecord> PassedRecords = null;

        // Lets do a Virtual Watershed Search
        ModelRunManager.getAvailable(ps, null, ((records) => Setter(ref PassedRecords, records)));

        // Now we wait for this guy to finish
        while (PassedRecords == null)
        {
            //Debug.LogError("TEST");
            yield return new WaitForEndOfFrame();
        }

        Debug.LogError(PassedRecords.Count);
        if (searchCounter == Count)
        {
            listViewManager.Clear();
            ApplyToUI(ModelRunManager.QueryModelRuns(ps, or, ps.limit));
        }
        else
        {
            Debug.Log("FAILURE");
        }

        

        yield return null;
    }


    // A search function used to search for avaliable datarecords
    // Attach this to a button and as a callback for string requests
    public void search(Text query)
    {
        //Debug.Log("SEARCHING NOW!!!!");
        SystemParameters parameters;
        parameters = buildList(query);
        //Debug.Log(parameters.query);
        //Debug.Log(parameters.location);
        searchCounter++;
        //StartCoroutine(searchForAvailable(searchCounter, parameters));
        StartCoroutine(SearchForModelRuns(searchCounter, parameters));
    }


    // buildList -- a behemoth of function that builds a SystemParameters object to be passed through our backend.
    public SystemParameters buildList(Text input)
    {
        string query = input.text;
        query.Replace(' ', '\0');
        string[] ques = query.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        SystemParameters parameters = new SystemParameters() ;

        foreach (var j in ques)
        {
            Debug.LogError(j);
            if (j.Contains("="))
            {

                string[] ws = j.Split(new char[] { '=' }, System.StringSplitOptions.RemoveEmptyEntries);
                string key = ws[0];
                string value = ws[1];

                switch (key)
                {
                    case "query":

                        //as the terrains are pre-generated, there is no use in searching for DEM's
                        //what should we do about this?
                        parameters.query = value;
                        break;

                    case "name":
                        parameters.name = value;
                        break;
                        
                    case "type":
                        parameters.TYPE = value;
                        break;

                    case "modelname":
                        parameters.modelname = value;
                        break;

                    case "location":
                        // Change this when get to on-the-fly generation of terrain :D.
                        parameters.location = value;
                        //parameters.location = ;
                        break;

                    case "state":
                        parameters.state = value;
                        break;

                    case "start":
                        parameters.timestamp_start = value;
                        break;

                    case "end":
                        parameters.timestamp_end = value;
                        break;
                    case "limit":
                        parameters.limit = int.Parse(value);
                        break;
                    case "offset":
                        parameters.offset = int.Parse(value);
                        break;
                    case "model_set_type":
                        parameters.model_set_type = value;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                parameters.query = j;
            }
        }
        //parameters.location = GlobalConfig.Location;
        return parameters;

    }
}
