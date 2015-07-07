using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
//using System.Threading;

namespace NetworkTest
{
    class Program
    {
        static String MimeUrlOne = "url://http://129.24.63.65//apps/my_app/datasets/2d78540a-fdea-4dc3-bf68-c9508bb1166f/services/ogc/wcs?request=GetCoverage&service=WCS&version=1.1.2&Identifier=DCEsqrExtent_epsg_4326_new&InterpolationType=NEAREST_NEIGHBOUR&format=image/bil&store=false&GridBaseCRS=urn:ogc:def:crs:epsg::4326&CRS=EPSG:4326&bbox=-116.22783700393,43.6501960099275,-116.015090235342,43.770631547369&width=1025&height=1025";
        static String MimeUrlTwo = "url://http://129.24.63.65//apps/my_app/datasets/794324ba-22c4-4847-8e0d-277a8d64cf75/services/ogc/wcs?request=GetCoverage&service=WCS&version=1.1.2&Identifier=n39w115&InterpolationType=NEAREST_NEIGHBOUR&format=image/bil&store=false&GridBaseCRS=urn:ogc:def:crs:epsg::4326&CRS=EPSG:4326&bbox=-115.001666666667,37.9983333333333,-113.998333333333,39.0016666666674&width=1025&height=1025";
        static String TestFileOne = "file://C:/Users/hpcvi_000/Desktop/NetworkingTest/NetworkingTest/NetworkingTest/NetworkTest/NetworkTest/bin/Debug/test.hdr";
        static String TestPNG = "url://http://129.24.63.65//apps/my_app//datasets/2d78540a-fdea-4dc3-bf68-c9508bb1166f/services/ogc/wms?SERVICE=wms&Request=GetMap&width=512&height=512&layers=DCEsqrExtent_epsg_4326_new&bbox=-116.227837004,43.6501960099,-116.015090235,43.7706315474&format=image/png&Version=1.1.1&srs=epsg:4326";
        static String WCSCapabilitiesS = "url://http://129.24.63.65//apps/my_app/datasets/48cdb02d-7df8-4149-a6cb-16706e696b8c/services/ogc/wcs?SERVICE=wcs&REQUEST=GetCapabilities&VERSION=1.1.2";
        static String WMSCapabilitiesS = "url://http://129.24.63.65//apps/my_app/datasets/712c4319-fb36-4e87-b670-90aac2f5e133/services/ogc/wms?SERVICE=wms&REQUEST=GetCapabilities&VERSION=1.1.1";
        static String WFSCapabilitiesS = "url://http://129.24.63.65//apps/my_app/datasets/712c4319-fb36-4e87-b670-90aac2f5e133/services/ogc/wfs?SERVICE=wfs&REQUEST=GetCapabilities&VERSION=1.0.0";
        static String WCSDescribeCoverageS = "url://http://129.24.63.65//apps/my_app/datasets/b51ce262-ee85-4910-ad2b-dcce0e5b2de7/services/ogc/wcs?request=DescribeCoverage&service=WCS&version=1.1.2&identifiers=output_srtm&";
        static String VWPString = "http://vwp-dev.unm.edu/";
        static VWClient vwc;
        static DataObserver obs;
        //static ModelRunManager ModelRunManager;
        static Simulator simu;
        static void Recieved(List<string> message)
        {
            Logger.WriteLine("This function has recieved: " + message.Count);
            if(message.Count > 0)
            Logger.WriteLine("THIS FUNCTION RECIEVED FOR FIRST: " + message[0]);
        }

        static void Main(string[] args)
        {
            /*Console.WriteLine("BLAH BLAH");
            //simu = new Simulator();
            //simu.Simulation(.1f);
            //Console.ReadKey();
            //return;

            Logger.WriteToFile();
            Logger.enable = true;
            //FileBasedCache.Clear();
            NetworkManager nm = new NetworkManager();
            //obs = new DataObserver();
            vwc = new VWClient(new DataFactory(nm), nm);
            //ModelRunManager = new ModelRunManager(vwc);
            ModelRunManager.client = vwc;
            ModelRunManager.Start();
            nm.Subscribe(vwc);
			ModelRunManager.SearchForModelRuns();

			SystemParameters testParameters = new SystemParameters ();
			testParameters.model_run_uuid = "80661ff3-2d25-4ae3-867b-74896b97d3c6";
			testParameters.model_set_type = "";
			ModelRunManager.getAvailable(testParameters,null,DoNothing);

			Logger.WriteLine ("Searching");
			Thread.Sleep (5000);*/

            //Logger.ReadKey();
			string Str = new StreamReader("/Users/appleseed/Desktop/wcsdescribecoverage.xml").ReadToEnd();
			var reader = System.Xml.XmlTextReader.Create(new System.IO.StringReader(Str));

			XmlSerializer serial = new XmlSerializer(typeof(DescribeCoverageWCS.CoverageDescriptions));
			DescribeCoverageWCS.CoverageDescriptions testc = new DescribeCoverageWCS.CoverageDescriptions();

			if (serial.CanDeserialize(reader))
			{

				testc = (DescribeCoverageWCS.CoverageDescriptions)serial.Deserialize(reader);
				/*foreach (var i in testc.CoverageDescription.Range.Field.Axis.AvailableKeys.Key) 
				{
					Logger.WriteLine (i);
				}*/
				//Logger.WriteLine (testc.CoverageDescription.Range.Field.Axis.AvailableKeys.Key.Count().ToString());
			}
			Logger.ReadKey();
        }

