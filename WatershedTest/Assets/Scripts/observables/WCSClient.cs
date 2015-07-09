using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
	public WCSClient(DataFactory Factory, DownloadType type = DownloadType.Record, string OutputPath = "", string OutputName = "",int operation = 0)
        : base(Factory,type,OutputPath,OutputName)
    {
        // Add states
        if(operation == 0)
       	{
        	StateList.Add(Operations.GetCapabilities);
        	StateList.Add(Operations.DescribeCoverage);
        	StateList.Add(Operations.GetCoverage);
        	StateList.Add(Operations.Done);
        }
        else if(operation==1)
        {
        	StateList.Add(Operations.GetCapabilities);
        	StateList.Add(Operations.Done);
        }
        else if(operation==2)
       	{
       		StateList.Add(Operations.DescribeCoverage);
       		StateList.Add(Operations.Done);
       	}
    }
    
    // Update
    public override string Update()
    {
        Logger.Log("WCS Client Token = " + Token);

        // Check if there is another state
        if (StateList.Count >= 1)
        {
            // Logger.WriteLine(StateList[0].ToString());

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
        	Logger.WriteLine("GETCAPABILITESWCS" + StateList.Count.ToString());
            return GetCapabilities();
        }
        else if (state == Operations.DescribeCoverage)
        {
			Logger.WriteLine("DESCRIBECOVERAGE");
            return DescribeCoverage();
        }
        else if (state == Operations.GetCoverage)
        {
			Logger.WriteLine("GETCOVERAGE");
            SystemParameters param = new SystemParameters();
            param.crs = CRS;
            param.boundingbox = records[0].bbox2;
            param.width = width;
            param.height = height;
            return GetCoverage(param);
        }
        else if (Operations.Done == state)
        {
            return "COMPLETE";
        }

        // Else
        return "";
    }

    // This guy will call GetCoverage -- This to be used with parameters that may not already exist
    public void GetData(DataRecord Record, SystemParameters param )
    {
        records = new List<DataRecord>();
        records.Add(Record);
        CRS = param.crs;
        boundingbox = param.boundingbox;
        width = param.width;
        height = param.height;
        interpolation = param.interpolation;
    }

    public override void CallBack()
	{
		Logger.WriteLine("WCS BEFORE CALLBACK");
        // Callback
        if (callback != null)
        {
        	Logger.WriteLine("WCS CALLBACK");
            callback(records);
        }
    }

    public override void Error()
    {
        state = Operations.Error;
    }

    //private string GetCoverage(string crs = "", string boundingbox = "", int width = 0, int height = 0, string interpolation = "nearest")
    private string GetCoverage(SystemParameters param)
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
        var a = gc.Parameter.First(x => { if (x.name == "Identifier") return true; return false; });//.Select(x => { if (x.name == "Identifier") return x; return null; });
        Logger.WriteLine("IDENTIFIERS: " + a.AllowedValues.Count().ToString());
        /*foreach(var i in a.AllowedValues)
        {
            Logger.WriteLine(i.ToString());
        }*/
        foreach (GetCapabilites.OperationsMetadataOperationParameter i in gc.Parameter)
        {
            foreach (string j in i.AllowedValues)
            {
                if (i.name == "format")
                {
                    // Hard CODENESS
                    parameters += i.name + "=" + i.AllowedValues[6] + "&";
                }
				else if (i.name == "identifiers")
                {
                	Logger.WriteLine("IDENTIFIER" + records[0].variableName);
					if(records[0].Identifier == "" || !a.AllowedValues.Contains(records[0].variableName) )
                    {
                        parameters += i.name + "=" + j + "&";
                    }
                    else
                    {
						parameters += i.name + "=" + records[0].variableName.Replace(' ','_') + "&";
                    }
                    break;
                }
                else
                {
					if(i.name == "InterpolationType" && param.interpolation != "" )
				    {
				       parameters += i.name + "=" + param.interpolation + "&";
				    }
				    else
				    {
                       parameters += i.name + "=" + j + "&";
                    }
                }
                Logger.WriteLine(i.name + " " + j);
                break;
            }
        }

        // Build Get Coverage String
        string req = gc.DCP.HTTP.Get.href + "request=GetCoverage&" + parameters + "CRS=" + "EPSG:4326" + "&bbox=" + records[0].bbox2 + "&width=" + width + "&height=" + height;//+height.ToString();
        Logger.WriteLine("WCS COVERAGE LINK: " + req);
        // Import
        factory.Import("WCS_BIL", records, "url://" + req);

        // Logger.Log(Token + ": " + req);
        return req;
    }

    private string GetCapabilities()
    {
        // Check if services contains "wcs"
        if (!records[0].services.ContainsKey("wcs"))
        {
            Logger.WriteLine("RETURNING" + records[0].name + "NO WCS CAPABILITIES");
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
        		if(i.name == "identifiers")
        		{
        			parameters += i.name + "=" + records[0].variableName+"&";
        		}
        		else
        		{
                	parameters += i.name + "=" + j + "&";
                }
                break;
            }
        }

        string req = gc.DCP.HTTP.Get.href + "request=DescribeCoverage&" + parameters;
         Logger.WriteLine(req);

        // Return
        // Logger.Log(Token + ": " + req);
        return req;
    }

    private string DescribeCoverage()
    {
        // Build Describe Coverage String
        string req = buildDescribeCoverage();

        // Import if Record
        if (type == DownloadType.Record)
        {
            factory.Import("WCS_DC", records, "url://" + req);
        }
        //else if (type == DownloadType.File)
        //{
        //    factory.Export("WCS_DC",)
        //    factory.Import("WCS_DC", records, "url://" + req);
        //}
        // Return
        // Logger.Log(Token + ": " + req);
        return req;
    }
}