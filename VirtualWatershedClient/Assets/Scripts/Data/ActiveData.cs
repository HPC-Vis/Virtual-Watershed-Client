using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using VTL.TrendGraph;
using System.IO;

public struct Frame
{
    public Sprite Picture;
    public DateTime starttime;
    public DateTime endtime;
    public float[,] Data;
    public DataRecord record;

    public Frame(DataRecord rec, bool init = false)
    {
        Picture = null;
        starttime = DateTime.MinValue;
        endtime = DateTime.MinValue;
        record = rec;
        if(init)
        {
            Data = new float[100, 100];
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    Data[i, j] = 0.0f;
                }
            }
        }
        else
        {
            Data = null;
        }        
    }
};

public class DataLoad
{
    public bool WMS;
    public ModelRun modelrun;
    public List<Frame> frames;
    public Rect BoundingBox;
    public string Projection;
    public int total;
    public bool active;
    public float Min;
    public float Max;
    public DateTime Start;
    public DateTime End;
    public string[] variable;

    public DataLoad(bool wms, ModelRun mr, List<Frame> fr, int t, bool a, string[] var)
    {
        WMS = wms;
        modelrun = mr;
        frames = fr;
        BoundingBox = new Rect();
        Projection = null;
        total = t;
        active = a;
        Min = float.MaxValue;
        Max = float.MinValue;
        Start = DateTime.MaxValue;
        End = DateTime.MinValue;
        variable = new string[var.Length];
        for(int i = 0; i < variable.Length; i++)
        {
            variable[i] = String.Copy(var[i]);
        }
    }

