using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using System.Net;
using System;
using System.Timers;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
 [TestFixture]
 [Category("Network Tests")]
 internal class NetworkTests
{
     DataFactory df;
     public NetworkTests()
     {
         //Debug.LogError("AFASD");
         NetworkManager nm = new NetworkManager();
         df = new DataFactory(nm);
     }
     [Test]
     [Category("Network Speed Tests")]
     public void SpeedTest()
     {
         //df.TestStringDownload("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?limit=12000");
         WebClient wc = new WebClient();
         for (int i = 0; i <= 13000; i += 100 )
             wc.DownloadString("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?offset="+i.ToString()+"&limit=100");
         Assert.Pass();
     }

     [Test]
     public void LongSpeedTest()
     {
         //df.TestStringDownload("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?limit=12000");
         WebClient wc = new WebClient();
         wc.DownloadString("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?&limit=100");
         Assert.Pass();
     }


     private class MyWebClient : WebClient
     {
         string result;
         public bool GotResult = false;
         public MyWebClient()
             : base()
         {
         }

         public string Download(string url)
         {
             Stopwatch st = new Stopwatch();
             st.Start();
             DownloadStringAsync(new Uri(url));
             while(!GotResult)
             {
                 if (st.Elapsed.TotalSeconds > 180)
                 {
                     return "";
                 }
             }
             st.Stop();
             
             return result;
         }

         protected override void OnDownloadStringCompleted(DownloadStringCompletedEventArgs args)
         {
             base.OnDownloadStringCompleted(args);
             GotResult = true;
             result = args.Result;
         }

         protected override WebRequest GetWebRequest(Uri uri)
         {
             WebRequest w = base.GetWebRequest(uri);
             w.Timeout = 30000;
             UnityEngine.Debug.LogError("HERERERE");
             return w;
         }

     }
     string unr = "http://h1.rd.unr.edu:8080";
     string unm = "http://vwp-dev.unm.edu";

     [Datapoint]
     string testunr = "http://h1.rd.unr.edu:8080" + "/apps/vwp/search/datasets.json?limit=100";
     [Datapoint]
     string testunm = "http://vwp-dev.unm.edu" + "/apps/vwp/search/datasets.json?limit=100";
     [Datapoint]
     string testunr2 = "http://h1.rd.unr.edu:8080" + "/apps/vwp/search/datasets.json?limit=10";
     [Datapoint]
     string testunm2 = "http://vwp-dev.unm.edu" + "/apps/vwp/search/datasets.json?limit=10";
     [Datapoint]
     string testunr3 = "http://h1.rd.unr.edu:8080" + "/apps/vwp/search/datasets.json?limit=1000";
     [Datapoint]
     string testunm3 = "http://vwp-dev.unm.edu" + "/apps/vwp/search/datasets.json?limit=1000";
     [Datapoint]
     string testunr4 = "http://h1.rd.unr.edu:8080" + "/apps/vwp/search/datasets.json?limit=1";
     [Datapoint]
     string testunm4 = "http://vwp-dev.unm.edu" + "/apps/vwp/search/datasets.json?limit=1";
     [Datapoint]
     string testunr5 = "http://h1.rd.unr.edu:8080" + "/apps/vwp/search/datasets.json?limit=2000";
     [Datapoint]
     string testunm5 = "http://vwp-dev.unm.edu" + "/apps/vwp/search/datasets.json?limit=2000";

     [Theory]
     public void LongSpeedTestWithJSONParsing(string url)
     {
         //df.TestStringDownload("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?limit=12000");
         Stopwatch t = new Stopwatch();
         MyWebClient wc = new MyWebClient();
         
         Logger.enable = false;
         t.Start();
      
         string test = wc.Download(url); //unr+"/apps/vwp/search/datasets.json?limit=1000");
         t.Stop();
         if(test =="")
         {
             Assert.Fail();
             return;
         }
         //wc.Dispose();
         double networktime = t.Elapsed.TotalSeconds;
         VW_JSON_Parser vw = new VW_JSON_Parser();
         t.Reset();
         t.Start();
         vw.Parse(new List<DataRecord> { new DataRecord()}, test);
         t.Stop();
         Logger.enable = true;
         Assert.Pass("DOWNLOAD TIME: " + networktime + " PARSE TIME: " + t.Elapsed.TotalSeconds);
     }

     [Test]
     public void AsyncSpeedTest()
     {
         //df.TestStringDownload("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?limit=12000");
         WebClient wc = new WebClient();
         WebClient wc2 = new WebClient();
         WebClient wc3 = new WebClient();
         WebClient wc4 = new WebClient();
         for (int i = 0; i <= 12000; i += 400)
         {
             wc.DownloadDataAsync(new System.Uri("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?offset=" + i.ToString() + "&limit=100"));
             wc2.DownloadDataAsync(new System.Uri("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?offset=" + (i+100).ToString() + "&limit=100"));
             wc3.DownloadDataAsync(new System.Uri("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?offset=" + (i+200).ToString() + "&limit=100"));
             wc4.DownloadDataAsync(new System.Uri("http://vwp-dev.unm.edu/apps/vwp/search/datasets.json?offset=" + (i+300).ToString() + "&limit=100"));
             while (wc.IsBusy || wc2.IsBusy || wc3.IsBusy || wc4.IsBusy) ;
             UnityEngine.Debug.LogError("DONE AT " + i.ToString());    
         }
         Assert.Pass();
     }

     [Test]
     public void QueueTest()
     {
         PriorityQueue<DownloadRequest> Ints = new PriorityQueue<DownloadRequest>();
         DownloadRequest dr = new DownloadRequest("apples", (ByteFunction)null);
         DownloadRequest dr2 = new DownloadRequest("apples2", (ByteFunction)null);

         Ints.Enqueue(dr);
         Ints.Enqueue(dr2);
         Assert.AreEqual("apples",Ints.Peek().Url);
     }

     [Test]
     public void QueueTest2()
     {
         PriorityQueue<DownloadRequest> Ints = new PriorityQueue<DownloadRequest>();
         DownloadRequest dr = new DownloadRequest("apples", (ByteFunction)null);
         DownloadRequest dr2 = new DownloadRequest("apples2", (ByteFunction)null,2);

         Ints.Enqueue(dr);
         Ints.Enqueue(dr2);
         Assert.AreEqual("apples2", Ints.Peek().Url);
     }
     

}
