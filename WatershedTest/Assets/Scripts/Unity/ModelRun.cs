using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// ModelRun class
/// </summary>
[Serializable]
public class ModelRun
{
    // We need a lock for the model run class due to the curse of caching
    readonly object LOCK; // Probably 

	// Fields
	public string Name="";
	public string ModelName="";
	public string ModelDataSetType="";
	public string ModelRunUUID="";
	public string Location = "None Specified";
	public string Description = "None";

	public DateTime? Start = null;
	public DateTime? End = null;
    public int Total;
    public int CurrentCapacity = 0;

	// Replace with modelrun variable class  ....   ***********************************************
    //public ModelRunManager ModelRunManager;
    // private Dictionary<string, GeoReference> references = new Dictionary<string, GeoReference>();
    private Dictionary<string, List<DataRecord>> references = new Dictionary<string, List<DataRecord>>();
	public Dictionary<string,bool> IsTemporal = new Dictionary<string, bool> ();
	// ************************************************

    // Dictionary used to add in the min and max
    public Dictionary<string, SerialVector2> MinMax = new Dictionary<string, SerialVector2>();

    public List<string> GetVariables()
    {
        return references.Keys.ToList(); 
    }

    // Constructors
    public ModelRun(string modelRunName,string modelRunUUID)
    {
        ModelName = modelRunName;
        ModelRunUUID = modelRunUUID;
    }

    public ModelRun()
    {

    }

    /*public ModelRun(string modelRunName, string modelRunUUID)
    {
        ModelName = modelRunName;
        ModelRunUUID = modelRunUUID;
        ModelRunManager = GM;
    }*/

    // Methods
    //public void addToModel(string label, GeoReference toAdd)
    public void Add(string label, List<DataRecord> toAdd)
    {
        // Check if already in the model
        if( references.ContainsKey(label) )
        {
            // Label already exists
            // Handle situation?
        }
        else
        {
            // Add to the model run
            references.Add(label, toAdd);
            MinMax.Add(label, new SerialVector2(new Vector2(float.MaxValue, float.MinValue)));
        }
    }

    /// <summary>
    /// Determines whether a DataRecord belongs to this particular ModelRun
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public bool BelongsTo(DataRecord record)
    {
        return record.modelRunUUID == ModelRunUUID;
    }
    
    /// <summary>
    /// Used by the Simulation class to fetch data.
    /// </summary>
    /// <param name="ModelVar"></param>
    /// <param name="CurrentTimeFrame"></param>
    public bool HasData(string ModelVar, int CurrentTimeFrame,bool texture=false)
    {
        //Need to determine the type of download.
        if (!texture)
        {

        }
        else
        {

        }
        // Returns whether it has the data...
        return false;
    }

    public int GetCount()
    {
        return CurrentCapacity;
    }

    public void DownloadData(string ModelVar, int CurrentTimeFrame,bool texture=false)
    {
        //Need to determine the type of download.
        if(!texture)
        {

        }
        else
        {

        }
        // Handle the download here
    }

    /// <summary>
    /// Inserts a Records into this ModelRun if it belongs..
    /// </summary>
    /// <param name="records"></param>
    /// <returns></returns>
    public bool Insert(DataRecord record)
    {
        // Check if the record belongs to the model run
        if(!BelongsTo(record)) 
        {
            //Debug.LogError("DOES NOT BELONG TO" + record.name);
            return false; 
        }

        // Determine whether or not we need to create a new List<DR>
        if( ! references.ContainsKey(record.variableName) )
        {
            references[record.variableName] = new List<DataRecord>();
            MinMax[record.variableName] = new SerialVector2(new Vector2(float.MaxValue, float.MinValue));
			IsTemporal[record.variableName] = false;
        }

        // Insert data record into appopritate georef object --- if it doesn't already exist in this model run..
        if( ! references[record.variableName].Contains(record) )
        {
            CurrentCapacity++;
            //Debug.Log("ADDED: " + CurrentCapacity + " " + Total);
            references[record.variableName].Add(record);
			IsTemporal[record.variableName] = ( record.IsTemporal() && (record.modelname.ToLower() != "reference") ) || IsTemporal[record.variableName];
        }
        
        //Debug.Log("VARIABLE: " + record.variableName + "RECORD NAME: " + record.name);
        // Return
        return true;
    }

