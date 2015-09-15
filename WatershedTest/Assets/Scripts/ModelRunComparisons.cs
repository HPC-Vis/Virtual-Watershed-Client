using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading;


public class ModelRunComparisons : MonoBehaviour {

    public ModelRunVisualizer table;
    public Spooler spool;
    public GameObject compareButton;
    public List<Frame> DataSetOne, DataSetTwo;
    public List<Frame> ComparedDataSet;
    public Queue<DataRecord> DataRecordOne, DataRecordTwo;
    public List<DataRecord> DeltaRecords;
    int OneTotal, TwoTotal;
    List<ModelRun> modelRuns;
    List<object[]> selectedRow;
    Thread ProcessThread;
    string BBOX, BBOX2, projection;
    SerialRect boundingbox;
    SystemParameters sp;
    float OneMax, OneMin, TwoMax, TwoMin;

	// Use this for initialization
	void Start ()
    {
        ProcessThread = null;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Check if the button needs to be set active
	    if(table.listView.GetSelectedModelRuns().Count == 2)
        {
            compareButton.GetComponent<Button>().interactable = true;
        }
        else if (compareButton.GetComponent<Button>().interactable)
        {
            compareButton.GetComponent<Button>().interactable = false;
        }
	}

    /// <summary>
    /// This is used once the compare button is clicked. It will get the necessary 
    /// model run info and begin to compare.
    /// </summary>
    public void StartComparison()
    {
        // Build variables
        DataRecordOne = new Queue<DataRecord>();
        DataRecordTwo = new Queue<DataRecord>();
        DeltaRecords = new List<DataRecord>();
        DataSetOne = new List<Frame>();
        DataSetTwo = new List<Frame>();
        OneMax = float.MinValue;
        OneMin = float.MaxValue;
        TwoMax = float.MinValue;
        TwoMin = float.MaxValue;

        // Get the model runs selected
        selectedRow = table.listView.GetSelectedRowContent();
        modelRuns = table.listView.GetSelectedModelRuns();

        // Setup the Records
        var RecordsOne = modelRuns[0].FetchVariableData(selectedRow[0][2].ToString());
        var RecordsTwo = modelRuns[1].FetchVariableData(selectedRow[1][2].ToString());
        OneTotal = RecordsOne.Count;
        TwoTotal = RecordsTwo.Count;
        sp = new SystemParameters();
        sp.interpolation = "bilinear";
        sp.width = 100;
        sp.height = 100;
        
        // Download the data
        if (!modelRuns[0].Description.ToLower().Contains("doqq") && !modelRuns[1].Description.ToLower().Contains("doqq"))
        {
            ModelRunManager.Download(RecordsOne, HandDataToOne, param: sp);
            ModelRunManager.Download(RecordsTwo, HandDataToTwo, param: sp);
        }

        // Now Begin the data processing
        Logger.WriteLine("Now Starting Thread for Comparison.");
        ProcessThread = new Thread(() => { ProcessData(); });
        ProcessThread.Start();
    }

    /// <summary>
    /// This will handle wcs related requests for the first record.
    /// </summary>
    /// <param name="Records">The list of records to add.</param>
    void HandDataToOne(List<DataRecord> Records)
    {
        try
        {
            foreach (var d in Records[0].Data)
            {
                foreach (var i in d)
                {
                    if (OneMax < i)
                    {
                        OneMax = i;
                    }
                    if (OneMin > i)
                    {
                        OneMin = i;
                    }
                }
            }
        }
        catch(System.Exception a)
        {
            OneTotal = OneTotal - 1;
            Debug.LogError("There was no data in record on one.");
            return;
        }
        DataRecordOne.Enqueue(Records[0]);
    }

    /// <summary>
    /// This will handle wcs related requests for the second record.
    /// </summary>
    /// <param name="Records">The list of records to add.</param>
    void HandDataToTwo(List<DataRecord> Records)
    {
        try
        {
            foreach (var d in Records[0].Data)
            {
                foreach (var i in d)
                {
                    if (TwoMax < i)
                    {
                        TwoMax = i;
                    }
                    if (TwoMin > i)
                    {
                        TwoMin = i;
                    }
                }
            }
        }
        catch (System.Exception a)
        {
            TwoTotal = TwoTotal - 1;
            Debug.LogError("There was no data in record on two.");
            return;
        }
        DataRecordTwo.Enqueue(Records[0]);
    }

    /// <summary>
    /// This will stop the thread that is downloading.
    /// </summary>
    public void OnDestroy()
    {
        OneTotal = 0;
        TwoTotal = 0;
        if(ProcessThread != null)
        {
            try
            {
                Debug.LogError("The Thread Should have been killed.");
                ProcessThread.Abort();
            }
            catch (System.Exception a)
            {
                Debug.LogError("Thread Abort Problem: " + a.Message);
            }
        }
    } 


