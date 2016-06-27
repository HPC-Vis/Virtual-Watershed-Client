using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using OSGeo.GDAL;
using System.IO;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;
using ProjNet;
using ProjNet.CoordinateSystems;
using Gavaghan.Geodesy;
using ASA.NetCDF4;

namespace CoordinateSystemTests
{
    [TestFixture]
    [Category("Coordinate System Tests")]
    internal class CoordinateSystemTests
    {
        [Test]
        public void CoordinateTest()
        {
            coordsystem.CS = new UTMCoordinateSystem();
            float longitude = -120;
            float latitude = 33;

            // Set the world origin
            // To be filled in

            //
            Debug.LogError(coordsystem.transformToUnity(new Vector2(longitude, latitude)) ) ;
        }

        // Testing two coordinates that span across two zones.
        [Test]
        [TestCase(33, 40, 33, -40)]
        [TestCase(33, 1, 33, -1)]
        [TestCase(33, 1, 33, 2)]
        [TestCase(33, 1, 33, 1.2f)]
        public void HaversineCoordinateTest(float long1, float latitude, float long2, float latitude2)
        {

            var CS = new UTMCoordinateSystem(CoordinateUtils.GetZone(latitude, long1));

            CS.WorldOrigin = new Vector2(long1, latitude);

            double haverdistance = CoordinateUtils.GetDistanceKM(long1, latitude, long2, latitude2) * 1000.0;

            var UnityPoint = CS.TranslateToUnity(new Vector2(long2, latitude2));
            Debug.LogError(Vector2.Distance(CS.UnityOrigin, UnityPoint) + " " + haverdistance);
            Assert.AreEqual(Vector2.Distance(CS.UnityOrigin, UnityPoint), (float)haverdistance, 500);
        }

        // Testing two coordinates that span across two zones.
        [Test]
        [TestCase(33, 40, 33, -40)]
        [TestCase(33, 1, 33, -1)]
        [TestCase(33, 1, 33, 2)]
        [TestCase(33, 1, 33, 1.2f)]
        public void VincentyCoordinateTest(float long1, float latitude, float long2, float latitude2)
        {

            var CS = new UTMCoordinateSystem(CoordinateUtils.GetZone(latitude, long1));

            CS.WorldOrigin = new Vector2(long1, latitude);

            double distance = CoordinateUtils.VincentyDistanceKM(long1, latitude, long2, latitude2) * 1000.0;

            var UnityPoint = CS.TranslateToUnity(new Vector2(long2, latitude2));
            Debug.LogError(Vector2.Distance(CS.UnityOrigin, UnityPoint) + " " + distance);
            Assert.AreEqual(Vector2.Distance(CS.UnityOrigin, UnityPoint), (float)distance);
        }


        [Test()]
        public void NetCDFWriteTest()
        {
            NcFile file;
            NcDim time = null;
            NcVar timeVar = null;
            //if (System.IO.File.Exists("test.nc"))
            //    System.IO.File.Delete("test.nc");
            //file = new NcFile("test.nc", NcFileMode.newFile);
            //else
            //file = new NcFile("test.nc", NcFileMode.write);
            double[] readBuffer = new double[11];
            for (int i = 0; i < 11; i++)
            {
                readBuffer[i] = 0;
                Debug.LogError("I: " + i);
            }
            //double[] writeBuffer = new double[] { 0, 2, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            //time = file.AddDim("time");
            //timeVar = file.AddVar("t", NcFloat.Instance, time);
            
            //timeVar.PutVar(new int[] { 0 }, new int[] { 11 }, writeBuffer);
            //timeVar.GetVar(new int[] { 1 },readBuffer);
            //for (int i = 0; i < 1; i++)
            //    Assert.AreEqual(readBuffer[i], (int)2);

            //file.Close();
            file = new NcFile(@"C:\Users\ccarthen\Downloads\animation.nc", NcFileMode.read);
            foreach(var i in file.GetVars())
            {
                Debug.LogError("VAR: " + i.Key);
            }
            timeVar = file.GetVar("sroff");
            Debug.LogError(timeVar.Shape.Length);
            Debug.LogError("WHY YOU FAIL");
            timeVar.GetVar(new int[] { 1,1,1 }, readBuffer);
            for (int i = 0; i < 1; i++)
            {
                //Assert.AreEqual(readBuffer[i], (int)2);
                Debug.LogError(readBuffer[i]);
            }
            Debug.LogError("SUCCESS");

        }

