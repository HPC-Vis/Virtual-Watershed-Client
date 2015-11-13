using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


class LogObserver : Observer
{
    public LogObserver()
    {

    }

    public override void OnDataComplete(string url)
    {
        base.OnDataComplete(url);
        Console.WriteLine("Data Complete: " + url);
    }

    public override void OnDataError(string url)
    {
        base.OnDataError(url);
        Console.WriteLine("Data Error: " + url);
    }

    public override void OnDataStart(string url)
    {
        base.OnDataStart(url);
        Console.WriteLine("Data Started: " + url);
    }

    public override void OnDownloadError(string url, string message)
    {
        base.OnDownloadError(url, message);
        Console.WriteLine("Download Error: " + url);
    }

    public override void OnDownloadComplete(string url)
    {
        base.OnDownloadComplete(url);
        Console.WriteLine("Download Completed: " + url);
    }

    public override void OnDownloadQueued(string url)
    {
        base.OnDownloadQueued(url);
        Console.WriteLine("Download Queued: " + url);
    }

    public override void OnDownloadStart(string url)
    {
        base.OnDownloadStart(url);
        Console.WriteLine("Download Started: " + url);
    }
}