    /// <summary>
    /// This is done by the thread to download the data, and build the Frame.
    /// </summary>
    void ProcessData()
    {
        // Run once to get the BBOX and projection
        while (DataRecordOne.Count == 0 && DataRecordTwo.Count == 0) ;
        if(DataRecordOne.Count > 0)
        {
            BBOX = DataRecordOne.Peek().bbox;
            BBOX2 = DataRecordOne.Peek().bbox2;
            boundingbox = DataRecordOne.Peek().boundingBox;
            projection = DataRecordOne.Peek().projection;
        }
        else
        {
            BBOX = DataRecordTwo.Peek().bbox;
            BBOX2 = DataRecordTwo.Peek().bbox2;
            boundingbox = DataRecordTwo.Peek().boundingBox;
            projection = DataRecordTwo.Peek().projection;
        }

        int dequeueOne, dequeueTwo;
        Debug.LogError("Running loop to this number: " + Math.Min(OneTotal, TwoTotal));
        while (DataSetTwo.Count < TwoTotal || DataSetOne.Count < OneTotal)
        {
            //Debug.LogError("Running the thread. " + DataSetTwo.Count + ", " + DataSetOne.Count);
            // Do the first dataset
            dequeueOne = DataRecordOne.Count;
            for (int i = 0; i < dequeueOne; i++)
            {
                Debug.LogError("Beginng one removal.");
                try
                {
                    DataRecord record = DataRecordOne.Dequeue();
                    BuildFrame(record, modelRuns[0].ModelRunUUID, DataSetOne, OneTotal);
                    Debug.LogError("DataSetOneAdded.");
                }
                catch(System.Exception a)
                {
                    OneTotal = OneTotal - 1;
                    Debug.LogError("There was an error adding a dataset to DataSetOne");
                }
            }

            // Do the second dataset
            dequeueTwo = DataRecordTwo.Count;
            for (int i = 0; i < dequeueTwo; i++)
            {
                Debug.LogError("Beginng two removal.");
                try
                {
                    DataRecord record = DataRecordTwo.Dequeue();
                    BuildFrame(record, modelRuns[1].ModelRunUUID, DataSetTwo, TwoTotal);
                    Debug.LogError("DataSetTwoAdded.");
                }
                catch(System.Exception a)
                {
                    TwoTotal = TwoTotal - 1;
                    Debug.LogError("There was an error adding a dataset to DataSetTwo");
                }
            }
        }

        Logger.WriteLine("We have now gotten all the data.");
        BuildDeltaSet();
    }

    /// <summary>
    /// Builds the frame of the data.
    /// </summary>
    /// <param name="record">The record to build the frame from.</param>
    /// <param name="selectedModelRun">The string of the required UUID.</param>
    /// <param name="dataset">The dataset that the frame is to be added to.</param>
    /// <param name="total">The grand total of the sent in dataset.</param>
    void BuildFrame(DataRecord record, string selectedModelRun, List<Frame> dataset, int total)
    {
        // Caching 
        if (!FileBasedCache.Exists(record.id))
        {
            //Debug.LogError("INSERTING INTO CACHE " + rec.id);
            FileBasedCache.Insert<DataRecord>(record.id, record);
        }

        // This is not the model run we want because something else was selected.
        if (record.modelRunUUID != selectedModelRun)
        {
            return;
        }

        for (int j = 0; j < record.Data.Count; j++)
        {
            // Build the Frame to pass in
            Frame frame = new Frame();

            frame.starttime = record.start.Value;
            frame.endtime = record.end.Value;
            frame.Data = record.Data[j];

            // Checks for NULL downloaded data
            if (record.Data == null)
            {
                Debug.LogError("The data at UUID = " + record.id + " was null.");
                return;
            }

            Insert(frame, dataset, total);
        }
    }

    /// <summary>
    /// Insert the Frame into the set list. 
    /// </summary>
    /// <param name="insert">The Frame to insert.</param>
    /// <param name="dataset">The dataset to place the fram into.</param>
    /// <param name="total">The total of all data needed.</param>
    void Insert(Frame insert, List<Frame> dataset, int total)
    {
        int index = dataset.BinarySearch(insert, new FrameEndDateAscending());

        // This is to help pinpoint too many records added to the Reel
        if (dataset.Count >= total)
        {
            Debug.LogError("Why is there more records being added to the Reel?");
            Debug.LogError("Here is out frame starttime: " + insert.starttime + " and the count is: " + dataset.Count);
        }

        //if index >= 0 there is a duplicate 
        if (index >= 0)
        {
            total--;
        }
        else
        {
            dataset.Insert(~index, insert);
        }
    }

