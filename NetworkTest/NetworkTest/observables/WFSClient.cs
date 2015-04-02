using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class WFSClient : Observerable
{
    // Enum
    private enum Operations { GetCapabilities, GetFeature, Done, Error };
    
    // Fields
    public string Root;
    public string App;
    private Operations state;
    private List<Operations> StateList = new List<Operations>();
    private string version;

    public WFSClient(DataFactory Factory,DownloadType type=DownloadType.Record,string OutputPath="",string OutputName="")
        : base(Factory,type,OutputPath,OutputName)
    {
        // Add states
        StateList.Add(Operations.GetFeature);
        StateList.Add(Operations.Done);
    }

    // Update
    public override string Update()
    {
        Console.WriteLine("UPDATE");
        Logger.Log("WFS, Token = " + Token);

        // Check if there is another state
        if (StateList.Count >= 1)
        {
            Console.WriteLine(StateList[0]);

            // Set the first state and remove from the list
            state = StateList[0];
            StateList.RemoveAt(0);
        }
        else
        {
            state = Operations.Error;
        }

        // Check states
        if(state == Operations.GetCapabilities)
        {

        }
        else if(state == Operations.GetFeature)
        {
            return GetFeature();
        }
        else if(state == Operations.Done)
        {
            return "COMPLETE";
        }

        // Else
        return "";
    }

    public void GetData(DataRecord Record, string Version = "1.0.0")
    {
        records = new List<DataRecord>();
        records.Add(Record);
        version = Version;
    }

    public override void Error()
    {
        state = Operations.Error;
    }

    public override void CallBack()
    {
        // Callback
        callback(records);
    }

    private string GetFeature()
    {
        string request = Root + App + "/datasets/" + records[0].id.Replace('"', ' ').Trim() + "/services/ogc/wfs?SERVICE=wfs&Request=GetFeature&" + "&version=" + version + "&typename=" + records[0].name.Trim(new char[] { '\"' }) + "&bbox=" + bboxSplit(records[0].bbox) + "&outputformat=gml2&" + "&srs=epsg:4326"; 
        if (!records[0].services.ContainsKey("wfs"))
        {
            state = Operations.Error;
            return "";
        }

        // Import
        if (type == DownloadType.Record)
        {
            factory.Import("WFS_GML", records, "url://" + request);
        }
        else
        {
            factory.Export("WFS_GML", "url://" + request, FilePath, FileName);
        }
        // Return
        Logger.Log(Token + ": " + request);
        return request;
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

