using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class WMSClient : Observerable
{
    enum WMSOperations { GetCapabilities, GetMap, Done, Error };
    WMSOperations state;
    public string Root;
    public string App;

    int width;
    int height;
    string SRS;
    string format;
    List<WMSOperations> StateList = new List<WMSOperations>();

    public WMSClient(DataFactory Factory)
        : base(Factory)
    {

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

    public override string Update()
    {
        Console.WriteLine("UPDATE");

        Logger.Log("WMS, Token = " + Token);

        if (StateList.Count >= 1)
        {
            Console.WriteLine(StateList[0]);
            state = StateList[0];
            StateList.RemoveAt(0);
        }
        else
        {
            state = WMSOperations.Error;
        }

        if(state == WMSOperations.GetCapabilities)
        {
            return GetCapabilities();
        }
        else if (state == WMSOperations.GetMap)
        {
            return GetMap();
        }
        else if (state == WMSOperations.Done)
        {
            return "COMPLETE";
        }

        return "";
    }

    public override void Error()
    {
        state = WMSOperations.Error;
    }

    public void GetData(DataRecord Record, int Width=100, int Height=100, string Format = "image/png")
    {
        StateList.Add(WMSOperations.GetCapabilities);
        StateList.Add(WMSOperations.GetMap);
        StateList.Add(WMSOperations.Done);
        record = Record;
        width = Width;
        height = Height;
        format = Format;
    }

    string GetMap()
    {  
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);
        // Build wms string
        string request = Root + App + "/datasets/" + record.id.Replace('"', ' ').Trim() +
            "/services/ogc/wms?SERVICE=wms&Request=GetMap&" + "width=" + width + "&height=" + height +
            "&layers=" + record.title + "&bbox=" + bboxSplit(record.bbox) +
            "&format=" + format + "&Version=1.1.1" + "&srs=epsg:4326";

        factory.Import("WMS_PNG", tempList, "url://" + request);

        Logger.Log(Token + ": " + request);

        return request;
    }

    string GetCapabilities()
    {
        if (!record.services.ContainsKey("wms"))
        {
            Console.WriteLine("RETURNING" + record.name);
            state = WMSOperations.Error;
            return "";
        }

        // In the final steps populate the data record with GetMap Download
        string wms_url = record.services["wms"];

        Logger.Log(Token + ": " + wms_url);

        // Register Job with Data Tracker
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);
        factory.Import("WMS_CAP", tempList, "url://" + wms_url);
        return wms_url;
    }
    public override void CallBack()
    {
        List<DataRecord> records = new List<DataRecord>();
        records.Add(record);
        Callback(records);
    }
}