    public List<DataRecord> Get(string label)
    {
        return references[label];
    }

    public int VariableCount()
    {
        return references.Count;
    }

    public void ClearData(string variable)
    {
        foreach (var record in references[variable])
        {
            record.Data = null;
        }        
    }

    // Other params
    public void DownloadDatasets(bool all=true,string operation="wcs",SystemParameters param=null)
    {
        // Create a default system parameter object
        if(param == null) { param = new SystemParameters(); }

        // Ensure the GRM reference is valid
        //if(ModelRunManager == null) { return; }

        // TODO Need to fill out the parameters

        // If all records are requests
        if(all)
        {
            // Simple Download Operation...
            foreach(var i in references)
            {
                ModelRunManager.Download(i.Value, null, "vwc", operation, param);
            }
        }

        // We can change the order of the downloads as opposed to downloading all.
    }

    public void FetchAll(string ModelVar,DataRecordSetter SendToSimulator,string service="wms",SystemParameters param=null)
    {
        
        // Let the downloading begin!
        ModelRunManager.Download(references[ModelVar], SendToSimulator, "vwc", service, param);
        //foreach (var i in references[ModelVar])
        //{
        //    List<DataRecord> Record = new List<DataRecord>();
        //    Record.Add(i);

        //    ModelRunManager.Download(Record, null, "vwc", service, param);
        //}
    }

    // For filebased simulations .... This is just case we don't have any data in June.
    public void FileBasedFetch()
    {

    }
    
    public List<DataRecord> Query(SystemParameters param=null, bool usingOR=true, int number=0)
    {
        //Debug.LogError("USING OR: " + usingOR);
        // Initialize variables
        int count = 0;
        List<DataRecord> records = new List<DataRecord>();

        // iterate through the list of datarecords __ variables in this case
        foreach (var variable in references)
        {
            // iterate through the list of datarecords contain in this paricular variable
            foreach(var record in variable.Value)
            {
                // Check if the case where everything is desired
                if(number > 0)
                {
                    count++;

                    // Add the record
                    records.Add(record);
                }
                //else if(count == number)
                //{

                //    // Return what you have
                //    //return records;
                //}
                else if(usingOR)
                {
                    Debug.LogError(record.modelname);
                    Debug.LogError(this.ModelName);
                    Debug.LogError(record.name);
                    Debug.LogError(param.name);
                    Debug.LogError(record.Type);
                    Debug.LogError(param.TYPE);
                    Debug.LogError(param.type);
                    Debug.LogError(param.state);
                    Debug.LogError(record.state);
                    // Check record using OR-Query
                    if(record.modelname == this.ModelName || record.name == param.name || record.Type == param.TYPE || 
                        //record.start.ToString() == param.starttime || record.end.ToString() == param.endtime || 
                        record.state == param.state )
                    {
                        // Add the record
                        records.Add(record);
                        count++;
                        Logger.WriteLine("OUCHHHHHHHH!!!!!!!!");
                    }
                }
                else if(!usingOR)
                {
                    // Check record using AND-Query
                    if ( (param.modelname == "" || param.modelname == this.ModelName) &&
                         (param.name == "" || param.name == record.name ) &&
                         (param.TYPE == "" || param.TYPE == record.Type) &&
                         (param.starttime == "" || param.starttime == record.start.ToString()) &&
                         (param.endtime == "" || param.endtime == record.end.ToString()) &&
                         (param.state == "" || param.state == record.state)
                        )
                    {
                        // Add the record
                        records.Add(record);
                        count++;
                    }
                }
            }
        }

        if(records.Count == 0)
        {
            //Debug.LogError("NOT FOUND");
            records = null;
        }

        return records;
    }


