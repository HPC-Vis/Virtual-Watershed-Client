using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// NetworkManager is more than a manager now. It also calls events now.

public class NetworkManager 
{
    // Members
    int index = 0;
    NetworkClient[] clients;
    List<Observer> observers = new List<Observer>();

    /// <summary>
    /// Constructor of the NetworkManager
    /// </summary>
    /// <param name="size">The number of NetworkClients conatined in the manager</param>
    public NetworkManager(int size = 4)
    {
        // Check if size is less than 0
        if (size <= 0) { size = 1; }

        // Allocate the clients
        clients = new NetworkClient[size];
        for( int i = 0; i < size; ++i )
        {
            clients[i] = new NetworkClient(this);
        }
    }

    /// <summary>
    /// Adds a download request to one of the NetworkClients to be processed
    /// </summary>
    /// <param name="req">The request to be added</param>
    public void AddDownload(DownloadRequest req)
    {
        //DataTracker.updateJob(req.Url, "Added to Network Client: " + index);
        //DataTracker.updateJob(req.Url, DataTracker.Status.QUEUED);
        // Add request at index
        clients[index].Download(req);

        // Increment index to the next queue (round robin)
        index = (index + 1) % clients.Length;
    }

    /// <summary>
    /// Subscribes the passed in observer to the list of observers
    /// </summary>
    /// <param name="toAdd">The observer to be subscribed</param>
    public void Subscribe(Observer toAdd)
    {
        if(observers.Contains(toAdd))
        {
            // Error and return
            return;
        }
        
        // Else
        observers.Add(toAdd);
    }

    /// <summary>
    /// Unsubscribes the passed in observer from the list of observers
    /// </summary>
    /// <param name="toRemove">The observer to be unsubscribed</param>
    public void Unsubscribe(Observer toRemove)
    {
        // Remove
        if (observers.Contains(toRemove))
        {
            observers.Remove(toRemove);
        }
    }

    /// <summary>
    /// Sends a message to each of the observers that a download has been queued
    /// </summary>
    /// <param name="url">The url of the queued download</param>
    public void CallDownloadQueued(String url) 
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDownloadQueued(url);
        }
    }

    /// <summary>
    /// Calls the OnDownloadStart function of each observer with the given url
    /// </summary>
    /// <param name="url">The url of the download that is to be started</param>
    public void CallDownloadStart(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDownloadStart(url);
        }
    }

    /// <summary>
    /// Calls the OnDownlaodComplete function of each observer with the given url
    /// </summary>
    /// <param name="url">The url of the job downlaod that has completed</param>
    public void CallDownloadComplete(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDownloadComplete(url);
        }
        for (int i = 0; i < clients.Length; i++)
        {
            if (clients[i].IsBusy)
            {
                GlobalConfig.loading = true;
                return;
            }
        }
        Logger.WriteLine("CHECKING IF BUSY IS FALSE");

        GlobalConfig.loading = false;
    }

    /// <summary>
    /// Calls the OnDownloadError function of each observer with teh fiven url and error message
    /// </summary>
    /// <param name="url">The url of the job that ran into an error</param>
    /// <param name="message">The message indicating the error that occured</param>
    public void CallDownloadError(String url, String message)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDownloadError(url, message);
        }
    }

    /// <summary>
    /// Calls the OnDataStart function of each observer with the given url
    /// </summary>
    /// <param name="url">The url of the job that is being processed</param>
    public void CallDataStart(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDataStart(url);
        }
    }

    /// <summary>
    /// Calls the OnDataComplete funciton of each observer with the given url
    /// </summary>
    /// <param name="url">The url of the job that has completed its data acquisition</param>
    public void CallDataComplete(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDataComplete(url);
        }
        //for (int i = 0; i < clients.Length; i++)
        //{
        //    if (clients[i].IsBusy)
        //    {
        //        GlobalConfig.loading = true;
        //        return;
        //    }
        //}
        //Logger.WriteLine("CHECKING IF BUSY IS FALSE");

        //GlobalConfig.loading = false;
    }

    /// <summary>
    /// Calls the onDataComplete function of each observer with the given notification and list of datarecords
    /// </summary>
    /// <param name="Notification">The message indicating data completion</param>
    /// <param name="Records">The list of datarecords that has been completed</param>
    public void CallDataComplete(string Notification, List<DataRecord> Records)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDataComplete(Notification,Records);
        }
    }

    /// <summary>
    /// Calls the OnDataError function of each observer with the given url
    /// </summary>
    /// <param name="url">The url of the job that ran into a data error</param>
    public void CallDataError(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDataError(url);
        }
    }

    /// <summary>
    /// Halts all activity of the NetworkManager
    /// </summary>
    public void Halt()
    {
        for(int i = 0; i < clients.Length; i++)
        {
            clients[i].Halt();
            clients[i].CancelAsync();
        }
    }

    /// <summary>
    /// Destructor of the NetworkManagerr
    /// </summary>
    ~NetworkManager()
    {
        observers.Clear();
        Logger.WriteLine("Network Manager Destructor Called");
        for(int i = 0; i < clients.Length; i++)
        {
            clients[i].Dispose();
        }
    }
}
