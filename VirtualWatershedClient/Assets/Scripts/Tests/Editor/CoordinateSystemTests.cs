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
                angle += 180.0f;
            }
            angle *= Mathf.Deg2Rad;
            Debug.LogError(angle);
            float haverdistance = (float)CoordinateUtils.GetDistanceKM(long1, lat1, long2, lat2);
            Debug.LogError(haverdistance);
            Debug.LogError(angle);
            lat1 *= Mathf.Deg2Rad;
            long1 *= Mathf.Deg2Rad;
            double latitude = Mathf.Asin(Mathf.Sin(lat1) * Mathf.Cos(haverdistance) + Mathf.Cos(lat1) * Mathf.Sin(haverdistance) * Mathf.Cos(angle));
            var dlon = Mathf.Atan2( Mathf.Sin(angle) * Mathf.Sin(haverdistance) * Mathf.Cos(lat1), Mathf.Cos(haverdistance) - Mathf.Sin(lat1) * Mathf.Sin((float)latitude));
            double longitude = (long1 - dlon + Mathf.PI) % (2.0f * Mathf.PI) - Mathf.PI;
            Debug.LogError(longitude*Mathf.Rad2Deg + " " + latitude*Mathf.Rad2Deg);
        }

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
    }
}
