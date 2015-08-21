using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using OSGeo.GDAL;
using System.IO;
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
            Gdal.SetConfigOption("GDAL_DATA", @"C:\Users\ccarthen\Downloads\release-1800-x64-gdal-mapserver-src\gdal\data\");
            Gdal.AllRegister();
            //System.Environment.SetEnvironmentVariable("GDAL_DATA", @"C:\Users\ccarthen\Downloads\release-1800-x64-gdal-mapserver-src\gdal\data\");
            //System.Console.WriteLine("GDAL DATA: " + System.Environment.GetEnvironmentVariable("GDAL_DATA"));
            Gdal.SetConfigOption("gdal_data", @"C:\Users\ccarthen\Downloads\release-1800-x64-gdal-mapserver-src\gdal\data\");
            //string test = Gdal.GetConfigOption("GDAL_DATA", @"C:\Users\ccarthen\Downloads\release-1800-x64-gdal-mapserver-src\gdal\data");
            //System.Console.WriteLine("TEST: " + test);
            //var ds = Gdal.Open("WMS:http://vwp-dev.unm.edu/apps/vwp/datasets/0e0812b4-2179-4d4e-bbb0-50a19cbafbb6/services/ogc/wms?", Access.GA_ReadOnly);
            //var sds = ds.GetMetadata("SUBDATASETS");
            
           // System.Console.WriteLine(sds[0]);
            //System.Console.WriteLine(sds.Length);
            var ds2 = Gdal.Open("WMS:http://vwp-dev.unm.edu/apps/vwp/datasets/0e0812b4-2179-4d4e-bbb0-50a19cbafbb6/services/ogc/wms?SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&LAYERS=Thick_Creek&SRS=EPSG:4326&BBOX=-120.01,31.17,-102.66,49.13", Access.GA_ReadOnly);
            var sr = new OSGeo.OSR.SpatialReference("");
            sr.ImportFromEPSG(4326);
            System.Console.WriteLine(ds2.RasterCount);
            var b = ds2.GetRasterBand(1);
            int[] ints = new int[2*2];
            b.ReadRaster(0, 0, 100, 100, ints, 2, 2, 0, 0);
            System.Console.WriteLine(ints[0]);
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
        }

	}
}