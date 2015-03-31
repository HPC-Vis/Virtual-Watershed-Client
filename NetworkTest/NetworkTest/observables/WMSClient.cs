using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    // Constructor
    public WMSClient(DataFactory Factory) : base(Factory)
    {
        // Add states
        StateList.Add(Operations.GetCapabilities);
        StateList.Add(Operations.GetMap);
        StateList.Add(Operations.Done);
    }

    // Update
    public override string Update()
    {
        Console.WriteLine("UPDATE");
        Logger.Log("WMS, Token = " + Token);

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

        // Else
        return "";
    }

    public void GetData(DataRecord Record, int Width=100, int Height=100, string Format = "image/png")
    {
        // Set the data
        records = new List<DataRecord>();
        records.Add(Record);
        width = Width;
        height = Height;
        format = Format;
    }

    public override void CallBack()
    {
        // Callback
        callback(records);
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
            "&layers=" + records[0].title + "&bbox=" + bboxSplit(records[0].bbox) +
            "&format=" + format + "&Version=1.1.1" + "&srs=epsg:4326";

        // Import from URL
        factory.Import("WMS_PNG", records, "url://" + request);

        Logger.Log(Token + ": " + request);

        // Return
        return request;
    }

    private string GetCapabilities()
    {
        // Check if services contains "wms" key
        if (!records[0].services.ContainsKey("wms"))
        {
            Console.WriteLine("RETURNING" + records[0].name);

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

