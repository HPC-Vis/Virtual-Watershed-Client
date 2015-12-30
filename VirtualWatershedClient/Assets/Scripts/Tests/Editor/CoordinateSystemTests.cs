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
        [TestCase(33,1,33,2)]
        [TestCase(33,1,33,1.2f)]
        public void HaversineCoordinateTest(float long1, float latitude, float long2, float latitude2)
        {
            try
            {
                var CS = new UTMCoordinateSystem(CoordinateUtils.GetZone(latitude, long1));

                CS.WorldOrigin = new Vector2(long1, latitude);

                double haverdistance = CoordinateUtils.GetDistanceKM(long1, latitude, long2, latitude2) * 1000.0;

                var UnityPoint = CS.TranslateToUnity(new Vector2(long2, latitude2));
                Debug.LogError(Vector2.Distance(CS.UnityOrigin, UnityPoint) + " " + haverdistance);
                Assert.AreEqual(Vector2.Distance(CS.UnityOrigin, UnityPoint), haverdistance, 500);
            }
            catch (System.Exception e)
            {
                Debug.LogError("There was a problem" + e.Message);
            }
            

        }

    }
}
