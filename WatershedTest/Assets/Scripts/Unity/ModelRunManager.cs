using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// This for notifying other objects that the georeference list has been populated.
/// This may be replaced with a observer that notifies objects.... maybe.
/// </summary>
/// <param name="RecievedGeoRefs"></param>
public delegate void GeoRefMessage(List<string> RecievedGeoRefs);

public static class ModelRunManager
{
    // Fields
    static private VWClient Client;

    // Global loading counter
    static public int Total = 0;
    static public int Counter = 0;

    /// <summary>
    ///  Make sure to set the client or OUCH!
    /// </summary>
    static public VWClient client
    {
        set
        {
            Client = value;
        }
        get
        {
            if (Client == null)
            {
                return null;
                //throw new NullReferenceException();
            }
            return Client;
        }
    }
    // Single List<DR> can be stored under a generic model run
    // Dictionary<string, GeoReference> storedGeoRefs = new Dictionary<string, GeoReference>();
    static ModelRun generalModelRun = new ModelRun("general", ""); // Update later?

    // TODO When a georef added, check which model run it belongs to and save the model run here
    // Questions that need to be answered: 
    // Should ModelRuns (a reference to) be stored in GeoRefs? (This will create a cycle of references GeoRef_1 -> ModelRun -> GeoRef_1 
    // ~ Half Answered use the GeoRefManager to get the original model run
    static Dictionary<string, ModelRun> modelRuns = new Dictionary<string, ModelRun>();

    // Filebased Cache entry.
    static string cacheBackupEntry = "backup";
    static string cacheRestoreEntry = "restore";

    // Utilities Utilities where for art thou Utilites 
    //static Utilities utilities = new Utilities();
    private static readonly object _Padlock = new object();
    // NOTE: Some way of sorting and returning back a sorted list based on metadata
    // NOTE: On download, a GeoRef might already exist. No code has been written to handle that case other than a check.
    public static void Start()
    {
        // Load up cache?
        if (FileBasedCache.Exists(cacheRestoreEntry))
        {
            Logger.WriteLine("Restoring Previous Session");

            // Don't know how to fix
            //storedGeoRefs = FileBasedCache.Get<Dictionary<string, GeoReference>>(cacheRestoreEntry);
        }
        // Initialize everything.....
    }

    // Constructors
    //static public ModelRunManager(VWClient refToClient) { client = refToClient; }

    // Methods
    static public List<KeyValuePair<string, ModelRun>> GetByName(string name)
    {
        return modelRuns.Where(x => x.Value.ModelName == name).ToList();
    }

    static public ModelRun GetByUUID(string uuid)
    {
        if (modelRuns.ContainsKey(uuid))
            return modelRuns[uuid];
        //Debug.LogError("RETURNING NULL");
        return null;
    }

    // Returns all model run uuids..
    static public List<string> GetKeys()
    {
        return modelRuns.Keys.ToList();
    }

    // TODO: Needs to be moved
    /* public void buildObject(string recordname,string buildType="")
    {
        // TODO
        // We need the utilities class here. -- This is in the Unity code right now...
        GeoReference obj = storedGeoRefs[recordname];
        
        // We need to use this obj to determine what to build.

        // if terrain build terrain
        if(buildType == "terrain")
        {
            obj.gameObject = utilities.buildTerrain(obj.records[0]);
        }
        // else if texture build texture
        else if( buildType == "texture")
        {
            // To be done 
        }

        // else if shape build shape
        else if( buildType == "shape")
        {
            obj.gameObject = utilities.buildShape(obj.records[0]);
        }
    } */

    static public void SearchForModelRuns(SystemParameters param = null)
    {
        // Create param if one does not exist
        if (param == null) { param = new SystemParameters(); }

        if (FileBasedCache.Exists("startup"))
        {
            Logger.WriteLine("<color=red>Aquired data from the cache system.</color>");
            modelRuns = FileBasedCache.Get<Dictionary<string, ModelRun>>("startup");
        }
        client.RequestModelRuns(OnGetModelRuns, param);

    }

