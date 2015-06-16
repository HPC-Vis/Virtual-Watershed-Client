using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public delegate void StringFunc (string str,List<DataRecord> Records);

class GenericObservable : Observerable
{
    // Enum
    protected enum States { Start, Done, Error };

    // Fields
    private string url;
    private States state;
    private List<States> StateList = new List<States>() { States.Start, States.Done };
    private StringFunc download;

    // Constructor
    public GenericObservable(DataFactory Factory,DownloadType type=DownloadType.Record,string OutputPath="",string OutputName="") : base(Factory,type,OutputPath,OutputPath) 
    {}

    // Update
    public override string Update()
    {
        // Check if there is another state
        if (StateList.Count >= 1)
        {
            Logger.WriteLine(StateList[0].ToString());

            // Set the state and remove the first
            state = StateList[0];
            StateList.RemoveAt(0);
        }
        else
        {
            state = States.Error;
            return "";
        }

        // Check the state
        if (state == States.Start)
        {
            // Download
            download(url,records);
            return url;
        }
        else if (state == States.Done)
        {
            // Return
            return "COMPLETE";
        }

        // Else
        return "";
    }

    // The download functions needs to be encapsulated in some function --- This for non chained downloads.
    public void Request(List<DataRecord> Records, StringFunc Download, string URL)
    {
        records = Records;
        download = Download;
        url = URL;
    }
    
    public override void CallBack()
    {
        // Logger.WriteLine("CALLBACK");

        // Callback with the data
        callback(records);
    }

    public override void Error()
    {
        state = States.Error;
    }
}

