using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

class WMSClient : Observerable
{
    // Enum
    private enum Operations { GetCapabilities, GetMap, Done, Error };

    // Fields
    public string Root;
    public string App;
    private int width;
    private int height;
    private string SRS;
    private string format;
    private Operations state;
    private List<Operations> StateList = new List<Operations>();

    // Constructor 0 means all operations or in this case getmap
	public WMSClient(DataFactory Factory, DownloadType type = DownloadType.Record, string OutputPath = "", string OutputName = "",int operations=0)
        : base(Factory,type,OutputPath,OutputName)
    {
        // Add states
		if (operations == 0) {
			
			StateList.Add (Operations.GetCapabilities);
			StateList.Add (Operations.GetMap);
			StateList.Add (Operations.Done);
		} else if (operations == 1) {
			StateList.Add (Operations.GetCapabilities);
			StateList.Add (Operations.Done);
			Logger.WriteLine ("GETCAPABILITIES");
		} 
		else 
		{
			StateList.Add (Operations.GetCapabilities);
			StateList.Add (Operations.Done);
		}
    }

    // Update
    public override string Update()
    {
        Logger.Log("WMS Client Token = " + Token);

        // Check if there is another state
        if (StateList.Count >= 1)
        {
            Logger.WriteLine(StateList[0].ToString());

            // Set the first state and remove from the list
            state = StateList[0];
            StateList.RemoveAt(0);
        }
        else
        {
            state = Operations.Error;
        }

        // Check the state
        if(state == Operations.GetCapabilities)
        {
            return GetCapabilities();
        }
        else if (state == Operations.GetMap)
        {
            return GetMap();
        }
        else if (state == Operations.Done)
        {
            return "COMPLETE";
        }
        Math.Abs(1);
        // Else
        return "";
    }

    public void GetData(DataRecord Record, SystemParameters param)
    {
        // Set the data
        records = new List<DataRecord>();
        records.Add(Record);
        width = param.width;
        height = param.height;
        format = param.format;
    }

    public override void CallBack()
    {
        // Callback
		if (callback != null) 
		{
			Logger.WriteLine ("CALLBACK WMS");
			callback (records);
		}
    }

    public override void Error()
    {
        state = Operations.Error;
    }

    private string GetMap()
    {
        // Build wms string
        string request = Root + App + "/datasets/" + records[0].id.Replace('"', ' ').Trim() +
            "/services/ogc/wms?SERVICE=wms&Request=GetMap&" + "width=" + width + "&height=" + height +
            "&layers=" + records[0].Identifier + "&bbox=" + bboxSplit(records[0].bbox) +
				"&format=" + format + "&Version=1.1.1" + "&srs=epsg:4326" + "&TRANSPARENT=TRUE";
		Debug.LogError(request);
        // Import from URL
        if (type == DownloadType.Record)
            factory.Import("WMS_PNG", records, "url://" + request);
        else
            factory.Export("WMS_PNG", "url://" + request, FilePath, FileName);
        Logger.Log("FILE: " + FileName + " " + FilePath);
        Logger.Log(Token + ": " + request);

        // Return
        return request;
    }

    public string GDALGetMap()
    {
        new Thread((() => { 
            factory.Import("GDAL", records, "url://" + records[0].GDALPath);
            factory.manager.CallDataComplete(records[0].GDALPath);
        })).Start();
        return records[0].GDALPath;
    }

    private string GetCapabilities()
    {
        // Check if services contains "wms" key
        if (!records[0].services.ContainsKey("wms"))
        {
            Logger.WriteLine("RETURNING" + records[0].name);

            state = Operations.Error;
            return "";
        }

        // In the final steps populate the data record with GetMap Download
        string wms_url = records[0].services["wms"];

        // Import from URL
        factory.Import("WMS_CAP", records, "url://" + wms_url);

        Logger.Log(Token + ": " + wms_url);
        return wms_url;
    }

    private string bboxSplit(string bbox)
    {
        bbox = bbox.Replace('[', ' ');
        bbox = bbox.Replace(']', ' ');
        bbox = bbox.Replace('\"', ' ');
        bbox = bbox.Replace(',', ' ');
        string[] coords = bbox.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        bbox = coords[0] + ',' + coords[1] + ',' + coords[2] + ',' + coords[3];
        return bbox;
    }
}