    // NOTE: Populating the data inside a datarecord. Something like building the texture.
    // Gonna need a parameter object for this ----- for now just defaults
    // TODO Consider Removing DataRecordSetter
    static public void Download(List<DataRecord> records, DataRecordSetter SettingTheRecord, string service = "vwc", string operation = "wcs", SystemParameters param = null)
    {
        //Logger.enable = false;
        // Create param if one does not exist
        if (param == null) { param = new SystemParameters(); }

        // TODO 
        if (service == "vwc")
        {
            if (operation == "fgdc")
            {
                client.GetMetaData(SettingTheRecord, records);
            }
            else
            {
                foreach (var i in records)
                {
                    //Debug.LogError(i.name);
                    if (operation == "wms")
                    {
                        // Lets check if it exists in the cache by uuid
                        if (FileBasedCache.Exists(i.id) && i.texture == null)
                        {
                            i.boundingBox = FileBasedCache.Get<DataRecord>(i.id).boundingBox;
                            i.texture = FileBasedCache.Get<DataRecord>(i.id).texture;
                            i.bbox2 = FileBasedCache.Get<DataRecord>(i.id).bbox2;
                            i.bbox = FileBasedCache.Get<DataRecord>(i.id).bbox;
                            SettingTheRecord(new List<DataRecord> { i });
                            continue;
                        }
                        else if (i.texture != null)
                        {
                            //i.texture = FileBasedCache.Get<DataRecord>(i.id).texture;
                            SettingTheRecord(new List<DataRecord> { i });
                            continue;
                        }

                        if (param.width == 0 || param.height == 0)
                        {
                            param.width = 100;
                            param.height = 100;
                        }
                        client.getMap(SettingTheRecord, i, param);
                    }
                    else if (operation == "wcs")
                    {
                        // Lets check if it exists in the cache by uuid
                        //Debug.LogError( "DATA: + " + (i.Data == null).ToString())
                        //Debug.LogError("ID: " + i.id); ;
                        if (FileBasedCache.Exists(i.id) && i.Data == null)
                        {
                            //Debug.LogError("EXISTS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!: " + i.id);
                            i.Data = FileBasedCache.Get<DataRecord>(i.id).Data;
                            i.bbox2 = FileBasedCache.Get<DataRecord>(i.id).bbox2;
                            i.bbox = FileBasedCache.Get<DataRecord>(i.id).bbox;
                            SettingTheRecord(new List<DataRecord> { i });
                            continue;
                        }
                        else if (i.Data != null)
                        {
                            //Debug.LogError("IN CACHE: " + FileBasedCache.Exists(i.id) + " Data: " + i.Data.GetLength(0) + " ID: " + i.id);
                            SettingTheRecord(new List<DataRecord> { i });
                            continue;
                        }
                        client.getCoverage(SettingTheRecord, i, param);
                    }
                    else if (operation == "wfs")
                    {
                        // Lets check if it exists in the cache by uuid
                        if (FileBasedCache.Exists(i.id) && i.Lines == null)
                        {
                            i.Lines = FileBasedCache.Get<DataRecord>(i.id).Lines;
                            SettingTheRecord(new List<DataRecord> { i });
                            continue;
                        }
                        else if (i.Lines != null)
                        {
                            //i.Lines = FileBasedCache.Get<DataRecord>(i.id).Lines;
                            SettingTheRecord(new List<DataRecord> { i });
                        }
                        Debug.LogError("PRIORITY: " + param.Priority);
                        client.getFeatures(SettingTheRecord, i, param);
                    }

                }
            }
        }
    }

    // We will return the data records that are specific to the query.
    static public List<DataRecord> Query(SystemParameters parameters = null, bool Or = true, int number = 0)
    {
        List<DataRecord> records = new List<DataRecord>();
        // Check in the list of model runs based on the query parameters everything will be an or operation...
        foreach (var i in modelRuns)
        {
            // Query inside of model run class... ---- AddRange append lists ---- I wonder what happens if you do List.AddRange(List) 
            records.AddRange(i.Value.Query(parameters, Or, number));
        }
        return records;
    }

    static public List<ModelRun> QueryModelRuns(SystemParameters parameters = null, bool Or = true, int number = 0)
    {
        List<ModelRun> Runs = new List<ModelRun>();

        foreach (var i in modelRuns)
        {
            // Find matching model runs
            // This can be optimized by storing additional information in the ModelRun class.
            if (parameters.model_set_type == i.Value.ModelDataSetType)
            {
                Runs.Add(i.Value);
            }
            else
            {
                var Records = i.Value.Query(parameters, Or, number);
                if (Records != null)
                {
                    Runs.Add(i.Value);
                }
            }
        }
        return Runs;
    }

    // NOTE: Build a parameter struct (name of struct = ServiceParameters)
    static public void getAvailable(SystemParameters param, GeoRefMessage Message = null, DataRecordSetter Setter = null)
    {
        // Debug.Log(param.query);
        // TODO
        if (Setter == null)
            client.RequestRecords(((List<DataRecord> records) => onGetAvailableComplete(records, Message)), param);
        else
            client.RequestRecords(((List<DataRecord> records) => onGetAvailableComplete(records, Setter)), param);
    }

