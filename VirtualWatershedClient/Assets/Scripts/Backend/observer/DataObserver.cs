using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// Sample class that listens for data
// This could be a standard class used by someone
class DataObserver : Observer
{
    public delegate void DataRecordSetter(List<DataRecord> Records);
    // A Dictionary that will hold callbacks to send things to a potential reciever.
    Dictionary<String, DataRecordSetter> Records = new Dictionary<string,DataRecordSetter>();
    public override void OnDataComplete(string Notification, List<DataRecord> record)
    {
        base.OnDataComplete(Notification, record);
        // Send Data out to people who are waiting on Data.
        Logger.WriteLine("COUNT: " + record.Count);
        Logger.WriteLine(Notification);
        if(Records.ContainsKey(Notification))
        {
            Records[Notification](record);
            Records.Remove(Notification);
        }
    }

    public void Register(string JobName, DataRecordSetter callback)
    {
        // Need to check if the job name is already inside of records
        Records[JobName] = callback;
    }
}

