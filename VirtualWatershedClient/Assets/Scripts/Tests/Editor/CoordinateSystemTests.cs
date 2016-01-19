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
    }
}