    private static void SetModelSetType(List<DataRecord> Records)
    {

        foreach (var i in Records)
        {
            if (modelRuns.ContainsKey(i.modelRunUUID))
            {
                // Get Model Set Type
                modelRuns[i.modelRunUUID].ModelDataSetType = i.model_set_type;
                modelRuns[i.modelRunUUID].Location = i.location;
                modelRuns[i.modelRunUUID].Description = i.description;
            }
            else
            {
                Debug.LogError("FAILURE MODEL RUN DOES NOT EXIST");
            }
        }
        Debug.LogError("Adding modelruns to file cache system.");
        FileBasedCache.Insert<Dictionary<string, ModelRun>>("startup", modelRuns);
    }


    private static void OnGetModelRuns(List<DataRecord> Records)
    {
        Dictionary<string, ModelRun> startRuns = new Dictionary<string, ModelRun>();

        // Debug.Log(Records == null);
        //if (FileBasedCache.Exists("startup"))
        //{
        //    Debug.LogError("Getting paid");
        //     startRuns = FileBasedCache.Get<Dictionary<string,ModelRun>>("startup");
        //}
        Logger.Log("Getting Model Runs");
        foreach (var i in Records)
        {
            if (!modelRuns.ContainsKey(i.modelRunUUID))
            {

                if (startRuns.ContainsKey(i.modelRunUUID))
                {
                    modelRuns.Add(i.modelRunUUID, startRuns[i.modelRunUUID]);
                }
                else
                {
                    modelRuns.Add(i.modelRunUUID, new ModelRun(i.modelname, i.modelRunUUID));
                    // SetModelSetType
                    SystemParameters sp = new SystemParameters();
                    sp.model_run_uuid = i.modelRunUUID;
                    sp.limit = 1;
                    sp.offset = 0;
                    client.RequestRecords(SetModelSetType, sp);
                }
            }
        }

        // Logger.WriteLine("MODEL RUNS: " + modelRuns.Count);
    }

    /// <summary>
    /// This needs to be tested.
    /// </summary>
    /// <param name="Records"></param>
    /// <param name="message"></param>
    static private void onGetAvailableComplete(List<DataRecord> Records, DataRecordSetter message)
    {
        // Debug.LogError ("TOTAL!!!: " + modelRuns[Records[0].modelRunUUID].Total + " Recieved Records: " + Records.Count + " Totals: " + modelRuns[Records[0].modelRunUUID].CurrentCapacity);
        // Logger.WriteLine(Records.Count.ToString());
        int count = 0;
        List<string> RecievedRefs = new List<string>();
        foreach (DataRecord rec in Records)
        {
            //Logger.WriteLine(rec.modelRunUUID);
            //Logger.WriteLine(rec.start.ToString());
            // We should play with the other of the if statements...
            // Normal Case
            if (modelRuns.ContainsKey(rec.modelRunUUID))
            {
                // Call insert operation
                //Logger.WriteLine("ADDED");
                //Logger.WriteLine("ADDED: " + rec.name);
                lock (_Padlock)
                {
                    modelRuns[rec.modelRunUUID].Insert(rec);
                }
                count++;
                //Logger.WriteLine(modelRuns[rec.modelRunUUID].Insert(rec).ToString());
                //Debug.LogError(rec.modelRunUUID + " " + modelRuns[rec.modelRunUUID].Total + "OUCHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH!!!!");
                // Replace with isFull Function
                if (modelRuns[rec.modelRunUUID].CurrentCapacity == modelRuns[rec.modelRunUUID].Total)
                {
                    // Cash it in!!!!
                    Debug.LogError("The model run is now in the cache.");
                    FileBasedCache.Insert<ModelRun>(rec.modelRunUUID, modelRuns[rec.modelRunUUID]);
                    // Debug.LogError("DONE CACHING YEAH!!!!");
                }
            }
            // Cache Case
            else if (FileBasedCache.Exists(rec.modelRunUUID))
            {
                // Handle it
            }
            // Normal Case
            else if (!modelRuns.ContainsKey(rec.modelRunUUID))
            {
                // Cache Case -- Check if cache has a georef

                // Normal Case -- Insert it into storedModelRuns
                Logger.WriteLine("ADDED: " + rec.name);

                modelRuns.Add(rec.modelRunUUID, new ModelRun(rec.modelname, rec.modelRunUUID));

                // Call the insert
                lock (_Padlock)
                {
                    // This is where things get interesting
                    if(rec != null)
                    {

                    }
                    modelRuns[rec.modelRunUUID].Insert(rec);
                    count++;
                    //  Testing Variable
                    if (!RecievedRefs.Contains(rec.modelRunUUID))
                    {
                        RecievedRefs.Add(rec.modelRunUUID);
                    }
                }
            }
        }
        foreach (var i in modelRuns)
        {
            break;
            //Logger.WriteLine( i.Value.ModelName + ":NAME "  + i.Value.ModelRunUUID + ":UUID " + i.Value.Count().ToString());
        }
        if (message != null)
        {
            message(Records);
        }
        Logger.WriteLine("<color=green>Created this many model runs: " + modelRuns.Count + "</color>");
        // Debug.LogError("NUMBER ADDED: " + count);
        //foreach (var i in modelRuns)
        //{
        //    i.Value.DownloadDatasets();
        //}
    }

