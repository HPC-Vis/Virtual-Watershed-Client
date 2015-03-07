using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

/// <summary>
/// This class simply follows the path of DataRecords and keeps track of the work done with these data records.
/// version 1.0 ~ This is a rough class, but is presented as a way to keep track everything going in the system.
/// NOT THREAD SAFE!!!!!!
/// </summary>
static class DataTracker
{
    static private Dictionary<string, string> Jobs = new Dictionary<string,string>();
    static private Dictionary<string, List<DataRecord>> Records = new Dictionary<string, List<DataRecord>>();
    /// <summary>
    /// This function submits a job into this class.
    /// </summary>
    /// <param name="url"></param>
    public static void submitJob(string url,List<DataRecord> records)
    {
        Jobs.Add(url, "Starting");
        Records.Add(url, records);
    }

    /// <summary>
    /// This will update the current status of a job.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="status"></param>
    public static void updateJob(string url, string status)
    {
        Jobs[url] = status;
    }

    /// <summary>
    /// Thsi will remove a job that currently finished in the system.
    /// </summary>
    /// <param name="url"></param>
    public static List<DataRecord> JobFinished(string url)
    {
        // Remove current job 
        Jobs.Remove(url);
        
        // Get current record for job.
        var Recordss = Records[url];
        Records.Remove(url);

        // Return the record for this job.
        return Recordss;
    }
    
    /// <summary>
    /// This will check the status of a current job.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string CheckStatus(string url)
    {
        return Jobs[url];
    }
}