		public static void DoNothing(List<DataRecord>records)
		{
			Logger.WriteLine("Doing Absolutely Nothing HERE" + records.Count);
			if (records.Count > 0) {
				//SystemParameters sp = new SystemParameters ();
				//vwc.getCapabilities (null, records [0], sp);
			}
		}

        public static int i = 1;
        public static List<DataRecord> recs;
        public static void DummyMethod(List<DataRecord> result)
        {
            if (i == 1)
            {
                recs = result; i++;
                //result[0]
                foo("INDEX", result);
                //vwc.getMap(GetMap, result[2]);
            }
            else
            {
                foo("INDEX", result);
            }
        }
        
        static void PrintDataRecords(List<DataRecord> Records)
        {
            List<DataRecord> RecordsList = new List<DataRecord>();
            foreach (var i in Records)
            {
                Logger.WriteLine("NAME: " + i.name);
                RecordsList.Add(i);

                List<DataRecord> temp = new List<DataRecord>();
                temp.Add(i);
                vwc.GetMetaData(PrintMetaData, temp);
            }
            
            // ================================================
            // TEST FOR DOWNLOAD VS CACHED DATA_RECORD EQUALITY
            // ================================================
            try
            {
                List<DataRecord> cachedRecords = FileBasedCache.Get<List<DataRecord>>("RECORD");

                for (int i = 0; i < Math.Min(cachedRecords.Count,RecordsList.Count); i++)
                {
                    if (RecordsList[i] == cachedRecords[i])
                    {
                        Logger.Log("Record " + i + " is equal!");
                    }
                    else
                    {
                        Logger.Log("Record " + i + " is NOT equal!");
                    }
                }
                //FileBasedCache.Clear();
                Logger.WriteLine("CACHE CLEARED");
            }
            catch(Exception e)
            {
                Logger.Log(e.Message);
            }

            // if all is well this should work
            foreach(var i in RecordsList)
            {
               // vwc.getMap(GetMap, i,type:DownloadType.File,OutputPath: "./", OutputName: i.name);
                //vwc.getCoverage(GetCoverage, i);
               // vwc.getFeatures(GetFeature, i);
            }

            FileBasedCache.Insert<List<DataRecord>>("RECORD", RecordsList);
            Logger.WriteLine("RECORDS LOADED TO CACHE");

            // ================================================
            // END OF TEST
            // ================================================

        }

        static void PrintMetaData(List<DataRecord> Records)
        {
            Logger.WriteLine("METADATA");
            foreach (var i in Records)
            {
                Logger.WriteLine(i.metaData.ToString());
            }

        }


        static void GetCoverage(List<DataRecord> Records)
        {
            foreach(var i in Records)
            {
                if (i.Data != null)
                    Logger.Log(i.Data.ToString());
                else
                    Logger.Log(i.name +" is null " + i.Data);
            }
        }

        static void GetMap(List<DataRecord> Records)
        {
            foreach (var i in Records)
            {
                if(i.texture != null)
                Logger.Log(i.texture.ToString());
            }
        }

        static void GetFeature(List<DataRecord> Records)
        {
            Logger.Log("THE OBSERVERS");
            foreach (var i in Records)
            {
                Logger.Log("GetFeature: " + i.Lines.ToString());
            }
        }

        static void download()
        {  
            Logger.WriteLine("Hello from another thread");
        }

        public static void foo(string index, List<DataRecord> RecordsList)
        {
            if(! FileBasedCache.Exists(index))
            {
                FileBasedCache.Insert<List<DataRecord>>(index, RecordsList);
                return;
            }

            try
            {
                List<DataRecord> cachedRecords = FileBasedCache.Get<List<DataRecord>>(index);
                
                for (int i = 0; i < Math.Min(cachedRecords.Count, RecordsList.Count); i++)
                {
                    if (RecordsList[i] == cachedRecords[i])
                    {
                        Logger.Log("Record " + i + " is equal!");
                    }
                    else
                    {
                        Logger.Log("Record " + i + " is NOT equal!");
                        

                        // Decide which one is more up to data OR has more records
                        RecordsList.Clear();
                        RecordsList.AddRange(cachedRecords);
                        tfunctflag = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
            }
        }

        public static bool tfunctflag = false;

        public static void tFunct(List<DataRecord> recs)
        {
            while(tfunctflag == false)
            {
                //Logger.WriteLine("LINE");
            }
            if(recs != null)
            foreach(var i in recs)
            {
                Logger.WriteLine(i.texture.ToString());
            }
            Logger.WriteLine("COMPLETENESSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS!");
        }
    }
}
