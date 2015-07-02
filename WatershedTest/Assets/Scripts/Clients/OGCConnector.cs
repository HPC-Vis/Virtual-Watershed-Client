using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// The OGCConnector is the start of a generic ogc interface.
/// </summary>
public class OGCConnector : Observer
{
    public string Name;
    public string App = "//apps/vwp";
    public string Description;
    public string Root;
    DataFactory factory;
    NetworkManager manager;

    // Holds URL -> Observable
    ThreadSafeDictionary<string, Observerable> active = new ThreadSafeDictionary<string, Observerable>();

    // Holds requests that are waiting
    PriorityQueue<Observerable> waiting = new PriorityQueue<Observerable>();

    int Limit = 10;


    System.Random R = new System.Random(1);
   

    public string GenerateToken(string FunctionName)
    {
        string Token = FunctionName + " " + R.Next();

        // Generate Random Token -- Wait until a random value is found that works ... We could do a counter system instead of random.
        Token = FunctionName + " " + R.Next();

        return Token;
    }

    public OGCConnector(DataFactory datafactory, NetworkManager networkmanager, string root = "http://vwp-dev.unm.edu")
    {
        factory = datafactory;  // Added by constructor instead of building a new one inside here
        manager = networkmanager;   // Added so worker threads can call events
        Root = root;
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
        if(active.ContainsKey(url))
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
        if( active.ContainsKey(url) )
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
        }
    }


    // Operation Functions --- Maybe split these guys into their own perspectives classes
    // For now I am replicating the VWClient ... 
    void GetMap()
    {

    }

    void GetFeatures()
    {

    }

    void GetCoverage()
    {

    }

    void GetCapabilities()
    {

    }

}
