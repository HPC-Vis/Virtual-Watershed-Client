using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LoggedObserver : Observer
{
    public override void OnDataComplete(string Notification, List<DataRecord> record)
    {
        Logger.Log(Notification);
    }

    public override void OnDataComplete(string url)
    {
        Logger.Log("Data Completed: " + url);
    }

    public override void OnDataError(string url)
    {
        Logger.Log("Data Error: " + url);
    }

    public override void OnDataStart(string url)
    {
        Logger.Log("Data Started: " + url);
    }

    public override void OnDownloadComplete(string url)
    {
        Logger.Log("Download Complete: " + url);
    }

    public override void OnDownloadError(string url, string message)
    {
        Logger.Log("Download Error: " + url);
    }

    public override void OnDownloadQueued(string url)
    {
        Logger.Log("Download Queued: " + url);
    }

    public override void OnDownloadStart(string url)
    {
        Logger.Log("Download Started: " + url);
    }
}
