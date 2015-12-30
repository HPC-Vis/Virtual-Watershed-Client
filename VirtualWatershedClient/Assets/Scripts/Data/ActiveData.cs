using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

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

    // List of objects subscribing to the active data
    public Spooler spool;

    // Locals
    private bool WMS;
    private bool init = false;
    public static int TOTAL = 0;
    private Queue<DataRecord> SliderFrames = new Queue<DataRecord>();
    private Rect BoundingBox;
    private string Projection;
    private string variable;
    private float Max;
    private float Min;
    private static List<Frame> CurrentData = new List<Frame>();


    void Update()
    {
        // This if statement is used for debugging code
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.LogError("The count / total: " + " / " + TOTAL);
        }

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
            ModelRun mr;
            Active.TryGetValue(variable, out mr);
            spool.UpdateData(BoundingBox, Projection, WMS, variable, Active[variable].ModelRunUUID);

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
            for (int i = 0; i < dequeueSize; i++)
            {
                DataRecord record = SliderFrames.Dequeue();
                spool.SliderFrames.Enqueue(record);
                textureBuilder(record);

                // Updates the min/max information across the necessary classes
                if (record.Max > Active[variable].MinMax[variable].y)
                {
                    Max = record.Max;
                    Active[variable].MinMax[variable] = new SerialVector2(new Vector2(Active[variable].MinMax[variable].x, Max));
                    spool.UpdateMinMax(Min, Max);
                }
                if (record.Min < Active[variable].MinMax[variable].x)
                {
                    Min = record.Min;
                    Active[variable].MinMax[variable] = new SerialVector2(new Vector2(Min, Active[variable].MinMax[variable].y));
                    spool.UpdateMinMax(Min, Max);
                }
            }

            DownloadTextbox.text = "Downloaded: " + ((float)CurrentData.Count / (float)TOTAL).ToString("P");
            spool.UpdateTimeDuration(CurrentData[0].starttime, CurrentData[CurrentData.Count - 1].endtime);
        }
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
        if (rec.modelRunUUID != Active[variable].ModelRunUUID)
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
                var vari = Active[variable].GetVariable(variable);
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
            Insert(frame);
        }
    }

    /// <summary>
    /// Inserts the frame into the Reel.
    /// </summary>
    /// <param name="frame">The frame to add to the reel.</param>
    void Insert(Frame frame)
    {
        // Does this handle duplicates..
        int index = CurrentData.BinarySearch(frame, new FrameEndDateAscending());

        // This is to help pinpoint too many records added to the Reel
        if (CurrentData.Count >= TOTAL)
        {
            //Debug.LogError("Why is there more records being added to the Reel?");
            //Debug.LogError("Here is out frame starttime: " + frame.starttime + " and the count is: " + count);
        }

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
            CurrentData.Insert(~index, frame);
        }
    }


    public static float GetDownload()
    {
        float numerator = 0.0f;
        float denominator = 0.0f;

        foreach (KeyValuePair<string, ModelRun> model in Active)
        {
            Variable variable = model.Value.GetVariable(model.Key);
            denominator += (float)(variable.TotalRecords);
            numerator += (float)(variable.Data.Count);
        }
        return numerator / denominator;
    }

    /// <summary>
    /// The next frame in the reel and updates the window texture.
    /// </summary>
    /// <param name="Time">The time on the time slider.</param>
    /// <returns>Location of the Reel the time is.</returns>
    public static int FindNearestFrame(DateTime Time)
    {
        Frame temp = new Frame();
        temp.starttime = Time;
        int index = CurrentData.BinarySearch(temp, new FrameEndDateAscending());
        return index < 0 ? ~index - 1 : index;
    }

    public static int GetCount()
    {
        return CurrentData.Count;
    }

    public static Frame GetFrameAt(int index)
    {
        return CurrentData[index];
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
        variable = seled[0][2].ToString();

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

    /// <summary>
    /// Used by the ModelRunComparison to buyild the Delta and add to spooler.
    /// </summary>
    public void SetupForDelta(string seled, string vari, int total, string uuid)
    {
        // Get the Model Run
        TOTAL = total;
        variable = vari;

        // Set the download based on the doqq in description
        WMS = false;
    }

}
