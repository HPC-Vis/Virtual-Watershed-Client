using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// WCSObserver will be as way to handle wcs service.
/// </summary>
class WCSObserver : ChainObserver
{
    enum wcs_download_state { Get_Capabilities, Describe_Coverage, Get_Coverage, Done, Error };
   
    public WCSObserver()
    {

    }
    public override void OnDownloadComplete(string url)
    {
        base.OnDownloadComplete(url);
    }

    /// <summary>
    /// GetCoverage initiates the first sequence of the chain which will be taken care progressively by this class.
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="record"></param>
    /// <param name="factory"></param>
    public void GetCoverage(string jobName, DataRecord record,DataFactory factory)
    {
        // Register Job with Data Tracker
        Console.WriteLine(jobName);
        DataTracker.updateJob(jobName, DataTracker.Status.RUNNING); // This may need to go away at some point..

        if (!record.services.ContainsKey("wcs"))
        {
            // On Data Error
            provider.CallDataError(jobName);
            Console.WriteLine("RETURNING" + record.name);
            DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
            return;
        }

        string wcs_url = record.services["wcs"];
        // Register Job with Data Tracker
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);
        
        // Insert to tracker for this observer.
        InsertJob(wcs_url, wcs_download_state.Describe_Coverage, jobName);

        // Get WCS Capabilities
        // Start first download
        factory.Import("WCS_CAP", tempList, "url://" + wcs_url);

        // Build DescribeCoverage String.
        //string dcString = buildDescribeCoverage(record);



        // Build GetCoverage string
        //string wcsCoverageString = BuildGetCoverage(record, "EPSG:" + "4326", record.bbox, 100, 100, interpolation: "bilinear");

        // GetCoverage
        //factory.Import("WCS_BIL", tempList, "url://" + wcsCoverageString);
        //while (DataTracker.CheckStatus(wcsCoverageString) != DataTracker.Status.FINISHED) { }//Console.WriteLine("WAITING"); }

        DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
        Console.WriteLine("FINISHED");
    }


    void DownloadDescribeCoverage(string previousUrl,DataFactory factory)
    {
        string jobName = DeleteJob(previousUrl);
        DataRecord record = DataTracker.GetRecords(jobName)[0];

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

        InsertJob(req, wcs_download_state.Get_Coverage, jobName);
        factory.Import("WCS_DC", DataTracker.GetRecords(jobName), "url://" + req);
    }

    void DownloadCoverage(string previousUrl, DataFactory factory, string crs = "", string boundingbox = "", int width = 0, int height = 0, string interpolation = "nearest")
    {
        string jobName = DeleteJob(previousUrl);
        DataRecord record = DataTracker.GetRecords(jobName)[0];

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


        string req = gc.DCP.HTTP.Get.href + "request=GetCoverage&" + parameters + "CRS=" + "EPSG:4326" + "&bbox=" + boundingbox + "&width=" + width + "&height=" + height;//+height.ToString();
        factory.Import("WCS_BIL", DataTracker.GetRecords(jobName), "url://" + req);
    }
}

