using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    /// <summary>
    /// A template class for ChainObserver.
    /// </summary>
abstract class ChainObserver : Observer
{
    // The first is the url for onDownloadComplete and the second is the enum that corresponds to a function that need to be called.
    // We could do call back functions here. ~ NOP
    protected Dictionary<String, Enum> Chains = new Dictionary<string, Enum>();

    // The first corresponds to the url of the previous job and the second is the current job name for tracking purposes in the dataTracker.
    protected Dictionary<string, string> ChainTracker = new Dictionary<string,string>();
    
    // Need to throw some error catching here.
    public void InsertJob(string url, Enum enu, string JobName)
    {
        Chains.Add(url, enu);
        ChainTracker.Add(url, JobName);
    }

    public string DeleteJob(string url)
    {
        Chains.Remove(url);
        var JobName = ChainTracker[url];
        ChainTracker.Remove(url);
        
        return JobName;
    }
}

