using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class WFSClient : Observerable
{
    enum WFSOperations { GetCapabilities, GetFeature,Done,Error };
    WFSOperations state;
    List<WFSOperations> StateList = new List<WFSOperations>();
    public string Root;
    public string App;
    string version;
    public WFSClient(DataFactory Factory)
        : base(Factory)
    {

    }
    public override void Error()
    {
        state = WFSOperations.Error;
    }
    public override string Update()
    {
        Console.WriteLine("UPDATE");

        Logger.Log("WFS, Token = " + Token);

        if (StateList.Count >= 1)
        {
            Console.WriteLine(StateList[0]);
            state = StateList[0];
            StateList.RemoveAt(0);
        }
        else
        {
            state = WFSOperations.Error;
        }

        if(state == WFSOperations.GetCapabilities)
        {

        }
        else if(state == WFSOperations.GetFeature)
        {
            GetFeature();
        }
        else if(state == WFSOperations.Done)
        {
            return "COMPLETE";
        }
        else if (state == WFSOperations.Error)
        {
            return "";
        }
        return "";
    }

    public void GetData(DataRecord Record, string Version = "1.0.0")
    {
        record = Record;
        version = Version;
        StateList.Add(WFSOperations.GetFeature);
        StateList.Add(WFSOperations.Done);
    }

    string GetFeature()
    {
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);
        string request = Root + App + "/datasets/" + record.id.Replace('"', ' ').Trim() + "/services/ogc/wfs?SERVICE=wfs&Request=GetFeature&" + "&version=" + version + "&typename=" + record.name.Trim(new char[] { '\"' }) + "&bbox=" + bboxSplit(record.bbox) + "&outputformat=gml2&" + "&srs=epsg:4326"; 
        if (!record.services.ContainsKey("wfs"))
        {
            state = WFSOperations.Error;
            return "";
        }

        Logger.Log(Token + ": " + request);

        factory.Import("WFS_GML", tempList, "url://" + request);
        return request;
    }

    string bboxSplit(string bbox)
    {
        bbox = bbox.Replace('[', ' ');
        bbox = bbox.Replace(']', ' ');
        bbox = bbox.Replace('\"', ' ');
        bbox = bbox.Replace(',', ' ');
        string[] coords = bbox.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        bbox = coords[0] + ',' + coords[1] + ',' + coords[2] + ',' + coords[3];
        return bbox;
    }
    public override void CallBack()
    {
        List<DataRecord> records = new List<DataRecord>();
        records.Add(record);
        Callback(records);
    }
}

