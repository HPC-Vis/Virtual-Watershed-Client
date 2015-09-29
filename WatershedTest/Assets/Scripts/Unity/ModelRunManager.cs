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
    static bool locationUpdate = false;

    // Global loading counter
    static public int Total = 0;
    static public int Counter = 0;

    static bool timeToCache = true;

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
            Debug.LogError("Restoring Previous Session");

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

    static public int ModelRunCount()
    {
        if (locationUpdate)
        {
            locationUpdate = false;
            return modelRuns.Count + 1;
        }

        return modelRuns.Count;
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
        Logger.enable = true;
        Logger.WriteLine("Searching for Model Runs");
        if (FileBasedCache.Exists("startup"))
        {
            Logger.WriteLine("<color=red>Aquired data from the cache system.</color>");
            modelRuns = FileBasedCache.Get<Dictionary<string, ModelRun>>("startup");
            Logger.WriteLine("The Size of Model Runs: " + modelRuns.Count);
        }
        client.RequestModelRuns(OnGetModelRuns, param);
    }

    // NOTE: Populating the data inside a datarecord. Something like building the texture.
    // Gonna need a parameter object for this ----- for now just defaults
    // TODO Consider Removing DataRecordSetter
    static public void Download(List<DataRecord> records, DataRecordSetter SettingTheRecord, string service = "vwc", string operation = "wcs", SystemParameters param = null)
    {
        //Logger.enable = false;
        Logger.enable = true;

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
                // Start Thread
                new Thread(() =>
                {

                    foreach (var i in records)
                    {
                        //Debug.LogError(i.name);
                        if (operation == "wms" && i.services.ContainsKey("wms"))
                        {
                            Debug.LogError("WMS");
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


                        else if (operation == "wcs" && i.services.ContainsKey("wcs"))
                        {
                            // Lets check if it exists in the cache by uuid
                            // Debug.LogError( "DATA: + " + (i.Data == null).ToString())
                            // Debug.LogError("ID: " + i.id);
                            if (FileBasedCache.Exists(i.id) && i.Data.Count == 0)
                            {
                                // Debug.LogError("Recieved the cache for UUDI: " + i.id);
                                i.Data = FileBasedCache.Get<DataRecord>(i.id).Data;
                                i.bbox2 = FileBasedCache.Get<DataRecord>(i.id).bbox2;
                                i.bbox = FileBasedCache.Get<DataRecord>(i.id).bbox;
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }
                            else if (i.Data.Count !=0)
                            {
                                //Debug.LogError("IN CACHE: " + FileBasedCache.Exists(i.id) + " Data: " + i.Data.GetLength(0) + " ID: " + i.id);
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }
                            client.getCoverage(SettingTheRecord, i, param);
                        }
                        else if (operation == "wfs" && i.services.ContainsKey("wfs"))

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
                        else if (i.services.ContainsKey("file"))
						{
							Debug.LogError("Loading FIle");
                            
                            /*if (FileBasedCache.Exists(i.id) && i.Data.Count == 0)
                            {
                                //Debug.LogError("EXISTS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!: " + i.id);
                                i.Data = FileBasedCache.Get<DataRecord>(i.id).Data;
                                i.bbox2 = FileBasedCache.Get<DataRecord>(i.id).bbox2;
                                i.bbox = FileBasedCache.Get<DataRecord>(i.id).bbox;
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }
                            else if (i.Data.Count != 0)
                            {
                                //Debug.LogError("IN CACHE: " + FileBasedCache.Exists(i.id) + " Data: " + i.Data.GetLength(0) + " ID: " + i.id);
                                SettingTheRecord(new List<DataRecord> { i });
                                continue;
                            }*/

                            RasterDataset rd = new RasterDataset(i.services["file"]);
                            if(rd.Open())
                            {

                                var da = rd.GetData();

                                //temporary patch is gross
                                Spooler.TOTAL = da.Count;

                                for (int j = 0; j < da.Count; j++)
                                {
                                    DataRecord recClone = i.Clone();
                                    recClone.Data.Add(da[j]);
                                    recClone.band_id = j + 1;

                                    var TS = i.end.Value - i.start.Value;
                                    double totalhours = TS.TotalHours / da.Count;
                                    Debug.LogError(totalhours);
                                    recClone.start += new TimeSpan((int)Math.Round((double)j * totalhours), 0, 0);
                                    recClone.end = recClone.start + new TimeSpan((int)Math.Round(totalhours), 0, 0);
                                    SettingTheRecord(new List<DataRecord> { recClone });
                                }
                            }
						}
                        else if(i.services.ContainsKey("nc"))
                        {
                            Debug.LogError("NCNESSS!!!!");
                        }
                    }
                }).Start();
                // End Thread
            }
        }
    }

    // Has Model Run Name
    static public bool HasModelRunName(string name)
    {
        foreach(var i in modelRuns)
        {
            if(i.Value.Name == name || i.Value.ModelName == name)
            {
                return true;
            }
        }
        return false;
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
        	if(parameters == null)
        	{
        		Runs.Add(i.Value);
        	}
            // Find matching model runs
            // This can be optimized by storing additional information in the ModelRun class.
            else if (parameters.model_set_type == i.Value.ModelDataSetType)
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
    static public void getAvailable(SystemParameters param, DataRecordSetter Setter = null)
    {
        // TODO
        client.RequestRecords(((List<DataRecord> records) => onGetAvailableComplete(records, Setter)), param);
    }

    private static readonly object CacheLock = new object();
    private static void SetModelSetType(List<DataRecord> Records)
    {

        foreach (var i in Records)
        {
            //Debug.LogError("The uuid is: " + i.modelRunUUID);
            //Debug.LogError(" The location: " + i.location);
            if (modelRuns.ContainsKey(i.modelRunUUID))
            {
                // Get Model Set Type
                modelRuns[i.modelRunUUID].ModelDataSetType = i.model_set_type;
                modelRuns[i.modelRunUUID].Location = i.location;
                modelRuns[i.modelRunUUID].Description = i.description;
            }
            else
            {
				Logger.WriteLine("FAILURE MODEL RUN DOES NOT EXIST");
            }
        }

        locationUpdate = true;

        // Cache the records
        /*
        lock (CacheLock)
        {
            Logger.WriteLine("Adding modelruns to file cache system.");
            FileBasedCache.Insert<Dictionary<string, ModelRun>>("startup", modelRuns);
        }
         */
    }


    private static void OnGetModelRuns(List<DataRecord> Records)
    {
        Dictionary<string, ModelRun> startRuns = new Dictionary<string, ModelRun>();

        Logger.Log("Getting Model Runs");
        foreach (var i in Records)
        {
            if (!modelRuns.ContainsKey(i.modelRunUUID))
            {
                modelRuns.Add(i.modelRunUUID, new ModelRun(i.modelname, i.modelRunUUID, i.description));
            }
        }
       
		foreach(var i in modelRuns.Values)
		{
			// SetModelSetType
			SystemParameters sp = new SystemParameters();
			sp.model_run_uuid = i.ModelRunUUID;
			sp.limit = 1;
			sp.offset = 0;
			client.RequestRecords(SetModelSetType, sp);
		}

        Logger.WriteLine("Adding modelruns to file cache system.");
        FileBasedCache.Insert<Dictionary<string, ModelRun>>("startup", modelRuns);
    }

	public static bool InsertDataRecord(DataRecord record, List<DataRecord> allRecords)
	{
        //modelRuns[record.modelRunUUID].successfulRuns.Add(record.id, record.id);
        if(modelRuns[record.modelRunUUID].CurrentCapacity == (modelRuns[record.modelRunUUID].Total))
        {
            GlobalConfig.caching = true;
            foreach (DataRecord rec in allRecords)
            {
                FileBasedCache.Insert<ModelRun>(rec.modelRunUUID, modelRuns[rec.modelRunUUID]);
                //Logger.WriteLine("The model run is now in the cache.");
            }
            GlobalConfig.caching = false;
        }
        else if (modelRuns[record.modelRunUUID].CurrentCapacity >= (modelRuns[record.modelRunUUID].Total * 0.95) && timeToCache)
        {
            // Start Thread
            timeToCache = false;
            new Thread(() =>
            {
                GlobalConfig.caching = true;
                foreach (DataRecord rec in allRecords)
                {
                    FileBasedCache.Insert<ModelRun>(rec.modelRunUUID, modelRuns[rec.modelRunUUID]);
                    //Logger.WriteLine("The model run is now in the cache.");
                }
                GlobalConfig.caching = false;
            }).Start();
        }

        return modelRuns[record.modelRunUUID].Insert(record);
	}

	public static void parseNetCDFRecords(List<DataRecord> record)
	{
		Logger.WriteLine ("PARSENETCDFRECORDS" + (record[0].WCSCoverages != null).ToString());
		if (record.Count > 0) 
		{
            // Add remove function here
            modelRuns[record[0].modelRunUUID].Remove(record[0]);
			if(record[0].WCSCoverages != null)
			{
                for (int i = 0; i < record[0].WCSCoverages.Length; i++ )
                {

                    var dr = record[0].Clone();
                    dr.band_id = 1;
                    dr.Identifier = record[0].WCSCoverages[i].Identifier;
                    dr.variableName = record[0].WCSCoverages[i].Identifier;
                    if (record[0].WCSCoverages.Count() > 1)
                        dr.Temporal = true;
                    Logger.WriteLine("A NEW RECORLDsss: " + record[0].WCSCoverages[i].Identifier);
                    //InsertDataRecord(dr, record);

                    // Run Describe Coverage on these guys to spawn the rest of the records ... Yay Propagations tasks .... harder to debug.
                    client.describeCoverage(((records) => InsertDataRecord(records[0],record)), dr, new SystemParameters());
                    // client.describeCoverage(CreateNewBands, dr, new SystemParameters());
                    //break;
                }
			}
			// See if layers variable is defined
			else if (record [0].wmslayers != null) 
			{
				// Lets create some new data records off of this one... and populate the boundingbox information with it as well.
				foreach (var i in record[0].wmslayers) 
				{
					DataRecord dr = record[0].Clone();
					dr.Identifier = i.Name;
					dr.variableName = i.Name; // similar to wcs identifier
					if(record[0].wmslayers.Count() > 1)
					dr.Temporal = true;
					// WMS Bounding Box
                    InsertDataRecord(dr, record);
					Logger.WriteLine (dr.variableName);

				}
			}
			else
			{
				// WFS CASE HERE FOR NOW
				record[0].band_id = 1;
				// Get WFS Name here
                InsertDataRecord(record[0], record);
			}

            


		}
	}

    /// <summary>
    /// Takes in a uint that is a date of the format yyyymmdd and converts it to a DateTime object.
    /// </summary>
    /// <param name="udate"></param>
    /// <returns></returns>
    static DateTime uintToDateTime(uint udate)
    {
        int date = (int)udate;
        Debug.LogError(date + " " + udate);
        int month = date / 1000000;
        int day = date / 10000 % 100;
        int year = date % 10000;
        Debug.LogError(year + " " + day + " " + month);
        return new DateTime(year,month,day);
    }

    /// <summary>
    /// Popluating start times for a netcdf record using the fdgc metadata that is in the virtual watershed. This should be moved to the vwclient at some point...
    /// </summary>
    /// <param name="records"></param>
    public static void PopulateStartTimes(List<DataRecord> records)
    {
        Logger.WriteLine("POPLULATING NETCDF START TIMES");
        Debug.LogError("POPLULATING NETCDF START TIMES");
        SystemParameters sp = new SystemParameters();
        if (records == null || records.Count == 0)
            return;

        // Lets set the start times of this single record and then do the ogc services
        var record = records[0];
        if (record.metaData != null && record.metaData.idinfo != null && record.metaData.idinfo.timeperd != null)
        {
            if (record.metaData.idinfo.timeperd.timeinfo.rngdates != null)
            {
                record.start = uintToDateTime(record.metaData.idinfo.timeperd.timeinfo.rngdates.begdate);
                record.end = uintToDateTime(record.metaData.idinfo.timeperd.timeinfo.rngdates.enddate);
                UnityEngine.Debug.LogError("A START RECORD TIME: " + record.start);
            }
        }
        else
        {
            record.start = DateTime.Today - new TimeSpan(365, 0, 0, 0, 0);
            record.end = DateTime.Today ;
        }
        //Debug.LogError(record.multiLayered); patch is record.multilayered..
        if (record.services.ContainsKey("wcs") )
        {
            sp.service = "wcs";
            Logger.WriteLine("WCS");
            client.getCapabilities(parseNetCDFRecords, record, sp);
        }

        else if (record.services.ContainsKey("wms") )
        {
            sp.service = "wms";
            Logger.WriteLine("WMS");
            client.getCapabilities(parseNetCDFRecords, record, sp);
        }
        else if (record.services.ContainsKey("wfs") )
        {
            sp.service = "wfs";
            Logger.WriteLine("WFS CAPABILTIERS FILTER HERE NOW ");
            client.getCapabilities(parseNetCDFRecords, record, sp);
        }
        Debug.LogError("DONE WITH START TIMES");
    }

    // No Longer Used
	public static void CreateNewBands(List<DataRecord> record)
	{
        Logger.WriteLine("RECORD KING: " + record[0].numbands.ToString() + " " + record[0].band_id.ToString() + "LIST COUNT: " + record.Count + " " + record[0].Identifier + "HAS TIME: " + record[0].start);

        // The first record contains the total overall period for a model run.. let calculate a average time period
        var timeperiod = record[0].end - record[0].start;
        double duration = Math.Round(timeperiod.Value.TotalHours / record[0].numbands);
        Debug.LogError("DURATION: " + duration);
        TimeSpan ts = new TimeSpan((int)duration, 0, 0);
        record[0].end = record[0].start + ts;
		// Lets create the records for this guy...
		for(int i =2; i <= record[0].numbands; i++)
		{
			DataRecord dr = record[0].Clone();
			dr.band_id = i;
			dr.id += dr.band_id.ToString() + dr.variableName;
            try
            {
                var ts2 = new TimeSpan((int)ts.TotalHours * (i - 1), 0, 0);
                var ts3 = new TimeSpan((int)ts.TotalHours * i, 0, 0);
                dr.start = record[0].start + ts2;
                dr.end = record[0].start + ts3;
                Debug.LogError("START: " + dr.start.HasValue);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }
            //Debug.LogError("END: " + dr.end);
            Logger.WriteLine("SUCCESSFUL ADD?: " + InsertDataRecord(dr, record).ToString());
		}
	}
	// Filter function goes here.
	static void filter(DataRecord record, List<DataRecord> allRecords)
	{
        Logger.WriteLine("FILTERING");
        // Add it to its perspective model run
        record.band_id = 1;
        //InsertDataRecord(record);

		Logger.WriteLine ("FILTER");
        foreach(var i in record.services.Keys)
        {
            Debug.LogError("SERVICES: " + i);
        }

        if(record.services.ContainsKey("nc"))
        {
            // Add it to its perspective model run
            record.band_id = 1;
            Debug.LogError("NETCDF");
            new Thread(() =>
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                var data = wc.DownloadData(record.services["nc"]);
                var ls = record.services["nc"].Split(new char[] { '/' },StringSplitOptions.RemoveEmptyEntries);
                string filename = "./"+ls[ls.Length - 1];
                Debug.LogError(filename);
                if (!System.IO.File.Exists(filename))
                {
                    var fdesc = System.IO.File.Create(filename);
                    fdesc.Write(data, 0, data.Length);
                    fdesc.Close();

                    string FileName = RasterDataset.GetGdalPath(filename);
                    RasterDataset modelData = new RasterDataset(FileName);
                    if (modelData.Open())
                    {
                        // 
                        //Debug.LogError("ITS ALIVE");



                        // Get any subdatasets associate to this file
                        List<string> subSets = modelData.GetSubDatasets();
                        if (subSets.Count == 0)
                        {
                            DateTime tempTime = new DateTime();
                            TimeSpan tempSpan = new TimeSpan();
                            DataRecord rec = new DataRecord(filename.ToString());
                            rec.variableName = filename;//str.Contains("NETCDF") ? str.Replace(FileName + ":", "") : str;
                            rec.name = rec.variableName;
                            //rec.Data = rd.GetData();
                            rec.modelname = rec.modelname;
                            rec.modelRunUUID = rec.modelRunUUID;
                            rec.id = Guid.NewGuid().ToString();
                            rec.location = GlobalConfig.Location;
                            rec.Temporal = modelData.IsTemporal();//(rec.Data.Count > 1);
                            rec.Type = "DEM";
                            modelData.GetTimes(out tempTime, out tempSpan);
                            rec.start = tempTime;
                            rec.end = tempTime + tempSpan;
                            rec.bbox = modelData.GetBoundingBox();
                            rec.projection = modelData.ReturnProjection();
                            rec.numbands = modelData.GetRasterCount();
                            rec.services["file"] = FileName.ToString();
                            ModelRunManager.InsertDataRecord(rec, new List<DataRecord>());
                        }

                        // Populate datarecords for each subdatasets
                        foreach (String str in subSets)
                        {
                            Debug.LogError("PROCESSING THIS STR: " + str);
                            RasterDataset rd = new RasterDataset(str);
                            if (rd.Open())
                            {

                                DateTime tempTime = new DateTime();
                                TimeSpan tempSpan = new TimeSpan();
                                DataRecord rec = new DataRecord(str);
                                rec.variableName = str.Contains("NETCDF") ? str.Replace(FileName + ":", "") : str;
                                rec.name = rec.variableName;
                                //rec.Data = rd.GetData();
                                rec.modelname = record.modelname;
                                rec.modelRunUUID = record.modelRunUUID;
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
                                ModelRunManager.InsertDataRecord(rec, new List<DataRecord>());
                            }

                        }
                    }
                }
            }).Start();
        }
        else if (record.services.Keys.Count >= 2 && record.multiLayered != null)
        {
            client.GetMetaData(PopulateStartTimes, new List<DataRecord> { record });
        }
		else 
		{
			// Add it to its perspective model run
			record.band_id = 1;
            InsertDataRecord(record, allRecords);
		}
	}

    /// <summary>
    /// This needs to be tested.
    /// </summary>
    /// <param name="Records"></param>
    /// <param name="message"></param>
    static private void onGetAvailableComplete(List<DataRecord> Records, DataRecordSetter message)
    {
    	Debug.LogError("onGetAvaliableComplete...");
         Logger.WriteLine(Records.Count.ToString());
        List<string> RecievedRefs = new List<string>();
        foreach (DataRecord rec in Records)
        {
            //Logger.WriteLine(rec.modelRunUUID);
            //Logger.WriteLine(rec.start.ToString());
            // We should play with the other of the if statements...
            // Normal Case
            if (modelRuns.ContainsKey(rec.modelRunUUID))
            {
				//Debug.LogError("modelRuns.ContatinsKey(rec.modelRunUUID)");

                // Call insert operation
                //Logger.WriteLine("ADDED");
                //Logger.WriteLine("ADDED: " + rec.name);
                lock (_Padlock)
                {
					filter (rec, Records);
                    //modelRuns[rec.modelRunUUID].Insert(rec);
                }
                //Logger.WriteLine(modelRuns[rec.modelRunUUID].Insert(rec).ToString());
                //Debug.LogError(rec.modelRunUUID + " " + modelRuns[rec.modelRunUUID].Total + "OUCHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH!!!!");
                // Replace with isFull Function
                //if (modelRuns[rec.modelRunUUID].CurrentCapacity <= modelRuns[rec.modelRunUUID].Total)
                //{
                    // Cash it in!!!!
				//	Logger.WriteLine("The model run is now in the cache.");
                //    FileBasedCache.Insert<ModelRun>(rec.modelRunUUID, modelRuns[rec.modelRunUUID]);
                //    Debug.LogError("DONE CACHING YEAH!!!!");
                //}
            }
            // Cache Case
            else if (FileBasedCache.Exists(rec.modelRunUUID))
            {
                // Handle it
            }
            // Normal Case
            else if (!modelRuns.ContainsKey(rec.modelRunUUID))
            {
                // Normal Case -- Insert it into storedModelRuns
                Logger.WriteLine("ADDED: " + rec.name);

                modelRuns.Add(rec.modelRunUUID, new ModelRun(rec.modelname, rec.modelRunUUID, rec.description));

                // Call the insert
                lock (_Padlock)
                {
                    // This is where things get interesting
                    if(rec != null)
                    {

                    }
                    modelRuns[rec.modelRunUUID].Insert(rec);
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
    }

    static public void OnModelRunDataAvaliable(List<DataRecord> Records)
    {
        // Debug.LogError("Populating Model Run");
        // Debug.LogError("RECIEVED DATA OF SIZE: " + Records.Count);
        Counter += Records.Count;
        onGetAvailableComplete(Records, (DataRecordSetter)null);
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

    static public int GetModelRunCount()
    {
        return modelRuns.Count;
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
