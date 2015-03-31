using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This is literally a WCSClient because it handles WCS operations.
/// Though it does not actually fetch any data it does handle data operations.
/// We should build a more general interface here so that one day we can trade our code out with something like gdal (maybe).
/// </summary>
class WCSClient : Observerable
{
    // Enum
    private enum Operations { GetCapabilities, DescribeCoverage, GetCoverage, Done, Error };

    // Fields
    public string current_url;
    private int width, height;
    private Rect bbox;
    private string boundingbox;
    private string CRS;
    private string LayerName; // This only handles one layer.
    private string interpolation;
    private Operations state;
    private List<Operations> StateList = new List<Operations>();

    // Constructor
    public WCSClient(DataFactory Factory) : base(Factory)
    {
        // Add states
        StateList.Add(Operations.GetCapabilities);
        StateList.Add(Operations.DescribeCoverage);
        StateList.Add(Operations.GetCoverage);
        StateList.Add(Operations.Done);
    }
    
    // Update
    public override string Update()
    {
        Console.WriteLine("UPDATE");
        Logger.Log("WCS, Token = " + Token);

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
        if (state == Operations.GetCapabilities)
        {
            return GetCapabilities();
        }
        else if (state == Operations.DescribeCoverage)
        {
            return DescribeCoverage();
        }
        else if (state == Operations.GetCoverage)
        {
            return GetCoverage(CRS, records[0].bbox, width, height);
        }
        else if (Operations.Done == state)
        {
            return "COMPLETE";
        }

        // Else
        return "";
    }

    // This guy will call GetCoverage -- This to be used with parameters that may not already exist
    public void GetData(DataRecord Record, string crs = "", string BoundingBox = "", int Width = 0, int Height = 0, string Interpolation = "nearest")
    {
        records = new List<DataRecord>();
        records.Add(Record);
        CRS = crs;
        boundingbox = BoundingBox;
        width = Width;
        height = Height;
        interpolation = Interpolation;
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

    private string GetCoverage(string crs = "", string boundingbox = "", int width = 0, int height = 0, string interpolation = "nearest")
    {
        // By this point the get coverage string should be built.
        GetCapabilites.OperationsMetadataOperation gc = new GetCapabilites.OperationsMetadataOperation();
        foreach (GetCapabilites.OperationsMetadataOperation i in records[0].WCSOperations)
        {
            if (i.name == "GetCoverage")
            {
                gc = i;
                break;
            }
        }

        string parameters = "";
        // For now picking first valid parameters
        foreach (GetCapabilites.OperationsMetadataOperationParameter i in gc.Parameter)
        {
            foreach (string j in i.AllowedValues)
            {
                if (i.name == "format")
                {
                    // Hard CODENESS
                    parameters += i.name + "=" + i.AllowedValues[6] + "&";
                }
                else
                {
                    parameters += i.name + "=" + j + "&";
                }
                break;
            }
        }

        // Build Get Coverage String
        string req = gc.DCP.HTTP.Get.href + "request=GetCoverage&" + parameters + "CRS=" + "EPSG:4326" + "&bbox=" + records[0].bbox + "&width=" + width + "&height=" + height;//+height.ToString();
        
        // Import
        factory.Import("WCS_BIL", records, "url://" + req);

        Logger.Log(Token + ": " + req);
        return req;
    }

    private string GetCapabilities()
    {
        // Check if services contains "wcs"
        if (!records[0].services.ContainsKey("wcs"))
        {
            Console.WriteLine("RETURNING" + records[0].name);
            return "";
        }
        string wcs_url = records[0].services["wcs"];
        
        // Import
        factory.Import("WCS_CAP", records, "url://" + wcs_url);

        // Return
        Logger.Log(Token + ": " + wcs_url);
        return wcs_url;
    }

    private string buildDescribeCoverage()
    {
        GetCapabilites.OperationsMetadataOperation gc = new GetCapabilites.OperationsMetadataOperation();
        foreach (GetCapabilites.OperationsMetadataOperation i in records[0].WCSOperations)
        {
            if (i.name == "DescribeCoverage")
            {
                gc = i;
                break;
            }
        }
        string parameters = "";

        // For now picking first valid parameters
        foreach (GetCapabilites.OperationsMetadataOperationParameter i in gc.Parameter)
        {
            foreach (string j in i.AllowedValues)
            {
                parameters += i.name + "=" + j + "&";
                break;
            }
        }

        string req = gc.DCP.HTTP.Get.href + "request=DescribeCoverage&" + parameters;
        Console.WriteLine(req);

        // Return
        Logger.Log(Token + ": " + req);
        return req;
    }

    private string DescribeCoverage()
    {
        // Build Describe Coverage String
        string req = buildDescribeCoverage();

        // Import
        factory.Import("WCS_DC", records, "url://" + req);

        // Return
        Logger.Log(Token + ": " + req);
        return req;
    }
}