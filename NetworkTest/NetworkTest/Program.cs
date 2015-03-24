using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
//using System.Threading.Tasks;

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
        static void Main( string[] args )
        {
            NetworkManager nm = new NetworkManager();
            DataFactory df = new DataFactory(nm);
            
            df.TestStringDownload(VWPString);

            Console.ReadKey();
        }

        static void download()
        {
            Console.WriteLine("Hello from another thread");
        }
    }
}
