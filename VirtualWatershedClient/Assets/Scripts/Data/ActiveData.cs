using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using VTL.TrendGraph;

public struct Frame
{
    public Sprite Picture;
    public DateTime starttime;
    public DateTime endtime;
    public float[,] Data;
};


public class FrameEndDateAscending : IComparer<Frame>
{
    public int Compare(Frame first, Frame second)
    {
        if (first.starttime > second.starttime) { return 1; }
        else if (first.starttime == second.starttime) { return 0; }
        else { return -1; }
    }
}


public class ActiveData : MonoBehaviour {
    // Variables
    public static Dictionary<String, ModelRun> Active = new Dictionary<String, ModelRun>();
    public ModelRunVisualizer temporalList;
    public Text DownloadTextbox;
    public static int TOTAL = 0;

    // List of objects subscribing to the active data
    public Spooler spool;
    public TrendGraphController trendGraph;

    // Locals
    private bool WMS;
    private bool init = false;
    private Queue<DataRecord> SliderFrames = new Queue<DataRecord>();
    private Rect BoundingBox;
    private string Projection;
    private float Max;
    private float Min;
    private static Dictionary<String, List<Frame>> CurrentData = new Dictionary<String, List<Frame>>();
    private static int CurrentIndex;
    
    void Update()
    {
        // This if statement is used for debugging code
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.LogError("The count / total: " + " / " + TOTAL);
            Sort();
        }

        // Initilize the necessary data
        if (init && SliderFrames.Count > 1)
        {
            DataRecord record = SliderFrames.Peek();
            Projection = record.projection;

            if (record.bbox2 != "" && record.bbox2 != null)
            {
                //Debug.LogError("We added BBox TWO.");
                BoundingBox = Utilities.bboxSplit(record.bbox2);

            }
            else
            {
                //Debug.LogError("We added BBox ONE.");
                BoundingBox = Utilities.bboxSplit(record.bbox);
            }

            // Push the new information to the subscribers
            spool.UpdateData(BoundingBox, Projection, WMS, record.variableName, Active[record.variableName].ModelRunUUID);
            trendGraph.UpdateData(BoundingBox, Projection, record.variableName);

            init = false;
        }

        // Temp Patch
        if (SliderFrames.Count > 0)
        {
            // Set a dequeue size for multiple dequeus in one update
            int dequeueSize = 5;
            if (SliderFrames.Count < dequeueSize)
            {
                dequeueSize = SliderFrames.Count;
            }

            // Run the dequeue dequeueSize times
            int runningTotal = 0;
            for (int i = 0; i < dequeueSize; i++)
            {
                DataRecord record = SliderFrames.Dequeue();
                textureBuilder(record);

                // Updates the min/max information across the necessary classes
                if (record.Max > Active[record.variableName].MinMax[record.variableName].y)
                {
                    Max = record.Max;
                    Active[record.variableName].MinMax[record.variableName] = new SerialVector2(new Vector2(Active[record.variableName].MinMax[record.variableName].x, Max));
                    spool.UpdateMinMax(Min, Max);
                    trendGraph.SetMax((int)Max);
                }
                if (record.Min < Active[record.variableName].MinMax[record.variableName].x)
                {
                    Min = record.Min;
                    Active[record.variableName].MinMax[record.variableName] = new SerialVector2(new Vector2(Min, Active[record.variableName].MinMax[record.variableName].y));
                    spool.UpdateMinMax(Min, Max);
                    trendGraph.SetMin((int)Min);
                }

                runningTotal = 0;
                foreach (var data in CurrentData)
                {
                    runningTotal += data.Value.Count;
                }
                spool.UpdateTimeDuration(CurrentData[record.variableName][0].starttime, CurrentData[record.variableName][runningTotal - 1].endtime);
                trendGraph.UpdateTimeDuration(CurrentData[record.variableName][0].starttime, CurrentData[record.variableName][runningTotal - 1].endtime);
            }

            DownloadTextbox.text = "Downloaded: " + ((float)runningTotal / (float)TOTAL).ToString("P");
        }
        
