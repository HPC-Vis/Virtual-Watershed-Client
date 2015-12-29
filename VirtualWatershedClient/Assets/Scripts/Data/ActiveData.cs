using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ActiveData : MonoBehaviour {
    // Variables
    public static Dictionary<String, ModelRun> Active = new Dictionary<String, ModelRun>();
    public ModelRunVisualizer temporalList;

    // List of objects subscribing to the active data
    public Spooler spool;

    // Locals
    private bool WMS;
    private bool init = false;
    private static int TOTAL;
    private Queue<DataRecord> SliderFrames = new Queue<DataRecord>();
    private Rect BoundingBox;
    private string Projection;
    private string variable;

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

            transform tran = new transform();
            //Debug.LogError("Coord System: " + record.projection);
            tran.createCoordSystem(record.projection); // Create a coordinate transform
            //Debug.Log("coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y)" + coordsystem.transformToUTM(record.boundingBox.x, record.boundingBox.y));

            // transfor a lat/long bounding box to UTM
            tran.setOrigin(coordsystem.WorldOrigin);
            Vector2 point = tran.transformPoint(new Vector2(BoundingBox.x, BoundingBox.y));
            Vector2 point2 = tran.transformPoint(new Vector2(BoundingBox.x + BoundingBox.width, BoundingBox.y - BoundingBox.height));

            // Here is a patch.
            if ((BoundingBox.x > -180 && BoundingBox.x < 180 && BoundingBox.y < 180 && BoundingBox.y > -180))
            {
                BoundingBox = new Rect(point.x, point.y, Math.Abs(point.x - point2.x), Math.Abs(point.y - point2.y));
            }


            // Push the new information to the subscribers
            ModelRun mr;
            Active.TryGetValue(variable, out mr);
            spool.UpdateData(BoundingBox, Projection, WMS, variable, mr.ModelRunUUID);

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
                spool.SliderFrames.Enqueue(SliderFrames.Dequeue());
            }
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
            Spooler.TOTAL = TOTAL;
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
}
