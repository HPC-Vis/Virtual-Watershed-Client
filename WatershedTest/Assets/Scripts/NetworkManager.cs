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

    // Constructor
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

    // Methods
    public void AddDownload(DownloadRequest req)
    {
        //DataTracker.updateJob(req.Url, "Added to Network Client: " + index);
        //DataTracker.updateJob(req.Url, DataTracker.Status.QUEUED);
        // Add request at index
        clients[index].Download(req);

        // Increment index to the next queue (round robin)
        index = (index + 1) % clients.Length;
    }

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

    public void Unsubscribe(Observer toRemove)
    {
        // Remove
        if (observers.Contains(toRemove))
        {
            observers.Remove(toRemove);
        }
    }

    public void CallDownloadQueued(String url) 
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDownloadQueued(url);
        }
    }

    public void CallDownloadStart(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDownloadStart(url);
        }
    }

    public void CallDownloadComplete(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDownloadComplete(url);
        }
    }

    public void CallDownloadError(String url, String message)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDownloadError(url, message);
        }
    }

    public void CallDataStart(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDataStart(url);
        }
    }

    public void CallDataComplete(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDataComplete(url);
        }
    }

    public void CallDataComplete(string Notification, List<DataRecord> Records)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDataComplete(Notification,Records);
        }
    }

    public void CallDataError(String url)
    {
        // Loop through the observers
        foreach (Observer obs in observers)
        {
            obs.OnDataError(url);
        }
    }

    // A PATCH For the Editor
    public void Halt()
    {
        for(int i = 0; i < clients.Length; i++)
        {
            clients[i].Halt();
            clients[i].CancelAsync();
        }
    }

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
