using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public abstract class Observer
{
    /// <summary>
    ///  Provider attribute and member variable.
    /// </summary>
    protected NetworkManager provider;

    public virtual void OnDownloadQueued(String url) 
    {
        Logger.WriteLine("--- In OnDownloadQueued ---");
    }

    public virtual void OnDownloadStart(String url)
    {
        Logger.WriteLine("--- In OnDownloadStart ---");
    }

    public virtual void OnDownloadComplete(String url)
    {
        Logger.WriteLine("--- In OnDownloadComplete ---");
    }

    public virtual void OnDownloadError(String url, String message)
    {
        Logger.WriteLine("--- In OnDownloadError ---");
    }

    public virtual void OnDataStart(String url)
    {
        Logger.WriteLine("--- In OnDataStart ---");
    }

    public virtual void OnDataComplete(String url)
    {
        Logger.WriteLine("--- In OnDataComplete ---");
    }

    public virtual void OnDataComplete(string Notification, List<DataRecord> record)
    {
        // For the Records!!
        Logger.WriteLine("--- In OnDataComplete ---");
    }

    public virtual void OnDataError(String url)
    {
        
        if(url != null)
        {
			Logger.WriteLine("--- In OnDataError " + url +  " ---");
        }
        else
        {
			Logger.WriteLine("--- In OnDataError ---");
        }
    }
}