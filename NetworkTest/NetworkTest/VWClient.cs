using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.IO;

public class VWClient : Observer
{
    public string Name;
    public string App = "//apps/my_app";
    public string Description;
    public string Root;
    DataFactory factory;
    NetworkManager manager;
    ThreadSafeDictionary<string, Thread> Threads = new ThreadSafeDictionary<string, Thread>(); // Alternative to Unity's threadpool

    // Needed still? 
    ThreadSafeDictionary<string, KeyValueTriple<string, string, DataRecord>> Requests = new ThreadSafeDictionary<string, KeyValueTriple<string, string, DataRecord>>();

    // Holds URL -> Observable
    Dictionary<string, Observerable> active = new Dictionary<string, Observerable>();

    // Holds requests that are waiting
    Queue<Observerable> waiting = new Queue<Observerable>();

    int Limit = 10;

    public VWClient(DataFactory datafactory, NetworkManager networkmanager, string root = "http://129.24.63.65")
    {
        factory = datafactory;  // Added by constructor instead of building a new one inside here
        manager = networkmanager;   // Added so worker threads can call events
        Root = root;
    }

    public override void OnDownloadComplete(string url)
    {
        // Loop through the active
        if( active.ContainsKey(url) )
        {
            // Update
            string result = active[url].Update();

            // Check if complete
            if (result == "COMPLETE")
            {
                // Remove from active
                active.Remove(url);

                // Call OnDataComplete event
                manager.CallDataComplete(url);
            }
            else if (result == "")
            {
                // Error code
                // Print Some Error Here.
            }
            else
            {
                // Add new request
                active[result] = active[url];

                // Remove old <url, observable>
                active.Remove(url);
            }
        }
    }

    void AddObservable(Observerable observable)
    {
        // If the number active is at threshold, move into waiting
        if (Limit > active.Count)
        {
            waiting.Enqueue(observable);
        }
        // Else Add the observable to "active" and "observables"
        else
        {
            string URL = observable.Update();
            active[URL] = observable;
        }
    }

    public void getCoverage(string crs = "", string BoundingBox = "", int Width = 0, int Height = 0, string Interpolation = "nearest") // Parameters TODO
    {
        // Build a WCS observable
        var client = new WCSClient(factory);
        client.GetData(crs, BoundingBox, Width, Height, Interpolation);

        AddObservable(client);
    }

    public void getMap() // Parameters TODO
    {
        // Build a WMS observable
        // AddObservable();
    }

    public void getFeatures() // Parameters TODO
    {
        // Build a WFS observable

        // AddObservable();
    }








    bool Activity()
    {
        return Threads.Count() != 0;
    }

