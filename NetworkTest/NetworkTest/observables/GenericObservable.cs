using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public delegate void StringFunc (string str,List<DataRecord> Records);
class GenericObservable : Observerable
{
    enum States { DoRequest, Done,Error };
    States state = States.DoRequest;
    List<States> StateList = new List<States>();
    public List<DataRecord> Records = new List<DataRecord>();
    StringFunc Download;
    string url;
    public GenericObservable(DataFactory Factory)
        : base(Factory)
    {
        
    }

    public override string Update()
    {
        Console.WriteLine("UPDATE");
        if (StateList.Count >= 1)
        {
            Console.Clear();
            Console.WriteLine("UPDATE");
            try
            {
                Console.WriteLine(StateList[0]);
                state = StateList[0];
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(e.StackTrace);
                Console.ReadKey();
            }
            StateList.RemoveAt(0);
        }
        else
        {
            state = States.Error;
            return "";
        }
        if(States.DoRequest == state)
        {
            Console.WriteLine("DoRequest");
            Download(url,Records);
            return url;
        }
        else if (States.Done == state)
        {
            Console.WriteLine("COMPLETE");
            return "COMPLETE";
        }
        return "";
    }

    // The download functions needs to be encapsulated in some function --- This for none chained downloads.
    public void Request(List<DataRecord> records,StringFunc download,string URL)
    {
        Records = records;
        Download = download;
        url = URL;
        StateList.Add(States.DoRequest);
        StateList.Add(States.Done);
    }
    public override void Error()
    {
        
    }
    public override void CallBack()
    {
        Console.WriteLine("CALLBACK");
        Callback(Records);
    }
}