        // Updates across objects
        trendGraph.SetDataIndex(CurrentIndex);
    }

    /// <summary>
    /// Takes the currently worked on record and adds it to the spooler.
    /// </summary>
    /// <param name="rec">The current record to add.</param>
    public void textureBuilder(DataRecord rec)
    {
        //return;
        // Caching 
        /* 
         if (!FileBasedCache.Exists (rec.id))  
         {
             //Debug.LogError("INSERTING INTO CACHE " + rec.id);
             FileBasedCache.Insert<DataRecord>(rec.id,rec);
         }
          */

        // This is used to check that the record is correct
        if (rec == null)
        {
            Debug.LogError("The RUN was invalid");
            return;
        }
        
        if (rec.modelRunUUID != Active[rec.variableName].ModelRunUUID)
        {
            // This is not the model run we want because something else was selected.
            Debug.LogError("Ran This ITem");
            return;
        }
        var TS = rec.end.Value - rec.start.Value;
        double totalhours = TS.TotalHours / rec.Data.Count;
        float max, min, mean;
        if (rec.Data.Count > 1)
        {
            TOTAL = rec.Data.Count; // Patch
        }

        if (!rec.start.HasValue)
        {
            Debug.LogError("no start");
            rec.start = DateTime.MinValue;
        }
        if (!rec.end.HasValue)
        {
            Debug.LogError("no end");

            rec.end = DateTime.MaxValue;
        }

        for (int j = 0; j < rec.Data.Count; j++)
        {
            // Build the Frame to pass in
            Frame frame = new Frame();


            if (rec.Data.Count == 1)
            {
                frame.starttime = rec.start.Value;
                frame.endtime = rec.end.Value;
            }
            else
            {
                frame.starttime = rec.start.Value + new TimeSpan((int)Math.Round((double)j * totalhours), 0, 0);
                frame.endtime = rec.end.Value + new TimeSpan((int)Math.Round((double)(j + 1) * totalhours), 0, 0);
            }

            frame.Data = rec.Data[j];

            // Checks for NULL downloaded data
            if (rec.Data == null)
            {
                Debug.LogError("The data at UUID = " + rec.id + " was null.");
                return;
            }


            Logger.enable = true;
            Texture2D tex = new Texture2D(rec.width, rec.height);

            if (!WMS)
            {
                tex = Utilities.BuildDataTexture(rec.Data[j], out min, out max, out mean);
                rec.Min = Math.Min(min, rec.Min);
                rec.Max = Math.Max(max, rec.Max);
                rec.Mean = mean;
                var vari = Active[rec.variableName].GetVariable(rec.variableName);
                vari.meanSum += mean;
                vari.frameCount += 1;
                vari.Mean = vari.meanSum / vari.frameCount;

                //Debug.LogError("MIN AND MAX: " + rec.Min + " " + rec.Max);
            }
            else
            {
                // We need to change this ..... as this code is unreachable at the moment.
                tex.LoadImage(rec.texture);
            }

            // This will add a clear color on all the 
            for (int i = 0; i < tex.width; i++)
            {
                tex.SetPixel(i, 0, Color.clear);
                tex.SetPixel(i, tex.height - 1, Color.clear);
            }
            for (int i = 0; i < tex.height; i++)
            {
                tex.SetPixel(tex.width - 1, i, Color.clear);
                tex.SetPixel(0, i, Color.clear);
            }

            tex.Apply();
            frame.Picture = Sprite.Create(tex, new Rect(0, 0, 100, 100), Vector2.zero);
            Insert(frame, rec.variableName);
        }
    }

    /// <summary>
    /// Inserts the frame into the Reel.
    /// </summary>
    /// <param name="frame">The frame to add to the reel.</param>
    void Insert(Frame frame, String variableName)
    {
        // Does this handle duplicates..
        int index = CurrentData[variableName].BinarySearch(frame, new FrameEndDateAscending());

        // This is to help pinpoint too many records added to the Reel
        //if (CurrentData.Count >= TOTAL)
        //{
            //Debug.LogError("Why is there more records being added to the Reel?");
            //Debug.LogError("Here is out frame starttime: " + frame.starttime + " and the count is: " + count);
        //}

        //if index >= 0 there is a duplicate 
        if (index >= 0)
        {
            //handle the duplicate!
            //throw new Exception("Duplicate Handling not implemented!!!!!");
            TOTAL--;
        }
        else
        {
            // new item
            // Debug.LogError("INSERTTING FRAMME " + ~index);
            CurrentData[variableName].Insert(~index, frame);
        }
    }


    public static float GetDownload()
    {
        float numerator = 0.0f;
        float denominator = 0.0f;

        foreach (KeyValuePair<string, ModelRun> model in Active)
        {
            Variable vari = model.Value.GetVariable(model.Key);
            denominator += (float)(vari.TotalRecords);
            numerator += (float)(vari.Data.Count);
        }
        return numerator / denominator;
    }

    /// <summary>
    /// The next frame in the reel and updates the window texture.
    /// </summary>
    /// <param name="Time">The time on the time slider.</param>
    /// <returns>Location of the Reel the time is.</returns>
    public static int FindNearestFrame(DateTime Time, int location)
    {
        Frame temp = new Frame();
        temp.starttime = Time;

        int runningTotal = 0;
        foreach (var data in CurrentData)
        {
            runningTotal = data.Value.Count;
        }

        int index = 0;
        foreach (var data in CurrentData)
        {
            int current = data.Value.BinarySearch(temp, new FrameEndDateAscending());
            if (index != current)
            {
                //Debug.LogError("There was an incorrect index match: " + index + " with " + current);                
            }
            index = current;
        }

        int returnValue = index < 0 ? ~index - 1 : index;
        if (returnValue < 0)
        {
            returnValue = 0;
        }
        else if (returnValue >= runningTotal)
        {
            returnValue = runningTotal - 1;
        }

        CurrentIndex = returnValue;

        return returnValue;
    }

    public static int GetCount(int location)
    {
        int runningTotal = 0;
        foreach (var data in CurrentData)
        {
            runningTotal += data.Value.Count;
        }
        return runningTotal;
    }

    public static List<Frame> GetFrameAt(int index)
    {
        List<Frame> returnvalue = new List<Frame>();
        foreach (var data in CurrentData)
        {
            returnvalue.Add(CurrentData[data.Key][index]);
        }
        return returnvalue;
    }

    public static void Sort()
    {
        foreach(var data in CurrentData)
        {
            CurrentData[data.Key].Sort((s1, s2) => s1.starttime.CompareTo(s2.starttime));
        }        
    }

    /// <summary>
    /// This will handle wcs related requests
    /// </summary>
    /// <param name="Records">The list of records to add.</param>
    public void HandDataToSpooler(List<DataRecord> Records)
    {
        // Set the record
        SliderFrames.Enqueue(Records[0]);
    }

    /// <summary>
    /// Gets the selected model run, begins the record download, and updates page data.
    /// </summary>
    public void LoadSelected()
    {
        // Load this 
        var temp = temporalList.listView.GetSelectedModelRuns();
        var seled = temporalList.listView.GetSelectedRowContent();
        string variable = seled[0][2].ToString();

        // Set the data of new model run
        // TODO
        // selectedVariableString = "Current Model Run: " + seled[0][0].ToString() + " Variable: " + variable;
        Debug.LogError("Current Model Run: " + seled[0][0].ToString() + " Variable: " + variable);

        // Only run if what was selected returned a value
        if (temp != null)
        {
            // Time to load some things
            SystemParameters sp = new SystemParameters();
            sp.interpolation = "bilinear";
            sp.width = 100;
            sp.height = 100;

            // Get the Model Run
            var Records = temp[0].FetchVariableData(variable);
            TOTAL = Records.Count;
            ModelRun modelrun = ModelRunManager.GetByUUID(temp[0].ModelRunUUID);
            init = true;
            Logger.WriteLine("Load Selected: " + variable + " with Number of Records: " + Records.Count);

            // Set the active data
            Active.Add(variable, modelrun);
            CurrentData.Add(variable, new List<Frame>());

            // Set the download based on the doqq in description
            if (temp[0].Description.ToLower().Contains("doqq"))
            {
                WMS = true;
                ModelRunManager.Download(Records, HandDataToSpooler, param: sp, operation: "wms");
            }
            else
            {
                WMS = false;
                ModelRunManager.Download(Records, HandDataToSpooler, param: sp);
            }
        }
    }
}
