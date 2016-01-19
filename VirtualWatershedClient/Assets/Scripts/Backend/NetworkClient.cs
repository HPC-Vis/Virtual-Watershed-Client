﻿using System;
using System.Collections;
using System.Net;
using System.Threading;
using System.Collections.Generic;

//http://stackoverflow.com/questions/1488099/call-an-eventhandler-with-arguments
/**Call Back Functions
 * The following functions are used to populate DataRecords with Downloaded data.
 * The ByteFunction and StringFunction delegates are used to pass downloaded data into the DataRecords.
 * **/
public delegate void StringFunction(string str);
public delegate void ByteFunction(byte[] bytes);

/** 
 * @authors Chase Carthen, Thomas Rushton, Shubham Gogna
 */
public class NetworkClient : WebClient
{
    public delegate void DownloadBytesCallback(Object sender, DownloadDataCompletedEventArgs e);
    NetworkManager netmanager;
    PriorityQueue<DownloadRequest> DownloadRequests = new PriorityQueue<DownloadRequest>();
    Dictionary<string, DownloadDataCompletedEventHandler> DataCompletedEventHandlers = new Dictionary<string, DownloadDataCompletedEventHandler>();
    private readonly object _Padlock = new object();
    int count = 0;
    DownloadRequest CurrentDownload;

    public int size { get {  return DownloadRequests.Count(); } }

    public NetworkClient(NetworkManager manager) : base()
    {
        netmanager = manager;
        DownloadProgressChanged += DownloadProgressCallback;
    }

    /// <summary>
    /// Halts all activity of the network client
    /// </summary>
    public void Halt()
    {
        Logger.WriteLine("HALTING");
        CurrentDownload = null;
        DownloadRequests.Clear();
    }

    //WebClient wc = new WebClient();
    //http://stackoverflow.com/questions/2042258/webclient-downloadfileasync-download-files-one-at-a-time
    //http://stackoverflow.com/questions/1488099/call-an-eventhandler-with-arguments

    
   /// <summary>
   /// Enqueues a download request to the network manager
   /// </summary>
   /// <param name="req">The download request to be made (could be requested in string or byte format)</param>
    public void Download( DownloadRequest req )
    {
        // Enqueue the current request and call the event
        lock (_Padlock)
        {
            DownloadRequests.Enqueue(req);
        }
        netmanager.CallDownloadQueued(req.Url);

        // Check if not busy
        if (!IsBusy)
        {
            // This call will actually run a download in another thread, 
            // but with a catch it will come back to thread of this webclient.
            // Downloads bytes asynchronously.
            StartNextDownload();
        }
    }

    /// <summary>
    /// OnDownloadDataCompleted is the default WebClient function that is called after data has been downloaded.
    /// </summary>
    /// <param name="args">The format of the data to be returned in</param>
    protected override void OnDownloadDataCompleted(DownloadDataCompletedEventArgs args)
    {
        count++;
        // Call the base function
        base.OnDownloadDataCompleted(args);
        if (args.Error != null)
        {
            Logger.WriteLine("<color=red>Failed to download data from: " + CurrentDownload.Url + "</color>");
            Logger.WriteLine("PRIORITY: " + CurrentDownload.Priority.ToString());
            netmanager.CallDataError(CurrentDownload.Url);
            CurrentDownload = null;
            StartNextDownload();
        }
        // get current download callback
        try
        {
            CurrentDownload.Callback(args.Result);
        }
        catch(Exception e)
        {
            if (CurrentDownload.Priority != 1)
            {
                CurrentDownload.Priority = 1;
                DownloadRequests.Enqueue(CurrentDownload);
            }
            DownloadRequests.Enqueue(CurrentDownload);
            Logger.WriteLine(e.Message + " " + e.StackTrace);
            Logger.WriteLine("<color=red>Failed to download data from: " + CurrentDownload.Url + "</color>");
            netmanager.CallDataError(CurrentDownload.Url);
            CurrentDownload = null;
            StartNextDownload();
            return;
        }

        // Need some way of notifying that this download is finished --- errors,success
        Logger.WriteLine("Completed byte download, passed to callback function.");
        netmanager.CallDownloadComplete(CurrentDownload.Url);

        //GlobalConfig.loading = false;

        // Start the next one
        CurrentDownload = null;
        StartNextDownload();
    }

    /// <summary>
    /// OnDownloadStringCompleted is the default WebClient function that is called after a string has been downloaded.
    /// </summary>
    /// <param name="args">The format of the data to be returned in</param>
    protected override void OnDownloadStringCompleted(DownloadStringCompletedEventArgs args)
    {
        count++;
        // Call the base function
        base.OnDownloadStringCompleted(args);
        if(args.Error != null)
        {
            Logger.WriteLine("<color=red>Failed to download data from: " + CurrentDownload.Url + "</color>");
            Logger.WriteLine("PRIORITY: " + CurrentDownload.Priority.ToString());
            netmanager.CallDataError(CurrentDownload.Url);
            CurrentDownload = null;
            StartNextDownload();
        }

        // get current download callback
        try
        {
            CurrentDownload.Callback(args.Result);
        }
        catch(Exception e)
        {
            if (CurrentDownload.Priority != 1)
            {
                CurrentDownload.Priority = 1;
                DownloadRequests.Enqueue(CurrentDownload);
            }
            Logger.WriteLine(e.Message + " " + e.StackTrace);
            Logger.WriteLine("<color=red>Failed to download data from: " + CurrentDownload.Url + "</color>");
            Logger.WriteLine("PRIORITY: " + CurrentDownload.Priority.ToString());
            netmanager.CallDataError(CurrentDownload.Url);
            CurrentDownload = null;
            StartNextDownload();
            return;
        }

        // Need some way of notifying that this download is finished --- errors,success
        // Logger.WriteLine("Completed string download, passed to callback function. " + Req.Url);
        netmanager.CallDownloadComplete(CurrentDownload.Url);

        //GlobalConfig.loading = false;

        // Start the next one
        CurrentDownload = null;
        StartNextDownload();
    }

    // Finaly something that tells us the current progress of a downlaod
    private static void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
    {
        // Displays the operation identifier, and the transfer progress.
         //Logger.WriteLine(
         //   (string)e.UserState + " " + 
         //   e.BytesReceived + " " + 
         //   e.TotalBytesToReceive + " " +
         //   e.ProgressPercentage);
        GlobalConfig.loading = true;
    }
    
    /// <summary>
    /// Starts the next download in the DownloadRequest queue
    /// </summary>
    private void StartNextDownload()
    {
        // If there are any downloads remaining do them!
        lock (_Padlock)
        {
            if (DownloadRequests.Count() > 0 && !IsBusy && CurrentDownload == null)
            {
                // Start the next one
                CurrentDownload = DownloadRequests.Dequeue();
                netmanager.CallDownloadStart(CurrentDownload.Url);

                // Check if its a byte
                if (CurrentDownload.isByte)
                {
                    DownloadDataAsync(new System.Uri(CurrentDownload.Url));
                }
                else
                {
                    DownloadStringAsync(new System.Uri(CurrentDownload.Url));
                }
            }
        }
    }

    /// <summary>
    /// Destructor of the NetworkClient
    /// </summary>
    ~NetworkClient()
    {
        this.CancelAsync();
    }

}