    public string RequestRecords(int offset, int limit, string model_set_type = "vis", string service = "", string query = "", string starttime = "", string endtime = "", string location = "", string state = "", string modelname = "", string timestamp_start = "", string timestamp_end = "", string model_vars = "")
    {
        List<DataRecord> Records = new List<DataRecord>();

        // Make a request to the root html service. -- Need to pass in a call back function...
        string req = Root + App + "/search/datasets.json" + "?offset=" + offset + "&model_set_type=" + model_set_type + "&limit=" + limit + "&version=3";
        if (starttime != "" && endtime != "")
        {
            req += "&startime=" + starttime + "&endtime=" + endtime;
        }
        if (query != "")
        {
            req += "&query=" + query;
        }
        if (location != "")
        {
            req += "&location=" + location;
        }
        if (modelname != "")
        {
            req += "&modelname=" + modelname;
        }
        if (state != "")
        {
            req += "&state=" + state;
        }
        if (timestamp_start != "")
        {
            req += "&timestamp_start=" + timestamp_start;
        }
        if (timestamp_end != "")
        {
            req += "&timestamp_end=" + timestamp_end;
        }
        if (model_vars != "")
        {
            req += "&model_vars=" + model_vars;
        }
            
        // Make the request and enqueue it...
        // Request Download -- 
        factory.Import("VW_JSON", Records, "url://" + req);
        return req;
    }
    void doService(string url, DataRecord record, string service)
    {
        Console.WriteLine(Limit);
        if (Threads.Count() < Limit)
        {
            // Do stuff here with record
            if (service == "wms")
            {
                // Get Map Here
                List<DataRecord> templist = new List<DataRecord>();
                templist.Add(record);
                DataTracker.submitJob(url, templist);
                var t = new Thread(() => GetMap(url,record));

                Threads[url] =  t;
                t.Start();
            }
            else if (service == "wcs")
            {
                // Get Coverage Here
                List<DataRecord> templist = new List<DataRecord>();
                templist.Add(record);
                DataTracker.submitJob(url, templist);
                var t = new Thread(() => GetCoverage(url,record));

                Threads[url] =  t;
                t.Start();
            }

            else if (service == "wfs")
            {
                // Get Feature here
                List<DataRecord> templist = new List<DataRecord>();
                templist.Add(record);
                DataTracker.submitJob(url, templist);
                var t = new Thread(() => GetFeature(url,record));
                Threads[url] =  t;
                t.Start();
            }
            else
            {
                Console.WriteLine("FGDC");
                List<DataRecord> templist = new List<DataRecord>();
                templist.Add(record);
                DataTracker.submitJob(url, templist);
                var t = new Thread(() => GetMetaData(url,record));
                Threads[url] =  t;
                t.Start();

            }

        }
        else
        {
            //Console.ReadKey();
            Requests[url] = new KeyValueTriple<string, string, DataRecord>(url, service, record);
            // new KeyValuePair<KeyValuePair<string, string>, DataRecord>(new KeyValuePair<string, string>(url, service), record);
        }
    }
    // This function will have a lot of paramters that are default for the different services......
    public void Download(string url, DataRecord record, string service)
    {
        doService(url, record, service);
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

    void GetMap(string jobName,DataRecord record,int width=100, int height=100,string format = "image/png")
    {
        // Register Job with Data Tracker
        Console.WriteLine(jobName);
        DataTracker.updateJob(jobName, DataTracker.Status.RUNNING);
        // Call Get Capabilites Here....
        if (!record.services.ContainsKey("wms"))
        {
            Finished(record, jobName);
            Console.WriteLine("RETURNING" + record.name);
            DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
            return;
        }

        // In the final steps populate the data record with GetMap Download
        string wms_url = record.services["wms"];
        // Register Job with Data Tracker
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);
        factory.Import("WMS_CAP", tempList, "url://" + wms_url);
        while (DataTracker.CheckStatus(wms_url) != DataTracker.Status.FINISHED) { Console.WriteLine("WAITING"); }
        Console.WriteLine("BBOX: " + record.bbox);
        Console.ReadKey();
        // Build wms string
        string wms_get_map_string = "";
        string request = Root + App + "/datasets/" + record.id.Replace('"', ' ').Trim() + 
            "/services/ogc/wms?SERVICE=wms&Request=GetMap&" + "width=" + width + "&height=" + height + 
            "&layers=" + record.title + "&bbox=" + bboxSplit(record.bbox) + 
            "&format=" + format + "&Version=1.1.1" + "&srs=epsg:4326";

        factory.Import("WMS_PNG", tempList, "url://" + request);
        while (DataTracker.CheckStatus(request) != DataTracker.Status.FINISHED) { }//Console.WriteLine("WAITING"); }



        // Finish everything up
        Finished(record, jobName);
        DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
        Console.WriteLine("FINISHED");
            
            
    }

    string buildDescribeCoverage(DataRecord record)
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

    string BuildGetCoverage(DataRecord record, string crs = "", string boundingbox = "", int width = 0, int height = 0, string interpolation = "nearest")
    {

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
        return req;
    }


    void GetCoverage(string jobName, DataRecord record)
    {
        // Register Job with Data Tracker
        Console.WriteLine(jobName);
        DataTracker.updateJob(jobName, DataTracker.Status.RUNNING);

        if (!record.services.ContainsKey("wcs"))
        {
            Finished(record, jobName);
            Console.WriteLine("RETURNING" + record.name);
            DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
            return;
        }
        string wcs_url = record.services["wcs"];
        // Register Job with Data Tracker
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);