        // http://stackoverflow.com/questions/3225803/calculate-endpoint-given-distance-bearing-starting-point
        [Test]
        [TestCase(33, 40, 33, -40)]
        [TestCase(33, 1, 33, -1)]
        [TestCase(33, 1, 33, 6)]
        [TestCase(33, 1, 33, 1.2f)]
        public void TrueCourseTest(float long1,float lat1, float long2, float lat2)
        {
            Vector2 one = new Vector2(long1, lat1);
            Vector2 two = new Vector2(long2, lat2);
            Vector2 heading = two - one;
            heading.Normalize();
            float angle = Vector2.Angle(heading, Vector2.up);
            if (heading.y < 0)
            {
                //Debug.LogError("HERE");
                angle = -angle;
            }
            //angle *= Mathf.Deg2Rad;
            //Debug.LogError(angle);
            float haverdistance = (float)CoordinateUtils.GetDistanceKM(long1, lat1, long2, lat2) * 1000.0f;
            //Debug.LogError(haverdistance);
            //Debug.LogError(angle);
            Debug.LogError(CoordinateUtils.CalculateProjectedPointHaversine(one, angle, haverdistance));

             //Debug.LogError(long2 + " " + lat2);
            
            
        }

        [Test]
        [TestCase(33, 40, 33, -40)]
        [TestCase(33, 1, 33, -1)]
        [TestCase(33, 1, 33, 6)]
        [TestCase(33, 1, 33, 1.2f)]
        public void TrueCourseTest2(float long1, float lat1, float long2, float lat2)
        {
            // Calaculate bearing
            Vector2 one = new Vector2(long1, lat1);
            Vector2 two = new Vector2(long2, lat2);
            Vector2 heading = two - one;
            heading.Normalize();
            float angle = Vector2.Angle(heading, Vector2.up);
            if (heading.y < 0)
            {
                //Debug.LogError("HERE");
                angle = -angle;
            }

            // Using haverdistance for calculation           
            float haverdistance = (float)CoordinateUtils.GetDistanceKM(long1, lat1, long2, lat2)*1000.0f;

            float distance = (float)CoordinateUtils.VincentyDistanceKM(long1, lat1, long2, lat2) * 1000.0f;

            Debug.LogError(CoordinateUtils.CalculateProjectedPointVincenty(one, angle, haverdistance));
            Debug.LogError(CoordinateUtils.CalculateProjectedPointVincenty(one, angle, distance));
            Debug.LogError(distance);
            Debug.LogError(haverdistance);
        }


        // Incorporate a speed test.
        

        [Test]
        [TestCase(0, 0, 0, 1)]
        [TestCase(0, 0, 1, 0)]
        [TestCase(40, 40, 50, 50)]
        [TestCase(0, 0, 0, -1)]
        [TestCase(0, 0, -1, 0)]
        [TestCase(0, 0, -1, -1)]
        public void AngleTest(float long1, float lat1, float long2, float lat2)
        {
            Vector2 one = new Vector2(long1, lat1);
            Vector2 two = new Vector2(long2, lat2);
            Vector2 heading = two - one;
            heading.Normalize();
            float angle = Vector2.Angle(heading, Vector2.up);
            if(heading.y < 0)
            {
                angle += 180.0f;
            }
            angle *= Mathf.Deg2Rad;
            Debug.LogError(angle);
        }

        [TestCase(30, 40, 37, 41)]
        public void UTMTest(float Long1, float Lat1, float Long2, float Lat2)
        {
            Vector2 a = new Vector2(Long1, Lat1);
            Vector2 b = new Vector2(Long2, Lat2);

            Vector2 direction = (b-a).normalized;

            if (a.x == 0)
            {
                // Horizontal line
            }
            else
            {
                float slope = a.y / a.x;

                float projectedLat = slope * (36-b.x) + b.y;
                Debug.LogError(projectedLat);

                float distance = Vector2.Distance(CoordinateUtils.transformToUTMWithZone(36, projectedLat, CoordinateUtils.GetZone(a.y, a.x)), CoordinateUtils.transformToUTM(a.x, a.y));
                distance += Vector2.Distance( CoordinateUtils.transformToUTM(b.x, b.y),CoordinateUtils.transformToUTM(36,projectedLat)) ;


                Debug.LogError(CoordinateUtils.GetDistanceKM(Long1, Lat1, Long2, Lat2) * 1000);
                Debug.LogError(distance);
            }
            
            // Get Distance
            //float distance = a
        }
        float ScaleFactor(float value)
        {
            Debug.LogError( "SCALE: " + Mathf.Cos((value - 500000) / 6378000));
            return .9996f / Mathf.Cos( (value-500000) / 6378000); 
        }

        float SimponsRule(float Long1, float Lat1, float Long2, float Lat2, int zone)
        {
            var point1 = CoordinateUtils.transformToUTMWithZone((float)Long1, (float)Lat1, zone);
            var point2 = CoordinateUtils.transformToUTMWithZone((float)Long2, (float)Lat2, zone);
            double XDist6 = (point2.x - point1.x) / 6;
            float simpson = (float)XDist6 * (ScaleFactor(point1.x) + 4 * ScaleFactor((float)XDist6) + ScaleFactor(point2.x));
            float ydist = point2.y - point1.y;
            float Dist2 = Mathf.Sqrt(simpson * simpson + ydist * ydist);
            return Dist2;
        }