    // These can be replaced with inserts from datarecords.
    public DateTime GetBeginModelTime()
    {
        DateTime time = DateTime.MaxValue;
        //Logger.WriteLine(time.ToString());
        foreach (var i in references)
        {
           // Logger.WriteLine(i.Value.Count.ToString());
            //Logger.WriteLine("ORIGINAL: " + references[i.Key][0].start);
            foreach(var j in i.Value)
            {
                //Logger.WriteLine(j.start.ToString());
                if(time > j.start.Value)
                {
                    time = j.start.Value;
                }
            }
        }
        Logger.WriteLine("BEGINNING OF TIME!!!!!!! " + time);
        return time;
    }

    public DateTime GetEndModelTime()
    {
        DateTime time = DateTime.MinValue;
        //Logger.WriteLine(time.ToString());
        foreach (var i in references)
        {
            //Logger.WriteLine(i.Value.Count.ToString());
            //Logger.WriteLine("ORIGINAL: " + i.Value[0].end);
            foreach (var j in i.Value)
            {
                //Logger.WriteLine(j.end.ToString());
				if (j.end > time)
                {
					time = j.end.Value;
                }
            }
        }
        Logger.WriteLine("END OF TIME!!!!!!! " + time);
        return time;
    }

	public void SetModelRunTime()
	{
		DateTime MinTime = DateTime.MaxValue;
		DateTime MaxTime = DateTime.MinValue;
		
		
		foreach (var i in references)
		{;
			foreach (var j in i.Value)
			{
				//Logger.WriteLine(j.end.ToString());
				if (j.end> MaxTime)
				{
					MaxTime = j.end.Value;
				}
				if(j.start < MinTime)
				{
					MinTime = j.start.Value;
				}
			}
		}
		Start = MinTime;
		End = MaxTime;
	}

    public DataRecord FetchNearestDataPoint(string VariableName, DateTime pointOfInterest)
    {
        // Check if ModelRun exists here
        if(!references.ContainsKey(VariableName))
        {
            return null;
        }

        // For now a linear search -- This can be improved
        foreach(var i in references[VariableName])
        {
            if(i.start != null && i.end != null && pointOfInterest <= i.end && pointOfInterest >= i.start)
            {
                return i;
            }
        }

        return null;
    }


    public List<DataRecord> FetchVariableData(string vari)
    {
        if(references.ContainsKey(vari))
        {
            return references[vari];
        }
        return null;
    }

    // Use the default timestep contained within this class

    /// <summary>
    /// An update function that is to give the next step in a simulation.
    /// </summary>
    /// <param name="ModelVar"></param>
    /// <param name="CurrentTimeStep"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public int Update(string ModelVar, int CurrentTimeStep, DateTime current)
    {
        //if (SetToStepThrough == null || previousRecord < 0 || SetToStepThrough.Count <= previousRecord)
        //{
            // Throw Error
            //return;
        //}


        // Creating a compartor
        DataRecordComparers.StartDateDescending compare = new DataRecordComparers.StartDateDescending();

        // Sort the list..
        if (!references.ContainsKey(ModelVar))
            return -1;
        references[ModelVar].Sort(compare);

        // Find next record -- Assuming that the list is in order at this point.
        for (int i = CurrentTimeStep + 1; i < references[ModelVar].Count; i++)
        {
            //Logger.WriteLine(SetToStepThrough[previousRecord].start.ToString() + " " + SetToStepThrough[previousRecord].end.ToString());
            //  Logger.WriteLine(SetToStepThrough[i].start.ToString() + " " + SetToStepThrough[i].end.ToString());
            //Logger.WriteLine(current.ToString());
            if (references[ModelVar][i].start >= current)
            {
                //TimeSpan delta = start - NextTime;
                return i;
            }
        }
        return -1;
    }
}