    /// <summary>
    /// Builds the delta set with the two downloaded sets.
    /// </summary>
    void BuildDeltaSet()
    {
        // return if there is no data
        if (Math.Min(OneTotal, TwoTotal) == 0)
        {
            return;
        }

        // Build the model run
        ModelRun deltarun = new ModelRun("Delta " + selectedRow[0][2].ToString() + " " + selectedRow[1][2].ToString(), modelRuns[1].ModelRunUUID + modelRuns[0].ModelRunUUID);
        deltarun.Description = "Delta " + selectedRow[0][2].ToString() + " minus " + selectedRow[1][2].ToString();
        deltarun.Location = GlobalConfig.Location;
        ModelRunManager.InsertModelRun(modelRuns[1].ModelRunUUID + modelRuns[0].ModelRunUUID, deltarun);
        VariableReference.AddDescription("Delta_" + selectedRow[0][2].ToString() + "_" + selectedRow[1][2].ToString(), "Delta " + selectedRow[0][2].ToString() + " minus " + selectedRow[1][2].ToString());
        spool.SetupForDelta("Delta " + selectedRow[0][2].ToString() + " minus " + selectedRow[1][2].ToString(), "Delta_" + selectedRow[0][2].ToString() + "_" + selectedRow[1][2].ToString(), Math.Min(OneTotal, TwoTotal), modelRuns[1].ModelRunUUID + modelRuns[0].ModelRunUUID);
        float NoData = float.MaxValue;
        float NoDataAssignment = Math.Min(OneMin, TwoMin) - Math.Max(OneMax, TwoMax);
        Debug.LogError("The values: " + OneMax + ", " + OneMin + ", " + TwoMax + ", " + TwoMin);
        Debug.LogError("The No data value is: " + NoDataAssignment);

        foreach(var i in DataSetOne[0].Data)
        {
            if(i < NoData)
            {
                NoData = i;
            }
        }

        Debug.LogError("The Totals: " + OneTotal + ", " + TwoTotal);
        // Build DataRecord
        for(int k = 0; k < Math.Min(OneTotal, TwoTotal); k++)
        {
            try
            {
                DataRecord insert = new DataRecord(selectedRow[0][2].ToString() + "-" + selectedRow[1][2].ToString());
                insert.location = GlobalConfig.Location;
                insert.modelname = deltarun.Name;
                insert.Temporal = true;
                insert.modelRunUUID = modelRuns[1].ModelRunUUID + modelRuns[0].ModelRunUUID;
                insert.id = Guid.NewGuid().ToString();
                insert.variableName = "Delta_" + selectedRow[0][2].ToString() + "_" + selectedRow[1][2].ToString();
                insert.bbox = BBOX;
                insert.bbox2 = BBOX2;
                insert.boundingBox = boundingbox;
                insert.projection = projection;

                insert.Min = float.MaxValue;
                insert.Max = float.MinValue;

                insert.start = DataSetOne[k].starttime;
                insert.end = DataSetOne[k].endtime;

                // Add the Datarecord to the ModelRunManager
                ModelRunManager.InsertDataRecord(insert.Clone(), DeltaRecords);

                insert.Data = new List<float[,]>();
                insert.Data.Add(new float[sp.height, sp.width]);

                for(int i = 0; i < sp.width; i++)
                {
                    for(int j = 0; j < sp.height; j++)
                    {
                        // Add the data
                        insert.Data[0][i, j] = DataSetOne[k].Data[i, j] - DataSetTwo[k].Data[i, j];

                        // Check minmax
                        if(insert.Data[0][i, j] > insert.Max)
                        {
                            insert.Max = insert.Data[0][i, j];
                        }
                        if (insert.Data[0][i, j] < insert.Min)
                        {
                            insert.Min = insert.Data[0][i, j];
                        }

                        if (DataSetOne[k].Data[i, j] == NoData && DataSetTwo[k].Data[i, j] == NoData)
                        {
                            insert.Data[0][i, j] = NoDataAssignment;
                        }
                    }
                }

                // Add to the spooler
                Debug.LogError("Adding Data to the Spooler.");
            
                DeltaRecords.Insert(0, insert);
                spool.HandDataToSpooler(DeltaRecords);
            }
            catch (System.Exception a)
            {
                Debug.LogError("The Error: " + a.Message);
                Debug.LogError(a.StackTrace);
            }
        }

        //Cache the ModelRuns again
        Debug.LogError("Adding the comparison to the filebased cache.");
        try
        {
            FileBasedCache.Insert<ModelRun>(deltarun.ModelRunUUID, deltarun);
            if (FileBasedCache.Exists("startup"))
            {
                Debug.LogError("The Startup cache existed.");
                var startCache = FileBasedCache.Get<Dictionary<string, ModelRun>>("startup");
                startCache.Add(deltarun.ModelRunUUID, deltarun);
                FileBasedCache.Insert<Dictionary<string, ModelRun>>("startup", startCache);
            }
        }
        catch(System.Exception a)
        {
            Debug.LogError("This was the error: " + a.Message);
            Debug.LogError(a.StackTrace);
        }
    }
}
