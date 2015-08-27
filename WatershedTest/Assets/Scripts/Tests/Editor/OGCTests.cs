using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using OSGeo.GDAL;
using System.IO;
using System.Xml;
using System.Net;
namespace OGC_Tests
{
   
	[TestFixture]
	[Category("OGC Tests")]
	internal class OGCTests
	{
        OGCConnector ogc;
        public OGCTests()
        {
            Debug.LogError("AFASD");
            NetworkManager nm = new NetworkManager();
            DataFactory df = new DataFactory(nm);
            ogc = new OGCConnector(df,nm);
            nm.Subscribe(ogc);
        }

		[Test]
		public void TestTest()
		{
			Assert.That (1 == 1);
		}
		
		// The very first utm test :D.
		[Test, TestCaseSource("VectorCases")]
        public void UTMTransformationTest(Vector2 testvec,Vector2 Result)
        {
			var testvec2 = coordsystem.transformToUTM (testvec.x, testvec.y);
			double[] a = coordsystem.transformToUTMDouble (testvec.x, testvec.y);
			Assert.AreEqual (Result.x, a [0], 1);
			Assert.AreEqual (Result.y, a [1], 1);
        }
		static object[] VectorCases =
		{
			new object[] {new Vector2(-119.8152367f,39.5436008f), new Vector2(258081.4f,4380889f)},
			new object[] {new Vector2(-119.8152367f,39.5436008f-2), new Vector2(251279.8f,4158903.7f)},
			new object[] {new Vector2(-119.8152367f+2,39.5436008f),  new Vector2(429950.1f,4377420.7f)},
			new object[] {new Vector2(-119.8152367f+2,39.5436008f-2), new Vector2(427982.5f,4155490.8f)}
		};

		[Test]
		public void UTMZoneOriginAndBoundaryTest( [NUnit.Framework.Range(-180,180,36)] int bound, [NUnit.Framework.Range(-90,90,30)] float long1, [NUnit.Framework.Range(-90,90,30)]float long2)
		{
			var test = coordsystem.transformToUTMDouble(bound, long1);
			var test2 = coordsystem.transformToUTMDouble (bound, long2);
			if (bound % 6 == 3 || Mathf.Abs(long1) == Mathf.Abs(long2)) 
			{
				Assert.AreEqual (test [0], test2 [0],1.0);
			} 
			else 
			{
				Assert.AreNotEqual(test[0],test2[0]);
			}
		}

		// This test compares two methods of getting the distance between two points across zones
		// first being reprojecting the two points into the same zone
		// second being calculating the width of the zone with respect to the latitude and calculating some offsets
		// The first 2 tests will fail but they will show us how much distortion occurs beyond the utm boundary
		// The third test will fail because it is not across zones, but the same point.
		[Test]
		[TestCase(33,40,37,40)] 
		[TestCase(37,40,33,40)] 
		[TestCase(37,40,37,40)]
		public void UTMAcrossZoneComparisonOfTwoMethodsTest(float Longitude1, float Latitude1, float Longitude2, float Latitude2)
		{
			Assert.AreEqual (Latitude1, Latitude2);

			int refcordzone = coordsystem.GetZone (Latitude1, Longitude1);
			int othercordzone = coordsystem.GetZone (Latitude2, Longitude2);
			coordsystem.localzone = refcordzone;
			// Transformed based on local zone
			var result = coordsystem.transformToUTM (Longitude1, Latitude1);
			var result2 = coordsystem.transformToUTM (Longitude2, Latitude2);

			// Not Transformed based on local zone, but the utms actual zone
			var result3 = coordsystem.transformToUTMDouble (Longitude1, Latitude1);
			var result4 = coordsystem.transformToUTMDouble (Longitude2, Latitude2);


			Assert.AreNotEqual (refcordzone, othercordzone);

			double origin1 = GetUtmZoneOrigin ((int)Longitude1,(int) Latitude1);
			double origin2 = GetUtmZoneOrigin ((int)Longitude2, (int)Latitude2);
			double zone1width = GetUtmZoneHalfWidth ((int)Longitude1, (int)Latitude1);
			double zone2width = GetUtmZoneHalfWidth ((int)Longitude2, (int)Latitude2);
			Debug.LogError (zone1width);
			Debug.LogError (zone2width);
			Debug.LogError (origin1);
			Debug.LogError (origin2);

			// Longitude1 < Longitude2
			if (refcordzone < othercordzone) {
				double offset = origin1 + zone1width - result3 [0];
				offset += (result4 [0] - (origin2 - zone2width));
				Assert.AreEqual (result2.x - result.x, offset);
			} 

			// Longitude1 > Longitude2
			else 
			{
				double offset = System.Math.Abs(origin1 - zone1width - result3 [0]);
				offset += System.Math.Abs(result4 [0] - (origin2 + zone2width));
				Assert.AreEqual (result.x - result2.x, offset);
			}
			//double offset1fromcenterx = zone1width

		}