        // Get WCS Capabilities
        factory.Import("WCS_CAP", tempList, "url://" + wcs_url);
        while (DataTracker.CheckStatus(wcs_url) != DataTracker.Status.FINISHED) { }//Console.WriteLine("WAITING"); }

        // Build DescribeCoverage String.
        string dcString = buildDescribeCoverage(record);
        StreamWriter sw = new StreamWriter("test.txt");
        sw.Write(dcString);
        sw.Close();

        // DescribeCoverage
        factory.Import("WCS_DC", tempList, "url://" + dcString);
        while (DataTracker.CheckStatus(dcString) != DataTracker.Status.FINISHED) { }//Console.WriteLine("WAITING"); }
        Console.WriteLine(record.width + " " + record.height);
        Console.WriteLine(record.bbox);
        // Build GetCoverage string
        string wcsCoverageString = BuildGetCoverage(record, "EPSG:" + "4326", record.bbox, 100, 100, interpolation: "bilinear");

        // GetCoverage
        factory.Import("WCS_BIL", tempList, "url://" + wcsCoverageString);
        while (DataTracker.CheckStatus(wcsCoverageString) != DataTracker.Status.FINISHED) { }//Console.WriteLine("WAITING"); }

        DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
        Console.WriteLine("FINISHED");

        Finished(record,jobName);
    }

    void GetFeature(string jobName, DataRecord record, string version = "1.0.0")
    {
        // Register Job with Data Tracker
        Console.WriteLine(jobName);
        DataTracker.updateJob(jobName, DataTracker.Status.RUNNING);
        Console.WriteLine(record.id);
        Console.ReadKey();
        string request = Root + App + "/datasets/" + record.id.Replace('"', ' ').Trim() + "/services/ogc/wfs?SERVICE=wfs&Request=GetFeature&" + "&version=" + version + "&typename=" + record.name.Trim(new char[] { '\"' }) + "&bbox=" + bboxSplit(record.bbox) + "&outputformat=gml2&" + "&srs=epsg:4326";
        if (!record.services.ContainsKey("wfs"))
        {
            Finished(record, jobName);
            Console.WriteLine("RETURNING" + record.name);
            DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
            return;
        }
        //string wcs_url = record.services["wcs"];
        // Register Job with Data Tracker
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);

        factory.Import("WFS_GML", tempList, "url://" + request);
        while (DataTracker.CheckStatus(request) != DataTracker.Status.FINISHED) { }//Console.WriteLine("WAITING"); }

        // Register Job with Data Tracker
        Finished(record, jobName);
        DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
        Console.WriteLine("FINISHED");
            
    }

    void GetMetaData(string jobName, DataRecord record)
    {
        Console.WriteLine(jobName);
        DataTracker.updateJob(jobName, DataTracker.Status.RUNNING);
        foreach(var i in record.services)
        {
            Console.WriteLine(i);
        }
        if (!record.services.ContainsKey("xml_fgdc"))
        {
            Finished(record, jobName);
            Console.WriteLine("RETURNING" + record.name);
            DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
            return;
        }
        string xml_url =  record.services["xml_fgdc"];
        // Register Job with Data Tracker
        List<DataRecord> tempList = new List<DataRecord>();
        tempList.Add(record);
        factory.Import("VW_FGDC", tempList, "url://"+xml_url);
        while (DataTracker.CheckStatus(xml_url) != DataTracker.Status.FINISHED) { Console.WriteLine("WAITING"); }

        Finished(record,jobName);
        DataTracker.updateJob(jobName, DataTracker.Status.FINISHED);
        Console.WriteLine("FINISHED");
    }

    // need some thread safety here..
    void Finished(DataRecord record, string url)
    {
        Threads.Remove(url);
        Console.WriteLine(url);
        if(Requests.Count() > 0)
        {
                
            var front = Requests[url];
            Requests.Remove(url);;
            //Console.WriteLine("REMOVING !!!!" + front.Key.Key);
            Console.WriteLine("REMOVING !!!!" + front.Key);
            // May want to replace with custom tuple class ~ Do not use .net 4.0 ~ Unity issues....
            
            // doService(front.Key.Key, front.Value, front.Key.Value);
            doService(front.Key, front.VTwo, front.VOne);
        }
    }
}

