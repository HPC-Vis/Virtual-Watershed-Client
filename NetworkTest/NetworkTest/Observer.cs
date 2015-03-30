using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Observer
{
    /// <summary>
    ///  Provider attribute and member variable.
    /// </summary>
    protected NetworkManager provider;

    public virtual void OnDownloadQueued(String url) 
    {
        Console.WriteLine("--- In OnDownloadQueued ---");
    }

    public virtual void OnDownloadStart(String url)
    {
        Console.WriteLine("--- In OnDownloadStart ---");
    }

    public virtual void OnDownloadComplete(String url)
    {
        Console.WriteLine("--- In OnDownloadComplete ---");
    }

    public virtual void OnDownloadError(String url, String message)
    {
        Console.WriteLine("--- In OnDownloadError ---");
    }

    public virtual void OnDataStart(String url)
    {
        Console.WriteLine("--- In OnDataStart ---");
    }

    public virtual void OnDataComplete(String url)
    {
        Console.WriteLine("--- In OnDataComplete ---");
    }

    public virtual void OnDataComplete(string Notification, List<DataRecord> record)
    {
        // For the Records!!
        Console.WriteLine("--- In OnDataComplete ---");
    }

    public virtual void OnDataError(String url)
    {
        Console.WriteLine("--- In OnDataError ---");
    }
}