		double GetUtmZoneOrigin(int Longitude, int Latitude)
		{
			Longitude = GetUTMMerridian (Longitude);
			//Debug.LogError ("UTM MERRIDIAN: " + Longitude);
			var Out2 = coordsystem.transformToUTMDouble(Longitude,Latitude);
			return Out2[0];
		}

		double GetUtmZoneHalfWidth(int Longitude, int Latitude)
		{
			double origin = GetUtmZoneOrigin (Longitude, Latitude);
			int Side = GetUTMMerridian (Longitude) + 3;
			var End = coordsystem.transformToUTMDouble (Side, Latitude);
			return System.Math.Abs(End [0] - origin);
		}
		int GetUTMMerridian(int Longitude)
		{
			int remainder = Longitude % 6;
			Longitude = Longitude - remainder + 3;
			return Longitude;
		}


        [Datapoint]
        public string randomlink = "http://www.ncddc.noaa.gov/arcgis/services/DataAtlas/CMECS_Salinity_Summer/MapServer/WCSServer?request=GetCapabilities&version=1.1.2&service=WCS";

        [Datapoint]
        public string randomlink2 = "http://vwp-dev.unm.edu/apps/vwp/datasets/ebce3b6e-85b5-4c63-8388-21dc71e942ee/services/ogc/wcs?SERVICE=wcs&REQUEST=GetCapabilities&VERSION=1.1.2";

        [Datapoint]
        public string randomlink3 = "http://vwp-dev.unm.edu/apps/vwp/datasets/05d8bd21-105e-4d0a-8e63-fe4f148cde01/services/ogc/wcs?SERVICE=wcs&REQUEST=GetCapabilities&VERSION=1.1.2";

        // Not really magic just us testing whether we can take a single link of a random ogc service and see it works
        [Theory, Timeout(120000)]
        [MaxTime(120000)]
        public void MagicTest(string url)
        {
            Assert.That(ogc.MagicFunction(url));
        }