    static public void OnModelRunDataAvaliable(List<DataRecord> Records)
    {
        // Debug.LogError("Populating Model Run");
        // Debug.LogError("RECIEVED DATA OF SIZE: " + Records.Count);
        Counter += Records.Count;
        onGetAvailableComplete(Records, (DataRecordSetter)null);
    }

    /// <summary>
    /// This needs to be tested.
    /// </summary>
    /// <param name="Records"></param>
    /// <param name="message"></param>
    static private void onGetAvailableComplete(List<DataRecord> Records, GeoRefMessage message)
    {
        // Logger.WriteLine(Records.Count.ToString());
        List<string> RecievedRefs = new List<string>();
        foreach (DataRecord rec in Records)
        {
            // Logger.WriteLine(rec.modelRunUUID);
            // Logger.WriteLine(rec.start.ToString());
            // We should play with the other of the if statements...
            // Normal Case
            if (modelRuns.ContainsKey(rec.modelRunUUID))
            {
                // Call insert operation
                // Logger.WriteLine("ADDED");
                modelRuns[rec.modelRunUUID].Insert(rec);
                //modelRuns[rec.modelRunUUID].CurrentCapacity++;
                // Replace with isFull Function
                if (modelRuns[rec.modelRunUUID].CurrentCapacity == modelRuns[rec.modelRunUUID].Total)
                {
                    // Cash it in!!!!
                    FileBasedCache.Insert<ModelRun>(rec.modelRunUUID, modelRuns[rec.modelRunUUID]);
                }
            }
            // Cache Case
            else if (FileBasedCache.Exists(rec.modelRunUUID))
            {
                // Handle it
            }
            // Normal Case -- create a model run
            else if (!modelRuns.ContainsKey(rec.modelRunUUID))
            {
                // Cache Case -- Check if cache has a georef

                // Normal Case -- Insert it into storedModelRuns
                // Logger.WriteLine("ADDED");
                modelRuns.Add(rec.modelRunUUID, new ModelRun(rec.modelname, rec.modelRunUUID));

                // Call the insert
                modelRuns[rec.modelRunUUID].Insert(rec);

                //  Testing Variable
                if (!RecievedRefs.Contains(rec.modelRunUUID))
                {
                    RecievedRefs.Add(rec.modelRunUUID);
                }
            }
        }

        if (message != null)
        {
            message(RecievedRefs);
        }
        // Logger.WriteLine("CREATED THIS MANY MODEL RUNS: " + modelRuns.Count);
        foreach (var i in modelRuns)
        {
            // Logger.WriteLine(i.Value.VariableCount().ToString());
            i.Value.DownloadDatasets();
        }
    }

    static public void OnClose()
    {
        // Save things to cache
        //FileBasedCache.Insert<Dictionary<string, GeoReference>>(cacheRestoreEntry, storedGeoRefs);
        // or should we clear the cache!!!!!
    }

    static public void PopulateModelRunData(string ModelRunUUID)
    {
        // Get the amount of Records for this particular ModelRun --- NOTE ---- 
        // This is working off the single tiff which will need to be replaced in the future to handle Netcdf....
        // We will need to spin off a thread somewhere maybe.
        SystemParameters sp = new SystemParameters();
        sp.model_run_uuid = ModelRunUUID;
        client.PopulateModelRun(sp);

    }

    static public void RemoveRecordData(string ModelRunUUID)
    {
        //client.RemoveJobsByModelRunUUID (ModelRunUUID);
    }

    static public void InsertModelRun(string uuid, ModelRun mr)
    {
        modelRuns[uuid] = mr;
    }
    /// Closing on terrain 
    /// load cache on startup ---- check
    /// Closing on Program --- check
    /// Cache Model runs
    /// Cache User parameters
}
