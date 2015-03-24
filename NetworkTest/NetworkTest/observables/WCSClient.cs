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
    DataFactory factory;

    public WCSClient(DataFactory Factory)
    {
        factory = Factory;
    }

    enum WCS_OPERATION { GetCapabilities, DescribeCoverage, GetCoverage, Done, Error, None };
    WCS_OPERATION state = WCS_OPERATION.None;
    List<WCS_OPERATION> StateList = new List<WCS_OPERATION>();

    public string current_url;
    // Now we can store parameters in this class or create another class to hold them!
    int width, height;
    Rect bbox;
    string boundingbox;
    string CRS;
    string LayerName; // This only handles one layer.
    string interpolation;
    DataRecord record; // The datarecord to apply changes too.
    public string Update()
    {
        if (StateList.Count >= 1)
        {
            StateList.RemoveAt(0);
            state = StateList[0];
        }
        else
        {
            state = WCS_OPERATION.None;
        }

        if (state == WCS_OPERATION.GetCapabilities)
        {
            return GetCapabilities();
        }
        else if (state == WCS_OPERATION.DescribeCoverage)
        {
            return DescribeCoverage();
        }
        else if (state == WCS_OPERATION.GetCoverage)
        {
            return GetCoverage(CRS, record.bbox, width, height);
        }
        else if (state == WCS_OPERATION.Error)
        {
            // Report Error
            return "";
        }
        else if (WCS_OPERATION.Done == state)
        {
            // Report Done
        }
        return "";
    }

    // This guy will call GetCoverage -- This to be used with parameters that may not already exist
    public string GetData(string crs = "", string BoundingBox = "", int Width = 0, int Height = 0, string Interpolation = "nearest")
    {
        CRS = crs;
        boundingbox = BoundingBox;
        width = Width;
        height = Height;
        interpolation = Interpolation;
        StateList.Add(WCS_OPERATION.GetCapabilities);
        StateList.Add(WCS_OPERATION.DescribeCoverage);
        StateList.Add(WCS_OPERATION.GetCoverage);
        StateList.Add(WCS_OPERATION.None);
        return GetCapabilities();
    }

    public string GetCoverage(string crs = "", string boundingbox = "", int width = 0, int height = 0, string interpolation = "nearest")
    {

        state = WCS_OPERATION.GetCoverage;
        // By this point the get coverage string should be built.
        GetCapabilites.OperationsMetadataOperation gc = new GetCapabilites.OperationsMetadataOperation();
        foreach (GetCapabilites.OperationsMetadataOperation i in record.WCSOperations)
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
        string req = gc.DCP.HTTP.Get.href + "request=GetCoverage&" + parameters + "CRS=" + "EPSG:4326" + "&bbox=" + boundingbox + "&width=" + width + "&height=" + height;//+height.ToString();

        List<DataRecord> records = new List<DataRecord>();

        records.Add(record);

        // Download Coverage
        factory.Import("WCS_BIL", records, "url://" + req);
        return req;

    }

    public string GetCapabilities()
    {
        state = WCS_OPERATION.GetCapabilities;
        if (!record.services.ContainsKey("wcs"))
        {
            Console.WriteLine("RETURNING" + record.name);
            return "";
        }
        string wcs_url = record.services["wcs"];
        // Register Job with Data Tracker
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);

        // Get WCS Capabilities
        factory.Import("WCS_CAP", tempList, "url://" + wcs_url);
        return wcs_url;
    }

    string buildDescribeCoverage()
    {
        GetCapabilites.OperationsMetadataOperation gc = new GetCapabilites.OperationsMetadataOperation();
        foreach (GetCapabilites.OperationsMetadataOperation i in record.WCSOperations)
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

        return req;
    }

    public string DescribeCoverage()
    {
        state = WCS_OPERATION.DescribeCoverage;
        // Build Describe Coverage String
        string req = buildDescribeCoverage();

        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);

        // Download Describe Coverage
        factory.Import("WCS_DC", tempList, "url://" + req);
        return req;
    }

}