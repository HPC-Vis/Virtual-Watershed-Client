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
        public void HaversineCoordinateTest()
        {

        }

    }
}