        [Test]
        public void WMSTestGDAL()
        {
            //System.Console.WriteLine("GDAL DATA ORIG: " + System.Environment.GetEnvironmentVariable("GDAL_DATA"));
            Gdal.SetConfigOption("GDAL_DATA", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+@"\..\..\data\");
            Gdal.AllRegister();
            System.Environment.SetEnvironmentVariable("GDAL_DATA", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\data\");
            //System.Console.WriteLine("GDAL DATA: " + System.Environment.GetEnvironmentVariable("GDAL_DATA"));
            //Gdal.SetConfigOption("gdal_data", @"C:\Users\ccarthen\Downloads\release-1800-x64-gdal-mapserver-src\gdal\data\");
            //string test = Gdal.GetConfigOption("GDAL_DATA", @"C:\Users\ccarthen\Downloads\release-1800-x64-gdal-mapserver-src\gdal\data\");
            //System.Console.WriteLine("TEST: " + test);
            /*var ds = Gdal.Open("WMS:http://vwp-dev.unm.edu/apps/vwp/datasets/189c7ae0-de72-4a43-9f1c-9b15a0f9064a/services/ogc/wms?", Access.GA_ReadOnly);
            var sds = ds.GetMetadata("SUBDATASETS");
            foreach(var i in sds)
            {
                Debug.LogError(i);
            }*/
            int count = 0;
            RasterDataset rd = new RasterDataset("WMS:http://vwp-dev.unm.edu/apps/vwp/datasets/0afd433c-846b-475d-9bd2-57193e16b40a/services/ogc/wms?");
            
            if (rd.Open())
            {
                System.Console.Write(rd.HasSubDatasets(out count));
                System.Console.WriteLine(count);
                var sd = rd.GetSubDatasets();
                if (sd != null && sd.Count != 0)
                {
                    rd = new RasterDataset(sd[1]);
                    if (rd.Open())
                    {
                        System.Console.WriteLine(rd.GetData().Count);
                    }
                }
            }
           
           // System.Console.WriteLine(sds[0]);
            //System.Console.WriteLine(sds.Length);
            //var ds2 = Gdal.Open("WMS:http://vwp-dev.unm.edu/apps/vwp/datasets/0e0812b4-2179-4d4e-bbb0-50a19cbafbb6/services/ogc/wms?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&LAYERS=Thick_Creek&SRS=EPSG:4326&BBOX=-120.01,31.17,-102.66,49.13", Access.GA_ReadOnly);
            //var sr = new OSGeo.OSR.SpatialReference("");
           // sr.ImportFromEPSG(4326);
           // System.Console.WriteLine(ds2.RasterCount);
           // var b = ds2.GetRasterBand(1);
          //  int[] ints = new int[2*2];
           // b.ReadRaster(0, 0, 100, 100, ints, 2, 2, 0, 0);
          //  System.Console.WriteLine(ints[0]);
        }

        [Test]
        public void WFSTestGDAL()
        {
            
            Gdal.AllRegister();
            var ds = Gdal.Open("WFS:http://vwp-dev.unm.edu/apps/vwp/datasets/ce691ff6-da2a-46b9-a7ab-acb8bf94738d/services/ogc/wfs?SERVICE=WFS", Access.GA_ReadOnly);
        }

        [Test]
        public void WCSTestGDAL()
        {
            Gdal.AllRegister();
            // XML Code to go here.
            string xml = @"<WCS_GDAL><ServiceURL>http://vwp-dev.unm.edu/apps/vwp/datasets/0e0812b4-2179-4d4e-bbb0-50a19cbafbb6/services/ogc/wcs?</ServiceURL><CoverageName>Thick_Creek</CoverageName></WCS_GDAL>";
            var t = Gdal.Open(xml, Access.GA_ReadOnly);
            System.Console.WriteLine(t != null);
            System.Console.WriteLine(t.RasterCount);
            
        }

        void datatoImage(string fname, float[,] data)
        {
               
            Texture2D ab = new Texture2D(data.GetLength(0), data.GetLength(1));
            Color[] cs = new Color[data.GetLength(0) * data.GetLength(1)];
            float max = 0;
            float min = float.MaxValue;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if(data[i,j] < 0)
                    {
                        data[i, j] = 0;
                    }
                    min = Mathf.Min(min, data[i, j]);
                    max = Mathf.Max(max, data[i, j]);
                }
            }
            System.Console.WriteLine(min + " " + max  + " " + float.MaxValue);
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    float val = (data[i, j] - min) / (max - min);
                    
                    if(val<.01)
                    cs[i * data.GetLength(1) + j] = Color.Lerp(Color.red, Color.blue, val);
                    else if (val < .4)
                        cs[i * data.GetLength(1) + j] = Color.Lerp(Color.blue, Color.black, val);
                    else if(val < .6)
                        cs[i * data.GetLength(1) + j] = Color.Lerp(Color.black, Color.gray, val);
                    else if (val < .8)
                        cs[i * data.GetLength(1) + j] = Color.Lerp(Color.gray, Color.green, val);
                    else if (val < 1)
                        cs[i * data.GetLength(1) + j] = Color.Lerp(Color.gray, Color.yellow, val);
                }
            }
            ab.SetPixels(cs);
            ab.Apply();
            var bytes = ab.EncodeToJPG();