    public void Toggle()
    {
        active = !active;
    }
}


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
    public static Dictionary<String, DataLoad> Active = new Dictionary<String, DataLoad>();
    public ModelRunLoader temporalList;
    public Dropdown Options;
    public Text DownloadTextbox;
    public static int GRAND_TOTAL = 0;
    public InputField minField;
    public InputField maxField;
    public static bool Updated = false;

    // List of objects subscribing to the active data
    public Spooler spool;
    public TrendGraphController trendGraph;

    // Locals
    private Queue<DataRecord> SliderFrames = new Queue<DataRecord>();
    private static int CurrentIndex;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.LogError("KEY CODE I PRESSED");
            CurrentFrameToFile(format:"tif");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.LogError("HERE");
            ModelRunManager.sessionData.SaveSessionData(Utilities.GetFilePath("test.json"));
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

                if (record == null)
                {
                    Debug.LogError("Null: " + record);
                }
                else
                {
                    // Initilize the necessary data
                    if (Active[record.variableName].frames.Count == 0)
                    {
                        DataLoad temp = Active[record.variableName];
                        temp.Projection = record.projection;

                        if (record.bbox2 != "" && record.bbox2 != null)
                        {                        
                            temp.BoundingBox = Utilities.bboxSplit(record.bbox2);
                        }
                        else
                        {
                            temp.BoundingBox = Utilities.bboxSplit(record.bbox);                        
                        }

                        Active[record.variableName] = temp;

                        // Push the new information to the subscribers
                        if (Active[record.variableName].active)
                        {
                            spool.UpdateData(record.variableName);
                            trendGraph.UpdateData(record.variableName);
                        }                        
                    }

                    // Build the texture
                    textureBuilder(record);

                    // Updates the min/max information across the necessary classes
                    if (record.Max > Active[record.variableName].modelrun.MinMax[record.variableName].y)
                    {
                        Active[record.variableName].modelrun.MinMax[record.variableName] = new SerialVector2(new Vector2(Active[record.variableName].modelrun.MinMax[record.variableName].x, record.Max));
                        if (Active[record.variableName].Max < record.Max)
                        {
                            Active[record.variableName].Max = record.Max;
                            // Only run these update is the record is on the active stack
                            if (Active[record.variableName].active)
                            {
                                maxField.text = ((int)record.Max).ToString();
                                spool.UpdateMinMax();
                                trendGraph.UpdateMinMax();
                            }
                        }
                    }
                    if (record.Min < Active[record.variableName].modelrun.MinMax[record.variableName].x)
                    {
                        Active[record.variableName].modelrun.MinMax[record.variableName] = new SerialVector2(new Vector2(record.Min, Active[record.variableName].modelrun.MinMax[record.variableName].y));
                        if (Active[record.variableName].Min > record.Min)
                        {
                            Active[record.variableName].Min = record.Min;
                            // Only run these update is the record is on the active stack
                            if (Active[record.variableName].active)
                            {
                                minField.text = ((int)record.Min).ToString();
                                spool.UpdateMinMax();
                                trendGraph.UpdateMinMax();
                            }
                        }
                    }

                    // Update the time
                    if (Active[record.variableName].Start > Active[record.variableName].frames[0].starttime || Active[record.variableName].End < Active[record.variableName].frames[Active[record.variableName].frames.Count - 1].endtime)
                    {
                        Active[record.variableName].Start = Active[record.variableName].frames[0].starttime;
                        Active[record.variableName].End = Active[record.variableName].frames[Active[record.variableName].frames.Count - 1].endtime;
                        // Only run these update is the record is on the active stack
                        if (Active[record.variableName].active)
                        {
                            spool.UpdateTimeDuration(Active[record.variableName].Start, Active[record.variableName].End);
                            trendGraph.UpdateTimeDuration(Active[record.variableName].Start, Active[record.variableName].End);
                        }
                    }           
                }
            }

            int runningTotal = 0;
            foreach (var data in Active)
            {
                runningTotal += data.Value.frames.Count;
            }
            DownloadTextbox.text = "Downloaded: " + ((float)runningTotal / (float)GRAND_TOTAL).ToString("P");
        }
        
        // Updates across objects
        trendGraph.SetDataIndex(CurrentIndex);

        if(Updated)
        {
            Updated = false;
            foreach(var item in Active)
            {
                if(item.Value.active)
                {
                    spool.UpdateData(item.Key);
                    spool.UpdateMinMax();
                    trendGraph.UpdateData(item.Key);
                    trendGraph.UpdateMinMax();
                }                
            }

            List<string> tempRef = GetCurrentAvtive();
            float min, max;
            if (tempRef.Count > 1)
            {
                min = Active[tempRef[0]].Min - Active[tempRef[1]].Max;
                max = Active[tempRef[0]].Max - Active[tempRef[1]].Min;               
            }
            else
            {
                min = Active[tempRef[0]].Min;
                max = Active[tempRef[0]].Max;
            }
            
            // fix the text
            minField.text = ((int)min).ToString();
            maxField.text = ((int)max).ToString();
        }
    }


    public static void ChangeActive(string name)
    {
        char[] splitval = { ':' };
        string[] variable = name.Split(splitval);
        
        // Toggle off any active
        foreach(var item in Active)
        {
            if(item.Value.active)
            {
                item.Value.Toggle();
            }
        }

        // Make variable[0] active
        try
        {
            Active[variable[0]].Toggle();            
        }
        catch (Exception e)
        {
            Dictionary<string, DataLoad>.Enumerator enumerator = Active.GetEnumerator();
            enumerator.MoveNext();
            enumerator.Current.Value.Toggle();
            throw new ArgumentException("The value does not exist in the Active set: " + variable[0] + " " + e.Message);
        }

        // Ensure there is an update
        ActiveData.Updated = true;
    }


    /// <summary>
    /// Takes the currently worked on record and adds it to the spooler.
    /// </summary>
    /// <param name="rec">The current record to add.</param>
    private void textureBuilder(DataRecord rec)
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
        
        if (rec.modelRunUUID != Active[rec.variableName].modelrun.ModelRunUUID)
        {
            // This is not the model run we want because something else was selected.
            Debug.LogError("Ran This ITem");
            return;
        }
        var TS = rec.end.Value - rec.start.Value;
        double totalhours = TS.TotalHours / rec.Data.Count;
        float mean;
        ValueContainer max, min;
        if (rec.Data.Count > 1)
        {
            UpdateTotal(rec.variableName, rec.Data.Count);
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
            Frame frame = new Frame(rec);


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

            if (!Active[rec.variableName].WMS)
            {
                tex = Utilities.BuildDataTexture(rec.Data[j], out min, out max, out mean);
                rec.MinContainer = min;
                rec.MaxContainer = max;
                rec.Mean = mean;
                var vari = Active[rec.variableName].modelrun.GetVariable(rec.variableName);
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
    private void Insert(Frame frame, String variableName)
    {
        // Does this handle duplicates..
        int index = Active[variableName].frames.BinarySearch(frame, new FrameEndDateAscending());

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
            UpdateTotal(variableName, Active[variableName].total - 1);
        }
        else
        {
            // new item
            // Debug.LogError("INSERTTING FRAMME " + ~index);
            Active[variableName].frames.Insert(~index, frame);
        }
    }

    public static void UpdateTotal(String location, int value)
    {
        if (Active.ContainsKey(location))
        {
            DataLoad temp = Active[location];
            GRAND_TOTAL -= temp.total;
            temp.total = value;
            GRAND_TOTAL += temp.total;
            Active[location] = temp;
        }
    }


    public static float GetDownload()
    {
        float numerator = 0.0f;
        float denominator = 0.0f;

        foreach (KeyValuePair<string, DataLoad> model in Active)
        {
            Variable vari = model.Value.modelrun.GetVariable(model.Key);
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
    public static int FindNearestFrame(string variable, DateTime Time)
    {
        Frame temp = new Frame();
        temp.starttime = Time;

        int index = 0;
        int runningTotal = runningTotal = Active[variable].frames.Count;
        int current = Active[variable].frames.BinarySearch(temp, new FrameEndDateAscending());
        index = current;

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

    public static Vector2 GetMinMax(string variable)
    {
        return new Vector2(Active[variable].Min, Active[variable].Max);
    }

    public static List<String> GetCurrentAvtive()
    {
        List<String> returnval = new List<string>();
        foreach(var item in Active)
        {
            if(item.Value.active)
            {
                foreach(string val in item.Value.variable)
                {
                    returnval.Add(val);
                }                
            }
        }
        return returnval;
    }

    public static Rect GetBoundingBox(String variable)
    {       
        return Active[variable].BoundingBox;
    }

    public static string GetProjection(String variable)
    {
        return Active[variable].Projection;
    }

    public static bool GetWMS(String variable)
    {
        return Active[variable].WMS;
    }

    public static int GetCount(String variable)
    {
        return Active[variable].frames.Count;
    }

    public static Frame GetFrameAt(String variable, int index)
    {
        Frame returnvalue;

        if(Active[variable].frames.Count <= index)
        {
            if(Active[variable].frames.Count <= 0)
            {
                returnvalue = new Frame(null, true);
            }
            else
            {
                returnvalue = Active[variable].frames[Active[variable].frames.Count - 1];
            }
        }
        else
        {
            returnvalue = Active[variable].frames[index];
        }            
        
        return returnvalue;
    }

    public static void Sort()
    {
        foreach(var data in Active)
        {
            Active[data.Key].frames.Sort((s1, s2) => s1.starttime.CompareTo(s2.starttime));
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
        Debug.Log("Active Data Load Called");
        // Load this 
        var temp = temporalList.listView.GetSelectedModelRuns();
        var seled = temporalList.listView.GetSelectedRowContent();
        
        int maxindex = temp.Count;
        if(temp.Count > 2)
        {
            maxindex = 2;
        }

        // Only run if what was selected returned a value
        for (int index = 0; index < maxindex; index++)
        {
            // Time to load some things
            SystemParameters sp = new SystemParameters();
            sp.interpolation = "bilinear";
            sp.width = 100;
            sp.height = 100;

            // Check if there is a need to clear -- if greater than 2
            if (Active.Count > 1)
            {
                // Initilize the Time
                Active.Clear();
                spool.Clear();
                trendGraph.Clear();
                GRAND_TOTAL = 0;
            }

            // Get the Model Run
            string variable = seled[index][2].ToString();
            var Records = temp[index].FetchVariableData(variable);
            GRAND_TOTAL += Records.Count;
            Logger.WriteLine("Load Selected: " + variable + " with Number of Records: " + Records.Count);

            // Add the new loaded to the list
            Options.options.Add(new Dropdown.OptionData(variable + ": " + VariableReference.GetDescription(variable)));
            
            // Check if there are now two loaded, if so add to list
            if(Active.Count == 1)
            {
                Dictionary<string, DataLoad>.Enumerator e = Active.GetEnumerator();
                e.MoveNext();
                string secondVar = e.Current.Key;
                Options.options.Add(new Dropdown.OptionData(variable + "_" + secondVar + ": " + "Delta " + variable + " - " + secondVar));
                Options.options.Add(new Dropdown.OptionData(secondVar + "_" + variable + ": " + "Delta " + secondVar + " - " + variable));
                Active.Add(variable + "_" + secondVar, new DataLoad(true, null, new List<Frame>(), Records.Count, false, new string[] { variable, secondVar }));
                Active.Add(secondVar + "_" + variable, new DataLoad(true, null, new List<Frame>(), Records.Count, false, new string[] { secondVar, variable }));
                VariableReference.AddDescription(variable + "_" + secondVar, "Delta " + variable + " - " + secondVar);
                VariableReference.AddDescription(secondVar + "_" + variable, "Delta " + secondVar + " - " + variable);
            }

            // Set the download based on the doqq in description
            if (temp[index].Description.ToLower().Contains("doqq"))
            {
                Active.Add(variable, new DataLoad(true, ModelRunManager.GetByUUID(temp[index].ModelRunUUID), new List<Frame>(), Records.Count, Active.Count == 0, new string[]{variable}));
                ModelRunManager.Download(Records, HandDataToSpooler, param: sp, operation: "wms");
            }
            else
            {
                Active.Add(variable, new DataLoad(false, ModelRunManager.GetByUUID(temp[index].ModelRunUUID), new List<Frame>(), Records.Count, Active.Count == 0, new string[] { variable }));
                ModelRunManager.Download(Records, HandDataToSpooler, param: sp);
            }
        }
    }

    /// <summary>
    /// This will send all the data from the current frame to a file
    /// </summary>
    public void currentframeToFile()
    {
        List<String> tempFrameRef = ActiveData.GetCurrentAvtive();
        foreach (var name in tempFrameRef)
        {
            String pathDownload = Utilities.GetFilePath(name + "_frameToFile.csv");
            using (StreamWriter file = new StreamWriter(@pathDownload))
            {
                for (int i = 0; i < ActiveData.GetFrameAt(name, CurrentIndex).Data.GetLength(1); i++)
                {
                    for (int j = ActiveData.GetFrameAt(name, CurrentIndex).Data.GetLength(0) - 1; j >= 0 ; j--)
                    {
                        file.Write(ActiveData.GetFrameAt(name, CurrentIndex).Data[i, j] + ", ");
                    }
                    file.Write("\n");
                }
            }
        }
    }

    public void CurrentFrameToFile(string format="tif")
    {

        List<String> tempFrameRef = ActiveData.GetCurrentAvtive();
        foreach (var name in tempFrameRef)
        {
            
            if (format == "tif")
            {
                String pathDownload = name + "_frameToFile.tif";
                ActiveData.GetFrameAt(name, CurrentIndex).record.boundingBox = ActiveData.GetBoundingBox(name);
                Utilities.SaveTif(pathDownload, ActiveData.GetFrameAt(name, CurrentIndex).record);
            }
            /*using (StreamWriter file = new StreamWriter(@pathDownload))
            {
                for (int i = 0; i < ActiveData.GetFrameAt(name, CurrentIndex).Data.GetLength(1); i++)
                {
                    for (int j = 0; j < ActiveData.GetFrameAt(name, CurrentIndex).Data.GetLength(0); j++)
                    {
                        file.Write(ActiveData.GetFrameAt(name, CurrentIndex).Data[i, j] + ", ");
                    }
                    file.Write("\n");
                }
            }*/
        }


    }
}