        [TestCase(35, 40, 37, 41)]
        public void DistanceTest(double Long1, double Lat1, double Long2, double Lat2)
        {
            double Dist = CoordinateUtils.GetDistanceKM(Long1, Lat1, Long2, Lat2) * 1000.0;
            double Dist3 = CoordinateUtils.VincentyDistanceKM(Long1, Lat1, Long2, Lat2) * 1000.0;
            int zone = CoordinateUtils.GetZone(Lat1, Long1);
            int zone2 = CoordinateUtils.GetZone(Lat2, Long2);
            Debug.LogError(zone - zone2);
            var point1 = CoordinateUtils.transformToUTMWithZone((float)Long1, (float)Lat1, zone);
            var point2 = CoordinateUtils.transformToUTMWithZone((float)Long2, (float)Lat2, zone);

            double XDist6 = (point2.x - point1.x) / 6;
            float simpson = (float)XDist6 * (ScaleFactor(point1.x) + 4 * ScaleFactor((float)XDist6) + ScaleFactor(point2.x));
            float ydist = point2.y - point1.y;
            float Dist2 = Mathf.Sqrt(simpson * simpson + ydist * ydist);
            Debug.ClearDeveloperConsole();
            Debug.LogError(Dist);
            Debug.LogError(Dist2);
            Debug.LogError(Dist3);
            Debug.LogError(Dist - Dist2);
            Debug.LogError(Dist2 - Dist3);
            Debug.LogError(Vector2.Distance(point1, point2));

        }

        [TestCase("test.tif")]
        public void SaveTifTest(string filename)
        {
            DataRecord test = new DataRecord();
            test.projection = "epsg:4326";
            test.boundingBox = new Rect(33, 33, 10, 10);
            var data = new float[100, 100];
            for(int i = 0; i < 100; i++)
            {
                for(int j =0; j < 100; j++)
                {
                    data[i, j] = i + j;
                }
            }
            test.Data.Add(data);
            Utilities.SaveTif(filename, test);


            //Gdal.ReprojectImage()
        }
        [Test]
        public void TestGC()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            Debug.LogError(System.GC.GetTotalMemory(false));
        }
        [TestCase(@"C:\Users\ccarthen\Downloads\animation.nc")]
        public void LoadNetCDF2(string filename)
        {
            NcFile file = new NcFile(filename, NcFileMode.read);
            
            Debug.LogError("FILENESS");
            bool once = true;
            var nothing = 1;
            Debug.LogError(nothing);
            foreach( var i in file.GetAtts())
            {
                Debug.LogError(i.Value.GetName());
            }

            foreach( var i in file.GetDims())
            {
                Debug.LogError(i.Value.GetName());
                Debug.LogError("DIM SIZE: " + i.Value.GetSize());
            }
            
            var vars = file.GetVars();
            Debug.LogError("--------");
            foreach(var i in vars)
            {
                Debug.LogError(i.Value.GetName());
                Debug.LogError(i.Value.GetNcType().GetTypeClassName());
                Debug.LogError(i.Value.Shape.Length);
                Debug.LogError(i.Value.Rank);
                if(i.Value.GetNcType().GetTypeClass() == NcTypeEnum.NC_CHAR )
                {
                    //byte[] bytes = new byte[i.Value.Shape[0]];
                    foreach(var j in i.Value.GetAtts())
                    {
                        Debug.LogError(j.Value.GetName());
                        Debug.LogError(j.Value.GetValues());
                    }
                    //Debug.LogError(System.Text.Encoding.ASCII.GetString(obj));
                }
            }
            //Debug.LogError(file.GetVarCount());
            //NCFile file = new NNcFileMode.read
            file.Close();
            //file = null;
            Debug.LogError(file.IsNull());
            System.GC.Collect();
        }

        [TestCase()]
        public void Netcdftests()
        {
            double[] doubleness = new double[1000 * 1000 * 500];
            doubleness = null;
            
            System.GC.Collect(System.GC.MaxGeneration);
            Debug.LogError("NOT CLEARING EH");
        }

        [TestCase()]
        public void ReprojectTest()
        {
            Gdal.AllRegister();
            var ds = Gdal.Open(@"C:\Users\ccarthen\Desktop\I_lw_frameToFile(15).tif", Access.GA_ReadOnly);
            var ds2 = ds.GetDriver().CreateCopy("testor2.tif", ds, 0, new string[] { }, null, null);
            var doubleo = new double[6];
            ds2.GetGeoTransform(doubleo);
            doubleo [0] = doubleo[0] + 1;
            ds2.SetGeoTransform(doubleo);

            Gdal.ReprojectImage(ds, ds2, ds.GetProjection(), ds.GetProjection(), ResampleAlg.GRA_Average, 10000, 10, null, null, new string[] { });

            // hopefully this worked
            Debug.LogError("IT WORKS!!!");
            ds.Dispose();
            ds2.Dispose();
            
        }

        [Test()]
        public void Rect()
        {
            Rect abc = new Rect(-100, -100, 100, 100);
            Debug.LogError(abc.xMin);
            Debug.LogError(abc);
            Debug.LogError(abc.xMax);
        }
    }


}
