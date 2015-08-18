using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// The OGCConnector will be used for processing OGC services of anytype
/// </summary>
public class OGCConnector : Observer
{
    public string Name;
    public string Description;
    DataFactory factory;
    NetworkManager manager;

    // Holds URL -> Observable
    ThreadSafeDictionary<string, Observerable> active = new ThreadSafeDictionary<string, Observerable>();

    // Holds requests that are waiting
    PriorityQueue<Observerable> waiting = new PriorityQueue<Observerable>();

    int Limit = 10;


    System.Random R = new System.Random(1);

    bool TESTDONE = false;

    public string GenerateToken(string FunctionName)
    {
        string Token = FunctionName + " " + R.Next();

        // Generate Random Token -- Wait until a random value is found that works ... We could do a counter system instead of random.
        Token = FunctionName + " " + R.Next();

        return Token;
    }

    public OGCConnector(DataFactory datafactory, NetworkManager networkmanager)
    {
        factory = datafactory;  // Added by constructor instead of building a new one inside here
        manager = networkmanager;   // Added so worker threads can call events

    }

    public override void OnDataComplete(string url)
    {
        if (active.ContainsKey(url))
        {
            active[url].CallBack();
            active.Remove(url);
            if (waiting.Count() > 0)
            {
                var job = waiting.Dequeue();
                AddObservable(job);
            }
        }
    }

    public override void OnDataError(string url)
    {
        Logger.WriteLine("There was an error in: " + url);
        if (active.ContainsKey(url))
        {
            active.Remove(url);
            if (waiting.Count() > 0)
            {
                var job = waiting.Dequeue();
                AddObservable(job);
            }

            // Send second error message to notify other observers there was an error with obtaining the data record?

        }
        base.OnDataError(url);
    }

    public override void OnDownloadComplete(string url)
    {
        // Loop through the active
        if (active.ContainsKey(url))
        {
            // Update
            string result = active[url].Update();
            // Logger.WriteLine("RESULT: " + result);
            // Check if complete
            if (result == "COMPLETE")
            {
                // Remove from active
                //active.Remove(url);

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


    /// <summary>
    /// Adds an observable to the list
    /// </summary>
    /// <param name="observable"></param>
    /// <returns></returns>
    void AddObservable(Observerable observable)
    {
        /// Add Lock here
        // If the number active is at threshold, move into waiting
        if (Limit == active.Count())
        {
            waiting.Enqueue(observable);
        }
        // Else Add the observable to "active" and "observables"
        else
        {
            string URL = observable.Update();
            active[URL] = observable;
            Debug.LogError("ADDED " + URL);
        }
    }


    // Operation Functions --- Maybe split these guys into their own perspectives classes
    // For now I am replicating the VWClient ... 
    void GetMap()
    {

    }

    void GetFeature()
    {

    }

    public void getCoverage(DataRecordSetter Setter, DataRecord Record, SystemParameters param)
    {
        // Build a WCS observable
        Logger.WriteLine("WCS getCoverage Called");
        var client = new WCSClient(factory, param.type, param.outputPath, param.outputPath);
        client.GetData(Record, param);
        client.Token = GenerateToken("GetCoverage");
        client.callback = Setter;
        client.Priority = param.Priority;
        client.ModelRunUUID = Record.modelRunUUID;
        AddObservable(client);
    }

    public void getCapabilities(DataRecordSetter Setter, DataRecord Record, SystemParameters param)
    {
        // We need an observable here -- for now we assume a WMS Request
        if (Record.services.ContainsKey("wms") && param.service == "wms")
        {
            Logger.WriteLine("CALLBACK IS: " + (Setter == null).ToString());
            // Let the magic begin
            var client = new WMSClient(factory, param.type, param.outputPath, param.outputName, 1);
            client.GetData(Record, param);
            client.Token = GenerateToken("GetCapabilitiesWMS");
            client.callback = Setter;
            client.Priority = param.Priority;
            client.ModelRunUUID = Record.modelRunUUID;
            AddObservable(client);
        }
        else if (Record.services.ContainsKey("wcs") && param.service == "wcs")
        {
            Logger.WriteLine("CALLBACK WCS IS: " + (Setter == null).ToString());
            // Let the magic begin
            var client = new WCSClient(factory, param.type, param.outputPath, param.outputName, 1);
            //client = App;
            //client.Root = Root;
            client.GetData(Record, param);
            client.Token = GenerateToken("GetCapabilitiesWCS");
            client.callback = Setter;
            client.Priority = param.Priority;
            client.ModelRunUUID = Record.modelRunUUID;
            AddObservable(client);
        }
        else if (Record.services.ContainsKey("wfs") && param.service == "wfs")
        {
            Logger.WriteLine("CALLBACK WFS IS: " + (Setter == null).ToString());
            var client = new WFSClient(factory, param.type, param.outputPath, param.outputName, 1);
            client.GetData(Record, param);
            client.Token = GenerateToken("GetCapabilitiesWFS");
            client.callback = Setter;
            client.Priority = param.Priority;
            client.ModelRunUUID = Record.modelRunUUID;
            AddObservable(client);
        }
    }

    void GetTestCoverage(List<DataRecord> Records)
    {
        Debug.LogError("GetTestCoverage");
        var sp = new SystemParameters();
        sp.interpolation = "bilinear";
        sp.width = 100;
        sp.height = 100;
        getCoverage(TestDone, Records[0], sp);
        
    }

    void TestDone(List<DataRecord> Records)
    {
        if(Records[0].Data == null)
        {
            Debug.LogError("THE DATA IS NULL");
        }
        else
        {
            Debug.LogError("Origin: " + Records[0].Data[0, 0]);
        }
        TESTDONE = true;
    }

    // Based on the url download with the appropriate service
    public bool MagicFunction(string url)
    {
        // Check if it contains wcs
        if (url.ToLower().Contains("service=wcs"))
        {

            var Now = DateTime.UtcNow;
            // GetCapabilities
            var sp = new SystemParameters();
            var record = new DataRecord();
            record.services["wcs"] = url;
            sp.service = "wcs";
            getCapabilities(GetTestCoverage, record, sp);
            Debug.LogError("HERE:  " + (DateTime.UtcNow - Now).TotalSeconds);
            while (true)
            {
                if (TESTDONE)
                {
                    Debug.LogError("DONE");
                    return true;
                }
                if ((DateTime.UtcNow - Now).TotalSeconds > 120.0)
                {
                    return false;
                }
            }
            Debug.LogError("COUNT NOT GREATER THAN ZERO");
            return false;
        }
        return false;
    }
}
