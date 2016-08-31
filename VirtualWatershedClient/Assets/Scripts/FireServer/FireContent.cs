using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Networking;
using UnityEngine.Networking;


public class FireContent : MonoBehaviour {

    private const string URL = "http://134.197.66.31:5000";
    private string getResults = "/api/get_final_results";
    private string upload = "/upload";
    private string csv;
    private string uuid;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public FireContent()
    {
        uuid = Guid.NewGuid().ToString();
    }

    public void Go()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        UnityWebRequest client = UnityWebRequest.Get(URL + getResults);

        client.downloadHandler = new DownloadHandlerBuffer();
        yield return client.Send();

        if (client.isError)
        {
            Debug.LogError(client.error);
        }
        else
        {
            while (!client.isDone)
            {
                Debug.LogError("Waiting");
            }
            Debug.LogError(client.downloadHandler.text);
            Debug.LogError(client.downloadedBytes);

            csv = client.downloadHandler.text;
        }
    }

    public void GetTextCall()
    {
        UnityWebRequest client = UnityWebRequest.Get(URL + getResults);

        client.downloadHandler = new DownloadHandlerBuffer();
        client.Send();

        if (client.isError)
        {
            Debug.LogError(client.error);
        }
        else
        {
            while(!client.isDone)
            {
                Debug.LogError("Waiting");
            }
            Debug.LogError(client.downloadHandler.text);
            Debug.LogError(client.downloadedBytes);

            csv = client.downloadHandler.text;
            // ParseFireCSV(client.downloadHandler.text);
            //byte[] results = client.downloadHandler.data;
            //Debug.LogError(System.Text.Encoding.Default.GetString(results));
        }
    }

    public int GetCount()
    {
        return 2000;
    }

    public ModelRun GetModelRun()
    {
        ModelRun mr = new ModelRun();
        mr.ModelRunUUID = uuid;
        mr.Add("Fire Data", new List<DataRecord>());
        return mr;
    }

    private void ParseFireCSV(DataRecordSetter handDataToSpooler)
    {
        Debug.LogError("Running the parse");
        char[] endline = { '\n' };
        char[] colon = { ':' };
        char[] comma = { ',' };
        string [] csvLines = csv.Split(endline);

        Vector2 TopLeft = new Vector2((float)double.Parse(csvLines[0].Split(colon)[1].Trim()), (float)double.Parse(csvLines[1].Split(colon)[1].Trim()));
        Vector2 BottomRight = new Vector2((float)double.Parse(csvLines[2].Split(colon)[1].Trim()), (float)double.Parse(csvLines[3].Split(colon)[1].Trim()));
        int rows = int.Parse(csvLines[4].Split(colon)[1].Trim());
        int cols = int.Parse(csvLines[5].Split(colon)[1].Trim());
        int iterations = int.Parse(csvLines[6].Split(colon)[1].Trim());
        int nodata = int.Parse(csvLines[7].Split(colon)[1].Trim());

        int[,] tempData = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            string[] row = csvLines[i + 8].Split(comma);
            for (int j = 0; j < cols; j++)
            {
                tempData[i,j] = int.Parse(row[j]);
            }
        }

        DateTime? prev = new DateTime(2016, 1, 1);
        for (int k = 0; k <= iterations; k++)
        {
            DataRecord rec = new DataRecord();
            rec.variableName = "Fire Data";
            rec.projection = "EPSG:26911";
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
                        rec.Data[0][i, j] = 1.0f;
                    }
                    else
                    {
                        rec.Data[0][i,j] = 0.0f;
                    }
                }
            }
            handDataToSpooler(new List<DataRecord> { rec });
        }        

        Debug.LogError("FireContent.cs: " + TopLeft + " " + BottomRight + " " + rows + " " + cols + " " + iterations + " " + nodata);
    }

    public void Run(DataRecordSetter handDataToSpooler)
    {
        GetTextCall();
        new Thread(() =>
        {
            ParseFireCSV(handDataToSpooler);
        }).Start();
    }
}