            FileStream fs = File.Create(fname);

            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            System.Console.WriteLine(Path.GetFullPath("."));
        }

        [Test]
        public void DownloadAllBandsWithParser()
        {
            System.Console.WriteLine("HERERERERE");
            System.Net.WebClient wc = new System.Net.WebClient();
            var data = wc.DownloadData("http://vwp-dev.unm.edu/apps/vwp/datasets/5f080b22-1c7d-4121-b7bf-5021e14025a7/services/ogc/wcs?request=GetCoverage&service=WCS&version=1.1.2&Identifier=downwelling_shortwave_flux_in_air&InterpolationType=bilinear&format=image/bil&store=false&GridBaseCRS=urn:ogc:def:crs:epsg::4326&CRS=EPSG:4326&bbox=-116.142921741559,43.7293760210743,-116.137597499034,43.7327467931015&width=100&height=100");
            mimeparser mp = new mimeparser();
            System.Console.WriteLine(data.Length);
            string header = "";
            byte[] bytes = new byte[1];
            mp.parseBIL(data, ref header, ref bytes);
            var d =  bilreader.parse(header, data);
            int counter = 0;
            foreach (var i in d)
            {
                datatoImage("./out" + counter + ".png", i);
                counter++;
            }
            Debug.LogError(header);
        }

        [Test]
        public void WCSTest2GDAL()
        {
            Gdal.AllRegister();
            string xml = @"<WCS_GDAL><ServiceURL>http://vwp-dev.unm.edu/apps/vwp/datasets/5f080b22-1c7d-4121-b7bf-5021e14025a7/services/ogc/wcs?</ServiceURL>
<CoverageName>downwelling_longwave_flux_in_air</CoverageName><Version>1.1.0</Version><GridBaseCRS>EPSG:4326</GridBaseCRS></WCS_GDAL>";
            RasterDataset rd = new RasterDataset(xml);

            if (rd.Open())
                Assert.Pass();
            else
                Assert.Fail();
            //var data = rd.GetData()[0];
            //var sw = new System.IO.StreamWriter("./happy.txt");
            //Texture2D ab = new Texture2D(data.GetLength(0), data.GetLength(1));
            //Color[] cs = new Color[data.GetLength(0) * data.GetLength(1)];
            //float max = float.MinValue;
            //float min = float.MaxValue;
            //for(int i = 0; i< data.GetLength(0); i++)
            //{
            //    for(int j =0; j < data.GetLength(1); j++)
            //    {
            //        sw.Write(data[i, j] + ",");
            //        min = Mathf.Min(min, data[i, j]);
            //        max = Mathf.Max(max, data[i, j]);
            //    }
            //    sw.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            //}

            //for (int i = 0; i < data.GetLength(0); i++)
            //{
            //    for (int j = 0; j < data.GetLength(1); j++)
            //    {
            //        float val = (data[i, j] - min) / (max - min);
            //        cs[i * data.GetLength(1)+j] = Color.Lerp(Color.red, Color.blue, val);
            //    }
            //    sw.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            //}
            //ab.SetPixels(cs);
            //ab.Apply();
            //var bytes = ab.EncodeToJPG();
       
            //sw.Close();
            //FileStream fs = File.Create("./test3.jpg");
            
            //fs.Write(bytes,0,bytes.Length);
            //fs.Close();
            //System.Console.WriteLine(Path.GetFullPath("."));
            
        }

        [Test]
        public void NetCDFTest()
        {
            var nc = Gdal.GetDriverByName("NETCDF");
            if(nc != null)
            Debug.LogError(nc.LongName);
            string file = @"NETCDF:"+'"'+ @"C:\Users\ccarthen\Downloads\twoweek_inputs_with_zlib.nc" + '"';
            RasterDataset rd = new RasterDataset(file);
            if(rd.Open())
            {
                System.Console.WriteLine("OPENED");
                var s = rd.GetSubDatasets();
                rd = new RasterDataset(s[0]);
                if(rd.Open())
                {
                    System.Console.WriteLine("OPENED 2" + s[0]);
                    System.Console.WriteLine(rd.GetData().Count);
                }
            }
        }

        [Test]
        public void XMLTest()
        {
            XmlDocument doc = new XmlDocument();
            var rootNode = doc.CreateElement("WCS_GDAL");
            doc.AppendChild(rootNode);

            XmlNode userNode = doc.CreateElement("user");
            XmlAttribute attribute = doc.CreateAttribute("age");
            attribute.Value = "42";
            userNode.Attributes.Append(attribute);
            userNode.InnerText = "John Doe";
            rootNode.AppendChild(userNode);


            //doc.CreateNode("WCS_GDAL","","");
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                System.Console.WriteLine(stringWriter.GetStringBuilder().ToString());
            }
        }

        [Test]
        public void XMLPARSE()
        {
            WebClient wc = new WebClient();
            string s = wc.DownloadString("http://vwp-dev.unm.edu/apps/vwp/datasets/0afd433c-846b-475d-9bd2-57193e16b40a/services/ogc/wcs?request=DescribeCoverage&service=WCS&version=1.1.2&identifiers=soil_temperature&");
            WCS_DescribeCoverage_Parser parser = new WCS_DescribeCoverage_Parser();
            parser.parseDescribeCoverage(new DataRecord(), s);
            //Assert.Pass();
        }

	}
}