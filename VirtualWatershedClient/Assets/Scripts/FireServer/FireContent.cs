using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;


public class FireContent : MonoBehaviour {

    private const string URL = "http://134.197.66.19:5000";
    private string getResults = "/api/get_final_results";
    private string upload = "/upload";
    private string[] csvLines;
    private string uuid;

    // Fire information
    private int iterations;
    private int nodata;
    private int rows;
    private int cols;
    Vector2 TopLeft, BottomRight;

    // Parsing Values
    private char[] endline = { '\n' };
    private char[] colon = { ':' };
    private char[] comma = { ',' };

    public FireContent()
    {
        uuid = Guid.NewGuid().ToString();

        // Temp patch for the location
        TopLeft = new Vector2((float)-115.676788, (float)36.292502);
        BottomRight = new Vector2((float)-115.582055, (float)36.236491);

        // Pull the CSV
        GetTextCall();
        GetMetaData();
    }

    public void GetTextCall()
    {
        UnityWebRequest client = UnityWebRequest.Get(URL + getResults);

        client.downloadHandler = new DownloadHandlerBuffer();
        client.Send();

        if (client.isNetworkError)
        {
            Debug.LogError(client.error);
        }
        else
        {
            while (!client.isDone);
            string csv = client.downloadHandler.text;            
            csvLines = csv.Split(endline);
        }
    }

    public int GetCount()
    {
        return iterations;
    }

    public ModelRun GetModelRun()
    {
        ModelRun mr = new ModelRun();
        mr.ModelRunUUID = uuid;
        mr.Add("Fire Data", new List<DataRecord>());
        return mr;
    }

    /// <summary>
    /// Get the header information out of the file
    /// </summary>
    private void GetMetaData()
    {
        //TopLeft = new Vector2((float)double.Parse(csvLines[0].Split(colon)[1].Trim()), (float)double.Parse(csvLines[1].Split(colon)[1].Trim()));
        //BottomRight = new Vector2((float)double.Parse(csvLines[2].Split(colon)[1].Trim()), (float)double.Parse(csvLines[3].Split(colon)[1].Trim()));

        rows = int.Parse(csvLines[4].Split(colon)[1].Trim());
        cols = int.Parse(csvLines[5].Split(colon)[1].Trim());

        iterations = int.Parse(csvLines[6].Split(colon)[1].Trim());
        nodata = int.Parse(csvLines[7].Split(colon)[1].Trim());
    }

    /// <summary>
    /// Make the single CSV into frames through time.
    /// </summary>
    /// <param name="handDataToSpooler">The callback to send data to the ActiveData class.</param>
    private void ParseFireCSV(DataRecordSetter handDataToSpooler)
    {
        // Get the data to an array
        int[,] tempData = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            string[] row = csvLines[i + 8].Split(comma);
            for (int j = 0; j < cols; j++)
            {
                tempData[i, j] = int.Parse(row[j]);
            }
        }

        DateTime? prev = new DateTime(2016, 1, 1);
        for (int k = 0; k <= iterations; k++)
        {
            DataRecord rec = new DataRecord();
            rec.variableName = "Fire Data";
            rec.projection = "EPSG:4326";
            // Top Left "36."
            rec.bbox = TopLeft.x.ToString() + "," + BottomRight.y.ToString() + "," + BottomRight.x.ToString() + "," + TopLeft.y.ToString();
            rec.bbox2 = TopLeft.x.ToString() + "," + BottomRight.y.ToString() + "," + BottomRight.x.ToString() + "," + TopLeft.y.ToString();
            rec.modelRunUUID = uuid;
            rec.start = prev;
            rec.end = rec.start.Value + new TimeSpan(1, 0, 0);
            prev = rec.end;
            rec.Data.Add(new float[rows,cols]);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    
                    if(tempData[i,j] <= k)
                    {
                        rec.Data[0][i, j] = tempData[i, j];
                    }
                    else
                    {
                        rec.Data[0][i,j] = 0.0f;
                    }
                    
                    //rec.Data[0][i, j] = tempData[i, j];
                }
            }
            rec.Data[0] = Utilities.interpolateValues(1000, rows, cols, rec.Data[0]);
            handDataToSpooler(new List<DataRecord> { rec });
        }        

        // Debug.LogError("FireContent.cs: " + TopLeft + " " + BottomRight + " " + rows + " " + cols + " " + iterations + " " + nodata);
    }

    public void Run(DataRecordSetter handDataToSpooler)
    {
        new Thread(() =>
        {
            ParseFireCSV(handDataToSpooler);
        }).Start();
    }
}
