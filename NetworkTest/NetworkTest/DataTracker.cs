using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

/// <summary>
/// @author: Chase Carthen, Shub Gogna
/// This class simply follows the path of DataRecords and keeps track of the work done with these data records.
/// version 1.0 ~ This is a rough class, but is presented as a way to keep track everything going in the system.
/// NOT THREAD SAFE!!!!!!
/// </summary>
static class DataTracker
{
    // Enum
    public enum Status
    {
        QUEUED, RUNNING, FINISHED, ERROR
    };

    //Tracking the status of jobs
    static private Dictionary<string, Status> Jobs = new Dictionary<string,Status>();
    
    //Tracks the records for a current request
    static private Dictionary<string, List<DataRecord>> Records = new Dictionary<string, List<DataRecord>>();

    public static List<DataRecord> GetRecords(string url)
    {
        return Records[url];
    }

    /// <summary>
    /// This function submits a job into this class.
    /// </summary>
    /// <param name="url"></param>
    public static void submitJob(string url,List<DataRecord> records)
    {
        Jobs.Add(url, Status.RUNNING);
        Records.Add(url, records);
    }

    /// <summary>
    /// This will update the current status of a job.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="status"></param>
    public static void updateJob(string url, Status status)
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
        var Recordz = Records[url];
        Records.Remove(url);

        // Return the record for this job.
        return Recordz;
    }
    
    /// <summary>
    /// This will check the status of a current job.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static Status CheckStatus(string url)
    {
        return Jobs[url];
    }
}

