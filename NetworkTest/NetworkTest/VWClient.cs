using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

//http://www.grumpydev.com/2010/02/25/thread-safe-dictionarytkeytvalue/
public class SafeDictionary<TKey, TValue>
{

    private readonly object _Padlock = new object();

    private readonly Dictionary<TKey, TValue> _Dictionary = new Dictionary<TKey, TValue>();

    public void Remove(TKey key)
    {
        lock(_Padlock)
        {
            _Dictionary.Remove(key);
        }
    }

    public int Count()
    {
        lock(_Padlock)
        {
           return  _Dictionary.Count;
        }
    }
    public TValue this[TKey key]
    {

        get
        {

            lock (_Padlock)
            {

                return _Dictionary[key];

            }

        }

        set
        {

            lock (_Padlock)
            {

                _Dictionary[key] = value;

            }

        }

    }


    public bool TryGetValue(TKey key, out TValue value)
    {

        lock (_Padlock)
        {
            return _Dictionary.TryGetValue(key, out value);
        }
    }

}

    class VWClient
    {
        public string Name;
        public string App = "//apps/my_app";
        public string Description;
        public string Root;
        DataFactory dataFactory = new DataFactory();
        // This will hold any unprocessed requests
        
        SafeDictionary<string,KeyValuePair<KeyValuePair<string,string>,DataRecord>> Requests = new SafeDictionary<string,KeyValuePair<KeyValuePair<string,string>,DataRecord>>();
        
        int Limit = 10;

        // We will create this not to mess with "Unity's threadpool"
        SafeDictionary<string, Thread> Threads = new SafeDictionary<string, Thread>();


        public VWClient(string root = "http://129.24.63.65")
        {
            Root = root;
        }
        bool Activity()
        {
            return Threads.Count() != 0;
        }

        public string RequestRecords(int offset, int limit, string model_set_type = "vis", string service = "", string query = "", string starttime = "", string endtime = "", string location = "", string state = "", string modelname = "", string timestamp_start = "", string timestamp_end = "", string model_vars = "")
        {
            List<DataRecord> Records = new List<DataRecord>();

            // Make a request to the root html service. -- Need to pass in a call back function...
            string req = Root + App + "/search/datasets.json" + "?offset=" + offset + "&model_set_type=" + model_set_type + "&limit=" + limit + "&version=3";
            if (starttime != "" && endtime != "")
            {
                req += "&startime=" + starttime + "&endtime=" + endtime;
            }
            if (query != "")
            {
                req += "&query=" + query;
            }
            if (location != "")
            {
                req += "&location=" + location;
            }
            if (modelname != "")
            {
                req += "&modelname=" + modelname;
            }
            if (state != "")
            {
                req += "&state=" + state;
            }
            if (timestamp_start != "")
            {
                req += "&timestamp_start=" + timestamp_start;
            }
            if (timestamp_end != "")
            {
                req += "&timestamp_end=" + timestamp_end;
            }
            if (model_vars != "")
            {
                req += "&model_vars=" + model_vars;
            }
            
            // Make the request and enqueue it...
            // Request Download -- 
            dataFactory.Import("VW_JSON", Records, "url://" + req);
            return req;
        }
        void doService(string url, DataRecord record, string service)
        {
            Console.WriteLine(Limit);
            if (Threads.Count() < Limit)
            {
                // Do stuff here with record
                if (service == "wms")
                {
                    // Get Map Here
                    var t = new Thread(() => GetMap(url,record));

                    Threads[url] =  t;
                    t.Start();
                }
                else if (service == "wcs")
                {
                    // Get Coverage Here
                    var t = new Thread(() => GetCoverage(url,record));
                    Threads[url] =  t;
                    t.Start();
                }

                else if (service == "wfs")
                {
                    // Get Feature here
                    var t = new Thread(() => GetFeature(url,record));
                    Threads[url] =  t;
                    t.Start();
                }
                else
                {
                    var t = new Thread(() => GetMetaData(url,record));
                    Threads[url] =  t;
                    t.Start();

                }

            }
            else
            {
                Console.ReadKey();
                Requests[url] = new KeyValuePair<KeyValuePair<string, string>, DataRecord>(new KeyValuePair<string, string>(url, service), record);
            }
        }
        // This function will have a lot of paramters that are default for the different services......
        public void Download(string url, DataRecord record, string service)
        {
            doService(url, record, service);
        }


        void GetMap(string url,DataRecord record)
        {
            Finished(record,url);
        }

        void GetCoverage(string url,DataRecord record)
        {
            Finished(record,url);
        }

        void GetFeature(string url,DataRecord record)
        {
            Finished(record,url);
        }

        void GetMetaData(string url, DataRecord record)
        {
            Finished(record,url);
        }

        // need some thread safety here..
        void Finished(DataRecord record, string url)
        {
            Threads.Remove(url);
            Console.WriteLine(url);
            if(Requests.Count() > 0)
            {
                
                var front = Requests[url];
                Requests.Remove(url);;
                Console.WriteLine("REMOVING !!!!" + front.Key.Key);
                // May want to replace with custom tuple class ~ Do not use .net 4.0 ~ Unity issues....
                doService(front.Key.Key, front.Value, front.Key.Value);
            }
        }
    }

